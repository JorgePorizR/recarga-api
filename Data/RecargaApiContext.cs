using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RecargaApi.Models;

namespace RecargaApi.Data
{
    public class RecargaApiContext : DbContext
    {
        public RecargaApiContext (DbContextOptions<RecargaApiContext> options)
            : base(options)
        {
        }
        public DbSet<RecargaApi.Models.Billetera> Billetera { get; set; } = default!;
        public DbSet<RecargaApi.Models.Movimiento> Movimiento { get; set; } = default!;

    }
}
