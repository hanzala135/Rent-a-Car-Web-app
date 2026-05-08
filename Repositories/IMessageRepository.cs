using SmartCarRentACar.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartCarRentACar.Repositories
{
    public interface IMessageRepository
    {
        Task<IEnumerable<Message>> GetAll();
        Task Add(Message message);
    }
}
