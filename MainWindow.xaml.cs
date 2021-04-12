using CefSharp;
using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace zhihu_preserver {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	class WindowBasicInfoItem {
		public string Address { get; set; }
		public string Title { get; set; }

		public WindowBasicInfoItem(string addr, string title) {
			Address = addr; Title = title;
		}
	}

	class TaskItem {
		public int TypeNo { get; set; }
		public string Type { get; set; }
		public string URL { get; set; }
		public string Title { get; set; }

		public TaskItem(int TypeNo, string Type, string URL) {
			this.TypeNo = TypeNo; this.Type = Type; this.URL = URL;
		}

		public TaskItem(int TypeNo, string Type, string URL, string Title) {
			this.TypeNo = TypeNo; this.Type = Type; this.URL = URL; this.Title = Title;
		}
	}

	public partial class MainWindow : Window {
		internal static readonly MainWindow ThisWindow = Application.Current.MainWindow as MainWindow;
		internal static SortedDictionary<IntPtr, WindowBasicInfoItem> OpenBrowserWindowInfo = new();
		internal static LinkedList<TaskItem> TaskQueue = new();

		internal static TextBlock LogBlock = new() {
			TextWrapping = TextWrapping.Wrap
		};

		public MainWindow() {
			InitializeComponent();
		}

		static internal void ModifyBrowserWindowInfo(IntPtr hwnd, string address, string title) {
			OpenBrowserWindowInfo[hwnd] = new WindowBasicInfoItem(address, title);
			ThisWindow.OpenBrowserWindowList.Items.Refresh();
		}

		static internal void DeleteBrowserWindowInfo(IntPtr hwnd) {
			OpenBrowserWindowInfo.Remove(hwnd);
			ThisWindow.OpenBrowserWindowList.Items.Refresh();
		}

		static internal void WriteToLog(string type, string content) {
			Application.Current.Dispatcher.Invoke(() => {
				LogBlock.Text += "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] [" + type + "] " + content + Environment.NewLine;
			});
		}

		private void WriteOnStatusBar(string content) {
			MainStatusBar.Content = content;
		}

		static internal void NewSettingsWindow() {
			SettingsWindow window = new();
			window.Show();
		}

		static internal void NewBrowserWindowHome() {
			BrowserWindow window = new();
			window.Show();
		}

		static internal void NewBrowserWindow(string URL) {
			BrowserWindow window = new(URL);
			window.Show();
		}

		static internal void AddTask() {
			TaskQueue.AddLast(new TaskItem(ThisWindow.TaskType.SelectedIndex, ThisWindow.TaskType.Text, ThisWindow.TaskURL.Text));
			ThisWindow.TaskListView.Items.Refresh();
		}

		static internal void AddTask(int TypeNo, string Type, string URL, string Title) {
			TaskQueue.AddLast(new TaskItem(TypeNo, Type, URL, Title));
			ThisWindow.TaskListView.Items.Refresh();
		}

		private void Menu_Program_Multiboxing_Click(object sender, RoutedEventArgs e) {
			Process.Start(Global.Const["AppPathname"]);
		}

		private void Menu_Program_Settings_Click(object sender, RoutedEventArgs e) {
			NewSettingsWindow();
		}

		private void Menu_Edit_New_Browser_Window_Home_Click(object sender, RoutedEventArgs e) {
			NewBrowserWindowHome();
		}

		private void Menu_Edit_New_Browser_Window_Blank_Click(object sender, RoutedEventArgs e) {
			NewBrowserWindow("about:blank");
		}

		private void MainForm_Loaded(object sender, RoutedEventArgs e) {
			// Load basic constants
			Global.Const.Add("AppName", AppDomain.CurrentDomain.FriendlyName);
			Global.Const.Add("AppPath", Directory.GetCurrentDirectory());
			Global.Const.Add("AppPathname", Global.Const["AppPath"] + '\\' + Global.Const["AppName"]);
			Global.Const.Add("CachePath", Global.Const["AppPath"] + @"\cache");
			Global.Const.Add("CfgPath", Global.Const["AppPath"] + @"\cfg");
			Global.Const.Add("DefaultCfg", Global.Const["CfgPath"] + @"\config.xml");

			SettingsWindow.LoadSettings(Global.Const["DefaultCfg"]);

			// Load CefSettings
			CefSharpSettings.ShutdownOnExit = true;
			Cef.EnableHighDPISupport();

			CefSettings settings = new();
			// Increase the log severity so CEF outputs detailed information, useful for debugging
			settings.LogSeverity = LogSeverity.Verbose;
			// By default CEF uses an in memory cache, to save cached data e.g. to persist cookies you need to specify a cache path
			// NOTE: The executing user must have sufficient privileges to write to this folder.
			settings.CachePath = Global.Const["CachePath"];
			Cef.Initialize(settings);

			// Controls initialization
			OpenBrowserWindowList.ItemsSource = OpenBrowserWindowInfo.Values;
			TaskListView.ItemsSource = TaskQueue;

			LogBlockSlot.Content = LogBlock;

			WriteToLog(Properties.Resources.Information, Properties.Resources.SystemLoaded);
		}

		private void LogBlockSlot_ScrollChanged(object sender, ScrollChangedEventArgs e) {
			ScrollViewer sv = sender as ScrollViewer;
			bool AutoScrollToEnd = true;
			if (e.ExtentHeightChange == 0) {// user scroll i.e. contents aren't modified
				AutoScrollToEnd = (sv.ScrollableHeight == sv.VerticalOffset); // if scroll bar isn't at the bottom, then don't scroll, because the user may be viewing the contents
			}
			else {// contents are modified
				if (AutoScrollToEnd == true) sv.ScrollToEnd();
			}
			return;
		}

		private void AddTask_Click(object sender, RoutedEventArgs e) {
			AddTask();
		}
	}
}
