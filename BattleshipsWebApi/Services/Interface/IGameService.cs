using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Interface
{
    public interface IGameService
    {
        /// <summary>
        /// First time create game board with ship placement for start the game
        /// </summary>
        /// <returns></returns>
        List<Player> GeneratePalyer();

        /// <summary>
        /// This API service for vailded user input and return computer hitting
        /// </summary>
        /// <param name="players"></param>
        /// <param name="input"></param>
        List<Player> EnemyShot(List<Player> players, string input);
    }
}
