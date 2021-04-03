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
			string HomePageURL = Global.CfgRoot.SelectSingleNode("/Settings/Browsing/HomePage").InnerText;
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
	}
}
