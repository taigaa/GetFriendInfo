using GetFriendInfo.Models;
using Livet;
using Livet.Messaging;
using Livet.Messaging.Windows;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Linq;
using System.Reactive.Linq;

namespace GetFriendInfo.ViewModels
{
    class EditWindowViewModel : ViewModel
    {
        private Member _Origin;
        private Member _Member;
        public Member EditTarget {
            get { return this._Member; }
            private set
            {
                if (_Origin == value) return;
                _Origin = value;
                _Member = new Member();
                RaisePropertyChanged("EditTarget");
            }
        }
        public ReactiveCommand AddCommand { get; private set; }
        public ReactiveCommand GetMembers { get; private set; }
        private Member serchResultMember;

        public EditWindowViewModel(Member edit)
        {
            this.EditTarget = edit;

            this.AddCommand = this.EditTarget.ObserveProperty(m => m.Board)
                .Select(b => b != null)
                .ToReactiveCommand();
            this.AddCommand.Subscribe(_ => this.Add());

            this.GetMembers = new ReactiveCommand();
            // テキストボックスの状態に応じてコマンドを発行したいけどそうすると入力途中でも処理が走るのでしゃぁなしでLostFocus
            //this.GetMembers = this.EditTarget.ObserveProperty(m => m)
            //    .Select(m => (!(string.IsNullOrWhiteSpace(m.Number) || !string.IsNullOrWhiteSpace(m.Name))) && string.IsNullOrWhiteSpace(m.Board))
            //    .ToReactiveCommand();
            this.GetMembers.Subscribe(_ => this.SetMemberInfo());
        }

        public void Initialize()
        {

        }

        public void SetMemberInfo()
        {
            /* LostFocusイベントのために条件をごりごり書く */

            // 入力なしなら何もしない
            if (string.IsNullOrWhiteSpace(this._Member.Number) && (string.IsNullOrWhiteSpace(this._Member.Name)))
            {
                return;
            }

            // 2重検索防止(追加ボタン、閉じるででもう1回動いてしまう)
            if (this.serchResultMember != null
                && this._Member.Number.Equals(this.serchResultMember.Number)
                && this._Member.Name.Equals(this.serchResultMember.Name)
                && !string.IsNullOrWhiteSpace(this.serchResultMember.Board))
            {
                return;
            }

            // 社員検索結果の件数に応じて情報をセット
            var members = MemberInfoBuilder.GetMembersInfo(this._Member.Number, this._Member.Name);
            switch (members.Count())
            {
                case 0:
                    break;

                case 1:
                    var member = members.First();
                    this._Member.Number = member.Number;
                    this._Member.Board = member.Board;
                    this._Member.Name = member.Name;
                    RaisePropertyChanged("EditTarget");

                    this.serchResultMember = new Member()
                    {
                        Number = member.Number,
                        Board = member.Board,
                        Name = member.Name
                    };

                    break;

                default:
                    var selected = new Member();
                    using (var vm = new ChoiceMemberWindowViewModel(members, selected))
                    {
                        this.Messenger.Raise(new TransitionMessage(vm, "ChoiceMemberWindowOpen"));
                    }

                    this._Member.Number = selected.Number;
                    this._Member.Board = selected.Board;
                    this._Member.Name = selected.Name;
                    RaisePropertyChanged("EditTarget");

                    this.serchResultMember = new Member()
                    {
                        Number = selected.Number,
                        Board = selected.Board,
                        Name = selected.Name
                    };

                    break;
            }
        }

        public void Add()
        {
            _Origin.Id = _Member.Id;
            _Origin.Number = _Member.Number;
            _Origin.Board = _Member.Board;
            _Origin.Name = _Member.Name;
            _Origin.Email = _Member.Email;
            this.Messenger.Raise(new WindowActionMessage(WindowAction.Close, "Close"));
        }
    }
}
