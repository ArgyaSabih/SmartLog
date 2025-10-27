using System;
using System.Collections.Generic;
using wpf.libs;

namespace wpf.models;

/// <summary>
/// INHERITANCE & POLYMORPHISM: Base class untuk semua tipe User
/// ENCAPSULATION: Protected fields dengan property validation
/// </summary>
public abstract class User
{
    // ENCAPSULATION: Private fields
    private string? _email;
    private string? _passwordHash;

    public string? Email
    {
        get => _email;
        set
        {
            // ENCAPSULATION: Validation
            if (!string.IsNullOrWhiteSpace(value) && !value.Contains("@"))
                throw new ArgumentException("Format email tidak valid");
            _email = value;
        }
    }

    public string? PasswordHash
    {
        get => _passwordHash;
        protected set => _passwordHash = value;
    }

    public string? Nama { get; set; }

    // POLYMORPHISM: Virtual method yang bisa di-override
    public virtual bool Login(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return false;

        return this.Email == email && this.PasswordHash == Utils.HashPassword(password);
    }

    // POLYMORPHISM: Virtual method dengan default implementation
    public virtual string GetUserInfo()
    {
        return $"User: {Nama}, Email: {Email}";
    }

    // ENCAPSULATION: Protected method untuk set password
    protected void SetPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password tidak boleh kosong");
        this.PasswordHash = Utils.HashPassword(password);
    }
}

/// <summary>
/// INHERITANCE: Admin mewarisi dari User base class
/// POLYMORPHISM: Override methods dari parent class
/// </summary>
public class Admin : User
{
    public long AdminId { get; set; }
    public string? Role { get; set; }
    public bool IsVerified { get; set; }

    public Admin()
    {
        Role = "Admin";
        IsVerified = false;
    }

    // POLYMORPHISM: Override Login dengan behavior khusus Admin
    public override bool Login(string email, string password)
    {
        // Admin harus verified untuk bisa login
        if (!IsVerified)
        {
            Console.WriteLine($"Admin {Nama} belum diverifikasi. Login ditolak.");
            return false;
        }

        bool loginSuccess = base.Login(email, password);
        if (loginSuccess)
        {
            Console.WriteLine($"Admin {Nama} ({Role}) berhasil login.");
        }
        return loginSuccess;
    }

    // POLYMORPHISM: Override GetUserInfo
    public override string GetUserInfo()
    {
        return $"{base.GetUserInfo()}, Role: {Role}, Verified: {IsVerified}";
    }

    // Method untuk registrasi Admin
    public void Register(string email, string password, string nama, string role = "Admin")
    {
        this.Email = email;
        this.SetPassword(password);
        this.Nama = nama;
        this.Role = role;
        this.IsVerified = false;
        Console.WriteLine($"Admin {Nama} registered with email {Email}. Waiting verification.");
    }

    // Method khusus Admin
    public void VerifyAdmin(string verifierName)
    {
        IsVerified = true;
        Console.WriteLine($"Admin {Nama} telah diverifikasi oleh {verifierName}");
    }

    public List<Customer> GetAllCustomers()
    {
        Console.WriteLine($"Admin {Nama} mengakses daftar semua customer.");
        return new List<Customer>();
    }

    public void ApproveShipment(long shipmentId)
    {
        Console.WriteLine($"Admin {Nama} menyetujui pengiriman ID: {shipmentId}");
    }
}