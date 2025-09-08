using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using wpf.libs;
using static wpf.libs.Utils;

namespace wpf.models;

public class Customer
{
    public long CustomerId { get; set; }
    public string? Email { get; set; }
    public string? PasswordHash { get; set; }
    public string? Nama { get; set; }
    public string? Alamat { get; set; }
    public string? NomorTelepon { get; set; }

    public List<Pengiriman> DaftarPengiriman { get; set; } = new();

    // Register: hash password dan simpan data customer
    public void Register(string email, string password, string nama, string alamat, string nomorTelepon)
    {
        Email = email;
        PasswordHash = HashPassword(password);
        Nama = nama;
        Alamat = alamat;
        NomorTelepon = nomorTelepon;
        // Simpan ke database jika ada
        Console.WriteLine($"Customer {Nama} berhasil registrasi.");
    }

    // Login: verifikasi email dan password
    public bool Login(string email, string password)
    {
        if (this.Email == email && this.PasswordHash == Utils.HashPassword(password))
        {
            return true;
        }
        return false;
    }

    // Membuat pengiriman baru
    public void BuatPengiriman(Pengiriman pengirimanBaru)
    {
        pengirimanBaru.CustomerId = CustomerId;
        DaftarPengiriman.Add(pengirimanBaru);
        Console.WriteLine($"Pengiriman baru dibuat untuk Customer {CustomerId}");
    }

    // Melacak pengiriman berdasarkan ID pengiriman
    public string LacakPengiriman(long pengirimanId)
    {
        var pengiriman = DaftarPengiriman.Find(p => p.PengirimanId == pengirimanId);
        if (pengiriman != null)
        {
            return $"Status: {pengiriman.StatusPengiriman}, Lokasi: {pengiriman.LokasiSaatIni}";
        }
        else
        {
            return "Pengiriman tidak ditemukan.";
        }
    }
}