using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using KopiStock.Models;
using KopiStock.Services;
using KopiStock.Views;
namespace KopiStock;
public partial class MainWindow : Window
{
    private readonly StockDatabase _db = new();
    private UserAccount? _user;
    public MainWindow()
    {
        InitializeComponent();
        HeaderDate.Text = DateTime.Today.ToString("dd MMMM yyyy", CultureInfo.GetCultureInfo("id-ID"));
        try { _db.Initialize(); UiPreferences.Initialize(_db.FolderPath); ShowLogin(); }
        catch(Exception ex) { MessageBox.Show($"Gagal menyiapkan database: {ex.Message}","Kesalahan",MessageBoxButton.OK,MessageBoxImage.Error); Close(); }
    }
    private long UserId => _user?.Id ?? throw new InvalidOperationException("Belum login.");
    private void ShowLogin()
    {
        _user=null; Shell.Visibility=Visibility.Collapsed; LoginHost.Visibility=Visibility.Visible;
        LoginHost.Content=new LoginView(_db, user=>{ _user=user; LoginHost.Visibility=Visibility.Collapsed; Shell.Visibility=Visibility.Visible; RefreshUser(); OpenDashboard(); });
    }
    private void RefreshUser() { if(_user!=null) SideUser.Text=_db.GetUser(UserId).DisplayName; }
    private void Navigate(string title, UserControl page, Button active)
    {
        HeaderTitle.Text=title;
        MainContent.Content=page;
        MainScroll.ScrollToTop();
        foreach(var b in new[]{NavDashboard,NavMaster,NavReceiving,NavStock,NavHistory,NavProfile,NavSettings,NavHelp})
            b.Style=(Style)FindResource(b==active?"SidebarButtonActive":"SidebarButton");
    }
    private void OpenDashboard()=>Navigate("Dashboard",new DashboardView(_db,()=>Navigate("Penerimaan",new ReceivingView(_db,UserId),NavReceiving),()=>Navigate("Riwayat",new HistoryView(_db),NavHistory),()=>OpenStock()),NavDashboard);
    private void OpenStock(long? initialId=null)=>Navigate("Stok & Pengeluaran",new StockView(_db,UserId,ShowDetail,initialId),NavStock);
    private void ShowDetail(long id)=>Navigate("Stok & Pengeluaran / Detail Lot",new LotDetailView(_db,id,()=>OpenStock(),()=>OpenStock(id)),NavStock);
    private void Dashboard_Click(object sender,RoutedEventArgs e)=>OpenDashboard();
    private void Master_Click(object sender,RoutedEventArgs e)=>Navigate("Master Data",new MasterView(_db),NavMaster);
    private void Receiving_Click(object sender,RoutedEventArgs e)=>Navigate("Penerimaan",new ReceivingView(_db,UserId),NavReceiving);
    private void Stock_Click(object sender,RoutedEventArgs e)=>OpenStock();
    private void History_Click(object sender,RoutedEventArgs e)=>Navigate("Riwayat",new HistoryView(_db),NavHistory);
    private void Profile_Click(object sender,RoutedEventArgs e)=>Navigate("Profil",new ProfileView(_db,UserId,RefreshUser),NavProfile);
    private void Settings_Click(object sender,RoutedEventArgs e)=>Navigate("Pengaturan",new SettingsView(_db),NavSettings);
    private void Help_Click(object sender,RoutedEventArgs e)=>Navigate("Pusat Bantuan",new HelpView(),NavHelp);
    private void Logout_Click(object sender,RoutedEventArgs e)
    {
        if(MessageBox.Show("Keluar dari KopiStock?","Konfirmasi",MessageBoxButton.YesNo,MessageBoxImage.Question)==MessageBoxResult.Yes) ShowLogin();
    }
}
