using System.Security.Cryptography;
using System.Text;

namespace wpf.libs;

public class Utils
{
    public static string HashPassword(string password)
    {
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        StringBuilder builder = new();
        foreach (var b in bytes)
        {
            builder.Append(b.ToString("x2"));
        }
        return builder.ToString();
    }
}