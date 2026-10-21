namespace TorneoGalactico.Domain
{
    public class Species
    {
        public const int MaxNameLength = 100;
        public const int MaxAbilityLength = 500;

        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string NormalizedName { get; private set; } = string.Empty;
        public int PowerLevel { get; private set; }
        public string SpecialAbility { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; private set; }

        // Constructor privado: lo usa EF Core; desde fuera solo se crea con Create()
        private Species() { }

        public static Species Create(string name, int powerLevel, string specialAbility)
        {
            var cleanName = name?.Trim() ?? string.Empty;
            var cleanAbility = specialAbility?.Trim() ?? string.Empty;

            if (cleanName.Length == 0)
                throw new DomainException("El nombre es obligatorio.");
            if (cleanName.Length > MaxNameLength)
                throw new DomainException($"El nombre no puede superar {MaxNameLength} caracteres.");
            if (powerLevel < 0)
                throw new DomainException("El nivel de poder no puede ser negativo.");
            if (cleanAbility.Length == 0)
                throw new DomainException("La habilidad especial es obligatoria.");
            if (cleanAbility.Length > MaxAbilityLength)
                throw new DomainException($"La habilidad especial no puede superar {MaxAbilityLength} caracteres.");

            return new Species
            {
                Id = Guid.NewGuid(),
                Name = cleanName,
                NormalizedName = Normalize(cleanName),
                PowerLevel = powerLevel,
                SpecialAbility = cleanAbility,
                CreatedAt = DateTime.UtcNow
            };
        }

        /// <summary>Forma canónica del nombre para comparar unicidad sin distinguir mayúsculas.</summary>
        public static string Normalize(string name) => name.Trim().ToUpperInvariant();
    }
}
