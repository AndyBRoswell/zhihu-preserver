using CefSharp;
using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
	internal class WindowBasicInfoItem {
		public string Address { get; set; }
		public string Title { get; set; }

		public WindowBasicInfoItem(string addr, string title) {
			Address = addr; Title = title;
		}
	}
	public partial class MainWindow : Window {
		//static internal List<IntPtr> OpenBrowserHwnd = new();
		static internal SortedDictionary<IntPtr, WindowBasicInfoItem> OpenWindowInfo = new();
		public MainWindow() {
			InitializeComponent();
		}

		private void Menu_Program_Multiboxing_Click(object sender, RoutedEventArgs e) {
			Process.Start(Global.AppPathname);
		}

		private void Menu_Program_Settings_Click(object sender, RoutedEventArgs e) {
			SettingsWindow window = new();
			window.Show();
		}

		static internal void AddToWindowList(IntPtr hwnd, string URL) {
			OpenWindowInfo.Add(hwnd, new WindowBasicInfoItem(URL, URL));
		}

		private void Menu_Edit_New_Browser_Window_Home_Click(object sender, RoutedEventArgs e) {
			BrowserWindow browser = new();
			browser.Show();
		}

		private void Menu_Edit_New_Browser_Window_Blank_Click(object sender, RoutedEventArgs e) {

		}

		private void MainForm_Loaded(object sender, RoutedEventArgs e) {
			SettingsWindow.LoadSettings(Global.DefaultCfg);

			// Load settings
			CefSharpSettings.ShutdownOnExit = true;
			Cef.EnableHighDPISupport();

			CefSettings settings = new();
			// Increase the log severity so CEF outputs detailed information, useful for debugging
			settings.LogSeverity = LogSeverity.Verbose;
			// By default CEF uses an in memory cache, to save cached data e.g. to persist cookies you need to specify a cache path
			// NOTE: The executing user must have sufficient privileges to write to this folder.
			settings.CachePath = Global.CachePath;
			Cef.Initialize(settings);

			// Part of controls
			OpenWindowList.ItemsSource = OpenWindowInfo.Values;
			AddToWindowList((IntPtr)0, "test0");
			//AddToWindowList((IntPtr)1, "test1");
			AddToWindowList((IntPtr)2, "test2");
			//AddToWindowList((IntPtr)3, "test3");
			//AddToWindowList((IntPtr)4, "test4");
			//AddToWindowList((IntPtr)5, "test5");
			//AddToWindowList((IntPtr)6, "test6");
			//AddToWindowList((IntPtr)7, "test7");
		}
	}
}
