using Microsoft.EntityFrameworkCore;
using wpf.Data;
using wpf.models;

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
        _db.Kapals.Update(kapal);
        await _db.SaveChangesAsync();
        return kapal;
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
        _db.Pengirimans.Add(pengiriman);
        await _db.SaveChangesAsync();
        return pengiriman;
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
