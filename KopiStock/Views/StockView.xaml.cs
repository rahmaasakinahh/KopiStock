using System.Windows;
using System.Windows.Controls;
using KopiStock.Models;
using KopiStock.Services;
using Microsoft.Data.Sqlite;
namespace KopiStock.Views;
public partial class StockView : UserControl
{
 private readonly StockDatabase _db;private readonly long _userId;private readonly Action<long> _showDetail;
 public StockView(StockDatabase db,long userId,Action<long> showDetail,long? initialId=null)
 {InitializeComponent();_db=db;_userId=userId;_showDetail=showDetail;LotGrid.RowHeight=UiPreferences.RowHeight;OutDate.SelectedDate=DateTime.Today;Reload();if(initialId.HasValue)LotGrid.SelectedItem=((System.Collections.IEnumerable)LotGrid.ItemsSource).Cast<Lot>().FirstOrDefault(x=>x.Id==initialId.Value);}
 private void Reload(){var lots=_db.GetLots(Search.Text);LotGrid.ItemsSource=lots; StockSummary.Text=$"{lots.Count(x=>x.BalanceGrams>0)} lot aktif · Total {Units.Format(lots.Sum(x=>x.BalanceGrams))}";}
 private void Search_TextChanged(object sender,TextChangedEventArgs e){if(LotGrid!=null && _db!=null)Reload();}
 private void Refresh_Click(object sender,RoutedEventArgs e)=>Reload();
 private void LotGrid_SelectionChanged(object sender,SelectionChangedEventArgs e)
 {if(LotGrid.SelectedItem is Lot x){SelectedLot.Text=$"{x.Code} / {x.CoffeeName}";InSelected.Text=x.InLabel;OutSelected.Text=x.OutLabel;BalanceSelected.Text=x.BalanceLabel;StockAvailable.Text=x.BalanceLabel;}else{SelectedLot.Text="Belum ada lot yang dipilih";InSelected.Text=OutSelected.Text=BalanceSelected.Text=StockAvailable.Text="—";}}
 private void Details_Click(object sender,RoutedEventArgs e)
 {if(LotGrid.SelectedItem is Lot x)_showDetail(x.Id);else MessageBox.Show("Pilih lot untuk melihat detail.");}
 private void Dispatch_Click(object sender,RoutedEventArgs e)
 {
  try{
   if(LotGrid.SelectedItem is not Lot x)throw new ArgumentException("Pilih lot kopi terlebih dahulu.");
   if(OutDate.SelectedDate is not DateTime date)throw new ArgumentException("Tanggal pengeluaran wajib diisi.");
   long grams=Units.ParseKg(Weight.Text);
   _db.Dispatch(x.Id,grams,date,Destination.Text,_userId);
   Feedback.Text=$"Berhasil: {Units.Format(grams)} keluar dari lot {x.Code}.";
   Weight.Clear();Destination.Clear();var id=x.Id;Reload();LotGrid.SelectedItem=((System.Collections.IEnumerable)LotGrid.ItemsSource).Cast<Lot>().FirstOrDefault(l=>l.Id==id);
  }
  catch(Exception ex) when(ex is ArgumentException or SqliteException or OverflowException)
  {MessageBox.Show(ex.Message,"Validasi Pengeluaran",MessageBoxButton.OK,MessageBoxImage.Warning);}
 }
}
