namespace TorneoGalactico.Application.Common;

/// <summary>El recurso pedido no existe (404).</summary>
public class NotFoundException(string message) : Exception(message);

/// <summary>La operación choca con el estado actual, por ejemplo un nombre duplicado (409).</summary>
public class ConflictException(string message) : Exception(message);