using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;

namespace SignalRChat
{
    public class ChatHub : Hub
    {
       
        private readonly static Game Game = new Game(); 
        private readonly static DeckOfCard DeckOfCard = new DeckOfCard();


        public void StartPlay()
        {
            DeckOfCard.Shuffle();
            for (int i = 0; i < 4; i++)
            {
                Player player = Game.GetPlayer(i);
                List<Card> cards = DeckOfCard.GetCardList();
                if (player.cardList.Count != 0)
                {
                    player.cardList.Clear();
                }
                List<Card> subList = cards.GetRange(i*8, 8);
                player.AddCardList(subList);
            }

            for (int i = 0; i < 4; i++)
            {
                String connectionId = Game.GetConnectionId(i);
                List<Card> cards = Game.GetFirstPartOfCardsOfAPlayer(i);
                Clients.Client(connectionId).showCards(cards, connectionId == Context.ConnectionId);
            }
        }

       

        public void Bidding()
        {
                Game.MakeConnectionIdListOfCaller();
                String connectionId = Game.GetConnectionIdOfNextCaller();
               
                Clients.Client(connectionId).bid();                              
              
        }

        public void SetBid(int bid)
        {
            String connectionId = Context.ConnectionId;
            Player player = Game.GetPlayerByConnectionId(connectionId);
            player.BidPoint = bid;
            Clients.All.showBid(bid,player.Name);
            if (Game.ConnectionIdListOfCaller.Count != 0)
            {
                Clients.Client(Game.GetConnectionIdOfNextCaller()).bid();
            }

            else
            {
                Clients.Client(Game.GetConnectionIdOfHighestBidder()).selectTrump();
            }
        }

        public void SetTrump(String trump)
        {
            Game.SetTrump(trump);
            GiveSecondPartOfCard();
            
        }

        public void ShowTrump()
        {
            Clients.All.broadcastTrump(Game.GetTrump());
        }

        public void GiveSecondPartOfCard()
        {
            for (int i = 0; i < 4; i++)
            {
                String connectionId = Game.GetConnectionId(i);
                List<Card> cards = Game.GetSecondPartOfCardsOfAPlayer(i);
                Clients.Client(connectionId).showSecondPartCards(cards, connectionId == Context.ConnectionId);

                
            }
        }

        public void StartCardThrowing()
        {
            Clients.Client(Game.GetConnectionIdOfNextCardThrowingPlayer()).messageToThrowCard();
        }

        public void ShowPlayingCard(int cardId)
        {
            Game.IdListOfThrowingCard.Add(cardId);
            Card card = DeckOfCard.GetCardById(cardId);
            Clients.All.broadcastPlayingCard(card.ImagePath);

            if (Game.ConnectionIdListOfCardThrowingPlayer.Count != 0)
            {
                Clients.Client(Game.GetConnectionIdOfNextCardThrowingPlayer()).messageToThrowCard();
            }

            else
            {
                Clients.All.removeThrowingCard();
                Player player = Game.DecideValuableCardThrowingPlayer();
                Clients.Client(player.ConnectionId).messageOfBoardWinner(Game.NumberOfPlayingCards == 8);
                if (Game.NumberOfPlayingCards == 8)
                {
                    Game.NumberOfPlayingCards = 0;
                    if (Game.IsGameOver())
                    {
                        Team winningTeam = Game.GetWinningTeam();

                        for (int i = 0; i < 4; i++)
                        {
                            Player tempPlayer = Game.GetPlayer(i);

                            if (tempPlayer.ConnectionId == winningTeam.players[0].ConnectionId ||
                                tempPlayer.ConnectionId == winningTeam.players[0].ConnectionId)
                            {
                                Clients.Client(tempPlayer.ConnectionId).winningMessage(true);
                            }

                            else
                            {
                                Clients.Client(tempPlayer.ConnectionId).winningMessage(false);
                            }
                        }
                    }
                    else
                    {
                        StartPlay();
                    }
                }

                
            }
        }

        public void Authentication(string name)
        {
           
                string connectionId = Context.ConnectionId;
                Game.Add(name,connectionId);              

            if (Game.Count == 4)
            {
                Game.MakeTeam();
                Clients.All.authenticationComplete();
            }
        }

    }
}