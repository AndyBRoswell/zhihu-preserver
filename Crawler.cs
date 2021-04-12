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
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace zhihu_preserver {
	static class Crawler {
		internal static IHtmlCollection<IElement> CrawlQuestion(TaskItem task) {
			return null;
		}

		internal static IHtmlCollection<IElement> CrawlQuestion(ChromiumWebBrowser browser) {
			// Get HTML and parse.
			Task<string> GetHTML = browser.GetMainFrame().GetSourceAsync();
			GetHTML.Wait();
			HtmlParser parser = new();
			var document = parser.ParseDocument(GetHTML.Result);



			return null;
		}

		internal static IHtmlCollection<IElement> CrawlAnswer() {
			return null;
		}

		internal static IHtmlCollection<IElement> CrawlAnswerComments() {
			return null;
		}
	}
}
