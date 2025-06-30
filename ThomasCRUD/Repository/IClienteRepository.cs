using Microsoft.AspNetCore.Mvc;
using ThomasCRUD.DTOs;
using ThomasCRUD.DTOs.Clientes;
using ThomasCRUD.Models;

namespace ThomasCRUD.Repository
{
    public interface IClienteRepository
    {
        public Task<Cliente> GetByIdAsync(int id);
        public Task<IEnumerable<Cliente>> GetAllAsync();
        public Task<Cliente> AddAsync(Cliente cliente);
        public Task<Cliente?> UpdateAsync(int id, Cliente cliente);
        public Task DeleteAsync(int id);

    }
}
