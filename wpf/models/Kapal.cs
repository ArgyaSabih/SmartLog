using System;
using System.Collections.Generic;

namespace wpf.models;

/// <summary>
/// ENCAPSULATION: Private fields dengan property validation
/// POLYMORPHISM: Virtual methods untuk berbagai tipe kapal
/// </summary>
public class Kapal
{
    // ENCAPSULATION: Private fields
    private decimal _kapasitasTon;
    private string? _statusVerifikasi;
    private string? _lokasiSekarang;

    public long KapalId { get; set; }
    public string? NamaKapal { get; set; }
    public int NomorRegistrasi { get; set; }

    // ENCAPSULATION: Property dengan validation
    public decimal KapasitasTon
    {
        get => _kapasitasTon;
        set
        {
            if (value <= 0)
                throw new ArgumentException("Kapasitas harus lebih dari 0 ton");
            _kapasitasTon = value;
        }
    }

    public string? StatusVerifikasi
    {
        get => _statusVerifikasi;
        set
        {
            string[] validStatus = { "Pending", "Verified", "Rejected" };
            if (!string.IsNullOrWhiteSpace(value) && !Array.Exists(validStatus, s => s.Equals(value, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException($"Status verifikasi '{value}' tidak valid");
            _statusVerifikasi = value;
        }
    }

    public string? LokasiSekarang
    {
        get => _lokasiSekarang;
        private set => _lokasiSekarang = value;
    }

    public Kapal()
    {
        StatusVerifikasi = "Pending";
        _lokasiSekarang = "Pelabuhan";
    }

    // ENCAPSULATION: Public method untuk update lokasi dengan validation
    public void UpdateLokasi(string lokasiBaru)
    {
        if (string.IsNullOrWhiteSpace(lokasiBaru))
            throw new ArgumentException("Lokasi tidak boleh kosong");

        LokasiSekarang = lokasiBaru;
        Console.WriteLine($"Lokasi kapal {NamaKapal} diperbarui ke: {lokasiBaru}");
    }

    // Override untuk compatibility dengan kode lama
    public void UpdateLokasi()
    {
        List<string> lokasiBaru = new List<string> {
            "Pelabuhan Tanjung Priok",
            "Laut Jawa",
            "Selat Sunda",
            "Pelabuhan Tanjung Perak"
        };

        int randomIndex = new Random().Next(lokasiBaru.Count);
        UpdateLokasi(lokasiBaru[randomIndex]);
    }

    // POLYMORPHISM: Virtual method untuk kalkulasi biaya operasional
    public virtual decimal CalculateBiayaOperasional(int hariOperasi)
    {
        // Biaya dasar per hari berdasarkan kapasitas
        decimal biayaPerHari = KapasitasTon * 50000;
        return biayaPerHari * hariOperasi;
    }

    // POLYMORPHISM: Virtual method untuk tipe kapal
    public virtual string GetTipeKapal()
    {
        return "Kapal Kargo Standar";
    }

    public List<string> LihatDaftarMuatan()
    {
        List<string> tmp = [
            "Muatan 1",
            "Muatan 2",
            "Muatan 3"
        ];
        return tmp;
    }

    // Method tambahan untuk mendapatkan info kapal
    public string GetKapalInfo()
    {
        return $"Kapal: {NamaKapal}, Tipe: {GetTipeKapal()}, Kapasitas: {KapasitasTon} Ton, Status: {StatusVerifikasi}, Lokasi: {LokasiSekarang}";
    }

    // Method untuk cek ketersediaan kapal
    public bool IsAvailable()
    {
        return StatusVerifikasi == "Verified" && LokasiSekarang == "Pelabuhan";
    }

    // Method untuk verifikasi kapal
    public void Verify()
    {
        if (StatusVerifikasi == "Pending")
        {
            StatusVerifikasi = "Verified";
            Console.WriteLine($"Kapal {NamaKapal} telah diverifikasi.");
        }
        else
        {
            Console.WriteLine($"Kapal {NamaKapal} sudah dalam status {StatusVerifikasi}.");
        }
    }
}