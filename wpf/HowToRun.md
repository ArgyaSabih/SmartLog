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

## Instalasi PostgreSQL

### Opsi 1: Installer Manual (Recommended untuk Pemula)

1. **Download PostgreSQL Installer**

   - Kunjungi: https://www.postgresql.org/download/windows/
   - Download installer sesuai arsitektur Windows Anda (x86/x64)

2. **Instalasi**

   - Jalankan installer yang sudah didownload
   - Ikuti wizard instalasi:
     - Pilih komponen: PostgreSQL Server, pgAdmin 4, Command Line Tools
     - Pilih direktori instalasi (default: `C:\Program Files\PostgreSQL\16\`)
     - **Penting**: Catat password untuk user `postgres` yang Anda buat
     - Port default: `5432`
     - Locale: `Indonesian, Indonesia` atau `English`
   - Klik "Finish" setelah selesai

3. **Verifikasi Instalasi**
   ```bash
   # Buka Command Prompt atau PowerShell
   psql --version
   ```
   Output yang diharapkan: `psql (PostgreSQL) 16.x`

## Setup Database

### Langkah 1: Login ke PostgreSQL

```bash
# Buka Command Prompt atau PowerShell
# Login sebagai superuser postgres
psql -U postgres -h localhost -p 5432

# Masukkan password yang dibuat saat instalasi
```

### Langkah 2: Buat Database dan User Baru

Jalankan perintah SQL berikut di prompt `psql`:

```sql
-- Buat database smartlog
CREATE DATABASE smartlog;

-- Buat user khusus untuk aplikasi
CREATE USER smartlog_user WITH PASSWORD 'admin';

-- Berikan semua privilege ke user
GRANT ALL PRIVILEGES ON DATABASE smartlog TO smartlog_user;

-- Untuk PostgreSQL 15+, berikan akses ke schema public
\c smartlog
GRANT ALL ON SCHEMA public TO smartlog_user;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO smartlog_user;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO smartlog_user;

-- Keluar dari psql
\q
```

### Langkah 3: Verifikasi Koneksi Database

```bash
# Test koneksi dengan user baru
psql -U smartlog_user -h localhost -p 5432 -d smartlog -W

# Masukkan password: SmartLog2024!
# Jika berhasil, Anda akan masuk ke prompt psql

# Keluar
\q

```

### Langkah 2: Restore NuGet Packages

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
    "DefaultConnection": "Host=localhost;Port=5432;Database=smartlog;Username=smartlog_user;Password=admin"
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

### Langkah 2: Amankan Credentials (Production)

Untuk production, gunakan **User Secrets** atau **Environment Variables**:

```bash
# Set connection string via environment variable
setx CONNECTIONSTRINGS__DEFAULTCONNECTION "Host=localhost;Port=5432;Database=smartlog;Username=smartlog_user;Password=admin"
```

---

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

### Langkah 4: Verifikasi Tabel di Database

```bash
# Login ke PostgreSQL
psql -U smartlog_user -h localhost -d smartlog -W

# List semua tabel
\dt

# Output yang diharapkan:
#  Schema |       Name        | Type  |     Owner
# --------+-------------------+-------+---------------
#  public | __EFMigrationsHistory | table | smartlog_user
#  public | admins            | table | smartlog_user
#  public | customers         | table | smartlog_user
#  public | kapals            | table | smartlog_user
#  public | pengirimans       | table | smartlog_user

# Lihat struktur tabel
\d admins

# Keluar
\q
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
