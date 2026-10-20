using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using KopiStock.Services;
namespace KopiStock.Views;
public partial class SettingsView:UserControl
{
 private readonly StockDatabase _db;
 public SettingsView(StockDatabase db){InitializeComponent();_db=db;PathDisplay.Text=db.DatabasePath;DensityCompact.IsChecked=UiPreferences.RowHeight<45;DensityComfort.IsChecked=UiPreferences.RowHeight>=45;}
 private void Apply_Click(object sender,RoutedEventArgs e)
 {try{UiPreferences.Save(DensityCompact.IsChecked==true?37:49);PreferenceFeedback.Text="Preferensi tersimpan. Buka kembali halaman tabel untuk melihat perubahan.";}
 catch(Exception ex){MessageBox.Show("Gagal menyimpan preferensi: "+ex.Message);}}
 private void OpenFolder_Click(object sender,RoutedEventArgs e)
 {try{Process.Start(new ProcessStartInfo("explorer.exe",_db.FolderPath){UseShellExecute=true});}
 catch(Exception ex){MessageBox.Show("Gagal membuka folder: "+ex.Message);}}
 private void Copy_Click(object sender,RoutedEventArgs e)
 {try{Clipboard.SetText(_db.DatabasePath);MessageBox.Show("Lokasi database berhasil disalin.");}
 catch(Exception ex){MessageBox.Show("Gagal menyalin: "+ex.Message);}}
}
