using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUAVVY_FusionHacks2.Models
{
    public class ProductViewModel : BaseViewModel
    {
        public bool SelectMode { get; set; } = false;
        public bool IsGrid { get; set; } = false;
        /// <summary>
        /// List of Products
        /// </summary>
        public List<Product> Products { get; set; } = new List<Product>();
        /// <summary>
        /// Selected Product for editing or viewing details
        /// </summary>
        public Product SelectedProduct { get; set; } = new Product();
        public string LoadedPhoto { get; set; }
        public string Search { get; set; }
        public string LoadedPhotoPath { get; set; }
    }
}
