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
	/// Interaction logic for Settings.xaml
	/// </summary>
	public partial class SettingsWindow : Window {
		internal readonly TreeViewItem CfgTreeRoot = new();
		internal readonly XmlDocument CfgDoc = new();
		public SettingsWindow() {
			InitializeComponent();
			CfgDoc.Load(Global.AppPath + @"\cfg\config.xml");
		}
		public static void LoadSettings(string ymlFile) {
			
		}
		public static void SaveSettings(string ymlFile) {
			
		}
	}
}
