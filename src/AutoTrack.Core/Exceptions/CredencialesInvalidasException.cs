namespace AutoTrack.Core.Exceptions;

public class CredencialesInvalidasException : Exception
{
    public CredencialesInvalidasException(string mensaje) : base(mensaje)
    {
    }
}
