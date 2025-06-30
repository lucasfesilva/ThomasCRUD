namespace ThomasCRUD.Models
{
    public class Logradouro
    {
        public int Id { get; set; }
        public string Rua { get; set; }
        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; }
    }
}
