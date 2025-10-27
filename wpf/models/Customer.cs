using System;
using System.Collections.Generic;
using wpf.libs;

namespace wpf.models;

/// <summary>
/// INHERITANCE: Customer mewarisi dari User base class (di Admin.cs)
/// POLYMORPHISM: Override methods dengan behavior khusus Customer
/// ENCAPSULATION: Private list dengan public controlled access
/// </summary>
public class Customer : User
{
    // ENCAPSULATION: Private field dengan public property
    private List<Pengiriman> _daftarPengiriman;

    public long CustomerId { get; set; }
    public string? Alamat { get; set; }
    public string? NomorTelepon { get; set; }

    // ENCAPSULATION: Public getter, data hanya bisa diubah via method
    public List<Pengiriman> DaftarPengiriman
    {
        get => _daftarPengiriman;
        private set => _daftarPengiriman = value;
    }

    public Customer()
    {
        _daftarPengiriman = new List<Pengiriman>();
    }

    // POLYMORPHISM: Method Overloading - nama sama, parameter berbeda
    public void Register(string email, string password, string nama)
    {
        Register(email, password, nama, "", "");
    }

    public void Register(string email, string password, string nama, string alamat, string nomorTelepon)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Email dan password harus diisi");

        this.Email = email;
        this.SetPassword(password); // Gunakan protected method dari parent
        this.Nama = nama;
        this.Alamat = alamat;
        this.NomorTelepon = nomorTelepon;
        Console.WriteLine($"Customer {Nama} berhasil registrasi.");
    }

    // POLYMORPHISM: Override Login dengan behavior khusus Customer
    public override bool Login(string email, string password)
    {
        bool loginSuccess = base.Login(email, password);
        
        if (loginSuccess)
        {
            Console.WriteLine($"Customer {Nama} berhasil login. Total pengiriman: {_daftarPengiriman.Count}");
        }
        else
        {
            Console.WriteLine("Login gagal. Email atau password salah.");
        }
        
        return loginSuccess;
    }

    // POLYMORPHISM: Override GetUserInfo
    public override string GetUserInfo()
    {
        return $"{base.GetUserInfo()}, Alamat: {Alamat}, Telepon: {NomorTelepon}, Total Pengiriman: {_daftarPengiriman.Count}";
    }

    // ENCAPSULATION: Method dengan validation untuk menambah pengiriman
    public void BuatPengiriman(Pengiriman pengirimanBaru)
    {
        if (pengirimanBaru == null)
            throw new ArgumentNullException(nameof(pengirimanBaru), "Pengiriman tidak boleh null");

        pengirimanBaru.CustomerId = CustomerId;
        _daftarPengiriman.Add(pengirimanBaru);
        Console.WriteLine($"Pengiriman baru dengan ID {pengirimanBaru.PengirimanId} dibuat untuk Customer {Nama}");
    }

    public string LacakPengiriman(long pengirimanId)
    {
        var pengiriman = _daftarPengiriman.Find(p => p.PengirimanId == pengirimanId);
        if (pengiriman != null)
        {
            return $"Status: {pengiriman.StatusPengiriman}, Lokasi: {pengiriman.LokasiSaatIni}";
        }
        else
        {
            return "Pengiriman tidak ditemukan.";
        }
    }

    // Method tambahan untuk encapsulation
    public List<Pengiriman> GetRiwayatPengiriman()
    {
        Console.WriteLine($"Customer {Nama} melihat riwayat {_daftarPengiriman.Count} pengiriman.");
        return new List<Pengiriman>(_daftarPengiriman); // Return copy untuk data protection
    }

    public bool BatalkanPengiriman(long pengirimanId)
    {
        var pengiriman = _daftarPengiriman.Find(p => p.PengirimanId == pengirimanId);
        
        if (pengiriman != null && pengiriman.StatusPengiriman != "Selesai")
        {
            pengiriman.UpdateStatus("Dibatalkan");
            Console.WriteLine($"Pengiriman {pengirimanId} berhasil dibatalkan.");
            return true;
        }
        
        Console.WriteLine($"Pengiriman {pengirimanId} tidak dapat dibatalkan.");
        return false;
    }
}