using System.Windows;
using System.Windows.Controls;
using KopiStock.Services;
namespace KopiStock.Views;
public partial class LotDetailView : UserControl
{
 private readonly Action _back,_dispatch;
 public LotDetailView(StockDatabase db,long id,Action back,Action dispatch)
 {
  InitializeComponent();_back=back;_dispatch=dispatch;
  var lot=db.GetLot(id)??throw new ArgumentException("Lot tidak ditemukan.");
  LotCode.Text=lot.Code;CoffeeName.Text=lot.CoffeeName;SupplierName.Text=lot.SupplierName;
  Grade.Text=lot.Grade;Origin.Text=lot.Origin;ReceivedDate.Text=lot.ReceivedAt;
  InValue.Text=lot.InLabel;OutValue.Text=lot.OutLabel;BalanceValue.Text=lot.BalanceLabel;
  MovementsGrid.ItemsSource=db.GetMovements(lotId:id);MovementsGrid.RowHeight=UiPreferences.RowHeight;
 }
 private void Back_Click(object sender,RoutedEventArgs e)=>_back();
 private void Dispatch_Click(object sender,RoutedEventArgs e)=>_dispatch();
}
