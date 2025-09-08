
using System;

namespace wpf.models;

public class Pengiriman
{
    public long PengirimanId { get; set; }

    public DateTime TanggalMulai { get; set; }
    public DateTime TanggalSelesaiEstimasi { get; set; }

    public string? StatusPengiriman { get; set; }
    public string? LokasiSaatIni { get; set; }

    public long CustomerId { get; set; }
    public long KapalId { get; set; }

    public string? NamaBarang { get; set; }
    public decimal BeratKg { get; set; }
    public string? DeskripsiBarang { get; set; }

    public void UpdateStatus(string newStatus)
    {
        StatusPengiriman = newStatus;
        Console.WriteLine($"Status pengiriman {PengirimanId} diperbarui menjadi: {StatusPengiriman}");
    }

    public void CetakLaporan()
    {
        Console.WriteLine("=== Laporan Pengiriman ===");
        Console.WriteLine($"ID            : {PengirimanId}");
        Console.WriteLine($"Tanggal Mulai : {TanggalMulai}");
        Console.WriteLine($"Estimasi Selesai : {TanggalSelesaiEstimasi}");
        Console.WriteLine($"Status        : {StatusPengiriman}");
        Console.WriteLine($"Lokasi Saat Ini : {LokasiSaatIni}");
        Console.WriteLine($"Customer ID   : {CustomerId}");
        Console.WriteLine($"Kapal ID      : {KapalId}");
        Console.WriteLine($"Nama Barang   : {NamaBarang}");
        Console.WriteLine($"Berat (kg)    : {BeratKg}");
        Console.WriteLine($"Deskripsi     : {DeskripsiBarang}");
    }
}