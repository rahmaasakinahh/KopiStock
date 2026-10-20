
using System.Globalization;
using System.IO;
using System.Reflection;
using Microsoft.Data.Sqlite;
using KopiStock.Models;

namespace KopiStock.Services;

public sealed class StockDatabase
{
    public string FolderPath { get; } =
        Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData),
            "KopiStock");

    public string DatabasePath =>
        Path.Combine(FolderPath, "kopistock.db");

    private SqliteConnection Open()
    {
        var connection = new SqliteConnection(
            new SqliteConnectionStringBuilder
            {
                DataSource = DatabasePath,
                Mode = SqliteOpenMode.ReadWriteCreate,
                Pooling = false
            }.ToString());

        connection.Open();

        using var cmd = connection.CreateCommand();
        cmd.CommandText =
            "PRAGMA foreign_keys = ON; PRAGMA busy_timeout = 5000;";
        cmd.ExecuteNonQuery();

        return connection;
    }

    private static void P(
        SqliteCommand cmd,
        string name,
        object? value)
    {
        cmd.Parameters.AddWithValue(
            name, value ?? DBNull.Value);
    }

    private static string Date(DateTime date) =>
        date.ToString(
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture);

    private static void Required(
        string? value,
        string label,
        int max = 120)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException(
                $"{label} wajib diisi.");

        if (value.Trim().Length > max)
            throw new ArgumentException(
                $"{label} maksimal {max} karakter.");
    }

    public void Initialize()
    {
        Directory.CreateDirectory(FolderPath);

        using var conn = Open();
        using var schema = conn.CreateCommand();

        using var stream =
            Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(
                    "KopiStock.Database.schema.sql")
            ?? throw new InvalidOperationException(
                "Berkas skema SQLite tidak ditemukan.");

        using var reader = new StreamReader(stream);

        schema.CommandText = reader.ReadToEnd();
        schema.ExecuteNonQuery();

        // Menjaga database lama tetap kompatibel.
        // Transaksi lama tanpa jam tidak diberi waktu palsu.
        using var columns = conn.CreateCommand();
        columns.CommandText =
            "PRAGMA table_info(StockMovements);";

        var hasRecordedAt = false;

        using (var cols = columns.ExecuteReader())
        {
            while (cols.Read())
            {
                if (string.Equals(
                    cols.GetString(1),
                    "created_at",
                    StringComparison.OrdinalIgnoreCase))
                {
                    hasRecordedAt = true;
                }
            }
        }

        if (!hasRecordedAt)
        {
            using var upgrade = conn.CreateCommand();
            upgrade.CommandText =
                "ALTER TABLE StockMovements ADD COLUMN created_at TEXT;";
            upgrade.ExecuteNonQuery();
        }

        using var check = conn.CreateCommand();
        check.CommandText = "SELECT COUNT(*) FROM Users;";

        if (Convert.ToInt64(check.ExecuteScalar()) == 0)
        {
            using var seed = conn.CreateCommand();

            seed.CommandText =
                "INSERT INTO Users(username,password_hash,display_name) " +
                "VALUES ('admin', $hash, 'Admin Gudang');";

            P(seed, "$hash", PasswordSecurity.Hash("Admin123!"));
            seed.ExecuteNonQuery();
        }
    }

    public UserAccount? Authenticate(
        string username,
        string password)
    {
        using var conn = Open();
        using var cmd = conn.CreateCommand();

        cmd.CommandText =
            "SELECT id,username,display_name,password_hash " +
            "FROM Users WHERE username=$name LIMIT 1;";

        P(cmd, "$name", username.Trim());

        using var rd = cmd.ExecuteReader();

        if (!rd.Read())
            return null;

        var hash = rd.GetString(3);

        return PasswordSecurity.Verify(password, hash)
            ? new UserAccount(
                rd.GetInt64(0),
                rd.GetString(1),
                rd.GetString(2))
            : null;
    }

    public UserAccount GetUser(long userId)
    {
        using var conn = Open();
        using var cmd = conn.CreateCommand();

        cmd.CommandText =
            "SELECT id,username,display_name FROM Users WHERE id=$id;";

        P(cmd, "$id", userId);

        using var rd = cmd.ExecuteReader();

        if (!rd.Read())
            throw new InvalidOperationException(
                "Akun tidak ditemukan.");

        return new(
            rd.GetInt64(0),
            rd.GetString(1),
            rd.GetString(2));
    }

    public void UpdateDisplayName(
        long userId,
        string name)
    {
        Required(name, "Nama pengguna", 70);

        using var conn = Open();
        using var cmd = conn.CreateCommand();

        cmd.CommandText =
            "UPDATE Users SET display_name=$name WHERE id=$id;";

        P(cmd, "$name", name.Trim());
        P(cmd, "$id", userId);

        cmd.ExecuteNonQuery();
    }

    public List<CoffeeType> GetCoffeeTypes()
    {
        var result = new List<CoffeeType>();

        using var conn = Open();
        using var cmd = conn.CreateCommand();

        cmd.CommandText =
            "SELECT id,name,status FROM CoffeeTypes " +
            "WHERE status=1 ORDER BY name;";

        using var rd = cmd.ExecuteReader();

        while (rd.Read())
        {
            result.Add(new(
                rd.GetInt64(0),
                rd.GetString(1),
                rd.GetInt64(2) == 1));
        }

        return result;
    }

    public List<Supplier> GetSuppliers()
    {
        var result = new List<Supplier>();

        using var conn = Open();
        using var cmd = conn.CreateCommand();

        cmd.CommandText =
            "SELECT id,name,contact,status FROM Suppliers " +
            "WHERE status=1 ORDER BY name;";

        using var rd = cmd.ExecuteReader();

        while (rd.Read())
        {
            result.Add(new(
                rd.GetInt64(0),
                rd.GetString(1),
                rd.GetString(2),
                rd.GetInt64(3) == 1));
        }

        return result;
    }

    public List<CoffeeTypeSummary> GetCoffeeTypeSummaries()
    {
        var result = new List<CoffeeTypeSummary>();

        using var conn = Open();
        using var cmd = conn.CreateCommand();

        cmd.CommandText = @"
            SELECT
                c.id,
                c.name,
                c.status,
                COALESCE(GROUP_CONCAT(DISTINCT l.origin),''),
                COUNT(l.id)
            FROM CoffeeTypes c
            LEFT JOIN CoffeeLots l
                ON l.coffee_type_id=c.id
            GROUP BY c.id,c.name,c.status
            ORDER BY c.status DESC,c.name;";

        using var rd = cmd.ExecuteReader();

        while (rd.Read())
        {
            result.Add(new(
                rd.GetInt64(0),
                rd.GetString(1),
                rd.GetInt64(2) == 1,
                rd.GetString(3),
                rd.GetInt64(4)));
        }

        return result;
    }

    public List<Supplier> GetAllSuppliers()
    {
        var result = new List<Supplier>();

        using var conn = Open();
        using var cmd = conn.CreateCommand();

        cmd.CommandText =
            "SELECT id,name,contact,status FROM Suppliers " +
            "ORDER BY status DESC,name;";

        using var rd = cmd.ExecuteReader();

        while (rd.Read())
        {
            result.Add(new(
                rd.GetInt64(0),
                rd.GetString(1),
                rd.GetString(2),
                rd.GetInt64(3) == 1));
        }

        return result;
    }

    public void SaveCoffeeType(
        long? id,
        string name,
        bool active)
    {
        Required(name, "Nama jenis kopi", 80);

        using var conn = Open();
        using var cmd = conn.CreateCommand();

        cmd.CommandText = id.HasValue
            ? "UPDATE CoffeeTypes SET name=$name,status=$status WHERE id=$id;"
            : "INSERT INTO CoffeeTypes(name,status) VALUES($name,$status);";

        P(cmd, "$name", name.Trim());
        P(cmd, "$status", active ? 1 : 0);

        if (id.HasValue)
            P(cmd, "$id", id.Value);

        if (cmd.ExecuteNonQuery() != 1)
            throw new ArgumentException(
                "Jenis kopi tidak ditemukan.");
    }

    // KONTAK PEMASOK SEKARANG WAJIB
    public void SaveSupplier(
        long? id,
        string name,
        string contact,
        bool active)
    {
        Required(name, "Nama pemasok", 100);
        Required(contact, "Kontak pemasok", 100);

        using var conn = Open();
        using var cmd = conn.CreateCommand();

        cmd.CommandText = id.HasValue
            ? "UPDATE Suppliers SET name=$name,contact=$contact,status=$status WHERE id=$id;"
            : "INSERT INTO Suppliers(name,contact,status) VALUES($name,$contact,$status);";

        P(cmd, "$name", name.Trim());
        P(cmd, "$contact", contact.Trim());
        P(cmd, "$status", active ? 1 : 0);

        if (id.HasValue)
            P(cmd, "$id", id.Value);

        if (cmd.ExecuteNonQuery() != 1)
            throw new ArgumentException(
                "Pemasok tidak ditemukan.");
    }

    public long Receive(
        string code,
        long typeId,
        long supplierId,
        string grade,
        string origin,
        DateTime date,
        long grams,
        long userId)
    {
        Required(code, "Kode lot", 40);
        Required(grade, "Grade", 40);
        Required(origin, "Asal daerah", 100);

        if (grams <= 0)
            throw new ArgumentException(
                "Berat masuk harus positif.");

        if (date.Date > DateTime.Today)
            throw new ArgumentException(
                "Tanggal penerimaan tidak boleh di masa depan.");

        using var conn = Open();
        using var tx = conn.BeginTransaction();

        using var lot = conn.CreateCommand();
        lot.Transaction = tx;

        lot.CommandText =
            "INSERT INTO CoffeeLots(" +
            "code,coffee_type_id,supplier_id,grade,origin,received_at) " +
            "VALUES($code,$type,$supplier,$grade,$origin,$date) RETURNING id;";

        P(lot, "$code", code.Trim());
        P(lot, "$type", typeId);
        P(lot, "$supplier", supplierId);
        P(lot, "$grade", grade.Trim());
        P(lot, "$origin", origin.Trim());
        P(lot, "$date", Date(date));

        var lotId = Convert.ToInt64(lot.ExecuteScalar());

        using var move = conn.CreateCommand();
        move.Transaction = tx;

        move.CommandText =
            "INSERT INTO StockMovements(" +
            "lot_id,type,quantity_g,happened_at,destination,user_id,created_at) " +
            "VALUES($lot,'IN',$qty,$date,'',$user,$recorded);";

        P(move, "$recorded",
            DateTimeOffset.Now.ToString(
                "yyyy-MM-ddTHH:mm:sszzz",
                CultureInfo.InvariantCulture));

        P(move, "$lot", lotId);
        P(move, "$qty", grams);
        P(move, "$date", Date(date));
        P(move, "$user", userId);

        move.ExecuteNonQuery();
        tx.Commit();

        return lotId;
    }

    public void Dispatch(
        long lotId,
        long grams,
        DateTime date,
        string destination,
        long userId)
    {
        if (grams <= 0)
            throw new ArgumentException(
                "Berat keluar harus positif.");

        Required(destination, "Tujuan pengeluaran", 100);

        if (date.Date > DateTime.Today)
            throw new ArgumentException(
                "Tanggal pengeluaran tidak boleh di masa depan.");

        using var conn = Open();
        using var tx = conn.BeginTransaction();

        using var balance = conn.CreateCommand();
        balance.Transaction = tx;

        balance.CommandText =
            "SELECT COALESCE(SUM(" +
            "CASE WHEN type='IN' THEN quantity_g ELSE -quantity_g END" +
            "),0) FROM StockMovements WHERE lot_id=$id;";

        P(balance, "$id", lotId);

        var available =
            Convert.ToInt64(balance.ExecuteScalar());

        using var lotCheck = conn.CreateCommand();
        lotCheck.Transaction = tx;

        lotCheck.CommandText =
            "SELECT COUNT(*) FROM CoffeeLots WHERE id=$id;";

        P(lotCheck, "$id", lotId);

        if (Convert.ToInt64(lotCheck.ExecuteScalar()) != 1)
            throw new ArgumentException(
                "Lot kopi tidak ditemukan.");

        if (grams > available)
            throw new ArgumentException(
                $"Stok tidak mencukupi. Tersedia {Units.Format(available)}.");

        using var move = conn.CreateCommand();
        move.Transaction = tx;

        move.CommandText =
            "INSERT INTO StockMovements(" +
            "lot_id,type,quantity_g,happened_at,destination,user_id,created_at) " +
            "VALUES($lot,'OUT',$qty,$date,$destination,$user,$recorded);";

        P(move, "$recorded",
            DateTimeOffset.Now.ToString(
                "yyyy-MM-ddTHH:mm:sszzz",
                CultureInfo.InvariantCulture));

        P(move, "$lot", lotId);
        P(move, "$qty", grams);
        P(move, "$date", Date(date));
        P(move, "$destination", destination.Trim());
        P(move, "$user", userId);

        move.ExecuteNonQuery();
        tx.Commit();
    }

    public List<Lot> GetLots(string search = "")
    {
        var result = new List<Lot>();

        using var conn = Open();
        using var cmd = conn.CreateCommand();

        cmd.CommandText = @"
            SELECT
                l.id,l.code,c.name,s.name,
                l.grade,l.origin,l.received_at,
                COALESCE(SUM(
                    CASE WHEN m.type='IN'
                    THEN m.quantity_g ELSE 0 END),0) total_in,
                COALESCE(SUM(
                    CASE WHEN m.type='OUT'
                    THEN m.quantity_g ELSE 0 END),0) total_out
            FROM CoffeeLots l
            JOIN CoffeeTypes c
                ON c.id=l.coffee_type_id
            JOIN Suppliers s
                ON s.id=l.supplier_id
            LEFT JOIN StockMovements m
                ON m.lot_id=l.id
            WHERE l.code LIKE $search ESCAPE '\'
            GROUP BY
                l.id,l.code,c.name,s.name,
                l.grade,l.origin,l.received_at
            ORDER BY l.id DESC;";

        string safe = search
            .Replace("\\", "\\\\")
            .Replace("%", "\\%")
            .Replace("_", "\\_");

        P(cmd, "$search", "%" + safe + "%");

        using var rd = cmd.ExecuteReader();

        while (rd.Read())
        {
            result.Add(new(
                rd.GetInt64(0),
                rd.GetString(1),
                rd.GetString(2),
                rd.GetString(3),
                rd.GetString(4),
                rd.GetString(5),
                rd.GetString(6),
                rd.GetInt64(7),
                rd.GetInt64(8)));
        }

        return result;
    }

    public Lot? GetLot(long id) =>
        GetLots().FirstOrDefault(x => x.Id == id);

    public (
        long InGrams,
        long OutGrams,
        long InCount,
        long OutCount
    ) GetMovementTotals()
    {
        using var conn = Open();
        using var cmd = conn.CreateCommand();

        cmd.CommandText =
            "SELECT " +
            "COALESCE(SUM(CASE WHEN type='IN' THEN quantity_g ELSE 0 END),0)," +
            "COALESCE(SUM(CASE WHEN type='OUT' THEN quantity_g ELSE 0 END),0)," +
            "COALESCE(SUM(CASE WHEN type='IN' THEN 1 ELSE 0 END),0)," +
            "COALESCE(SUM(CASE WHEN type='OUT' THEN 1 ELSE 0 END),0) " +
            "FROM StockMovements;";

        using var rd = cmd.ExecuteReader();

        rd.Read();

        return (
            rd.GetInt64(0),
            rd.GetInt64(1),
            rd.GetInt64(2),
            rd.GetInt64(3)
        );
    }

    public long CountMovements()
    {
        using var conn = Open();
        using var cmd = conn.CreateCommand();

        cmd.CommandText =
            "SELECT COUNT(*) FROM StockMovements;";

        return Convert.ToInt64(cmd.ExecuteScalar());
    }

    public List<Movement> GetMovements(
        string search = "",
        string type = "Semua",
        long? lotId = null,
        int limit = 500)
    {
        var result = new List<Movement>();

        using var conn = Open();
        using var cmd = conn.CreateCommand();

        cmd.CommandText = @"
            SELECT
                m.id,m.lot_id,l.code,m.type,
                m.quantity_g,m.happened_at,
                m.destination,u.display_name,m.created_at
            FROM StockMovements m
            JOIN CoffeeLots l ON l.id=m.lot_id
            JOIN Users u ON u.id=m.user_id
            WHERE l.code LIKE $search ESCAPE '\'
              AND ($type='Semua' OR m.type=$type)
              AND ($lot IS NULL OR m.lot_id=$lot)
            ORDER BY m.id DESC
            LIMIT $limit;";

        string safe = search
            .Replace("\\", "\\\\")
            .Replace("%", "\\%")
            .Replace("_", "\\_");

        P(cmd, "$search", "%" + safe + "%");
        P(cmd, "$type", type);
        P(cmd, "$lot", lotId);
        P(cmd, "$limit", limit);

        using var rd = cmd.ExecuteReader();

        while (rd.Read())
        {
            result.Add(new(
                rd.GetInt64(0),
                rd.GetInt64(1),
                rd.GetString(2),
                rd.GetString(3),
                rd.GetInt64(4),
                rd.GetString(5),
                rd.GetString(6),
                rd.GetString(7),
                rd.IsDBNull(8) ? "" : rd.GetString(8)
            ));
        }

        return result;
    }
}
