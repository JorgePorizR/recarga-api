using System.ComponentModel.DataAnnotations.Schema;

namespace RecargaApi.Models
{
    public class Billetera
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Saldo { get; set; }
    }
}
