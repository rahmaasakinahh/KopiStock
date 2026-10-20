namespace KopiStock.Models;

public record UserAccount(long Id, string Username, string DisplayName);
public record CoffeeType(long Id, string Name, bool Active)
{
    public string StatusLabel => Active ? "Aktif" : "Nonaktif";
}
// Origin is NOT fixed by coffee variety. A coffee type can have several origins across lots.
public record CoffeeTypeSummary(long Id, string Name, bool Active, string Origins, long LotCount)
{
    public string StatusLabel => Active ? "Aktif" : "Nonaktif";
    public string OriginLabel => string.IsNullOrWhiteSpace(Origins) ? "Belum ada lot" : Origins;
}
public record Supplier(long Id, string Name, string Contact, bool Active)
{
    public string StatusLabel => Active ? "Aktif" : "Nonaktif";
    public string ContactLabel => string.IsNullOrWhiteSpace(Contact) ? "Belum diisi" : Contact;
}
public record Lot(long Id, string Code, string CoffeeName, string SupplierName, string Grade, string Origin,
    string ReceivedAt, long InGrams, long OutGrams)
{
    public long BalanceGrams => InGrams - OutGrams;
    public string BalanceLabel => Units.Format(BalanceGrams);
    public string InLabel => Units.Format(InGrams);
    public string OutLabel => Units.Format(OutGrams);
}
public record Movement(long Id, long LotId, string LotCode, string Type, long QuantityGrams,
    string Date, string Destination, string EnteredBy, string RecordedAt)
{
    public string QuantityLabel => Units.Format(QuantityGrams);
    public string TypeLabel => Type == "IN" ? "Masuk" : "Keluar";
    public string DestinationLabel => string.IsNullOrWhiteSpace(Destination) ? "—" : Destination;
    public string TransactionDateLabel => System.DateTime.TryParseExact(Date, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture,
        System.Globalization.DateTimeStyles.None, out var date)
        ? date.ToString("dd MMM yyyy", System.Globalization.CultureInfo.GetCultureInfo("id-ID")) : Date;
    public string RecordedAtLabel => System.DateTimeOffset.TryParse(RecordedAt,
        System.Globalization.CultureInfo.InvariantCulture,
        System.Globalization.DateTimeStyles.None, out var recorded)
        ? recorded.ToString("dd MMM yyyy · HH.mm", System.Globalization.CultureInfo.GetCultureInfo("id-ID"))
        : "— (data lama)";
}
public static class Units
{
    // One kilogram = 1,000 grams. UI accepts only 0–3 decimal places.
    public static long ParseKg(string? input)
    {
        var s = (input ?? "").Trim();
        if (!System.Text.RegularExpressions.Regex.IsMatch(s, @"^[0-9]+([,.][0-9]{1,3})?$"))
            throw new ArgumentException("Isi berat dengan angka positif, maksimal 3 desimal. Contoh: 12,500");
        if (!decimal.TryParse(s.Replace(',', '.'), System.Globalization.NumberStyles.AllowDecimalPoint,
                System.Globalization.CultureInfo.InvariantCulture, out var kg) || kg <= 0 || kg > 9_000_000_000_000m)
            throw new ArgumentException("Berat harus lebih besar dari 0 kg dan berada dalam batas penyimpanan.");
        return checked((long)(kg * 1000m));
    }
    public static string Format(long grams)
    {
        var kg = grams / 1000m;
        return kg.ToString("0.###", System.Globalization.CultureInfo.GetCultureInfo("id-ID")) + " kg";
    }
}
