using System;
using System.Collections.Generic;
using System.Text;
using TorneoGalactico.Domain;

namespace TorneoGalactico.UnitTests.Domain
{
    public class SpeciesTests
    {
        [Fact]
        public void Create_trims_and_normalizes_name()
        {
            var s = Species.Create("  Zorgon  ", 10, "  Rayo  ");

            Assert.Equal("Zorgon", s.Name);
            Assert.Equal("ZORGON", s.NormalizedName);
            Assert.Equal("Rayo", s.SpecialAbility);
        }

        [Theory]
        [InlineData("", 10, "Rayo")]      // nombre vacío
        [InlineData("   ", 10, "Rayo")]   // nombre solo espacios
        [InlineData("Zorgon", -1, "Rayo")] // poder negativo
        [InlineData("Zorgon", 10, "")]    // habilidad vacía
        public void Create_rejects_invalid_data(string name, int power, string ability)
        {
            Assert.Throws<DomainException>(() => Species.Create(name, power, ability));
        }

        [Fact]
        public void Create_rejects_too_long_name()
        {
            var longName = new string('a', Species.MaxNameLength + 1);

            Assert.Throws<DomainException>(() => Species.Create(longName, 10, "Rayo"));
        }
    }
}
