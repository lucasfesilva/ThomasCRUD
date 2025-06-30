using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ThomasCRUD.Data;
using ThomasCRUD.DTOs.Logradouros;
using ThomasCRUD.Models;

namespace ThomasCRUD.Repository
{
    public class LogradouroRepository : ILogradouroRepository
    {
        private readonly AppDbContext _context;

        //Construtor do Repository
        public LogradouroRepository(AppDbContext context)
        {
            _context = context;
        }

        //Método para criar um novo Logradouro
        public async Task<Logradouro?> AddAsync(Logradouro logradouro)
        {
            var idParam = new SqlParameter
            {
                ParameterName = "@NewLogradouroId",
                SqlDbType = System.Data.SqlDbType.Int,
                Direction = System.Data.ParameterDirection.Output
            };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_InsertLogradouro @Rua = {0}, @ClienteId = {1}, @NewLogradouroId = @NewLogradouroId OUTPUT", //Chama a Stored Procedure para criar o novo Logradouro
                logradouro.Rua, logradouro.ClienteId, idParam);

            int id = (int)idParam.Value;
            var novoLogradouro = await _context.Logradouros.FindAsync(id);
            return novoLogradouro;
        }

        //Método para deletar um logradouro
        public async Task DeleteAsync(int id)
        {
            var logradouro = await GetByIdAsync(id);
            if (logradouro != null)
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC sp_DeleteLogradouro @Id = {0}", id); //Chama a Stored Procedure para deletar o Logradouro
            }
        }

        //Método para obter todos os Logradouros
        public async Task<IEnumerable<Logradouro>> GetAllAsync()
        {
            return await _context.Logradouros
                .ToListAsync();
        }


        //Método para obter um logradouro com base no ID
        public async Task<Logradouro> GetByIdAsync(int id)
        {
            return await _context.Logradouros
                .FirstOrDefaultAsync(l => l.Id == id);
        }


        //Método para atualizar um Logradouro
        public async Task<Logradouro?> UpdateAsync(int id, Logradouro logradouro)
        {
            var logradouroExistente = await _context.Logradouros.FindAsync(id);
            if (logradouroExistente == null)
                return null;

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_UpdateLogradouro @Id = {0}, @Rua = {1}, @ClienteId = {2}", //Chama a Stored Procedure para atualizar o Logradouro
                id, logradouro.Rua, logradouro.ClienteId);

            return await _context.Logradouros.FindAsync(id);
        }
    }
}
