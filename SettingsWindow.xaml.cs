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
using YamlDotNet.Serialization;

namespace zhihu_preserver {
	/// <summary>
	/// Interaction logic for Settings.xaml
	/// </summary>
	public partial class SettingsWindow : Window {
		internal TreeViewItem ConfigRoot = new();
		public SettingsWindow() {
			InitializeComponent();
			ConfigRoot.Header = "Layer 0";
			ConfigRoot.Items.Add(new TreeViewItem() { Header = "Layer 1" });
			Settings.Items.Add(ConfigRoot);
		}
		public static void LoadSettings(string ymlFile) {
			
		}
		public static void SaveSettings(string ymlFile) {
			
		}
	}
}
