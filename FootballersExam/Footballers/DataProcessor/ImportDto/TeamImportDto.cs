﻿namespace Footballers.DataProcessor.ImportDto
{
    public class TeamImportDto
    {
        public string? Name { get; set; }

        public string? Nationality { get; set; }

        public string? Trophies { get; set; }

        public int[] Footballers { get; set; }
    }
}
