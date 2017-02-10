using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalRChat
{
    public class Card
    {
        public int Id { get; set; }
        public String Suit { get; set; }
        public int Value { get; set; }
        public int PriorityValue { get; set; }
        public String ImagePath { get; set; }

        public Card(int id, String suit, int value,int priorityValue, String imagePath)
        {
            Id = id;
            Suit = suit;
            Value = value;
            PriorityValue = priorityValue;
            ImagePath = imagePath;
        }
    }
}