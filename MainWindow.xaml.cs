using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace zhihu_preserver {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		internal List<IntPtr> Hwnd = new();
		public MainWindow() {
			InitializeComponent();
			StreamReader reader = new StreamReader(Global.AppPath + "\\cfg\\config.cfg");
			
		}
		private void Menu_Edit_New_Browser_Window_Click(object sender, RoutedEventArgs e) {
			BrowserWindow browser = new();
			browser.Show();
			var wih = new WindowInteropHelper(browser);
			Hwnd.Add(wih.Handle);
			WebPageAddress.Items.Add("about:blank");
			WebPageTitle.Items.Add("about:blank");
		}
		private void Menu_Program_Settings_Click(object sender, RoutedEventArgs e) {
			SettingsWindow window = new();
			window.Show();
		}
	}
}
