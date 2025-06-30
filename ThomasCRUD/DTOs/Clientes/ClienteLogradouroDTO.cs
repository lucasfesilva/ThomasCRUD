using ThomasCRUD.DTOs.Logradouros;

namespace ThomasCRUD.DTOs.Clientes
{
    public class ClienteLogradouroDTO
    {
        public int Id { get; set; }
        public string NomeCliente { get; set; }
        public string EmailCliente { get; set; }
        public string LogotipoCliente { get; set; }
        public List<LogradouroDTO> Logradouros { get; set; }
    }
}
