using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalRChat
{
    public class Game
    {
        private readonly static List<Player> MappingPlayers= new List<Player>();
        public List<String> ConnectionIdListOfCaller = new List<string>();
        public List<int> IdListOfThrowingCard = new List<int>(); 
        public List<String> ConnectionIdListOfCardThrowingPlayer = new List<string>(); 
        private Team _team1, _team2;
        private String _trumpImage;
        private String _trumpType;
        private bool _isTrumpShow = false;
        private int _nextFirstCaller = 0;
        public int NumberOfPlayingCards;
        private Team _winningTeam;

        public void Add(String name, String connectionId)
        {
            Player player = new Player(name,connectionId);
            MappingPlayers.Add(player);
        }

        public void MakeTeam()
        {
            _team1 = new Team(MappingPlayers[0],MappingPlayers[2]);
            _team2 = new Team(MappingPlayers[1], MappingPlayers[3]);
        }

        public int Count
        {
            get
            {
                return MappingPlayers.Count;
            }
        }

        public String GetConnectionId(int index)
        {
            Player player = MappingPlayers[index];
            return player.ConnectionId;
        }

        public Player GetPlayer(int index)
        {
            return MappingPlayers[index];
        }

        public List<Card> GetFirstPartOfCardsOfAPlayer(int index)
        {
            List<Card> firstSetCards = MappingPlayers[index].cardList.GetRange(0, 4);
            return firstSetCards;
        }

        public List<Card> GetSecondPartOfCardsOfAPlayer(int index)
        {
            List<Card> secondSetCards = MappingPlayers[index].cardList.GetRange(4, 4);
            return secondSetCards;
        } 
        public Player GetPlayerByConnectionId(String connectionId)
        {
            foreach (var player in MappingPlayers)
            {
                if (player.ConnectionId == connectionId)
                {
                    return player;
                }
            }

            return null;
        }

        public void MakeConnectionIdListOfCaller()
        {
            NumberOfPlayingCards = 0;
            int temp = _nextFirstCaller;
            if (ConnectionIdListOfCaller.Count != 0)
            {
                ConnectionIdListOfCaller.Clear();
            }
            for (int i = 0; i < 4; i++)
            {
                ConnectionIdListOfCaller.Add(MappingPlayers[temp].ConnectionId);

                if (temp == 3)
                {
                    temp = 0;
                }
                else
                {
                    temp++;
                }
            }
            if (_nextFirstCaller == 3)
            {
                _nextFirstCaller = 0;
            }
            else
            {
                _nextFirstCaller++;
            }

            foreach (var connectionId in ConnectionIdListOfCaller)
            {
                ConnectionIdListOfCardThrowingPlayer.Add(connectionId);
            }
            
            
        }

        public String GetConnectionIdOfNextCaller()
        {
            String connectionId = ConnectionIdListOfCaller[0];
            ConnectionIdListOfCaller.RemoveAt(0);

            return connectionId;
        }

        public String GetConnectionIdOfHighestBidder()
        {
            int bid = 0;
            String connectionId = "";
            foreach (var player in MappingPlayers)
            {
                if (player.BidPoint > bid)
                {
                    bid = player.BidPoint;
                    connectionId = player.ConnectionId;
                }
            }

            SetBidOfTeam(connectionId,bid);

            return connectionId;
        }

        public void SetBidOfTeam(String connectionIdOfHighestBidder, int bid)
        {
            if (_team1.players[0].ConnectionId == connectionIdOfHighestBidder ||
                _team1.players[1].ConnectionId == connectionIdOfHighestBidder)
            {
                _team1.BidPoint = bid;
                _team1.IsTrumpSet = true;
            }

            else
            {
                _team2.BidPoint = bid;
                _team1.IsTrumpSet = true;
            }
        }

        public void SetTrump(String trump)
        {
            
            if (trump[0] == 's')
            {
                _trumpType = "spade";
                _trumpImage = "Images/2s.gif";
            }
            else if (trump[0] == 'c')
            {
                _trumpType = "clubs";
                _trumpImage = "Images/2c.gif";
            }
            else if (trump[0] == 'd')
            {
                _trumpType = "diamond";
                _trumpImage = "Images/2d.gif";
            }
            else
            {
                _trumpType = "hearts";
                _trumpImage = "Images/2h.gif";
            }
        }

        public String GetTrump()
        {
            _isTrumpShow = true;
            return _trumpType;
        }

        public Player DecideValuableCardThrowingPlayer()
        {
            NumberOfPlayingCards++;
            DeckOfCard deckOfCard = new DeckOfCard();
            Card valuableCard = deckOfCard.GetCardById(IdListOfThrowingCard[0]);
            int boardPoint = valuableCard.Value;
            for (int i = 1; i < 4; i++)
            {
                Card tempCard = deckOfCard.GetCardById(IdListOfThrowingCard[i]);
                boardPoint += tempCard.Value;

                if (_isTrumpShow)
                {
                    if (tempCard.Suit == _trumpType)
                    {
                        if (valuableCard.Suit != _trumpType || tempCard.PriorityValue > valuableCard.PriorityValue)
                        {
                            valuableCard = tempCard;
                        }
                    }
                    else
                    {
                        if (valuableCard.Suit == tempCard.Suit && tempCard.PriorityValue > valuableCard.PriorityValue)
                        {
                            valuableCard = tempCard;
                        }
                    }
                }
                else
                {
                    if (valuableCard.Suit == tempCard.Suit && tempCard.PriorityValue > valuableCard.PriorityValue)
                    {
                        valuableCard = tempCard;
                    }
                }
            }

            Player valuablePlayer = GetPlayerByCard(valuableCard);
            SetBoardPointOfValuableCardThrowingTeam(valuablePlayer,boardPoint);
            IdListOfThrowingCard.Clear();
            MakeConnectionIdListOfCardThrowingPlayer(valuablePlayer);

            if (NumberOfPlayingCards == 8)
            {
                SetGamePoint();
               
            }

            return valuablePlayer;
        }

        public Player GetPlayerByCard(Card card)
        {
            foreach (var player in MappingPlayers)
            {
                foreach (var playerCard in player.cardList)
                {
                    if (card.Id == playerCard.Id)
                    {
                        return player;
                    }
                }
            }

            return null;
        }

        public void SetBoardPointOfValuableCardThrowingTeam(Player valuablePlayer, int boardPoint)
        {
            if (_team1.players[0].ConnectionId == valuablePlayer.ConnectionId ||
               _team1.players[1].ConnectionId == valuablePlayer.ConnectionId)
            {
                _team1.Points += boardPoint;
            }

            else
            {
                _team2.Points += boardPoint;
            }
        }

        public void MakeConnectionIdListOfCardThrowingPlayer(Player player)
        {
            int temp = 0;
            
            foreach (var tempPlayer in MappingPlayers)
            {
       
                if (player.ConnectionId == tempPlayer.ConnectionId)
                {
                    
                    break;
                }

                temp++;
            }

            for (int i = 0; i < 4; i++)
            {
                ConnectionIdListOfCardThrowingPlayer.Add(MappingPlayers[temp].ConnectionId);

                if (temp == 3)
                {
                    temp = 0;
                }
                else
                {
                    temp++;
                }
            }
                          
        }

        public String GetConnectionIdOfNextCardThrowingPlayer()
        {
            String connectionId = ConnectionIdListOfCardThrowingPlayer[0];
            ConnectionIdListOfCardThrowingPlayer.RemoveAt(0);

            return connectionId;
        }

        public void SetGamePoint()
        {
            if (_team1.IsTrumpSet)
            {
                if (_team1.DecideBoardWinner())
                {
                    _team1.GamePoint++;
                }
                else
                {
                    _team1.GamePoint--;
                }
            }
            else
            {
                if (_team2.DecideBoardWinner())
                {
                    _team2.GamePoint++;
                }
                else
                {
                    _team2.GamePoint--;
                }
            }
        }

        public bool IsGameOver()
        {
            if (_team1.GamePoint == 6 || _team1.GamePoint == -6)
            {
                if (_team1.GamePoint == 6)
                {
                    _winningTeam = _team1;
                    return true;
                }
                else
                {
                    _winningTeam = _team2;
                    return true;
                }
            }
            else if (_team2.GamePoint == 6 || _team2.GamePoint == -6)
            {
                if (_team2.GamePoint == 6)
                {
                    _winningTeam = _team2;
                    return true;
                }
                else
                {
                    _winningTeam = _team1;
                    return true;
                }
            }

            return false;

        }

        public Team GetWinningTeam()
        {
            return _winningTeam;
        }


    }
}