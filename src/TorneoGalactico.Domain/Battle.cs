namespace TorneoGalactico.Domain
{
    public class Battle
    {
        public Guid Id { get; private set; }
        public Guid FighterAId { get; private set; }
        public Guid FighterBId { get; private set; }
        public Guid WinnerId { get; private set; }
        public DateTime FoughtAt { get; private set; }

        private Battle() { }

        public static Battle Fight(Species fighterA, Species fighterB)
        {
            var winner = BattleRules.DetermineWinner(fighterA, fighterB);

            return new Battle
            {
                Id = Guid.NewGuid(),
                FighterAId = fighterA.Id,
                FighterBId = fighterB.Id,
                WinnerId = winner.Id,
                FoughtAt = DateTime.UtcNow
            };
        }
    }
}
