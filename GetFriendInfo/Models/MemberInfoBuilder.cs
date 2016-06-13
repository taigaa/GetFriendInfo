using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace GetFriendInfo.Models
{
    class MemberInfoBuilder
    {
        public static IEnumerable<Member> GetMembersInfo(string number, string name)
        {
            var wc = new WebClient();
            wc.Encoding = Encoding.GetEncoding("euc-jp");
            var html = wc.DownloadString("http://www2.kh.jip.co.jp/whois/index.mp?do_search=%B8%A1%BA%F7&" + "code=" + number + "&name=" + HttpUtility.UrlEncode(name));

            // パーサに突っ込む
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var tables = doc.DocumentNode.SelectNodes("//table[@border='1']");
            if (tables == null)
            {
                return Enumerable.Empty<Member>();
            }

            var tableFormat = new Regex(
                @"<th>(\d{7})</th><th><a class=""thl"" href="".*?\?c=\d{7}"">(.*?)</a></th>.*?<a href=""/wb/wb.mp\?keywords=\d{10}&amp;mark=\d{7}"">(\d{10})</a>.*?<th>在席状態</th><td>.*?</td></tr>",
                RegexOptions.Singleline);

            var members = tables.Select(t => {
                return tableFormat.Matches(t.InnerHtml).Cast<Match>().Select(m => {
                    //return new
                    //{
                    //    Number = m.Groups[1].Value,
                    //    Board = m.Groups[3].Value,
                    //    Name = m.Groups[2].Value
                    //};
                    return new Member()
                    {
                        Number = m.Groups[1].Value,
                        Board = m.Groups[3].Value,
                        Name = m.Groups[2].Value
                    };
                });
            });

            return members.First();
        }
    }
}
