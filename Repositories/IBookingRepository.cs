using SmartCarRentACar.Models;
using System.Collections.Generic;

namespace SmartCarRentACar.Repositories
{
   public interface IBookingRepository
{
    Task<IEnumerable<Booking>> GetAll();
    Task<Booking?> GetById(int id);
    Task Add(Booking booking);
    Task Update(Booking booking);
}

}
