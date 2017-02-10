using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalRChat
{
    public class DeckOfCard
    {
        public List<Card> Cards = new List<Card>();

        public DeckOfCard()
        {
            String suits = "cdhs";
            String faces = "j9atkq87";
            String suit = "";
            int value = 0;
            int priority = 0;
            String imagePath = "";
            Card card;
            var indexer = 0;
           
            foreach (var suitType in suits)
            {

                foreach (var faceType in faces)
                {
                   
                    imagePath = "Images/"  + faceType + suitType + ".gif";

                    if (suitType == 'c')
                    {
                        suit = "clubs";
                    }

                    else if (suitType == 'd')
                    {
                        suit = "diamond";
                    }

                    else if (suitType == 'h')
                    {
                        suit = "hearts";
                    }

                    else
                    {
                        suit = "spade";
                    }

                    if (faceType == 'j')
                    {
                        
                        value = 3;
                        priority = 8;
                    }

                    else if (faceType == '9')
                    {
                        value = 2;
                        priority = 7;
                    }

                    else if (faceType == 'a')
                    {
                        value =1;
                        priority =6;
                    }

                    else if (faceType == 't')
                    {
                        value = 1;
                        priority = 5;
                    }

                    else if (faceType == 'k')
                    {
                        value = 0;
                        priority = 4;
                    }

                    else if (faceType == 'q')
                    {
                        value = 0;
                        priority = 3;
                    }

                    else if (faceType == '8')
                    {
                        value = 0;
                        priority = 2;
                    }

                    else 
                    {
                        value = 0;
                        priority = 1;
                    }

                    card = new Card(++indexer,suit,value,priority,imagePath );
                    Cards.Add(card);
                }
            }
        }

        public void Shuffle()
        {
            Random random = new Random();

            Cards = Cards.OrderBy(c => random.Next()).ToList();
        }

        public List<Card> GetCardList()
        {
            return Cards;
        }

        public Card GetCardById(int id)
        {
            foreach (var card in Cards)
            {
                if (card.Id == id)
                {
                    return card;
                }
            }

            return null;
        }
    }
}