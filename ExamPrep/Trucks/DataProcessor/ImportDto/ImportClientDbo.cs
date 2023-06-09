﻿using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Trucks.DataProcessor.ImportDto
{
    public class ImportClientDbo
    {
        [Required]
        [MaxLength(40)]
        [MinLength(3)]
        public string? Name { get; set; } 

        [Required]
        [MaxLength(40)]
        [MinLength(2)]
        public string? Nationality { get; set; } 

        [Required]
        public string? Type { get; set; }

       
        public int[]? Trucks { get; set; }
    }
}
