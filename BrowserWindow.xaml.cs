using CefSharp;
using CefSharp.Wpf;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.Xml;

namespace zhihu_preserver {
	/// <summary>
	/// Interaction logic for BrowserWindow.xaml
	/// </summary>
	public partial class BrowserWindow : Window {
		string InitialURL;
		string HomePageURL;

		internal bool ContinueToPressEnd = false;
		internal readonly KeyEvent KeyDownEnd = new() {
			FocusOnEditableField = false,
			IsSystemKey = false,
			Type = KeyEventType.KeyDown,
			WindowsKeyCode = 35
		};
		internal readonly KeyEvent KeyUpEnd = new() {
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
			HomePageURL = Global.CfgRoot.SelectSingleNode("/Settings/Browsing/HomePage").InnerText;
			Browser.Load(HomePageURL);
		}

		private void BrowserForm_Loaded(object sender, RoutedEventArgs e) {
			if (InitialURL == null) LoadHomePage();
			else Browser.Load(InitialURL);
			URLBox.Text = InitialURL;

			// Add to window list
			//var wih = new WindowInteropHelper(this);
			//MainWindow.OpenBrowserHwnd.Add(wih.Handle);
			//MainWindow.GetWindow().WebPageAddress.Items.Add("about:blank");
			//MainWindow.WebPageTitle.Items.Add("about:blank");

			// Event handlers
			Browser.AddressChanged += Browser_AddressChanged;
			Browser.FrameLoadStart += Browser_FrameLoadStart;
			Browser.FrameLoadEnd += Browser_FrameLoadEnd;
		}

		private void Browser_FrameLoadStart(object sender, FrameLoadStartEventArgs e) {
			BrowserStatusBar.Dispatcher.Invoke(new Action(() => {
				BrowserStatusBar.Content = Properties.Resources.Loading;
			}));
		}

		private void Browser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e) {
			BrowserStatusBar.Dispatcher.Invoke(new Action(() => {
				BrowserStatusBar.Content = Properties.Resources.LoadComplete;
			}));
		}

		private void Browser_AddressChanged(object sender, DependencyPropertyChangedEventArgs e) {
			URLBox.Text = Browser.Address;
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
					int delay = int.Parse(Global.CfgRoot.SelectSingleNode("/Settings/Browsing/KeyPressDelay").InnerText);
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
	}
}
