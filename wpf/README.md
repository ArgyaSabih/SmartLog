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

### Langkah 2: Masukkan file .env

Buka file `wpf/.env` dan masukan .env yang sudah dikirimkan

## Menginstall Aplikasi

Masukan perintah berikut

```sh
dotnet publish ./wpf/wpf.csproj -c Release -r win-x64 -o ./wpf/publish/win-x64-single -p:PublishSingleFile=true -p:SelfContained=true -p:PublishTrimmed=false -p:EnableCompressionInSingleFile=true
```

Maka file akan muncul di direktori ./wpf/publish/win-x64-selfcontained/

Pastikan untuk mengdistribusikan seluruh isi folder win-64-selfcontained ketika mendistribusikan hasil program

**© 2025 SmartLog Team - Junior Project Semester 5**
