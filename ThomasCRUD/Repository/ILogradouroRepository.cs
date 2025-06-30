using ThomasCRUD.Models;

namespace ThomasCRUD.Repository
{
    public interface ILogradouroRepository
    {
        public Task<Logradouro> GetByIdAsync(int id);
        public Task<IEnumerable<Logradouro>> GetAllAsync();
        public Task<Logradouro> AddAsync(Logradouro logradouro);
        public Task<Logradouro> UpdateAsync(int id, Logradouro logradouro);
        public Task DeleteAsync(int id);
    }
}
