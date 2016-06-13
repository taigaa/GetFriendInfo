using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GetFriendInfo.Models
{
    /// <summary>
    /// 表示するボードのHTML(TR要素)を作成する
    /// </summary>
    class BoardHtmlBuilder
    {
        /// <summary>
        /// 渡された部署リスト分のリクエストを非同期に投げて受け取ったObservableCollectionに格納する
        /// </summary>
        /// <param name="boards">部署コードのリスト</param>
        /// <param name="members">抽出したい社員のリスト</param>
        /// <param name="trSource">抽出したTRタグを突っ込むObservableCollection</param>
        /// <returns></returns>
        public static async Task GetTableAsync(IEnumerable<string> boards, IEnumerable<Member> members, ObservableCollection<string> trSource)
        {
            await Task.Run(async () => {
                foreach (var board in boards)
                {
                    await GetInfoAsync(board, members, trSource);
                }
            });
        }

        /// <summary>
        /// 受け取った部署ページにアクセスし受け取った社員リストに含まれる社員情報部分を切り取って受け取ったObservableCollectionに格納する
        /// </summary>
        /// <param name="boardNumber">アクセスする部署ページ番号</param>
        /// <param name="members">社員リスト</param>
        /// <param name="trSource"></param>
        /// <returns></returns>
        private static async Task GetInfoAsync(string boardNumber, IEnumerable<Member> members, ObservableCollection<string> trSource)
        {
            await Task.Run(() => {
                // ホワイトボードはEUCなので文字コードを指定してHTMLを取得する
                var wc = new WebClient();
                wc.Encoding = Encoding.GetEncoding("euc-jp");
                var html = wc.DownloadString(Properties.Settings.Default.BoardServerURI + "/wb/wb.mp?" + boardNumber);

                // パーサに突っ込む
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                // 組織が存在しないためエラーページだった場合はメンバー設定を促して終わる
                if (doc.DocumentNode.SelectSingleNode("//title").InnerText.Equals("White Board Error"))
                {
                    trSource.Add("<tr>異動したメンバーが居るようなのでメンバー設定からUpdateしてください</tr>");
                    return;
                }

                // THタグ(社員名部分)のうち社員リストに含まれている部分のみ残す
                var headers = doc.DocumentNode.SelectNodes("//th")
                .Where(th =>
                {
                    var numberRegex = new Regex(@"(?<number>\d{7})");
                    var matching = numberRegex.Match(th.OuterHtml);
                    return matching.Success && members.Contains(new Member() { Number = matching.Groups["number"].Value }, new MemberComparer());
                })
                .Select(th => new
                {
                    Number = (Func<string>)delegate
                    {
                        var numberRegex = new Regex(@"(?<number>\d{7})");
                        var matching = numberRegex.Match(th.OuterHtml);
                        return matching.Groups["number"].Value;
                    },
                    Header = th.OuterHtml
                    // ToDo:名前のリンクをサーバに書き換えてブラウザ表示させるように
                });

                // TDタグ(在籍状況部分)のうち社員リストに含まれている部分のみ残す
                var statuses = doc.DocumentNode.SelectNodes("//td")
                .Where(td =>
                {
                    var numberRegex = new Regex(@"(?<number>\d{7})");
                    var matching = numberRegex.Match(td.OuterHtml);
                    return matching.Success && members.Contains(new Member() { Number = matching.Groups["number"].Value }, new MemberComparer());
                })
                .Select(td => new
                {
                    Number = (Func<string>)delegate
                    {
                        var numberRegex = new Regex(@"(?<number>\d{7})");
                        var matching = numberRegex.Match(td.OuterHtml);
                        return matching.Groups["number"].Value;
                    },
                    Status = (Func<string>)delegate
                    {
                        // ToDo:在席のリンクをサーバに書き換えてブラウザ表示させるように
                        //return Regex.Replace(td.OuterHtml, @"/(?<img>wb/images/.*\.gif)", Properties.Settings.Default.BoardServerURI + "/${img}");
                        return Regex.Replace(
                            Regex.Replace(td.OuterHtml, @"/(?<img>wb/images/.*\.gif)", Properties.Settings.Default.BoardServerURI + "/${img}"),
                            @"javascript:wbin\('(?<number>\d{7})','(?<flag>\d)'\)",
                            Properties.Settings.Default.BoardServerURI + "/wb/input_top.mp?id=" + boardNumber + "&code=${number}&flag=${flag}");
                    }
                });

                // THタグとTDタグを社員番号をキーとして結合する
                var boardElements = headers.Join(statuses,
                                    h => h.Number.Invoke(),
                                    s => s.Number.Invoke(),
                                    (h, s) => new { Number = h.Number.Invoke(), h.Header, Status = s.Status.Invoke() });

                // 抽出したタグを格納する
                foreach(var tag in boardElements)
                {
                    trSource.Add("<tr>" + tag.Header + tag.Status + "</tr>");
                }
            });
        }
    }
}
