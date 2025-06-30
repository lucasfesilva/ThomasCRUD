using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ThomasCRUD.Data;
using ThomasCRUD.DTOs;
using ThomasCRUD.Models;

namespace ThomasCRUD.Repository
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly AppDbContext _context;

        //Construtor do Repository
        public ClienteRepository(AppDbContext context)
        {
            _context = context;
        }

        //Método para criar um novo Cliente
        public async Task<Cliente> AddAsync(Cliente cliente)
        {
            var IdParam = new SqlParameter
            {
                ParameterName = "@NewClienteId",
                SqlDbType = System.Data.SqlDbType.Int,
                Direction = System.Data.ParameterDirection.Output
            };
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_InsertCliente @Nome = {0}, @Email = {1}, @Logotipo = {2}, @NewClienteId = @NewClienteId OUTPUT", //Chama a Stored Procedure para criar o novo Cliente
                cliente.Nome, cliente.Email, cliente.Logotipo, IdParam);

            int id = (int)IdParam.Value;

            var novoCliente = await _context.Clientes.FindAsync(id);
            return novoCliente;
        }

        //Método para deletar um Cliente
        public async Task DeleteAsync(int id)
        {
            var cliente = await GetByIdAsync(id);
            if(cliente != null)
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC sp_DeleteCliente @Id = {0}", id); //Chama a Stored Procedure para deletar o Cliente
            }
        }

        //Método para obter todos os Clientes
        public async Task<IEnumerable<Cliente>> GetAllAsync()
        {
            return await _context.Clientes
                .Include(c => c.Logradouros)
                .ToListAsync();
        }

        //Método para obter um cliente com base no ID
        public async Task<Cliente> GetByIdAsync(int id)
        {
            return await _context.Clientes
                .Include(c => c.Logradouros)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        //Método para atualizar um Cliente
        public async Task<Cliente?> UpdateAsync(int id, Cliente cliente)
        {
            var clienteExistente = await _context.Clientes.FindAsync(id);

            if (clienteExistente == null)
                return null;

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_UpdateCliente @Id = {0}, @Nome = {1}, @Email = {2}, @Logotipo = {3}", //Chama a Stored Procedure para atualizar os dados do Cliente
                id, cliente.Nome, cliente.Email, cliente.Logotipo);

            return await _context.Clientes.FindAsync(id);
        }
    }
}
