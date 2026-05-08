using Microsoft.AspNetCore.Mvc;
using SmartCarRentACar.Repositories;
using SmartCarRentACar.Models;
using Microsoft.AspNetCore.SignalR;
using SmartCarRentACar.Hubs;


namespace SmartCarRentACar.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICarRepository _carRepo;
        private readonly IBookingRepository _bookingRepo;
        private readonly IMessageRepository _msgRepo;
        private readonly IHubContext<AdminHub> _hubContext;


        public HomeController(
     ICarRepository carRepo,
     IBookingRepository bookingRepo,
     IMessageRepository msgRepo,
     IHubContext<AdminHub> hubContext)
        {
            _carRepo = carRepo;
            _bookingRepo = bookingRepo;
            _msgRepo = msgRepo;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Index()
        {
            var cars = await _carRepo.GetAll();
            return View(cars);
        }


        public async Task<IActionResult> Cars()
        {
            var cars = await _carRepo.GetAll();
            return View(cars);
        }


        public async Task<IActionResult> CarDetails(int id)
        {
            var car = await _carRepo.GetById(id);
            if (car == null) return NotFound();
            return View(car);
        }

        [HttpGet]
        public IActionResult Booking()
        {

            return View();
        }
        public async Task<IActionResult> Reservations()
        {

            var bookings = await _bookingRepo.GetAll();
            return View(bookings);
        }


        [HttpPost]

        public async Task<IActionResult> Booking(Booking booking)
        {
            if (!ModelState.IsValid)
            {
                return View(booking);
            }

            booking.Status = "Pending";
            await _bookingRepo.Add(booking);


            await _hubContext.Clients.All.SendAsync("NewBookingAdded", new
            {
                id = booking.Id,
                customerName = booking.CustomerName,
                carName = booking.CarName,
                fromDate = booking.FromDate.ToString("dd/MM/yyyy"),
                toDate = booking.ToDate.ToString("dd/MM/yyyy"),
                status = booking.Status
            });

            TempData["Success"] = "Your booking request has been submitted!";
            return RedirectToAction("Index");
        }



        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]

        [HttpPost]
        public async Task<IActionResult> Contact(Message msg)
        {
            if (!ModelState.IsValid)
                return View(msg);

            await _msgRepo.Add(msg);


            await _hubContext.Clients.All.SendAsync("NewMessageReceived");

            TempData["Success"] = "Your message has been sent!";
            return RedirectToAction("Contact");
        }

    }
}
