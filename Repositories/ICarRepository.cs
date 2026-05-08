using SmartCarRentACar.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartCarRentACar.Repositories
{
    public interface ICarRepository
    {
        Task<IEnumerable<Car>> GetAll();
        Task<Car?> GetById(int id);
        Task Add(Car car);
        Task Update(Car car);
        Task Delete(int id);
    }
}
