using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUAVVY_FusionHacks2.Models
{
    public class ProductItemViewModel : BaseViewModel
    {
        public int Quantity { get; set; }
        public Product Item { get; set; }
    }
}
