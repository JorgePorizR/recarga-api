using System.ComponentModel.DataAnnotations.Schema;

namespace RecargaApi.Models
{
    public class Movimiento
    {
        public int Id { get; set; }
        public int Tipo { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Monto { get; set; }
        public int UserId { get; set; }
        public string Comprobante { get; set; }
        public string Descripcion { get; set; }
    }
}
