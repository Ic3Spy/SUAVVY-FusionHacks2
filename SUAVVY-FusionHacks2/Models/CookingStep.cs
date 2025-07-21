using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUAVVY_FusionHacks2.Models
{
    public class CookingStep
    {
        [PrimaryKey, AutoIncrement, NotNull]
        public int ID { get; set; }
        [NotNull]
        public int StepId { get; set; }
        [NotNull]
        public string StepDetails { get; set; }
    }
}
