using System.Windows;
using System.Windows.Controls;
using KopiStock.Models;
using KopiStock.Services;
namespace KopiStock.Views;
public partial class HistoryView : UserControl
{
 private readonly StockDatabase _db;
 public HistoryView(StockDatabase db){InitializeComponent();_db=db;HistoryGrid.RowHeight=UiPreferences.RowHeight;var totals=db.GetMovementTotals();SumIn.Text=Units.Format(totals.InGrams);SumOut.Text=Units.Format(totals.OutGrams);SumBalance.Text=Units.Format(totals.InGrams-totals.OutGrams);Reload();}
 private void Reload()
 {if(Search==null||TypeFilter==null||HistoryGrid==null)return;var value=(TypeFilter.SelectedItem as ComboBoxItem)?.Content?.ToString()??"Semua";var records=_db.GetMovements(Search.Text,value);HistoryGrid.ItemsSource=records;RowsInfo.Text=$"Menampilkan {records.Count} transaksi";}
 private void Filter_Changed(object sender,RoutedEventArgs e){if(_db!=null)Reload();}
 private void Reset_Click(object sender,RoutedEventArgs e){Search.Clear();TypeFilter.SelectedIndex=0;Reload();}
}
