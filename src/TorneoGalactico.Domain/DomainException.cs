namespace TorneoGalactico.Domain;

/// <summary>Se lanza cuando se viola una regla de negocio.</summary>
public class DomainException(string message) : Exception(message);
