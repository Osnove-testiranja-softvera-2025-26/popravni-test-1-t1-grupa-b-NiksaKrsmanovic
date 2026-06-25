

using OTS2026_PT1_GrupaA.Exceptions;
using OTS2026_PT1_GrupaA.Models;

namespace OTS2026_PT1_GrupaA
{
    public class Game
    {
        public Player Player { get; set; }
        public Map Map { get; set; }


        public Game(Position playerPosition, Position boatPosition)
        {
            Map = new Map();
            Map.InitializeMap();

            int playerX = playerPosition.X;
            int playerY = playerPosition.Y;

            int boatX = boatPosition.X;
            int boatY = boatPosition.Y;

            if (!Map.Fields[boatX, boatY].Zone.Equals(Zone.Land) || !Map.Fields[playerX, playerY].Zone.Equals(Zone.Land))
            {
                throw new InvalidPlayerPositionException("Player and boat must be in the Land zone!");
            }

            Player = new Player(playerPosition);
        }

        public void MovePlayer(Move move)
        {
            Position playerPositionAfterMove = Player.GetPositionAfterMove(move);

            if (ValidatePosition(playerPositionAfterMove))
            {
                Player.MakeMove(move);
            }
        }

        public bool ValidatePosition(Position position)
        {

            if (position == null)
                return false;

            int x = position.X;
            int y = position.Y;

            if (!ValidatePositionInsideValidZones(position))
            {
                return false;
            }
            if (Map.Fields[x, y].Zone.Equals(Zone.Pond))
            {
                return Player.HasBoat;
            }
            else
            {
                return true;
            }
        }

        private bool ValidatePositionInsideValidZones(Position position)
        {

            if (position == null)
                return false;

            int x = position.X;
            int y = position.Y;

            if (x < 0 || x >= Map.MapSize || y < 0 || y >= Map.MapSize)
            {
                return false;
            }
            if (Map.Fields[x, y].Zone.Equals(Zone.Invalid))
            {
                return false;
            }
            return true;
        }

        public void ResolvePlayerPosition(Position position)
        {
            FieldContent fieldContent = Map.Fields[Player.Position.X, Player.Position.Y].Content;

            if (fieldContent.Equals(FieldContent.Bait))
            {
                Player.AmountOfBait++;
            }
            else if (fieldContent.Equals(FieldContent.Fish))
            {
                if (Player.AmountOfBait > 0)
                {
                    Player.CatchFish();
                }
            }
            else if (fieldContent.Equals(FieldContent.Boat))
            {
                Player.HasBoat = true;
            }

            Map.EmptyTileOnPosition(Player.Position);

        }

        public enum Score
        {
            Bad,
            Average,
            Good
        }


        public Score CalculateIncome()
        {
            if (Player.AmountOfFish > 15)
            {
                return Score.Good;
            }
            if (Player.AmountOfBait >= 12 && Player.HasBoat)
            {
                if (Player.AmountOfFish > 8)
                {
                    return Score.Good;
                }
                else
                {
                    return Score.Average;
                }
            }
            return Score.Bad;
        }
    }
}
