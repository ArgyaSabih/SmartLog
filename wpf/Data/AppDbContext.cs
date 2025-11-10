using Microsoft.EntityFrameworkCore;
using wpf.models;

namespace wpf.Data;

/// <summary>
/// Database Context untuk aplikasi SmartLog
/// Menangani koneksi dan operasi database dengan PostgreSQL
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // DbSet untuk setiap model
    public DbSet<Admin> Admins { get; set; } = null!;
    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<Kapal> Kapals { get; set; } = null!;
    public DbSet<Pengiriman> Pengirimans { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Konfigurasi tabel Admin
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.ToTable("admins");
            entity.HasKey(e => e.AdminId);
            entity.Property(e => e.AdminId).HasColumnName("admin_id").ValueGeneratedOnAdd();
            entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(255);
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash").HasMaxLength(255);
            entity.Property(e => e.Nama).HasColumnName("nama").HasMaxLength(255);
            entity.Property(e => e.Role).HasColumnName("role").HasMaxLength(100);
            entity.Property(e => e.IsVerified).HasColumnName("is_verified");
            
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Konfigurasi tabel Customer
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("customers");
            entity.HasKey(e => e.CustomerId);
            entity.Property(e => e.CustomerId).HasColumnName("customer_id").ValueGeneratedOnAdd();
            entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(255);
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash").HasMaxLength(255);
            entity.Property(e => e.Nama).HasColumnName("nama").HasMaxLength(255);
            entity.Property(e => e.Alamat).HasColumnName("alamat").HasMaxLength(500);
            entity.Property(e => e.NomorTelepon).HasColumnName("nomor_telepon").HasMaxLength(50);
            
            entity.HasIndex(e => e.Email).IsUnique();
            
            // Ignore property DaftarPengiriman karena akan dihandle dengan relasi
            entity.Ignore(e => e.DaftarPengiriman);
        });

        // Konfigurasi tabel Kapal
        modelBuilder.Entity<Kapal>(entity =>
        {
            entity.ToTable("kapals");
            entity.HasKey(e => e.KapalId);
            entity.Property(e => e.KapalId).HasColumnName("kapal_id").ValueGeneratedOnAdd();
            entity.Property(e => e.NamaKapal).HasColumnName("nama_kapal").HasMaxLength(255);
            entity.Property(e => e.NomorRegistrasi).HasColumnName("nomor_registrasi");
            entity.Property(e => e.KodeRegistrasi).HasColumnName("kode_registrasi").HasMaxLength(32);
            entity.Property(e => e.KapasitasTon).HasColumnName("kapasitas_ton").HasPrecision(18, 2);
            entity.Property(e => e.StatusVerifikasi).HasColumnName("status_verifikasi").HasMaxLength(50);
                entity.Property(e => e.LokasiSekarang).HasColumnName("lokasi_sekarang").HasMaxLength(255);
                entity.Property(e => e.LokasiTujuan).HasColumnName("lokasi_tujuan").HasMaxLength(255);
            
            // Ensure uniqueness for both numeric and string registration (kode_registrasi)
            entity.HasIndex(e => e.NomorRegistrasi).IsUnique();
            entity.HasIndex(e => e.KodeRegistrasi).IsUnique();
        });

        // Konfigurasi tabel Pengiriman
        modelBuilder.Entity<Pengiriman>(entity =>
        {
            entity.ToTable("pengirimans");
            entity.HasKey(e => e.PengirimanId);
            entity.Property(e => e.PengirimanId).HasColumnName("pengiriman_id").ValueGeneratedOnAdd();
            entity.Property(e => e.TanggalMulai).HasColumnName("tanggal_mulai");
            entity.Property(e => e.TanggalSelesaiEstimasi).HasColumnName("tanggal_selesai_estimasi");
            entity.Property(e => e.StatusPengiriman).HasColumnName("status_pengiriman").HasMaxLength(100);
            entity.Property(e => e.LokasiSaatIni).HasColumnName("lokasi_saat_ini").HasMaxLength(255);
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.KapalId).HasColumnName("kapal_id");
            entity.Property(e => e.NamaBarang).HasColumnName("nama_barang").HasMaxLength(255);
            entity.Property(e => e.BeratKg).HasColumnName("berat_kg").HasPrecision(18, 2);
            entity.Property(e => e.DeskripsiBarang).HasColumnName("deskripsi_barang").HasMaxLength(1000);
        });

        // Seed data (untuk admin)
        // Use anonymous object in HasData so EF can set the column values without invoking protected setters
        modelBuilder.Entity<Admin>().HasData(new
        {
            AdminId = 1L,
            Email = "admin@gmail.com",
            Nama = "Admin 1",
            Role = "SuperAdmin",
            // Password: admin123 (SHA256 hashed)
            PasswordHash = "240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9",
            IsVerified = true
        });
    }
}
