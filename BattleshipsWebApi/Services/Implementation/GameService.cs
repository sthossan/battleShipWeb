using Models;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Services.Implementation
{
    public class GameService : IGameService
    {
        private const int BATTLESHIP = 5;
        private const int DESTROYER = 4;

        public Dictionary<char, int> _coordinates = new Dictionary<char, int>();

        public GameService()
        {
            _coordinates = PopulateDictionary();
        }

        /// <summary>
        /// First time create game board with ship placement for start the game
        /// </summary>
        /// <returns></returns>
        public List<Player> GeneratePalyer()
        {
            List<Player> players = new List<Player>();
            Player player1 = new Player("My Board");

            player1.Battleship = GeneratePosistion(BATTLESHIP, player1.AllShipsPosition);
            player1.Destroyer = GeneratePosistion(DESTROYER, player1.AllShipsPosition);

            char rowChar = 'A';
            for (int rowIndex = 1; rowIndex < 11; rowIndex++)
            {
                GameBoard gameBoard = new GameBoard
                {
                    Row = rowChar.ToString()
                };

                for (int colIndex = 1; colIndex < 11; colIndex++)
                {
                    Column column = new Column
                    {
                        Name = $"{rowChar}{colIndex}",
                        OccupationType = player1.Battleship.Any(t => t.x == rowIndex && t.y == colIndex) ? OccupationType.Battleship :
                                         player1.Destroyer.Any(t => t.x == rowIndex && t.y == colIndex) ? OccupationType.Destroyer : null,
                        Color = player1.AllShipsPosition.Any(t => t.x == rowIndex && t.y == colIndex) ? "black" : null,
                        IsShipShow = player1.AllShipsPosition.Any(t => t.x == rowIndex && t.y == colIndex),
                        x = rowIndex,
                        y = colIndex
                    };
                    gameBoard.Columns.Add(column);
                }
                player1.GameBoards.Add(gameBoard);
                rowChar++;

            }

            Player player2 = new Player("Computer");

            player2.Battleship = GeneratePosistion(BATTLESHIP, player2.AllShipsPosition);
            player2.Destroyer = GeneratePosistion(DESTROYER, player2.AllShipsPosition);

            rowChar = 'A';
            for (int rowIndex = 1; rowIndex < 11; rowIndex++)
            {
                GameBoard gameBoard = new GameBoard
                {
                    Row = rowChar.ToString()
                };

                for (int colIndex = 1; colIndex < 11; colIndex++)
                {
                    Column column = new Column
                    {
                        Name = $"{rowChar}{colIndex}",
                        OccupationType = player2.Battleship.Any(t => t.x == rowIndex && t.y == colIndex) ? OccupationType.Battleship :
                                         player2.Destroyer.Any(t => t.x == rowIndex && t.y == colIndex) ? OccupationType.Destroyer : null,
                        Color = player2.AllShipsPosition.Any(t => t.x == rowIndex && t.y == colIndex) ? "black" : null,
                        IsShipShow = false,
                        x = rowIndex,
                        y = colIndex
                    };
                    gameBoard.Columns.Add(column);
                }
                player2.GameBoards.Add(gameBoard);
                rowChar++;

            }

            players.Add(player1);
            players.Add(player2);

            return players;
        }


        /// <summary>
        /// This API service for vailded user input and return computer hitting
        /// </summary>
        /// <param name="players"></param>
        /// <param name="input"></param>
        public List<Player> EnemyShot(List<Player> players, string input)
        {
            Position position = new Position();
            char[] inputSplit = input.ToUpper().ToCharArray();
            try
            {
                position = AnalyzeInput(inputSplit);

                if (position.x == -1 || position.y == -1)
                {
                    throw new Exception("Invalid coordinates!");
                }

                if (players[0].FirePositions.Any(EFP => EFP.x == position.x && EFP.y == position.y))
                {
                    throw new Exception("This coordinate already being shot.");
                }


                var index = players[0].FirePositions.FindIndex(p => p.x == position.x && p.y == position.y);
                if (index == -1) players[0].FirePositions.Add(position);

                var enemyShotPos = Fire(players[1]);
                
                // change enemy column color by player hiting
                ColumnColor(players[1], players[0].FirePositions, position);
                CheckShipStatus(players[0].FirePositions, players[1]);

                // change player column color by computer hiting
                ColumnColor(players[0], players[1].FirePositions, enemyShotPos);
                CheckShipStatus(players[1].FirePositions, players[0]);

                return players;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This function hit to enemy;
        /// like computer hit to player
        /// </summary>
        /// <param name="player"></param>
        private Position Fire(Player player)
        {
            Random random = new Random();
            Position enemyShotPos = new Position();
            bool alreadyShot = false;
            do
            {
                enemyShotPos.x = random.Next(1, 11);
                enemyShotPos.y = random.Next(1, 11);
                alreadyShot = player.FirePositions.Any(EFP => EFP.x == enemyShotPos.x && EFP.y == enemyShotPos.y);
            }
            while (alreadyShot);

            player.FirePositions.Add(enemyShotPos);

            return enemyShotPos;
        }

        private void ColumnColor(Player player, List<Position> firePositionList, Position position)
        {
            var row = _coordinates.FirstOrDefault(t => t.Value == position.x).Key;

            var column = player.GameBoards.Where(t => t.Row == row.ToString() && t.Columns.Any(col => col.x == position.x && col.y == position.y)).FirstOrDefault()
                                 .Columns.Where(col => col.x == position.x && col.y == position.y).FirstOrDefault();
            //.Color = firePositionList.Any(t => t.x == position.x && t.y == position.y) ? "lost" : "hit";
            //.Select(s => new
            //{
            //    Color = firePositionList.Any(t => t.x == position.x && t.y == position.y) ? "lost" : "hit",
            //});

            column.Color = column.OccupationType != null ? "lost" : "hit";

        }

        /// <summary>
        /// check all ships or a single ship is sunk or not
        /// </summary>
        /// <param name="HitPositions"></param>
        /// <param name="player"></param>
        private void CheckShipStatus(List<Position> HitPositions, Player player)
        {
            player.IsBattleshipSunk = player.Battleship.Where(B => !HitPositions.Any(H => B.x == H.x && B.y == H.y)).ToList().Count == 0;
            player.IsDestroyerSunk = player.Destroyer.Where(D => !HitPositions.Any(H => D.x == H.x && D.y == H.y)).ToList().Count == 0;
            player.IsObliteratedAll = player.IsBattleshipSunk && player.IsDestroyerSunk;
        }

        /// <summary>
        /// Set player all ship position in a array
        /// </summary>
        /// <param name="size"></param>
        /// <param name="AllPosition"></param>
        /// <returns></returns>
        private List<Position> GeneratePosistion(int size, List<Position> AllPosition)
        {
            List<Position> positions = new List<Position>();

            bool IsExist = false;

            do
            {
                positions = GeneratePositionRandomly(size);
                IsExist = positions.Where(AP => AllPosition.Exists(ShipPos => ShipPos.x == AP.x && ShipPos.y == AP.y)).Any();
            }
            while (IsExist);

            AllPosition.AddRange(positions);

            return positions;
        }
        /// <summary>
        /// Set player ship randomly in game board
        /// </summary>
        /// <param name="size"></param>
        /// <param name="AllPosition"></param>
        /// <returns></returns>
        private List<Position> GeneratePositionRandomly(int size)
        {
            List<Position> positions = new List<Position>();
            Random random = new Random();

            int direction = random.Next(1, size);//odd for vertical and even for horizontal
                                                 //pick row and column
            int row = random.Next(1, 11);
            int col = random.Next(1, 11);

            if (direction % 2 != 0)// odd for vertical
            {
                if (row - size > 0) // bottom to top in column
                {
                    for (int i = 0; i < size; i++)
                    {
                        Position pos = new Position
                        {
                            x = row - i,
                            y = col
                        };
                        positions.Add(pos);
                    }
                }
                else // top to bottom in column
                {
                    for (int i = 0; i < size; i++)
                    {
                        Position pos = new Position
                        {
                            x = row + i,
                            y = col
                        };
                        positions.Add(pos);
                    }
                }
            }
            else //even for horizontal
            {
                if (col - size > 0) // right to left in a row
                {
                    for (int i = 0; i < size; i++)
                    {
                        Position pos = new Position
                        {
                            x = row,
                            y = col - i
                        };
                        positions.Add(pos);
                    }
                }
                else  // left to right in a row
                {
                    for (int i = 0; i < size; i++)
                    {
                        Position pos = new Position
                        {
                            x = row,
                            y = col + i
                        };
                        positions.Add(pos);
                    }
                }
            }
            return positions;
        }

        /// <summary>
        /// Create Coordinates for row and column name 
        /// </summary>
        /// <returns></returns>
        private Dictionary<char, int> PopulateDictionary()
        {
            Dictionary<char, int> Coordinate =
                     new Dictionary<char, int>
                     {
                         { 'A', 1 },
                         { 'B', 2 },
                         { 'C', 3 },
                         { 'D', 4 },
                         { 'E', 5 },
                         { 'F', 6 },
                         { 'G', 7 },
                         { 'H', 8 },
                         { 'I', 9 },
                         { 'J', 10 }
                     };

            return Coordinate;
        }

        /// <summary>
        /// Check user input for hitting is vaild or not
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private Position AnalyzeInput(char[] inputSplit)
        {
            Position pos = new Position();

            if (inputSplit.Length < 2 || inputSplit.Length > 3)
            {
                return pos;
            }

            if (_coordinates.TryGetValue(inputSplit[0], out int value))
            {
                pos.x = value;
            }
            else
            {
                return pos;
            }
            if (inputSplit.Length == 3)
            {
                if (inputSplit[1] == '1' && inputSplit[2] == '0')
                {
                    pos.y = 10;
                    return pos;
                }
                else
                {
                    return pos;
                }

            }
            if (inputSplit[1] - '0' > 9)
            {
                return pos;
            }
            else
            {
                pos.y = inputSplit[1] - '0';
            }
            return pos;
        }

    }
}
