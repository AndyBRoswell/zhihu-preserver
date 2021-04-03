using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zhihu_preserver {
	static class Global {
		public static readonly string AppPath = Directory.GetCurrentDirectory();
		public static readonly Dictionary<string, Dictionary<string, string>> config = new();
	}
}
