using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUAVVY_FusionHacks2.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Recipe { get; set; }
        public double Price { get; set; }
        public string ImageUrl { get; set; }
        public int Sales{ get; set; }
    }
}
