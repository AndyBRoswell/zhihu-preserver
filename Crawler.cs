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
using System.Threading.Tasks;

namespace zhihu_preserver {
	static class Crawler {
		static HtmlParser parser = new();

		internal static IHtmlCollection<IElement> CrawlQuestion(TaskItem task) {
			return null;
		}

		internal static IHtmlCollection<IElement> CrawlQuestion(ChromiumWebBrowser browser) {
			//IHtmlDocument SingleQuestionDoc = browser.GetMainFrame().;
			return null;
        }

		internal static IHtmlCollection<IElement> CrawlAnswer(TaskItem task) {
			return null;
		}

		internal static IHtmlCollection<IElement> CrawlAnswerComments(TaskItem task) {
			return null;
		}
	}
}
