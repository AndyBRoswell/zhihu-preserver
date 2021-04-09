using CefSharp;
using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Windows.Shapes;
using System.Xml;

namespace zhihu_preserver {
	/// <summary>
	/// Interaction logic for BrowserWindow.xaml
	/// </summary>
	public partial class BrowserWindow : Window {
		string InitialURL;
		string HomePageURL;
		IntPtr hwnd;

		bool ContinueToPressEnd = false;
		readonly KeyEvent KeyDownEnd = new() {
			FocusOnEditableField = false,
			IsSystemKey = false,
			Type = KeyEventType.KeyDown,
			WindowsKeyCode = 35
		};
		readonly KeyEvent KeyUpEnd = new() {
			FocusOnEditableField = false,
			IsSystemKey = false,
			Type = KeyEventType.KeyUp,
			WindowsKeyCode = 35
		};

		public BrowserWindow() {
			InitializeComponent();
		}

		public BrowserWindow(string URL) {
			InitializeComponent();
			InitialURL = URL;
		}

		public void LoadHomePage() {
			HomePageURL = SettingsWindow.QuerySettingItem("/Settings/Browsing/HomePage");
			Browser.Load(HomePageURL);
		}

		private void BrowserForm_Loaded(object sender, RoutedEventArgs e) {
			if (InitialURL == null) LoadHomePage();
			else Browser.Load(InitialURL);
			URLBox.Text = InitialURL;

			// Add to window list
			var wih = new WindowInteropHelper(this);
			hwnd = wih.Handle;

			// Event handlers
			Browser.AddressChanged += Browser_AddressChanged;
			Browser.FrameLoadStart += Browser_FrameLoadStart;
			Browser.FrameLoadEnd += Browser_FrameLoadEnd;
			Browser.TitleChanged += Browser_TitleChanged;
		}

		private void WriteOnStatusBar(string text) {
			BrowserStatusBar.Dispatcher.Invoke(new Action(() => {
				BrowserStatusBar.Content = text;
			}));
		}

		private void Browser_FrameLoadStart(object sender, FrameLoadStartEventArgs e) {
			WriteOnStatusBar(Properties.Resources.Loading);
		}

		private void Browser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e) {
			WriteOnStatusBar(Properties.Resources.LoadComplete);
			MainWindow.ThisWindow.Dispatcher.Invoke(new Action(() => {
				MainWindow.ModifyBrowserWindowInfo(hwnd, Browser.Address, Browser.Title);
			}));
		}

		private void Browser_AddressChanged(object sender, DependencyPropertyChangedEventArgs e) {
			URLBox.Text = Browser.Address;
			MainWindow.ModifyBrowserWindowInfo(hwnd, Browser.Address, Browser.Title);
		}

		private void Browser_TitleChanged(object sender, DependencyPropertyChangedEventArgs e) {
			Title = Browser.Title;
			MainWindow.ModifyBrowserWindowInfo(hwnd, Browser.Address, Browser.Title);
		}

		private void URLBox_KeyDown(object sender, KeyEventArgs e) {
			if (e.Key == Key.Enter) Browser.Load(URLBox.Text);
		}

		private void BtnHomePage_Click(object sender, RoutedEventArgs e) {
			LoadHomePage();
		}

		private void BtnRefresh_Click(object sender, RoutedEventArgs e) {
			Browser.Reload();
		}

		private void PageTurner_Click(object sender, RoutedEventArgs e) {
			switch (ContinueToPressEnd) {
				case false:
					ContinueToPressEnd = true;
					PageTurner.Content = "⏸️";
					int delay = int.Parse(SettingsWindow.QuerySettingItem("/Settings/Browsing/KeyPressDelay"));
					Task.Run(() => {
						while (ContinueToPressEnd == true) {
							Browser.GetBrowserHost().SendKeyEvent(KeyDownEnd);
							Browser.GetBrowserHost().SendKeyEvent(KeyUpEnd);
							Thread.Sleep(delay);
						}
					});
					break;
				case true:
					ContinueToPressEnd = false;
					PageTurner.Content = "⏬";
					break;
			}
		}

		private void BrowserForm_Unloaded(object sender, RoutedEventArgs e) {
			MainWindow.DeleteBrowserWindowInfo(hwnd);
		}

		private void Menu_WebPage_SaveWebPage_Click(object sender, RoutedEventArgs e) {
			Regex re = new Regex(@"\$[\][\w][^?\w]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
			MatchCollection matches = re.Matches("/Settings/Browsing/HTMLSavePath");
			foreach (Match match in matches) {
				Console.WriteLine("{0} ", match.Value);
            }
		}
	}
}
