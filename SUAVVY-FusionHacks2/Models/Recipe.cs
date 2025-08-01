﻿using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUAVVY_FusionHacks2.Models
{
    public class Recipe
    {
        [PrimaryKey]
        [NotNull]
        [AutoIncrement]
        public int ID { get; set; }

        [NotNull]
        public int UserID { get; set; }

        [NotNull]
        public string SKU { get; set; }

        [NotNull]
        public string Name { get; set; }

        [NotNull]
        public string Description { get; set; }

        [NotNull]
        public string PrepTimeInMinutes { get; set; }

        [NotNull]
        public string CookTimeInMinutes { get; set; }

        [NotNull]
        public string InitialServings { get; set; }
        public string Photo { get; set; }
        [NotNull]
        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
