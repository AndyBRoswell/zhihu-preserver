using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml;

namespace zhihu_preserver {
	static class Global {
		public static readonly string AppPath = Directory.GetCurrentDirectory();
		public static readonly string CfgPath = AppPath + @"\cfg";
		public static readonly string CachePath = AppPath + @"\cache";
		internal static readonly TreeViewItem CfgTreeViewRoot = new();
		internal static readonly XmlDocument CfgDoc = new();
		internal static XmlNode CfgRoot;
	}
}
