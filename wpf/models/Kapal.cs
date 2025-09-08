using System.Collections.Generic;

namespace wpf.models;

public class Kapal
{
    public long KapalId { get; set; }
    public string? NamaKapal { get; set; }
    public int NomorRegistrasi { get; set; }
    public decimal KapasitasTon { get; set; }
    public string? StatusVerifikasi { get; set; }

    public void UpdateLokasi()
    {
        // Implementasi logika pembaruan lokasi kapal
        List<string> lokasiBaru = new List<string> {
            "Lokasi A",
            "Lokasi B",
            "Lokasi C"
        };

        int randomIndex = new Random().Next(lokasiBaru.Count);
        string lokasiDipilih = lokasiBaru[randomIndex];
        // Misal kita hanya mencetak lokasi baru
        Console.WriteLine($"Lokasi kapal {NamaKapal} diperbarui ke: {lokasiDipilih}");

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
}