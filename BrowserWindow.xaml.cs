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

			// Event handlers
			
			// Show browser
			Browser = new(HomePageURL);
			URLBox.Text = HomePageURL;
			BrowserWindowGrid.Children.Add(Browser);
			Grid.SetRow(Browser, 2);
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
