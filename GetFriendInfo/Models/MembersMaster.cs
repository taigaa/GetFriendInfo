using Livet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetFriendInfo.Models
{
    class MembersMaster : NotificationObject
    {
        public ObservableCollection<Member> Members { get; private set; }

        #region singleton
        private static MembersMaster instance = new MembersMaster();
        public static MembersMaster Instance { get { return instance; } }
        #endregion

        private MembersMaster()
        {
            Members = new ObservableMembersCollection<Member>();
            Load();
        }

        /// <summary>
        /// SQLiteのデータを読み込む
        /// </summary>
        public void Load()
        {
            //Members.Clear();// ToDo:直接足すとイベントが走りまくるのでAddRangeしたい
            try
            {
                ((ObservableMembersCollection<Member>)Members).ReplaceRange(SqliteAccesser.SelectMembers());
            }
            catch (Exception)
            {
                /* ここで例外が発生するのはDBファイルが無いかテーブルが無いかなので例外はスローせず初期化処理を行う */
                SqliteAccesser.InitDatabase();
            }
        }
        /// <summary>
        /// SQLiteに追加する
        /// <param name="member">追加したいメンバー</param>
        /// </summary>
        public void Add(Member member)
        {
            SqliteAccesser.InsertMember(member);
            Members.Add(member);
        }
        /// <summary>
        /// SQLiteを更新する
        /// </summary>
        public void Update()
        {
            foreach (Member member in Members)
            {
                var newMemberInfo = MemberInfoBuilder.GetMembersInfo(member.Number, "").First();
                if (!newMemberInfo.Name.Equals(member.Name) || !newMemberInfo.Board.Equals(member.Board))
                {
                    SqliteAccesser.UpdateMember(newMemberInfo);
                }
            }

            /* 要素の変更をReactiveCollectionに伝播するのが面倒なので設定し直して画面は閉じてしまう力技を採用(このコメントは鶏卵問題) */
            Load();
        }
        /// <summary>
        /// SQLiteから削除する
        /// <param name="member">削除したいメンバー</param>
        /// </summary>
        public void Delete(Member member)
        {
            SqliteAccesser.DeleteMember(member);
            Members.Remove(member);
        }
    }

    class ObservableMembersCollection<T> : ObservableCollection<T>
    {
        public void ReplaceRange(IEnumerable<T> collection)
        {
            Items.Clear();
            foreach (var i in collection)
            {
                Items.Add(i);
            }
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}
