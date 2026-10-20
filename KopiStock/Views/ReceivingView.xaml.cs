using System.Windows;
using System.Windows.Controls;
using KopiStock.Models;
using KopiStock.Services;
using Microsoft.Data.Sqlite;
namespace KopiStock.Views;
public partial class ReceivingView : UserControl
{
 private readonly StockDatabase _db;private readonly long _userId;
 public ReceivingView(StockDatabase db,long userId)
 {InitializeComponent();_db=db;_userId=userId;CoffeeTypes.ItemsSource=db.GetCoffeeTypes();Suppliers.ItemsSource=db.GetSuppliers();ReceivedDate.SelectedDate=DateTime.Today;UpdateLastLot();}
 private void UpdateLastLot(){var last=_db.GetLots().FirstOrDefault();if(last is null){LastLotCode.Text="Belum ada penerimaan";LastLotWeight.Text="";LastLotDetails.Text="";}else{LastLotCode.Text=last.Code;LastLotWeight.Text=last.InLabel;LastLotDetails.Text=$"{last.CoffeeName} · {last.Grade} · {last.Origin}";}}
 private void Clear_Click(object sender,RoutedEventArgs e)
 {LotCode.Clear();CoffeeTypes.SelectedIndex=-1;Suppliers.SelectedIndex=-1;Grade.Clear();Origin.Clear();Quantity.Clear();ReceivedDate.SelectedDate=DateTime.Today;}
 private void Save_Click(object sender,RoutedEventArgs e)
 {
  try{
   if(CoffeeTypes.SelectedValue is not long typeId || Suppliers.SelectedValue is not long supplierId) throw new ArgumentException("Pilih jenis kopi dan pemasok terlebih dahulu.");
   if(ReceivedDate.SelectedDate is not DateTime date)throw new ArgumentException("Tanggal penerimaan wajib diisi.");
   long grams=Units.ParseKg(Quantity.Text);
   long id=_db.Receive(LotCode.Text,typeId,supplierId,Grade.Text,Origin.Text,date,grams,_userId);
   string code=LotCode.Text;Clear_Click(sender,e);UpdateLastLot();Status.Text=$"Penerimaan berhasil. Lot {code} (ID {id}) bertambah {Units.Format(grams)}.";
  }
  catch(Exception ex) when(ex is ArgumentException or SqliteException or OverflowException)
  {MessageBox.Show(ex is SqliteException?"Gagal menyimpan. Periksa apakah kode lot sudah dipakai dan master data valid.":ex.Message,"Validasi Penerimaan",MessageBoxButton.OK,MessageBoxImage.Warning);}
 }
}
