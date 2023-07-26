using CoinAction.Game;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoinAction.Lobby
{
    public class Room
    {
        public string Name { get; set; }

        public List<User> Players { get; set; }

        public Match Match { get; set; }
    }
}
