using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalRChat
{
    public class Team
    {
        public int Points { get; set; }
        public int GamePoint { get; set; }
        public int BidPoint { get; set; }
        public bool IsTrumpSet { get; set; }
        public List<Player> players= new List<Player>();

        public Team(Player player1, Player player2)
        {
            players.Add(player1);
            players.Add(player2);
        }

        public bool DecideBoardWinner()
        {
            return Points >= BidPoint;
        }
 
    }
}