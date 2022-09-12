using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.Entity;

namespace Scales.Context
{
    internal class ApplicationContext: DbContext
    {
        public ApplicationContext()
    : base("DbConnection")
        { }
        public DbSet<CarType> CarTypes { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Nomenclature> Nomenclatures { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
    }
}
