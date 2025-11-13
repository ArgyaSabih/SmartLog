# SmartLog WPF Application - Panduan Setup dan Instalasi

Panduan lengkap untuk setup aplikasi SmartLog WPF dengan PostgreSQL database dari awal.

## Daftar Isi

1. [Prerequisites](#prerequisites)
2. [Instalasi PostgreSQL](#instalasi-postgresql)
3. [Setup Database](#setup-database)
4. [Clone dan Setup Project](#clone-dan-setup-project)
5. [Konfigurasi Connection String](#konfigurasi-connection-string)
6. [Migrasi Database](#migrasi-database)
7. [Menjalankan Aplikasi](#menjalankan-aplikasi)
8. [Troubleshooting](#troubleshooting)

---

## Prerequisites

Pastikan komputer Anda sudah memiliki:

- **Windows 10/11**
- **.NET 9.0 SDK** - Download dari [dotnet.microsoft.com](https://dotnet.microsoft.com/download)
- **Visual Studio Code** dengan ekstensi C# Dev Kit
- **Git** - Download dari [git-scm.com](https://git-scm.com/)
- **PostgreSQL 14+** (akan dijelaskan di bawah)

---

### Langkah 1: Restore NuGet Packages

```bash
# Restore semua dependencies
dotnet restore

# Verifikasi packages ter-install dengan benar
dotnet list package
```

**Packages yang terinstall:**

- `Npgsql.EntityFrameworkCore.PostgreSQL` (9.0.4+)
- `Microsoft.EntityFrameworkCore.Design` (9.0.10+)
- `Microsoft.Extensions.Hosting` (9.0.10+)
- `Microsoft.Extensions.Configuration` (9.0.10+)

---

## Konfigurasi Connection String

### Langkah 1: Edit appsettings.json

Buka file `wpf/appsettings.json` dan sesuaikan connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=aws-1-ap-southeast-1.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.aktxszoizvolmkrgodtb;Password=TipsenYa123;SSL Mode=Require;Trust Server Certificate=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  }
}
```

## Migrasi Database

### Langkah 1: Install EF Core Tools

```bash
# Install dotnet-ef tools globally
dotnet tool install --global dotnet-ef

# Verifikasi instalasi
dotnet ef --version
```

Output yang diharapkan: `Entity Framework Core .NET Command-line Tools 9.0.x`

### Langkah 2: Buat Migration Pertama

```bash
# Buat migration
dotnet ef migrations add InitialCreate

# Folder Migrations/ akan dibuat dengan file migration
```

### Langkah 3: Apply Migration ke Database

```bash
# Apply migration ke database
dotnet ef database update
```

---

## Menjalankan Aplikasi

```bash
# Build project
dotnet build

# Jalankan aplikasi
dotnet run
```

## Troubleshooting

### Error: "No connection could be made..."

**Penyebab**: PostgreSQL server tidak berjalan atau connection string salah.

**Solusi**:

```bash
# Cek status PostgreSQL service
# Windows Services (services.msc) -> cari "postgresql-x64-16"
# Atau via PowerShell
Get-Service -Name *postgresql*

# Start service jika stopped
Start-Service postgresql-x64-16
```

### Error: "password authentication failed"

**Penyebab**: Password salah di connection string.

**Solusi**:

1. Reset password PostgreSQL user:
   ```sql
   psql -U postgres
   ALTER USER smartlog_user WITH PASSWORD 'PasswordBaru123';
   ```
2. Update `appsettings.json` dengan password yang baru

### Error: "relation does not exist"

**Penyebab**: Tabel belum dibuat (migration belum di-apply).

**Solusi**:

```bash
# Re-run migration
dotnet ef database update
```

### Error: "Could not load file or assembly..."

**Penyebab**: Package NuGet tidak ter-restore dengan benar.

**Solusi**:

```bash
# Clean dan rebuild
dotnet clean
dotnet restore
dotnet build
```

### Error: Migration "InitialCreate" sudah ada

**Penyebab**: Migration dengan nama yang sama sudah dibuat sebelumnya.

**Solusi**:

```bash
# Hapus migration terakhir
dotnet ef migrations remove

# Atau buat migration dengan nama berbeda
dotnet ef migrations add InitialCreate_v2
```

### Error: SSL connection error (untuk remote database)

**Solusi**: Tambahkan parameter SSL di connection string:

```
Host=localhost;Port=5432;Database=smartlog;Username=smartlog_user;Password=admin;SSL Mode=Require;Trust Server Certificate=true
```

### Koneksi lambat atau timeout

**Solusi**: Tambahkan timeout di connection string:

```
Host=localhost;Port=5432;Database=smartlog;Username=smartlog_user;Password=admin;Timeout=30;Command Timeout=30
```

---

## Struktur Database

### Tabel: admins

```sql
Column         | Type          | Nullable
---------------+---------------+----------
admin_id       | bigint        | NOT NULL (PK, Identity)
email          | varchar(255)  | NULL (Unique)
password_hash  | varchar(255)  | NULL
nama           | varchar(255)  | NULL
role           | varchar(100)  | NULL
is_verified    | boolean       | NOT NULL
```

### Tabel: customers

```sql
Column         | Type          | Nullable
---------------+---------------+----------
customer_id    | bigint        | NOT NULL (PK, Identity)
email          | varchar(255)  | NULL (Unique)
password_hash  | varchar(255)  | NULL
nama           | varchar(255)  | NULL
alamat         | varchar(500)  | NULL
nomor_telepon  | varchar(50)   | NULL
```

### Tabel: kapals

```sql
Column            | Type           | Nullable
------------------+----------------+----------
kapal_id          | bigint         | NOT NULL (PK, Identity)
nama_kapal        | varchar(255)   | NULL
nomor_registrasi  | integer        | NOT NULL (Unique)
kapasitas_ton     | decimal(18,2)  | NOT NULL
status_verifikasi | varchar(50)    | NULL
lokasi_sekarang   | varchar(255)   | NULL
```

### Tabel: pengirimans

```sql
Column                  | Type           | Nullable
------------------------+----------------+----------
pengiriman_id           | bigint         | NOT NULL (PK, Identity)
tanggal_mulai           | timestamp      | NOT NULL
tanggal_selesai_estimasi| timestamp      | NOT NULL
status_pengiriman       | varchar(100)   | NULL
lokasi_saat_ini         | varchar(255)   | NULL
customer_id             | bigint         | NOT NULL
kapal_id                | bigint         | NOT NULL
nama_barang             | varchar(255)   | NULL
berat_kg                | decimal(18,2)  | NOT NULL
deskripsi_barang        | varchar(1000)  | NULL
```

**© 2025 SmartLog Team - Junior Project Semester 5**
