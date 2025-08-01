using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUAVVY_FusionHacks2.Models
{
    public class HomeViewModel : BaseViewModel
    {
        //Inherited properties from BaseViewModel
        public string Search { get; set; }
        public List<Product> Popular { get; set; } = new List<Product>();
        public List<Product> TopWeek { get; set; } = new List<Product>();
    }
}
