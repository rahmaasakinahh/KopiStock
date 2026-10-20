using System.Windows;
using System.Windows.Controls;
namespace KopiStock.Views;
public partial class HelpView:UserControl
{
 public HelpView(){InitializeComponent();}
 private void SearchFaq_TextChanged(object sender,TextChangedEventArgs e)
 {
  if(Faq1==null)return;
  string q=FaqSearch.Text.Trim();
  foreach(var pair in new[]{(BoxQ1,Faq1),(BoxQ2,Faq2),(BoxQ3,Faq3),(BoxQ4,Faq4),(BoxQ5,Faq5),(BoxQ6,Faq6)})
  {
   string question=pair.Item2.Header?.ToString()??"";
   pair.Item1.Visibility=question.Contains(q,StringComparison.CurrentCultureIgnoreCase)?Visibility.Visible:Visibility.Collapsed;
  }
 }
 private void CopyReport_Click(object sender,RoutedEventArgs e)
 {
  string? category=(Category.SelectedItem as ComboBoxItem)?.Content?.ToString();
  if(string.IsNullOrWhiteSpace(Reporter.Text)||string.IsNullOrWhiteSpace(category)||string.IsNullOrWhiteSpace(Description.Text))
  {MessageBox.Show("Nama, kategori, dan deskripsi kendala wajib diisi.","Validasi",MessageBoxButton.OK,MessageBoxImage.Warning);return;}
  if(Reporter.Text.Length>100||Description.Text.Length>2000){MessageBox.Show("Nama maksimal 100 karakter dan deskripsi maksimal 2000 karakter.");return;}
  string report=$"LAPORAN KENDALA KOPISTOCK\nTanggal: {DateTime.Now:dd-MM-yyyy HH:mm}\nNama: {Reporter.Text.Trim()}\nKategori: {category}\nDeskripsi:\n{Description.Text.Trim()}";
  try{Clipboard.SetText(report);Feedback.Text="Laporan disalin. Kirim ke penanggung jawab gudang melalui kanal internal.";}
  catch(Exception ex){MessageBox.Show("Gagal menyalin laporan: "+ex.Message);}
 }
}
