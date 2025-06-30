using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ThomasCRUD.DTOs;
using ThomasCRUD.DTOs.Clientes;
using ThomasCRUD.DTOs.Logradouros;
using ThomasCRUD.Models;
using ThomasCRUD.Repository;

namespace ThomasCRUD.Controllers
{
    // Controlador responsável por gerenciar as operações de CRUD do Cliente
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteRepository _clienteRepository;

        //Construtor do ClienteController
        public ClienteController(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        // Recupera todos os Clientes cadastrados
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            //Obtem todos os clientes do banco de dados
            var clientes = await _clienteRepository.GetAllAsync();

            //Cria uma lista de Clientes para o DTO
            var dtos = clientes.Select(c => new ClienteLogradouroDTO
            {
                Id = c.Id,
                NomeCliente = c.Nome,
                EmailCliente = c.Email,
                LogotipoCliente = c.Logotipo,
                Logradouros = c.Logradouros.Select(l => new LogradouroDTO
                {
                    Id = l.Id,
                    RuaCliente = l.Rua,
                    ClienteId = l.ClienteId
                }).ToList()
            });

            //Retorna os dados do DTO para a API
            return Ok(dtos);
        }

        //Recupera um cliente específico pelo ID
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            //Realiza a consulta no banco de dados com base no ID fornecido
            var cliente = await _clienteRepository.GetByIdAsync(id);
            if (cliente == null)
                return NotFound();

            //Cria um objeto DTO para retorno para a API
            var dto = new ClienteLogradouroDTO
            {
                Id = cliente.Id,
                NomeCliente = cliente.Nome,
                EmailCliente = cliente.Email,
                LogotipoCliente = cliente.Logotipo,
                Logradouros = cliente.Logradouros.Select(l => new LogradouroDTO
                {
                    Id = l.Id,
                    RuaCliente = l.Rua
                }).ToList()
            };

            //Retorno para a API
            return Ok(dto);
        }

        //Cria um novo Cliente
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ClienteCreateDTO clienteDto)
        {
            //Obtem os clientes
            var clienteExistente = await _clienteRepository.GetAllAsync();
            var emailExistente = clienteExistente.Any(c => c.Email == clienteDto.EmailCliente);

            //Verifica se o cliente já tem o e-mail passado cadastrado
            if (emailExistente)
                return BadRequest("Já existe um cliente com este E-mail cadastrado!");

            //Construtor para criar um novo cliente
            var cliente = new Cliente
            {
                Nome = clienteDto.NomeCliente,
                Email = clienteDto.EmailCliente,
                Logotipo = clienteDto.LogotipoCliente
            };

            //Inserção do cliente novo no banco de dados
            await _clienteRepository.AddAsync(cliente);

            //Criação da classe de retorno para a API
            var resultado = new ClienteDTO
            {
                Id = cliente.Id,
                NomeCliente = cliente.Nome,
                EmailCliente = cliente.Email,
                LogotipoCliente = cliente.Logotipo
            };

            //Retorno para a API
            return CreatedAtAction(nameof(GetById), new { id = resultado.Id }, cliente);
        }

        //Atualiza um Cliente existente
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ClienteUpdateDTO clienteDto)
        {
            //Realiza uma consulta com base no ID para verificar se o cliente existe
            var cliente = await _clienteRepository.GetByIdAsync(id);
            if (cliente == null)
                return NotFound("Cliente não cadastrado!");
            if (cliente.Email == clienteDto.EmailCliente)
                return BadRequest("Este endereço de E-mail já está sendo utilizado por outro Cliente!");

            //Popula a classe cliente com os dados recebidos do DTO da API
            cliente.Nome = clienteDto.NomeCliente;
            cliente.Email = clienteDto.EmailCliente;
            cliente.Logotipo = clienteDto.LogotipoCliente;

            //Salva as alterações no banco de dados
            await _clienteRepository.UpdateAsync(id, cliente);

            //Cria um novo objeto DTO para retorno para API
            var resultado = new ClienteDTO
            {
                Id = cliente.Id,
                NomeCliente = cliente.Nome,
                EmailCliente = cliente.Email,
                LogotipoCliente = cliente.Logotipo
            };

            //Retorno para a API
            return Ok(resultado);
        }

        //Deleta um cliente com base no ID fornecido
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            //Consulta no banco se o cliente existe
            var cliente = await _clienteRepository.GetByIdAsync(id);
            if (cliente == null) 
                return NotFound("Cliente não encontrado!");

            //Delete do banco de dados o cliente
            await _clienteRepository.DeleteAsync(id);

            //Retorno para a API
            return Ok("Cliente deletado com sucesso!");
        }
    }
}
