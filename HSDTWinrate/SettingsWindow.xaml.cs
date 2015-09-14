using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HSDTWinrate
{
    /// <summary>
    /// Settings window
    /// </summary>

    public partial class SettingsWindow : MetroWindow
    {
        private ConfigProp timeOptions = null;

        public SettingsWindow()
        {
            InitializeComponent();
            this.timeOptions = new ConfigProp();
            this.timeOptions.TimeDeckProperty = Config.Instance.StatDeckTime;
            this.timeOptions.VersionDeckProperty = Config.Instance.StatDeckVer;
            this.DataContext = this.timeOptions;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Config.Save();
            Winrate.RefreshStats();
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = "Select plugin output path";
            dialog.SelectedPath = Config.Instance.OutputPath;
            dialog.ShowNewFolderButton = true;
            DialogResult result = dialog.ShowDialog(this);
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                Config.Instance.OutputPath = dialog.SelectedPath;                
            }
        }
    }
    
    /// <summary>
    /// Databinding converter for the radio button groups
    /// </summary>

    public class RadioButtonCheckedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return value.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return value.Equals(true) ? parameter : System.Windows.Data.Binding.DoNothing;
        }
    }

    /// <summary>
    /// Snippet for easy use of Forms for directory dialog
    /// </summary>

    public class Wpf32Window : System.Windows.Forms.IWin32Window
    {
        public IntPtr Handle { get; private set; }

        public Wpf32Window(Window wpfWindow)
        {
            Handle = new WindowInteropHelper(wpfWindow).Handle;
        }
    }

    public static class CommonDialogExtensions
    {
        public static DialogResult ShowDialog
                                   (this CommonDialog dialog,
                                         Window parent)
        {
            return dialog.ShowDialog(new Wpf32Window(parent));
        }
    }
}
