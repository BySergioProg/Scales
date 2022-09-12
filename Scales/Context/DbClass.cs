using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scales.Context
{
   public class CarType
    {
        public int Id { get; set; }
        public string Type { get; set; }
    }
    public class Car
    {
        public int Id { get; set; }
        public string CarType { get; set; }
        public string CarNo { get; set; }
        public double CarWeight { get; set; }
    }
    public class Nomenclature
    {
        public int Id { get; set; }
        public string NomenclatureName { get; set;}
    }
    public class Shipment
    {
        public int Id { get; set;}
        public DateTime ShipDateTime { get; set; }
        public string CarType { get; set; }
        public string CarNo { get; set;}
        public string NomenclatureName { get; set; }
        public double CarWeight { get; set; }
        public double Brutto { get; set; }
        public double Netto { get; set; }
    }

}
