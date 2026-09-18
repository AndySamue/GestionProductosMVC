namespace GestionProductosMVC.Services;

public class PokeApiException : Exception
{
    public PokeApiException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}
