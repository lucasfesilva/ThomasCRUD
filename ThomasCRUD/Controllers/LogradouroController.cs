using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ThomasCRUD.DTOs.Logradouros;
using ThomasCRUD.Models;
using ThomasCRUD.Repository;

namespace ThomasCRUD.Controllers
{
    // Controlador responsável por gerenciar as operações de CRUD relacionadas aos Logradouros
    [Route("api/[controller]")]
    [ApiController]
    public class LogradouroController : ControllerBase
    {
        private readonly ILogradouroRepository _logradouroRepository;

        //Construtor da Controller
        public LogradouroController(ILogradouroRepository logradouroRepository)
        {
            _logradouroRepository = logradouroRepository;
        }

        //Método responsável por obter a lista de todos os Logradouros
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            //Obtem a lista de logradouros do banco de dados
            var logradouros = await _logradouroRepository.GetAllAsync();

            //Popula o DTO do logradouro para retornar a API
            var dtos = logradouros.Select(l => new LogradouroDTO
            {
                Id = l.Id,
                RuaCliente = l.Rua,
                ClienteId = l.ClienteId
            });

            //Retorno da listagem de logradouros para a API
            return Ok(dtos);
        }

        //Método responsável por obter um logradouro com base no ID
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{id}")]
        public async Task <IActionResult> GetById(int id)
        {
            //Consulta no banco de dados se o logradouro com o ID especificado existe
            var logradouro = await _logradouroRepository.GetByIdAsync(id);
            if (logradouro == null)
                return NotFound("Logradouro não encontrado!");

            //Cria um objeto DTO do logradouro encontrado
            var dto = new LogradouroDTO
            {
                Id = logradouro.Id,
                RuaCliente = logradouro.Rua,
                ClienteId = logradouro.ClienteId
            };

            //Retorna o logradouro para a API
            return Ok(dto);
        }

        //Método responsável por criar um novo Logradouro
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LogradouroCreateDTO logradouroDto)
        {
            //cria um objeto Logradouro a partir da DTO
            var logradouro = new Logradouro
            {
                Rua = logradouroDto.RuaCliente,
                ClienteId = logradouroDto.ClienteId,
            };
            
            //Grava o novo Logradouro no banco de dados
            await _logradouroRepository.AddAsync(logradouro);

            //Popula o DTO para retorno para a API
            var resultado = new LogradouroDTO
            {
                Id = logradouro.Id,
                RuaCliente = logradouro.Rua,
                ClienteId = logradouro.ClienteId
            };

            //Retorno para a API
            return CreatedAtAction(nameof(GetById), new {id = resultado.Id}, logradouro);
        }

        //Método responsável por atualizar um Logradouro
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] LogradouroCreateDTO logradouroDto)
        {
            //Consulta no banco de dados se o Logradouro existe
            var logradouro = await _logradouroRepository.GetByIdAsync(id);
            if (logradouro == null)
                return NotFound("Logradouro não encontrado!");

            //Atualiza os dados do Logradouro
            logradouro.Rua = logradouroDto.RuaCliente;
            logradouro.ClienteId = logradouroDto.ClienteId;

            //Salva os dados atualizados no banco de dados
            await _logradouroRepository.UpdateAsync(id, logradouro);

            //Cria um objeto DTO para retornar a API
            var resultado = new LogradouroDTO
            {
                Id = logradouro.Id,
                RuaCliente = logradouro.Rua,
                ClienteId = logradouro.ClienteId
            };

            //Retorno para a API
            return Ok(resultado);
        }

        //Método responsável por deletar um Logradouro com base no ID
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            //Verifica no banco de dados se o Logradouro existe com base no ID
            var logradouro = await _logradouroRepository.GetByIdAsync(id);
            if (logradouro == null)
                return NotFound("Logradouto não encontrado!");

            //Apaga o Logradouro do banco de dados
            await _logradouroRepository.DeleteAsync(id);

            //Retorno para a API
            return Ok("Logradouro removido com sucesso!");
        }
    }
}
