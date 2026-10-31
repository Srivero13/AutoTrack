using System.Security.Cryptography;

namespace AutoTrack.Services.Seguridad;

public static class ProtectorClave
{
    private const int Iteraciones = 210000;

    public static string CrearHash(string clave)
    {
        var sal = RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            clave, sal, Iteraciones, HashAlgorithmName.SHA256, 32);

        return $"pbkdf2-sha256:{Iteraciones}:{Convert.ToBase64String(sal)}:{Convert.ToBase64String(hash)}";
    }

    public static bool Verificar(string clave, string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            return false;

        var partes = valor.Split(':');

        if (partes.Length != 4 || partes[0] != "pbkdf2-sha256" ||
            !int.TryParse(partes[1], out var iteraciones) ||
            iteraciones < 100000 || iteraciones > 1000000)
            return false;

        try
        {
            var sal = Convert.FromBase64String(partes[2]);
            var guardado = Convert.FromBase64String(partes[3]);

            if (sal.Length != 16 || guardado.Length != 32)
                return false;

            var calculado = Rfc2898DeriveBytes.Pbkdf2(
                clave, sal, iteraciones, HashAlgorithmName.SHA256, 32);

            return CryptographicOperations.FixedTimeEquals(guardado, calculado);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
