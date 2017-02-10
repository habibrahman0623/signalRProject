using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalRChat
{
    public class Player
    {
        public String Name { get; set; }
        public String ConnectionId { get; set; }
        public int BidPoint { get; set; }
        public List<Card> cardList;

        public Player(String name, String connectionId)
        {
            Name = name;
            ConnectionId = connectionId;
            cardList = new List<Card>();
        }

        public void AddCardList(List<Card> cards )
        {
            foreach (var card in cards)
            {
                cardList.Add(card);
            }
        }
    }
}