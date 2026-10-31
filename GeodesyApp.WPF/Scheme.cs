using System;
using System.Collections.Generic;
using System.Text;

namespace GeodesyApp
{
    public class Scheme
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Axes { get; set; } = "";        // Оси (новое поле)
        public string Mark { get; set; } = "";        // Отметка (теперь строка!)
        public decimal? Volume { get; set; }          // Объём (nullable — может быть NULL)
        public string Status { get; set; } = "";
    }
}
