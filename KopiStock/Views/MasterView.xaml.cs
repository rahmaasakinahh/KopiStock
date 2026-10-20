
using System.Windows;
using System.Windows.Controls;
using KopiStock.Models;
using KopiStock.Services;
using Microsoft.Data.Sqlite;

namespace KopiStock.Views;

public partial class MasterView : UserControl
{
    private readonly StockDatabase _db;
    private long? _coffeeId, _supplierId;

    public MasterView(StockDatabase db)
    {
        InitializeComponent();
        _db = db;

        CoffeeGrid.RowHeight = SupplierGrid.RowHeight =
            UiPreferences.RowHeight;

        Reload();
    }

    private void Reload()
    {
        var coffees = _db.GetCoffeeTypeSummaries();
        var suppliers = _db.GetAllSuppliers();

        CoffeeGrid.ItemsSource = coffees;
        SupplierGrid.ItemsSource = suppliers;

        CoffeeCount.Text = $"{coffees.Count} jenis";
        SupplierCount.Text = $"{suppliers.Count} pemasok";
    }

    private void SetCoffee(CoffeeTypeSummary type)
    {
        _coffeeId = type.Id;

        CoffeeName.Text = type.Name;
        CoffeeStatus.SelectedIndex = type.Active ? 0 : 1;

        CoffeeMessage.Text = "Mode: ubah jenis kopi terpilih";
    }

    private void SetSupplier(Supplier supplier)
    {
        _supplierId = supplier.Id;

        SupplierName.Text = supplier.Name;
        SupplierContact.Text = supplier.Contact;
        SupplierStatus.SelectedIndex = supplier.Active ? 0 : 1;

        SupplierMessage.Text = "Mode: ubah pemasok terpilih";
    }

    private void CoffeeGrid_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        if (CoffeeGrid.SelectedItem is CoffeeTypeSummary x)
        {
            SetCoffee(x);
        }
    }

    private void SupplierGrid_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        if (SupplierGrid.SelectedItem is Supplier x)
        {
            SetSupplier(x);
        }
    }

    private void CoffeeEdit_Click(
        object sender,
        RoutedEventArgs e)
    {
        if ((sender as Button)?.CommandParameter
            is CoffeeTypeSummary x)
        {
            CoffeeGrid.SelectedItem = x;
            SetCoffee(x);

            CoffeeDetailPanel.Visibility =
                Visibility.Collapsed;
        }
    }

    private void SupplierEdit_Click(
        object sender,
        RoutedEventArgs e)
    {
        if ((sender as Button)?.CommandParameter
            is Supplier x)
        {
            SupplierGrid.SelectedItem = x;
            SetSupplier(x);

            SupplierDetailPanel.Visibility =
                Visibility.Collapsed;
        }
    }

    private void CoffeeDetails_Click(
        object sender,
        RoutedEventArgs e)
    {
        if ((sender as Button)?.CommandParameter
            is not CoffeeTypeSummary x)
        {
            return;
        }

        CoffeeDetailName.Text = x.Name;
        CoffeeDetailStatus.Text =
            "Status: " + x.StatusLabel;

        CoffeeDetailOrigins.Text =
            "Asal lot tercatat: " + x.OriginLabel;

        CoffeeDetailLots.Text =
            $"Jumlah lot terkait: {x.LotCount}. " +
            "Daerah asal dapat berbeda untuk setiap lot.";

        CoffeeDetailPanel.Visibility =
            Visibility.Visible;
    }

    private void SupplierDetails_Click(
        object sender,
        RoutedEventArgs e)
    {
        if ((sender as Button)?.CommandParameter
            is not Supplier x)
        {
            return;
        }

        SupplierDetailName.Text = x.Name;

        SupplierDetailContact.Text =
            "Kontak: " + x.ContactLabel;

        SupplierDetailStatus.Text =
            "Status: " + x.StatusLabel;

        SupplierDetailLots.Text =
            $"Lot terkait: {_db.GetLots().Count(l => l.SupplierName == x.Name)}";

        SupplierDetailPanel.Visibility =
            Visibility.Visible;
    }

    private void ClearCoffee_Click(
        object sender,
        RoutedEventArgs e)
    {
        _coffeeId = null;

        CoffeeGrid.SelectedItem = null;
        CoffeeName.Clear();
        CoffeeStatus.SelectedIndex = 0;

        CoffeeMessage.Text = "Mode: tambah data baru";

        CoffeeDetailPanel.Visibility =
            Visibility.Collapsed;
    }

    private void ClearSupplier_Click(
        object sender,
        RoutedEventArgs e)
    {
        _supplierId = null;

        SupplierGrid.SelectedItem = null;
        SupplierName.Clear();
        SupplierContact.Clear();
        SupplierStatus.SelectedIndex = 0;

        SupplierMessage.Text = "Mode: tambah data baru";

        SupplierDetailPanel.Visibility =
            Visibility.Collapsed;
    }

    private void SaveCoffee_Click(
        object sender,
        RoutedEventArgs e)
    {
        try
        {
            _db.SaveCoffeeType(
                _coffeeId,
                CoffeeName.Text,
                CoffeeStatus.SelectedIndex == 0
            );

            ClearCoffee_Click(sender, e);
            Reload();

            CoffeeMessage.Text =
                "Jenis kopi berhasil disimpan.";
        }
        catch (Exception ex)
            when (ex is ArgumentException or SqliteException)
        {
            MessageBox.Show(
                ex is SqliteException
                    ? "Nama jenis kopi sudah digunakan atau database gagal menyimpan."
                    : ex.Message,
                "Validasi",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );
        }
    }

    // KONTAK PEMASOK WAJIB DIISI
    private void SaveSupplier_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(SupplierContact.Text))
        {
            MessageBox.Show(
                "Kontak pemasok wajib diisi dengan nomor telepon atau email.",
                "Validasi pemasok",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );

            SupplierContact.Focus();
            return;
        }

        try
        {
            _db.SaveSupplier(
                _supplierId,
                SupplierName.Text,
                SupplierContact.Text,
                SupplierStatus.SelectedIndex == 0
            );

            ClearSupplier_Click(sender, e);
            Reload();

            SupplierMessage.Text =
                "Pemasok berhasil disimpan.";
        }
        catch (Exception ex)
            when (ex is ArgumentException or SqliteException)
        {
            MessageBox.Show(
                ex.Message,
                "Validasi",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );
        }
    }
}
