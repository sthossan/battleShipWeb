using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class Column
    {
        public string Name { get; set; }
        public string OccupationType { get; set; }
        public string Color { get; set; }
        public bool IsShipShow { get; set; }
        public int x { get; set; } = -1;
        public int y { get; set; } = -1;
    }
}
