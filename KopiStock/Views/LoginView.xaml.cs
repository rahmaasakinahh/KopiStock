using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using KopiStock.Models;
using KopiStock.Services;
namespace KopiStock.Views;
public partial class LoginView : UserControl
{
    private readonly StockDatabase _db; private readonly Action<UserAccount> _success;
    public LoginView(StockDatabase db,Action<UserAccount> success){InitializeComponent();_db=db;_success=success;}
    private void Login_Click(object sender,RoutedEventArgs e)=>Attempt();
    private void Password_KeyDown(object sender,KeyEventArgs e){if(e.Key==Key.Enter)Attempt();}
    private void Attempt()
    {
        try {var user=_db.Authenticate(Username.Text,Password.Password);if(user is null){Feedback.Text="Username atau password salah.";Password.Clear();return;}_success(user);}
        catch(Exception ex){Feedback.Text="Gagal masuk: "+ex.Message;}
    }
}
