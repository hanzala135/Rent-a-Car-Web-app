using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SmartCarRentACar.Repositories;
using SmartCarRentACar.Models;
using Microsoft.AspNetCore.SignalR;
using SmartCarRentACar.Hubs;


namespace SmartCarRentACar.Controllers
{
    public class AdminController : Controller
    {

        private readonly ICarRepository _carRepo;
        private readonly IBookingRepository _bookingRepo;
        private readonly IMessageRepository _messageRepo;
        private readonly IHubContext<AdminHub> _hubContext;


        private const string AdminUser = "admin12345@pucit.edu.pk";
        private const string AdminPass = "aimen36";

        public AdminController(
    ICarRepository carRepo,
    IBookingRepository bookingRepo,
    IMessageRepository messageRepo,
    IHubContext<AdminHub> hubContext)
        {
            _carRepo = carRepo;
            _bookingRepo = bookingRepo;
            _messageRepo = messageRepo;
            _hubContext = hubContext;
        }


        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (username == AdminUser && password == AdminPass)
            {
                HttpContext.Session.SetString("AdminLoggedIn", "true");
                return RedirectToAction("Dashboard");
            }

            ViewBag.Error = "Invalid Username or Password";
            return View();
        }

        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("AdminLoggedIn") != "true")
                return RedirectToAction("Login");

            return View();
        }


        public async Task<IActionResult> ManageCars()
        {
            if (HttpContext.Session.GetString("AdminLoggedIn") != "true")
                return RedirectToAction("Login");

            var cars = await _carRepo.GetAll();
            return View(cars);
        }



        public async Task<IActionResult> ViewBooking()
        {
            if (HttpContext.Session.GetString("AdminLoggedIn") != "true")
                return RedirectToAction("Login");
            var bookings = await _bookingRepo.GetAll();
            return View(bookings);
        }




        public async Task<IActionResult> Messages()
        {
            if (HttpContext.Session.GetString("AdminLoggedIn") != "true")
                return RedirectToAction("Login");

            var messages = await _messageRepo.GetAll();
            return View(messages);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllMessages()
        {
            if (HttpContext.Session.GetString("AdminLoggedIn") != "true")
                return Unauthorized();

            var messages = await _messageRepo.GetAll();

            var messageList = messages.Select(m => new
            {
                id = m.Id,
                senderName = m.SenderName,
                email = m.Email,
                content = m.Content
            }).ToList();

            return Json(new { success = true, data = messageList });
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> ViewCars()
        {
            if (HttpContext.Session.GetString("AdminLoggedIn") != "true")
                return RedirectToAction("Login");

            var cars = await _carRepo.GetAll();
            return View(cars);
        }



        [HttpGet]
        public async Task<IActionResult> AddCar(int? id)

        {
            if (HttpContext.Session.GetString("AdminLoggedIn") != "true")
                return RedirectToAction("Login");

            if (id.HasValue)
            {
                var car = await _carRepo.GetById(id.Value);

                if (car == null) return NotFound();
                return View(car);
            }

            return View(new Car());
        }


        [HttpPost]
        public async Task<IActionResult> AddCar(Car car)
        {
            if (!ModelState.IsValid)
                return View(car);

            if (car.Id == 0)
            {
                await _carRepo.Add(car);
                TempData["Success"] = "New Car Added Successfully!";
            }
            else
            {
                await _carRepo.Update(car);
                TempData["Success"] = "Car Updated Successfully!";
            }

            return RedirectToAction("ManageCars");
        }



        [HttpPost]
        public async Task<IActionResult> DeleteCar(int id)
        {
            if (HttpContext.Session.GetString("AdminLoggedIn") != "true")
                return Json(new { success = false, message = "Unauthorized" });

            var car = await _carRepo.GetById(id);


            if (car == null)
                return Json(new { success = false, message = "Car not found" });

            await _carRepo.Delete(id);
            return Json(new { success = true });
        }


        [HttpPost]
        public async Task<IActionResult> UpdateBookingStatus(int id, string status)
        {
            if (HttpContext.Session.GetString("AdminLoggedIn") != "true")
            {
                return Json(new { success = false, message = "Unauthorized" });
            }


            var booking = await _bookingRepo.GetById(id);

            if (booking == null)
            {
                return Json(new { success = false, message = "Booking not found" });
            }

            booking.Status = status;
            await _bookingRepo.Update(booking);

            await _hubContext.Clients.All.SendAsync(
                "BookingStatusUpdated",
                booking.Id,
                booking.Status
            );

            return Json(new { success = true });

        }

    }
}
