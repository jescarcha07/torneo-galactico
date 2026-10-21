using System;
using System.Collections.Generic;
using System.Text;
using TorneoGalactico.Domain;

namespace TorneoGalactico.UnitTests.Domain
{
    public class BattleRulesTests
    {
        [Fact]
        public void Higher_power_wins_regardless_of_order()
        {
            var strong = Species.Create("Zorgon", 900, "Rayo cósmico");
            var weak = Species.Create("Abyssal", 100, "Sombra");

            Assert.Same(strong, BattleRules.DetermineWinner(strong, weak));
            Assert.Same(strong, BattleRules.DetermineWinner(weak, strong));
        }

        [Theory]
        [InlineData("Andromedan", "Zorgon", "Andromedan")]
        [InlineData("Kryll", "Kraken", "Kraken")]
        [InlineData("apex", "Zorgon", "apex")] // con comparación sensible a mayúsculas ganaría "Zorgon"
        public void Tie_is_won_by_alphabetically_first_name(string nameA, string nameB, string expectedWinner)
        {
            var a = Species.Create(nameA, 500, "Habilidad A");
            var b = Species.Create(nameB, 500, "Habilidad B");

            Assert.Equal(expectedWinner, BattleRules.DetermineWinner(a, b).Name);
            Assert.Equal(expectedWinner, BattleRules.DetermineWinner(b, a).Name);
        }

        [Fact]
        public void Species_cannot_fight_itself()
        {
            var s = Species.Create("Zorgon", 900, "Rayo cósmico");

            Assert.Throws<DomainException>(() => BattleRules.DetermineWinner(s, s));
        }

        [Fact]
        public void Battle_records_the_winner()
        {
            var a = Species.Create("Zorgon", 900, "Rayo");
            var b = Species.Create("Abyssal", 100, "Sombra");

            var battle = Battle.Fight(a, b);

            Assert.Equal(a.Id, battle.WinnerId);
            Assert.Equal(a.Id, battle.FighterAId);
            Assert.Equal(b.Id, battle.FighterBId);
        }
    }
}
