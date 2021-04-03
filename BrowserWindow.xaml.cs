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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;

namespace zhihu_preserver {
	/// <summary>
	/// Interaction logic for BrowserWindow.xaml
	/// </summary>
	public partial class BrowserWindow : Window {
		internal ChromiumWebBrowser Browser;
		string HomePageURL;

		internal bool ContinueToPressEnd = false;
		internal readonly KeyEvent KeyDownEnd = new() {
			FocusOnEditableField = false,
			IsSystemKey = false,
			Type = KeyEventType.KeyDown,
			WindowsKeyCode = (int)Key.End
		};
		internal readonly KeyEvent KeyUpEnd = new() {
			FocusOnEditableField = false,
			IsSystemKey = false,
			Type = KeyEventType.KeyUp,
			WindowsKeyCode = (int)Key.End
		};

		public BrowserWindow() {
			InitializeComponent();

			HomePageURL = Global.CfgRoot.SelectSingleNode("/Settings/Browsing/HomePage").InnerText;

			// Show browser
			Browser = new(HomePageURL);
			URLBox.Text = HomePageURL;
			BrowserWindowGrid.Children.Add(Browser);
			Grid.SetRow(Browser, 2);

			// Event handlers
			Browser.AddressChanged += Browser_AddressChanged;
			Browser.FrameLoadStart += Browser_FrameLoadStart;
			Browser.FrameLoadEnd += Browser_FrameLoadEnd;
		}

		private void Browser_FrameLoadStart(object sender, FrameLoadStartEventArgs e) {
			//BrowserStatusBar.Content = Properties.Resources.Loading;
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
			HomePageURL = Global.CfgRoot.SelectSingleNode("/Settings/Browsing/HomePage").InnerText;
			Browser.Load(HomePageURL);
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
							Browser.GetBrowser().GetHost().SendKeyEvent(KeyDownEnd);
							Browser.GetBrowser().GetHost().SendKeyEvent(KeyUpEnd);
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
