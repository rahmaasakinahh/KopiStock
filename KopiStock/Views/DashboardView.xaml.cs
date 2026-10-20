using System.Windows;
using System.Windows.Controls;
using KopiStock.Models;
using KopiStock.Services;
namespace KopiStock.Views;
public partial class DashboardView : UserControl
{
    private readonly Action _receiving, _history, _stock;
    public DashboardView(StockDatabase db, Action receiving, Action history, Action stock)
    {
        InitializeComponent(); _receiving=receiving; _history=history; _stock=stock;
        var lots=db.GetLots();
        TotalStock.Text=Units.Format(lots.Sum(l=>l.BalanceGrams));
        ActiveLots.Text=lots.Count(l=>l.BalanceGrams>0)+" lot";
        var totals=db.GetMovementTotals();
        TotalIn.Text=Units.Format(totals.InGrams); TotalOut.Text=Units.Format(totals.OutGrams);
        CountIn.Text=$"IN · {totals.InCount} transaksi"; CountOut.Text=$"OUT · {totals.OutCount} transaksi";
        var recent=db.GetMovements(limit:5); Recent.ItemsSource=recent; Empty.Visibility=recent.Count==0?Visibility.Visible:Visibility.Collapsed;
        var active=lots.Where(l=>l.BalanceGrams>0).Take(3).ToList();LotPreview.ItemsSource=active; NoLots.Visibility=active.Count==0?Visibility.Visible:Visibility.Collapsed;
        Recent.RowHeight=LotPreview.RowHeight=UiPreferences.RowHeight;
    }
    private void Receiving_Click(object sender,RoutedEventArgs e)=>_receiving();
    private void History_Click(object sender,RoutedEventArgs e)=>_history();
    private void Stock_Click(object sender,RoutedEventArgs e)=>_stock();
}
