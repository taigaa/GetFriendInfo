using Livet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetFriendInfo.Models
{
    /// <summary>
    /// メンバーモデル
    /// </summary>
    public class Member : NotificationObject
    {
        public long Id { get; set; }
        private string _Number;
        public string Number
        {
            get
            {
                return _Number;
            }
            set
            {
                if (_Number == value)
                {
                    return;
                }
                _Number = value;
                RaisePropertyChanged("Number");
            }
        }
        private string _Board;
        public string Board
        {
            get
            {
                return _Board;
            }
            set
            {
                if (_Board == value)
                {
                    return;
                }
                _Board = value;
                RaisePropertyChanged("Board");
            }
        }
        public string Name { get; set; }
        public string Email { get; set; }
    }

    /// <summary>
    /// 社員番号のみで判定するメンバーモデルのComparer
    /// </summary>
    class MemberComparer : EqualityComparer<Member>
    {
        public override bool Equals(Member x, Member y)
        {
            return x.Number == y.Number;
        }

        public override int GetHashCode(Member obj)
        {
            return obj.Number.GetHashCode();
        }
    }
}
