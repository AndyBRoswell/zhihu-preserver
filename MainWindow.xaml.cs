using CefSharp;
using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
	class WindowBasicInfoItem {
		public string Address { get; set; }
		public string Title { get; set; }

		public WindowBasicInfoItem(string addr, string title) {
			Address = addr; Title = title;
		}
	}

	public partial class MainWindow : Window {
		internal static readonly MainWindow ThisWindow = Application.Current.MainWindow as MainWindow;
		internal static SortedDictionary<IntPtr, WindowBasicInfoItem> OpenBrowserWindowInfo = new();

		internal static TextBlock LogBlock = new() {
			TextWrapping = TextWrapping.Wrap
		};

		public MainWindow() {
			InitializeComponent();
		}

		static internal void ModifyBrowserWindowInfo(IntPtr hwnd, string address, string title) {
			OpenBrowserWindowInfo[hwnd] = new WindowBasicInfoItem(address, title);
			ThisWindow.OpenBrowserWindowList.Items.Refresh();
		}

		static internal void DeleteBrowserWindowInfo(IntPtr hwnd) {
			OpenBrowserWindowInfo.Remove(hwnd);
			ThisWindow.OpenBrowserWindowList.Items.Refresh();
		}

		static internal void WriteToLog(string type, string content) {
			Application.Current.Dispatcher.Invoke(() => {
				LogBlock.Text += "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] [" + type + "] " + content + Environment.NewLine;
			});
		}

		private void Menu_Program_Multiboxing_Click(object sender, RoutedEventArgs e) {
			Process.Start(Global.Const["AppPathname"]);
		}

		private void Menu_Program_Settings_Click(object sender, RoutedEventArgs e) {
			SettingsWindow window = new();
			window.Show();
		}

		private void Menu_Edit_New_Browser_Window_Home_Click(object sender, RoutedEventArgs e) {
			BrowserWindow window = new();
			window.Show();
		}

		private void Menu_Edit_New_Browser_Window_Blank_Click(object sender, RoutedEventArgs e) {
			BrowserWindow window = new("about:blank");
			window.Show();
		}

		private void MainForm_Loaded(object sender, RoutedEventArgs e) {
			// Load basic constants
			Global.Const.Add("AppName", AppDomain.CurrentDomain.FriendlyName);
			Global.Const.Add("AppPath", Directory.GetCurrentDirectory());
			Global.Const.Add("AppPathname", Global.Const["AppPath"] + '\\' + Global.Const["AppName"]);
			Global.Const.Add("CachePath", Global.Const["AppPath"] + @"\cache");
			Global.Const.Add("CfgPath", Global.Const["AppPath"] + @"\cfg");
			Global.Const.Add("DefaultCfg", Global.Const["CfgPath"] + @"\config.xml");

			SettingsWindow.LoadSettings(Global.Const["DefaultCfg"]);

			// Load CefSettings
			CefSharpSettings.ShutdownOnExit = true;
			Cef.EnableHighDPISupport();

			CefSettings settings = new();
			// Increase the log severity so CEF outputs detailed information, useful for debugging
			settings.LogSeverity = LogSeverity.Verbose;
			// By default CEF uses an in memory cache, to save cached data e.g. to persist cookies you need to specify a cache path
			// NOTE: The executing user must have sufficient privileges to write to this folder.
			settings.CachePath = Global.Const["CachePath"];
			Cef.Initialize(settings);

			// Controls initialization
			OpenBrowserWindowList.ItemsSource = OpenBrowserWindowInfo.Values;
			LogBlockSlot.Content = LogBlock;

			WriteToLog(Properties.Resources.Information, Properties.Resources.SystemLoaded);
		}

		private void LogBlockSlot_ScrollChanged(object sender, ScrollChangedEventArgs e) {
			ScrollViewer sv = sender as ScrollViewer;
			bool AutoScrollToEnd = true;
			if (e.ExtentHeightChange == 0) {// user scroll i.e. contents aren't modified
				AutoScrollToEnd = (sv.ScrollableHeight == sv.VerticalOffset); // if scroll bar isn't at the bottom, then don't scroll, because the user may be viewing the contents
			}
			else {// contents are modified
				if (AutoScrollToEnd == true) sv.ScrollToEnd();
			}
			return;
		}
	}
}
