using System.Collections.Generic;
using System.Text;
using System.Windows.Browser;

namespace GazRouter.Common.Helpers
{
    public static class ExportHelper
    {
        public static string CreateHtmlRow(params object[] values)
        {
            var sb = new StringBuilder();
            foreach (var value in values)
            {
                sb.AppendFormat("<td>{0}</td>", HttpUtility.HtmlEncode(value.ToString()));
            }
            return string.Format("<tr>{0}</tr>", sb);
        }

        public static string CreateHtmlRowArr(IEnumerable<object> values)
        {
            var sb = new StringBuilder();
            foreach (var value in values)
            {
                sb.AppendFormat("<td>{0}</td>", HttpUtility.HtmlEncode(value.ToString()));
            }
            return string.Format("<tr>{0}</tr>", sb);
        }

        public static string CreateHtmlRowEx(IEnumerable<object> values, string color, int margin, bool[] bolds, int[] sizes)
        {
            var sb = new StringBuilder();
            int i = 0;
            foreach (var value in values)
            {
                if (value == null)
                {
                    sb.AppendFormat("<td bgcolor=\"{0}\"></td>", color);
                    continue;
                }

                var style = string.Format(" style=font-weight:{0};font-size:{1}px;margin-left:{2}px;", (bolds[i] ? "bold" : "normal"), sizes[i], i == 0 ? margin : 0);
                sb.AppendFormat("<td bgcolor=\"{0}\"{1}>{2}{3}</td>", color, style, i == 0 ? "'" : string.Empty, HttpUtility.HtmlEncode(value.ToString()));
                ++i;
            }
            return string.Format("<tr>{0}</tr>", sb);
        }
    }
}