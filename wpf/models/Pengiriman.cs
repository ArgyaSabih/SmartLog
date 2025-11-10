
using System;

namespace wpf.models;

/// <summary>
/// ENCAPSULATION: Private fields dengan property validation
/// POLYMORPHISM: Virtual methods yang bisa di-override untuk shipping types berbeda
/// </summary>
public class Pengiriman
{
    // ENCAPSULATION: Private fields
    private string? _statusPengiriman;
    private string? _lokasiSaatIni;
    private decimal _beratKg;
    private DateTime _lastUpdated;

    public long PengirimanId { get; set; }
    public DateTime TanggalMulai { get; set; }
    public DateTime TanggalSelesaiEstimasi { get; set; }

    // ENCAPSULATION: Public property dengan private setter
    public string? StatusPengiriman
    {
        get => _statusPengiriman;
        private set
        {
            _statusPengiriman = value;
            _lastUpdated = DateTime.UtcNow;
        }
    }

    public string? LokasiSaatIni
    {
        get => _lokasiSaatIni;
        private set => _lokasiSaatIni = value;
    }

    public long CustomerId { get; set; }
    public long KapalId { get; set; }
    public string? NamaBarang { get; set; }

    // ENCAPSULATION: Property dengan validation
    public decimal BeratKg
    {
        get => _beratKg;
        set
        {
            if (value <= 0)
                throw new ArgumentException("Berat harus lebih dari 0 kg");
            _beratKg = value;
        }
    }

    public string? DeskripsiBarang { get; set; }

    public Pengiriman()
    {
        _statusPengiriman = "Pending";
        // Use UTC for database storage (Postgres timestamptz expects UTC)
        TanggalMulai = DateTime.UtcNow;
        _lastUpdated = DateTime.UtcNow;
    }

    // ENCAPSULATION: Public method dengan validation untuk update status
    public void UpdateStatus(string newStatus)
    {
        if (string.IsNullOrWhiteSpace(newStatus))
            throw new ArgumentException("Status tidak boleh kosong");

        string[] validStatuses = { "Pending", "Diproses", "Dalam Perjalanan", "Tiba di Pelabuhan", "Selesai", "Dibatalkan", "Proses" };

        if (!Array.Exists(validStatuses, s => s.Equals(newStatus, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ArgumentException($"Status '{newStatus}' tidak valid");
        }

        StatusPengiriman = newStatus;
        Console.WriteLine($"Status pengiriman {PengirimanId} diperbarui menjadi: {StatusPengiriman}");
    }

    // ENCAPSULATION: Public method untuk update lokasi dengan validation
    public void UpdateLocation(string newLocation)
    {
        if (string.IsNullOrWhiteSpace(newLocation))
            throw new ArgumentException("Lokasi tidak boleh kosong");

        LokasiSaatIni = newLocation;
        _lastUpdated = DateTime.UtcNow;
        Console.WriteLine($"Lokasi pengiriman {PengirimanId} diperbarui ke: {newLocation}");
    }

    // POLYMORPHISM: Virtual method yang bisa di-override
    public virtual decimal CalculateCost()
    {
        // Biaya per kg untuk pengiriman standar
        decimal biayaPerKg = 10000;

        // Hitung durasi pengiriman
        int hariPengiriman = (TanggalSelesaiEstimasi - TanggalMulai).Days;

        // Biaya tambahan berdasarkan durasi
        decimal biayaTambahan = hariPengiriman * 5000;

        decimal totalCost = (BeratKg * biayaPerKg) + biayaTambahan;

        return totalCost;
    }

    // POLYMORPHISM: Virtual method untuk tipe pengiriman
    public virtual string GetShipmentType()
    {
        return "Pengiriman Laut Standar";
    }

    // Method untuk tracking info
    public string GetTrackingInfo()
    {
        return $"Pengiriman #{PengirimanId} - Status: {StatusPengiriman}, Lokasi: {LokasiSaatIni}, " +
               $"Estimasi Tiba: {TanggalSelesaiEstimasi:dd/MM/yyyy}";
    }

    public DateTime GetLastUpdated()
    {
        return _lastUpdated;
    }

    public void CetakLaporan()
    {
        Console.WriteLine("╔════════════════════════════════════════╗");
        Console.WriteLine("║      LAPORAN PENGIRIMAN DETAIL         ║");
        Console.WriteLine("╠════════════════════════════════════════╣");
        Console.WriteLine($"║ ID Pengiriman    : {PengirimanId,-20}║");
        Console.WriteLine($"║ Tanggal Mulai    : {TanggalMulai:dd/MM/yyyy HH:mm,-20}║");
        Console.WriteLine($"║ Estimasi Selesai : {TanggalSelesaiEstimasi:dd/MM/yyyy,-20}║");
        Console.WriteLine($"║ Status           : {StatusPengiriman,-20}║");
        Console.WriteLine($"║ Lokasi Saat Ini  : {LokasiSaatIni,-20}║");
        Console.WriteLine($"║ Customer ID      : {CustomerId,-20}║");
        Console.WriteLine($"║ Kapal ID         : {KapalId,-20}║");
        Console.WriteLine($"║ Nama Barang      : {NamaBarang,-20}║");
        Console.WriteLine($"║ Berat (kg)       : {BeratKg,-20}║");
        Console.WriteLine($"║ Tipe Pengiriman  : {GetShipmentType(),-20}║");
        Console.WriteLine($"║ Biaya Total      : Rp {CalculateCost(),-17:N0}║");
        Console.WriteLine($"║ Terakhir Update  : {_lastUpdated:dd/MM/yyyy HH:mm,-20}║");
        Console.WriteLine("╚════════════════════════════════════════╝");
    }

    public string GetEstimasiWaktuTiba()
    {
        // Compare using UTC to match stored timestamps
        TimeSpan sisaWaktu = TanggalSelesaiEstimasi - DateTime.UtcNow;

        if (sisaWaktu.TotalDays < 0)
            return "Pengiriman terlambat!";
        else if (sisaWaktu.TotalHours < 24)
            return $"Tiba dalam {sisaWaktu.Hours} jam";
        else
            return $"Tiba dalam {sisaWaktu.Days} hari";
    }
}