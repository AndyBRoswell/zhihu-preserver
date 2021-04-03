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
		public BrowserWindow() {
			InitializeComponent();
			XmlNode HomePageNode = Global.CfgRoot.SelectSingleNode("/Settings/Browsing/HomePage");
			ChromiumWebBrowser Browser = new(HomePageNode.FirstChild.Value);
			BrowserWindowGrid.Children.Add(Browser);
			Grid.SetRow(Browser, 2);
		}
	}
}
