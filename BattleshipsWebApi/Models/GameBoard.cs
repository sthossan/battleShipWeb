using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class GameBoard
    {
        public GameBoard()
        {
        }
        public string Row { get; set; }
        public List<Column> Columns { get; set; } = new List<Column>();

        
    }
}
