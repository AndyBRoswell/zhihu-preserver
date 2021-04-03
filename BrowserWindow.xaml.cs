using CefSharp;
using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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
		public BrowserWindow() {
			InitializeComponent();

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
			string HomePageURL = Global.CfgRoot.SelectSingleNode("/Settings/Browsing/HomePage").InnerText;

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
			string HomePageURL = Global.CfgRoot.SelectSingleNode("/Settings/Browsing/HomePage").InnerText;
			Browser.Load(HomePageURL);
		}

		private void BtnRefresh_Click(object sender, RoutedEventArgs e) {
			Browser.Reload();
		}
	}
}
