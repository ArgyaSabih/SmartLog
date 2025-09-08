using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using wpf.libs;
using static wpf.libs.Utils;

namespace wpf.models;


public class Admin
{
    public long AdminId { get; set; }

    public string? Email { get; set; }
    public string? PasswordHash { get; set; }

    public string? Role { get; set; }
    public string? Nama { get; set; }
    public bool Login(string email, string password)
    {
        if (this.Email == email && this.PasswordHash == Utils.HashPassword(password))
        {
            return true;
        }
        return false;
    }

    public void register()
    {
        // Implement registration logic here
        Console.WriteLine($"Admin {Nama} registered with email {Email}");
    }
}