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

namespace zhihu_preserver {
	/// <summary>
	/// Interaction logic for BrowserWindow.xaml
	/// </summary>
	public partial class BrowserWindow : Window {
		readonly ObservableCollection<WebPageTab> tab = new();
		public BrowserWindow() {
			InitializeComponent();
			tab.Add(new WebPageTab("google.com"));
			tab.Add(new WebPageTab("bing.com"));
			tab.Add(new WebPageTab("baidu.com"));
			DataContext = tab;
		}
	}
	public class WebPageTab {
		public string Header { get; set; }
		//public ObservableCollection<ChromiumWebBrowser> Browser { get; } = new ObservableCollection<ChromiumWebBrowser>();
		public string Address { get; set; }
		public WebPageTab() { Header = Address = "about:blank"; }
		public WebPageTab(string URL) { Header = Address = URL; }
	}
}
