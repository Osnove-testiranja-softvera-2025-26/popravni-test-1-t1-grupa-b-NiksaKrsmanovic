

using NUnit.Framework;
using OTS2026_PT1_GrupaA.Exceptions;
using OTS2026_PT1_GrupaA.Models;
using static OTS2026_PT1_GrupaA.Game;

namespace OTS2026_PT1_GrupaA.Test
{
    [TestFixture]
    public class GameTestAlternative
    {
        private Game game;

        [SetUp]
        public void SetUp()
        {
            game = new Game(new Position(12, 14), new Position(6, 6));
        }

        
        [Test]
        public void Game_ValidLandPositions_PlayerIsCreated()
        {
            Assert.That(game.Player, Is.Not.Null);
            Assert.That(game.Player.Position, Is.EqualTo(new Position(12, 14)));
        }

        
        [TestCase(5, 20)]
        [TestCase(29, 0)]
        public void Game_InvalidPlayerPosition_ThrowsException(int x, int y)
        {
            InvalidPlayerPositionException ex = Assert.Throws<InvalidPlayerPositionException>(() =>
            {
                new Game(new Position(x, y), new Position(6, 6));
            });

            Assert.That(ex.Message, Is.EqualTo("Player and boat must be in the Land zone!"));
        }

        
        [Test]
        public void Game_BoatOutsideLand_ThrowsException()
        {
            Assert.Throws<InvalidPlayerPositionException>(() =>
            {
                new Game(new Position(12, 14), new Position(5, 20));
            });
        }

        
        [TestCase(1, 1)]
        [TestCase(24, 12)]
        [TestCase(15, 15)]
        public void ValidatePosition_LandPosition_ReturnsTrue(int x, int y)
        {
            bool result = game.ValidatePosition(new Position(x, y));

            Assert.That(result, Is.True);
        }

        
        [TestCase(-1, 5)]
        [TestCase(30, 5)]
        [TestCase(5, -1)]
        [TestCase(5, 30)]
        public void ValidatePosition_PositionOutsideMap_ReturnsFalse(int x, int y)
        {
            bool result = game.ValidatePosition(new Position(x, y));

            Assert.That(result, Is.False);
        }

       
        [TestCase(9, 13)]
        [TestCase(26, 10)]
        [TestCase(3, 18)]
        public void ValidatePosition_InvalidZone_ReturnsFalse(int x, int y)
        {
            bool result = game.ValidatePosition(new Position(x, y));

            Assert.That(result, Is.False);
        }

        
        [Test]
        public void ValidatePosition_PondWithoutBoat_ReturnsFalse()
        {
            bool result = game.ValidatePosition(new Position(10, 20));

            Assert.That(result, Is.False);
        }

        
        [Test]
        public void ValidatePosition_PondWithBoat_ReturnsTrue()
        {
            game.Player.HasBoat = true;

            bool result = game.ValidatePosition(new Position(10, 20));

            Assert.That(result, Is.True);
        }

        
        [Test]
        public void ValidatePosition_Null_ReturnsFalse()
        {
            bool result = game.ValidatePosition(null);

            Assert.That(result, Is.False);
        }
        
       
        [Test]
        public void MovePlayer_ValidMoveRight_ChangesPosition()
        {
            game.MovePlayer(Move.Right);

            Assert.That(game.Player.Position, Is.EqualTo(new Position(13, 14)));
        }

        
        [Test]
        public void MovePlayer_MoveToInvalidZone_PositionUnchanged()
        {
            Game newGame = new Game(new Position(10, 13), new Position(6, 6));

            newGame.MovePlayer(Move.Left);

            Assert.That(newGame.Player.Position, Is.EqualTo(new Position(10, 13)));
        }

        
        [Test]
        public void MovePlayer_ToPondWithoutBoat_PositionUnchanged()
        {
            Game newGame = new Game(new Position(15, 19), new Position(6, 6));

            newGame.MovePlayer(Move.Down);

            Assert.That(newGame.Player.Position, Is.EqualTo(new Position(15, 19)));
        }

        
        [Test]
        public void MovePlayer_ToPondWithBoat_PositionChanged()
        {
            Game newGame = new Game(new Position(15, 19), new Position(6, 6));
            newGame.Player.HasBoat = true;

            newGame.MovePlayer(Move.Down);

            Assert.That(newGame.Player.Position, Is.EqualTo(new Position(15, 20)));
        }

        
        [Test]
        public void ResolvePlayerPosition_OnBait_IncreasesBait()
        {
            game.Map.Fields[12, 14].Content = FieldContent.Bait;

            game.ResolvePlayerPosition();

            Assert.That(game.Player.AmountOfBait, Is.EqualTo(1));
            Assert.That(game.Map.Fields[12, 14].Content, Is.EqualTo(FieldContent.Empty));
        }

        
        [Test]
        public void ResolvePlayerPosition_OnBoat_PlayerHasBoat()
        {
            game.Map.Fields[12, 14].Content = FieldContent.Boat;

            game.ResolvePlayerPosition();

            Assert.That(game.Player.HasBoat, Is.True);
            Assert.That(game.Map.Fields[12, 14].Content, Is.EqualTo(FieldContent.Empty));
        }

        
        [Test]
        public void ResolvePlayerPosition_OnFishWithBait_CatchesFish()
        {
            game.Player.Position = new Position(10, 20);
            game.Player.HasBoat = true;
            game.Player.AmountOfBait = 2;
            game.Map.Fields[10, 20].Content = FieldContent.Fish;

            game.ResolvePlayerPosition();

            Assert.That(game.Player.AmountOfFish, Is.EqualTo(1));
            Assert.That(game.Player.AmountOfBait, Is.EqualTo(1));
            Assert.That(game.Map.Fields[10, 20].Content, Is.EqualTo(FieldContent.Empty));
        }

        
        [Test]
        public void ResolvePlayerPosition_OnFishWithoutBait_DoesNotCatchFish()
        {
            game.Player.Position = new Position(10, 20);
            game.Player.HasBoat = true;
            game.Player.AmountOfBait = 0;
            game.Map.Fields[10, 20].Content = FieldContent.Fish;

            game.ResolvePlayerPosition();

            Assert.That(game.Player.AmountOfFish, Is.EqualTo(0));
            Assert.That(game.Player.AmountOfBait, Is.EqualTo(0));
        }

        
        [TestCase(16, 0, false, Game.Score.Good)]
        [TestCase(15, 0, false, Game.Score.Bad)]
        [TestCase(9, 12, true, Game.Score.Good)]
        [TestCase(8, 12, true, Game.Score.Average)]
        [TestCase(8, 12, false, Game.Score.Bad)]
        [TestCase(8, 11, true, Game.Score.Bad)]
        public void CalculateIncome_PlayerValues_ReturnsExpectedScore(
        int fish,
        int bait,
        bool hasBoat,
        Game.Score expected)
        {
            game.Player.AmountOfFish = fish;
            game.Player.AmountOfBait = bait;
            game.Player.HasBoat = hasBoat;

            Game.Score result = game.CalculateIncome();

            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
