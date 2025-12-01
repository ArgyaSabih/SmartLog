using System;
using System.Security.Cryptography;
using System.Text;

namespace wpf.libs;

/// <summary>
/// ENCAPSULATION: Static utility class dengan helper methods
/// </summary>
public class Utils
{
    // ENCAPSULATION: Public static method untuk hash password
    public static string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password tidak boleh kosong");

        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        StringBuilder builder = new();
        foreach (var b in bytes)
        {
            builder.Append(b.ToString("x2"));
        }
        return builder.ToString();
    }

    // Method tambahan untuk validasi email
    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        return email.Contains("@") && email.Contains(".");
    }

    // Method untuk generate ID unik
    public static long GenerateUniqueId()
    {
        return DateTime.Now.Ticks;
    }

    // Method untuk format currency
    public static string FormatCurrency(decimal amount)
    {
        return $"Rp {amount:N0}";
    }

    /// <summary>
    /// DEMONSTRASI OOP: Method untuk test semua konsep
    /// </summary>
    public static void DemonstrasiOOP()
    {
        Console.WriteLine("\n╔═══════════════════════════════════════════════╗");
        Console.WriteLine("║   DEMONSTRASI KONSEP OOP - SmartLog      ║");
        Console.WriteLine("╚═══════════════════════════════════════════════╝\n");

        // 1. INHERITANCE
        Console.WriteLine("1️⃣ INHERITANCE (Pewarisan):");
        Console.WriteLine("   Admin dan Customer mewarisi dari User base class");
        
        var admin = new wpf.models.Admin { AdminId = 1 };
        admin.Register("admin@test.com", "admin123", "Admin Utama", "Super Admin");
        admin.VerifyAdmin("System");
        
        var customer = new wpf.models.Customer { CustomerId = 1 };
        customer.Register("customer@test.com", "pass123", "Customer Satu", "Jl. Test", "08123");
        
        Console.WriteLine($"   ✓ {admin.GetUserInfo()}");
        Console.WriteLine($"   ✓ {customer.GetUserInfo()}\n");

        // 2. ENCAPSULATION
        Console.WriteLine("2️⃣ ENCAPSULATION (Enkapsulasi):");
        Console.WriteLine("   Private fields dengan property validation");
        
        try
        {
            var pengiriman = new wpf.models.Pengiriman
            {
                PengirimanId = 1,
                NamaBarang = "Test Barang",
                BeratKg = 100
            };
            Console.WriteLine($"   ✓ Pengiriman created: {pengiriman.GetTrackingInfo()}");
            
            // Test validation
            pengiriman.UpdateStatus("Dalam Perjalanan");
            Console.WriteLine($"   ✓ Status updated dengan validation\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   ❌ Error: {ex.Message}\n");
        }

        // 3. POLYMORPHISM
        Console.WriteLine("3️⃣ POLYMORPHISM (Polimorfisme):");
        Console.WriteLine("   Method overriding - Login() berbeda untuk Admin vs Customer");
        
        admin.Login("admin@test.com", "admin123");
        customer.Login("customer@test.com", "pass123");
        
        Console.WriteLine($"\n   Method Overloading - Register() dengan parameter berbeda:");
        var customer2 = new wpf.models.Customer();
        customer2.Register("test@email.com", "pass", "Test User"); // 3 parameter
        
        Console.WriteLine("\n╚═══════════════════════════════════════════════╝\n");
    }
}