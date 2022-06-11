using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models
{
    public class Player
    {
        Random random = new Random();
        public string Name { get; set; }

        public int StepsTaken { get; set; } = 0;
        public List<Position> Battleship { get; set; }
        public List<Position> Destroyer { get; set; }
        public List<Position> AllShipsPosition { get; set; } = new List<Position>();
        public List<Position> FirePositions { get; set; } = new List<Position>();
        public bool IsBattleshipSunk { get; set; } = false;
        public bool IsDestroyerSunk { get; set; } = false;
        public bool IsObliteratedAll { get; set; } = false;
        
        public List<GameBoard> GameBoards = new List<GameBoard>();
        public Player(string name)
        {
            Name = name;
            GameBoards = new List<GameBoard>();
        }
    }
}
