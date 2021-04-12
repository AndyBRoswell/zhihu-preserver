using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zhihu_preserver {
	static class DriveAccessor {
		internal static string NoIllegarCharOfFilename(string Filename) {
			string newFileName;
			newFileName = Filename.Replace('\\', '_')
									.Replace('/', '_')
									.Replace(':', '_')
									.Replace('*', '_')
									.Replace('?', '_')
									.Replace('\"', '_')
									.Replace('<', '_')
									.Replace('>', '_')
									.Replace('|', '_');
			return newFileName;
		}
	}
}
