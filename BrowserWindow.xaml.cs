using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using AngleSharp.Text;
using CefSharp;
using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace zhihu_preserver {
	/// <summary>
	/// Interaction logic for BrowserWindow.xaml
	/// </summary>
	public partial class BrowserWindow : Window {
		IntPtr hwnd;

		string InitialURL;
		string HomePageURL;

		readonly Regex PSVarPattern = new(@"\$[\?\w]+[^\?\w]?", RegexOptions.Compiled | RegexOptions.IgnoreCase);

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

			// Proxy Settings
			Cef.UIThreadTaskFactory.StartNew(delegate {
				var RequestContext = Browser.GetBrowserHost().RequestContext;
				var ProxyParam = new Dictionary<string, object> {
					["mode"] = "direct" // No proxy
				};
				string error;
				bool success = RequestContext.SetPreference("proxy", ProxyParam, out error);
			});
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
			MainWindow.ThisWindow.Dispatcher.Invoke(new Action(() => {
				MainWindow.ModifyBrowserWindowInfo(hwnd, Browser.Address, Browser.Title);
			}));
		}

		private void Browser_TitleChanged(object sender, DependencyPropertyChangedEventArgs e) {
			Title = Browser.Title;
			MainWindow.ThisWindow.Dispatcher.Invoke(new Action(() => {
				MainWindow.ModifyBrowserWindowInfo(hwnd, Browser.Address, Browser.Title);
			}));
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
			string SavePath = SettingsWindow.QuerySettingItem("/Settings/Browsing/HTMLSavePath");
			MatchCollection matches = PSVarPattern.Matches(SavePath);
			foreach (Match match in matches) {
				char[] t = match.Value[^1..].ToCharArray();
				string varname;
				if (char.IsLetterOrDigit(t[0]) == false && t[0] != '?') varname = match.Value[1..^1];
				else varname = match.Value[1..];
				SavePath = SavePath.Replace('$' + varname, Global.Const[varname]);
			}
			Task.Run(() => SaveWebPage(SavePath));
		}

		private static string NoAutoRefreshForDownloadedWebPage(string HTML) {
			HtmlParser parser = new();
			IHtmlDocument document = parser.ParseDocument(HTML);
			MainWindow.WriteToLog(Properties.Resources.Information, Properties.Resources.WebPageParseComplete);
			IHtmlCollection<IElement> AutoRefreshScript = document.QuerySelectorAll(@"script[id=js-clientConfig]");
			if (AutoRefreshScript.Length > 0) {
				MainWindow.WriteToLog(Properties.Resources.Information, Properties.Resources.AutoRefreshScriptFound);
				foreach (var element in AutoRefreshScript) { element.Parent.RemoveChild(element); }
				MainWindow.WriteToLog(Properties.Resources.Information, Properties.Resources.AutoRefreshScriptDeleted);
			}
			else MainWindow.WriteToLog(Properties.Resources.Error, Properties.Resources.AutoRefreshScriptNotFound);
			StringWriter writer = new();
			HtmlMarkupFormatter formatter = new();
			document.ToHtml(writer, formatter);
			return writer.ToString();
		}

		private void SaveWebPage(string SavePath) {
			if (SavePath.EndsWith('\\') == false) SavePath += '\\';
			string title = null;
			Browser.Dispatcher.Invoke(() => { title = Browser.Title; });
			MainWindow.WriteToLog(Properties.Resources.Information, Properties.Resources.WebPageDownloadStart + title);
			MainWindow.WriteToLog(Properties.Resources.Information, Properties.Resources.SavePath + SavePath);
			string HTML = Browser.GetBrowser().MainFrame.GetSourceAsync().Result;
			HTML = NoAutoRefreshForDownloadedWebPage(HTML);
			File.WriteAllText(SavePath + title + ".html", HTML);
			MainWindow.WriteToLog(Properties.Resources.Information, Properties.Resources.WebPageDownloadComplete);
		}

		private void Menu_WebPage_Settings_Click(object sender, RoutedEventArgs e) {
			MainWindow.NewSettingsWindow();
		}

		private void Menu_WebPage_Duplicate_Click(object sender, RoutedEventArgs e) {
			MainWindow.NewBrowserWindow(Browser.Address);
		}

		private void Menu_WebPage_NewBlank_Click(object sender, RoutedEventArgs e) {
			MainWindow.NewBrowserWindow("about:blank");
		}

		private void Menu_WebPage_NewHome_Click(object sender, RoutedEventArgs e) {
			MainWindow.NewBrowserWindowHome();
		}
	}
}
