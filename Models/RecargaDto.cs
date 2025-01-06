using System.ComponentModel.DataAnnotations.Schema;

namespace RecargaApi.Models
{
    public class RecargaDto
    {
        public int UserId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Monto { get; set; }
        public string Descripcion { get; set; }
        public IFormFile File { get; set; }
    }
}
