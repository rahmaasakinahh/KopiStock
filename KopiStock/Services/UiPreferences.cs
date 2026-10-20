using System.IO;
namespace KopiStock.Services;
/// <summary>Only a small UI preference, stored separately from inventory data.</summary>
public static class UiPreferences
{
 private static string _file = "";
 public static double RowHeight {get; private set;}=49;
 public static void Initialize(string appDataFolder)
 {
  Directory.CreateDirectory(appDataFolder);
  _file=Path.Combine(appDataFolder,"ui-density.txt");
  try {RowHeight=File.Exists(_file)&&File.ReadAllText(_file).Trim()=="compact"?37:49;}
  catch(IOException){RowHeight=49;}
 }
 public static void Save(double height)
 {
  if(string.IsNullOrWhiteSpace(_file))throw new InvalidOperationException("Pengaturan belum siap.");
  RowHeight=height<45?37:49;
  File.WriteAllText(_file,RowHeight<45?"compact":"comfortable");
 }
}
