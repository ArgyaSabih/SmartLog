using Microsoft.EntityFrameworkCore;
using wpf.Data;
using wpf.models;
using Npgsql;

namespace wpf.services;

/// <summary>
/// Service untuk operasi CRUD dengan PostgreSQL Database
/// Menggunakan Entity Framework Core untuk akses data
/// </summary>
public class PostgresService
{
    private readonly AppDbContext _db;

    public PostgresService(AppDbContext db)
    {
        _db = db;
    }

    // Extension: robust add with retry on unique constraint for kode_registrasi
    /// <summary>
    /// Tries to add a Kapal, generating/supplying KodeRegistrasi each attempt via generator function.
    /// If the DB rejects due to unique constraint on kode_registrasi, it retries up to maxAttempts.
    /// </summary>
    public async Task<Kapal> AddKapalWithRetriesAsync(Kapal kapal, Func<string> kodeGenerator, int maxAttempts = 5)
    {
        if (kapal == null) throw new ArgumentNullException(nameof(kapal));
        if (kodeGenerator == null) throw new ArgumentNullException(nameof(kodeGenerator));

        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            kapal.KodeRegistrasi = kodeGenerator();
            try
            {
                _db.Kapals.Add(kapal);
                await _db.SaveChangesAsync();
                return kapal;
            }
            catch (DbUpdateException dbEx)
            {
                // Detect Postgres unique violation (SQLSTATE 23505)
                var pgEx = dbEx.InnerException as PostgresException;
                if (pgEx != null && pgEx.SqlState == "23505")
                {
                    // Likely unique constraint violation. If last attempt, rethrow.
                    // Detach the entity so we can retry with a fresh one.
                    try
                    {
                        var entry = _db.Entry(kapal);
                        if (entry != null)
                        {
                            entry.State = EntityState.Detached;
                        }
                    }
                    catch { }

                    if (attempt == maxAttempts)
                    {
                        // rethrow original exception for caller to handle
                        throw;
                    }

                    // otherwise continue to next attempt with new kode
                    kapal = new Kapal
                    {
                        NamaKapal = kapal.NamaKapal,
                        KapasitasTon = kapal.KapasitasTon,
                        StatusVerifikasi = kapal.StatusVerifikasi,
                        LokasiTujuan = kapal.LokasiTujuan,
                        // keep numeric NomorRegistrasi fallback unique-ish
                        NomorRegistrasi = kapal.NomorRegistrasi
                    };
                    continue;
                }

                // Not a unique constraint or unknown provider, rethrow
                throw;
            }
        }

        // Should not reach here
        throw new InvalidOperationException("Failed to add kapal after retries.");
    }

    #region Admin Operations

    /// <summary>
    /// Mendapatkan semua admin dari database
    /// </summary>
    public async Task<List<Admin>> GetAllAdminsAsync()
    {
        return await _db.Admins.ToListAsync();
    }

    /// <summary>
    /// Mendapatkan admin berdasarkan ID
    /// </summary>
    public async Task<Admin?> GetAdminByIdAsync(long adminId)
    {
        return await _db.Admins.FindAsync(adminId);
    }

    /// <summary>
    /// Mendapatkan admin berdasarkan email
    /// </summary>
    public async Task<Admin?> GetAdminByEmailAsync(string email)
    {
        return await _db.Admins.FirstOrDefaultAsync(a => a.Email == email);
    }

    /// <summary>
    /// Menambahkan admin baru ke database
    /// </summary>
    public async Task<Admin> AddAdminAsync(Admin admin)
    {
        _db.Admins.Add(admin);
        await _db.SaveChangesAsync();
        return admin;
    }

    /// <summary>
    /// Update data admin
    /// </summary>
    public async Task<Admin> UpdateAdminAsync(Admin admin)
    {
        _db.Admins.Update(admin);
        await _db.SaveChangesAsync();
        return admin;
    }

    /// <summary>
    /// Menghapus admin dari database
    /// </summary>
    public async Task<bool> DeleteAdminAsync(long adminId)
    {
        var admin = await _db.Admins.FindAsync(adminId);
        if (admin == null) return false;

        _db.Admins.Remove(admin);
        await _db.SaveChangesAsync();
        return true;
    }

    #endregion

    #region Customer Operations

    /// <summary>
    /// Mendapatkan semua customer dari database
    /// </summary>
    public async Task<List<Customer>> GetAllCustomersAsync()
    {
        return await _db.Customers.ToListAsync();
    }

    /// <summary>
    /// Mendapatkan customer berdasarkan ID
    /// </summary>
    public async Task<Customer?> GetCustomerByIdAsync(long customerId)
    {
        return await _db.Customers.FindAsync(customerId);
    }

    /// <summary>
    /// Mendapatkan customer berdasarkan email
    /// </summary>
    public async Task<Customer?> GetCustomerByEmailAsync(string email)
    {
        return await _db.Customers.FirstOrDefaultAsync(c => c.Email == email);
    }

    /// <summary>
    /// Menambahkan customer baru ke database
    /// </summary>
    public async Task<Customer> AddCustomerAsync(Customer customer)
    {
        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();
        return customer;
    }

    /// <summary>
    /// Update data customer
    /// </summary>
    public async Task<Customer> UpdateCustomerAsync(Customer customer)
    {
        _db.Customers.Update(customer);
        await _db.SaveChangesAsync();
        return customer;
    }

    /// <summary>
    /// Menghapus customer dari database
    /// </summary>
    public async Task<bool> DeleteCustomerAsync(long customerId)
    {
        var customer = await _db.Customers.FindAsync(customerId);
        if (customer == null) return false;

        _db.Customers.Remove(customer);
        await _db.SaveChangesAsync();
        return true;
    }

    #endregion

    #region Kapal Operations

    /// <summary>
    /// Mendapatkan semua kapal dari database
    /// </summary>
    public async Task<List<Kapal>> GetAllKapalsAsync()
    {
        return await _db.Kapals.ToListAsync();
    }

    /// <summary>
    /// Mendapatkan kapal berdasarkan ID
    /// </summary>
    public async Task<Kapal?> GetKapalByIdAsync(long kapalId)
    {
        return await _db.Kapals.FindAsync(kapalId);
    }

    /// <summary>
    /// Mendapatkan kapal berdasarkan nomor registrasi (integer representation)
    /// </summary>
    public async Task<Kapal?> GetKapalByNomorRegistrasiAsync(int nomorRegistrasi)
    {
        return await _db.Kapals.FirstOrDefaultAsync(k => k.NomorRegistrasi == nomorRegistrasi);
    }

    /// <summary>
    /// Mendapatkan kapal berdasarkan kode registrasi string (contoh: REG-ABCDE)
    /// </summary>
    public async Task<Kapal?> GetKapalByKodeRegistrasiAsync(string kodeRegistrasi)
    {
        if (string.IsNullOrWhiteSpace(kodeRegistrasi)) return null;
        return await _db.Kapals.FirstOrDefaultAsync(k => k.KodeRegistrasi == kodeRegistrasi);
    }

    /// <summary>
    /// Mendapatkan kapal yang sudah verified
    /// </summary>
    public async Task<List<Kapal>> GetVerifiedKapalsAsync()
    {
        return await _db.Kapals
            .Where(k => k.StatusVerifikasi == "Verified")
            .ToListAsync();
    }

    /// <summary>
    /// Menambahkan kapal baru ke database
    /// </summary>
    public async Task<Kapal> AddKapalAsync(Kapal kapal)
    {
        _db.Kapals.Add(kapal);
        await _db.SaveChangesAsync();
        return kapal;
    }

    /// <summary>
    /// Update data kapal
    /// </summary>
    public async Task<Kapal> UpdateKapalAsync(Kapal kapal)
    {
        if (kapal == null) throw new ArgumentNullException(nameof(kapal));

        // Use a transaction so kapal update and related pengiriman location updates are atomic
        using (var tx = await _db.Database.BeginTransactionAsync())
        {
            // Load existing kapal to detect location change
            var existing = await _db.Kapals.AsNoTracking().FirstOrDefaultAsync(k => k.KapalId == kapal.KapalId);
            string? oldLocation = existing?.LokasiSekarang;

            // Determine if kapal has reached its tujuan
            bool kapalAtTujuan = string.Equals(kapal.LokasiSekarang?.Trim(), kapal.LokasiTujuan?.Trim(), StringComparison.OrdinalIgnoreCase);

            // If kapal reached tujuan, ensure kapal status is updated to 'Verified' in DB
            if (kapalAtTujuan && !string.Equals(kapal.StatusVerifikasi, "Verified", StringComparison.OrdinalIgnoreCase))
            {
                kapal.StatusVerifikasi = "Verified";
            }

            // Update kapal
            _db.Kapals.Update(kapal);
            await _db.SaveChangesAsync();

            // If lokasi changed and new location is non-empty, update pengiriman that are in processed state
            if (!string.IsNullOrWhiteSpace(kapal.LokasiSekarang) && !string.Equals(oldLocation, kapal.LokasiSekarang, StringComparison.Ordinal))
            {
                // Consider these statuses as 'processed' (keep in sync with UI logic)
                var processedStatuses = new[] { "Proses", "Diproses" };

                var toUpdate = await _db.Pengirimans
                    .Where(p => p.KapalId == kapal.KapalId && (p.StatusPengiriman == "Proses" || p.StatusPengiriman == "Diproses"))
                    .ToListAsync();

                // kapalAtTujuan already computed above; reuse that value here

                foreach (var p in toUpdate)
                {
                    try
                    {
                        // kapal.LokasiSekarang is not null/whitespace here because we checked earlier
                        p.UpdateLocation(kapal.LokasiSekarang!);
                        if (kapalAtTujuan)
                        {
                            // mark processed pengiriman as finished when kapal reached its tujuan
                            try { p.UpdateStatus("Selesai"); } catch { }
                        }
                        _db.Pengirimans.Update(p);
                    }
                    catch
                    {
                        // ignore individual update failures to continue bulk update; caller can inspect DB if needed
                    }
                }

                if (toUpdate.Count > 0)
                {
                    await _db.SaveChangesAsync();
                }
            }

            await tx.CommitAsync();
            return kapal;
        }
    }

    /// <summary>
    /// Menghapus kapal dari database
    /// </summary>
    public async Task<bool> DeleteKapalAsync(long kapalId)
    {
        var kapal = await _db.Kapals.FindAsync(kapalId);
        if (kapal == null) return false;

        _db.Kapals.Remove(kapal);
        await _db.SaveChangesAsync();
        return true;
    }

    #endregion

    #region Pengiriman Operations

    /// <summary>
    /// Mendapatkan semua pengiriman dari database
    /// </summary>
    public async Task<List<Pengiriman>> GetAllPengirimansAsync()
    {
        return await _db.Pengirimans.ToListAsync();
    }

    /// <summary>
    /// Mendapatkan pengiriman berdasarkan ID
    /// </summary>
    public async Task<Pengiriman?> GetPengirimanByIdAsync(long pengirimanId)
    {
        return await _db.Pengirimans.FindAsync(pengirimanId);
    }

    /// <summary>
    /// Mendapatkan pengiriman berdasarkan customer ID
    /// </summary>
    public async Task<List<Pengiriman>> GetPengirimansByCustomerIdAsync(long customerId)
    {
        return await _db.Pengirimans
            .Where(p => p.CustomerId == customerId)
            .OrderByDescending(p => p.TanggalMulai)
            .ToListAsync();
    }

    /// <summary>
    /// Mendapatkan pengiriman berdasarkan kapal ID
    /// </summary>
    public async Task<List<Pengiriman>> GetPengirimansByKapalIdAsync(long kapalId)
    {
        return await _db.Pengirimans
            .Where(p => p.KapalId == kapalId)
            .OrderByDescending(p => p.TanggalMulai)
            .ToListAsync();
    }

    /// <summary>
    /// Mendapatkan pengiriman berdasarkan status
    /// </summary>
    public async Task<List<Pengiriman>> GetPengirimansByStatusAsync(string status)
    {
        return await _db.Pengirimans
            .Where(p => p.StatusPengiriman == status)
            .ToListAsync();
    }

    /// <summary>
    /// Menambahkan pengiriman baru ke database
    /// </summary>
    public async Task<Pengiriman> AddPengirimanAsync(Pengiriman pengiriman)
    {
        if (pengiriman == null) throw new ArgumentNullException(nameof(pengiriman));

        // Ensure kapal exists
        if (pengiriman.KapalId <= 0)
            throw new ArgumentException("Pengiriman harus memiliki KapalId yang valid.", nameof(pengiriman));

        // Use a serializable transaction to avoid race conditions when checking capacity
        using (var tx = await _db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable))
        {
            // Reload kapal inside transaction
            var kapal = await _db.Kapals.FindAsync(pengiriman.KapalId);
            if (kapal == null)
                throw new InvalidOperationException($"Kapal dengan ID {pengiriman.KapalId} tidak ditemukan.");

            // Sum existing pengiriman berat (kg) for this kapal
            // Only consider pengirimans that are actively occupying capacity (e.g., "Proses" or "Diproses")
            decimal existingKg = 0m;
            try
            {
                existingKg = await _db.Pengirimans
                    .Where(p => p.KapalId == pengiriman.KapalId && (p.StatusPengiriman == "Proses" || p.StatusPengiriman == "Diproses"))
                    .SumAsync(p => (decimal?)p.BeratKg) ?? 0m;
            }
            catch
            {
                existingKg = 0m;
            }

            decimal kapalCapacityKg = kapal.KapasitasTon * 1000m;

            if (existingKg + pengiriman.BeratKg > kapalCapacityKg)
            {
                throw new InvalidOperationException($"Kapasitas kapal ({kapal.KapasitasTon} Ton) tidak mencukupi. Kapasitas tersisa: {(kapalCapacityKg - existingKg)} Kg.");
            }

            // All good, add pengiriman
            _db.Pengirimans.Add(pengiriman);
            await _db.SaveChangesAsync();

            await tx.CommitAsync();
            return pengiriman;
        }
    }

    /// <summary>
    /// Update data pengiriman
    /// </summary>
    public async Task<Pengiriman> UpdatePengirimanAsync(Pengiriman pengiriman)
    {
        _db.Pengirimans.Update(pengiriman);
        await _db.SaveChangesAsync();
        return pengiriman;
    }

    /// <summary>
    /// Menghapus pengiriman dari database
    /// </summary>
    public async Task<bool> DeletePengirimanAsync(long pengirimanId)
    {
        var pengiriman = await _db.Pengirimans.FindAsync(pengirimanId);
        if (pengiriman == null) return false;

        _db.Pengirimans.Remove(pengiriman);
        await _db.SaveChangesAsync();
        return true;
    }

    #endregion

    #region Authentication Operations

    /// <summary>
    /// Login untuk Admin
    /// </summary>
    public async Task<Admin?> LoginAdminAsync(string email, string password)
    {
        var admin = await GetAdminByEmailAsync(email);
        if (admin != null && admin.Login(email, password))
        {
            return admin;
        }
        return null;
    }

    /// <summary>
    /// Login untuk Customer
    /// </summary>
    public async Task<Customer?> LoginCustomerAsync(string email, string password)
    {
        var customer = await GetCustomerByEmailAsync(email);
        if (customer != null && customer.Login(email, password))
        {
            return customer;
        }
        return null;
    }

    #endregion

    #region Statistics Operations

    /// <summary>
    /// Mendapatkan jumlah total pengiriman
    /// </summary>
    public async Task<int> GetTotalPengirimansCountAsync()
    {
        return await _db.Pengirimans.CountAsync();
    }

    /// <summary>
    /// Mendapatkan jumlah total customer
    /// </summary>
    public async Task<int> GetTotalCustomersCountAsync()
    {
        return await _db.Customers.CountAsync();
    }

    /// <summary>
    /// Mendapatkan jumlah total kapal
    /// </summary>
    public async Task<int> GetTotalKapalsCountAsync()
    {
        return await _db.Kapals.CountAsync();
    }

    /// <summary>
    /// Mendapatkan pengiriman aktif (tidak selesai/dibatalkan)
    /// </summary>
    public async Task<List<Pengiriman>> GetActivePengirimansAsync()
    {
        return await _db.Pengirimans
            .Where(p => p.StatusPengiriman != "Selesai" && p.StatusPengiriman != "Dibatalkan")
            .ToListAsync();
    }

    #endregion
}
