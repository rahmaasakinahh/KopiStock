using System.Windows;
using System.Windows.Controls;
using KopiStock.Services;
namespace KopiStock.Views;
public partial class ProfileView:UserControl
{
 private readonly StockDatabase _db;private readonly long _userId;private readonly Action _onChanged;
 public ProfileView(StockDatabase db,long userId,Action onChanged)
 {InitializeComponent();_db=db;_userId=userId;_onChanged=onChanged;var u=_db.GetUser(userId);Username.Text=u.Username;DisplayName.Text=u.DisplayName;NameLabel.Text=u.DisplayName;}
 private void Save_Click(object sender,RoutedEventArgs e)
 {try{_db.UpdateDisplayName(_userId,DisplayName.Text);NameLabel.Text=DisplayName.Text.Trim();_onChanged();Feedback.Text="Nama tampilan berhasil diperbarui.";}
 catch(ArgumentException ex){MessageBox.Show(ex.Message,"Validasi",MessageBoxButton.OK,MessageBoxImage.Warning);}}
}
