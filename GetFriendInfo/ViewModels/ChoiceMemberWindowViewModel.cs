using GetFriendInfo.Models;
using Livet;
using Livet.Messaging.Windows;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetFriendInfo.ViewModels
{
    class ChoiceMemberWindowViewModel : ViewModel
    {
        public ReadOnlyReactiveCollection<Member> Members { get; private set; }
        public ReactiveProperty<Member> SelectedMember { get; private set; }
        public ReactiveCommand SetCommand { get; private set; }

        public ChoiceMemberWindowViewModel(IEnumerable<Member> members, Member selected)
        {
            var observableMembers = new ObservableCollection<Member>();
            members.ToList().ForEach(m => observableMembers.Add(m));
            this.Members = observableMembers
                .ToReadOnlyReactiveCollection(m => m)
                .AddTo(this.CompositeDisposable);

            this.SelectedMember = new ReactiveProperty<Member>();

            this.SetCommand = this.SelectedMember
                .Select(x => x != null)
                .ToReactiveCommand();
            this.SetCommand.Subscribe(_ => this.Set(selected));
        }

        public void Initialize() { }

        public void Set(Member selected)
        {
            selected.Number = this.SelectedMember.Value.Number;
            selected.Board = this.SelectedMember.Value.Board;
            selected.Name = this.SelectedMember.Value.Name;
            this.Messenger.Raise(new WindowActionMessage(WindowAction.Close, "Close"));
        }
    }
}
