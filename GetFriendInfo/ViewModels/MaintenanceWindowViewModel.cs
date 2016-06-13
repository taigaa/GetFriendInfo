using GetFriendInfo.Models;
using Livet;
using Livet.Messaging;
using Livet.Messaging.Windows;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetFriendInfo.ViewModels
{
    class MaintenanceWindowViewModel : ViewModel
    {
        public ReadOnlyReactiveCollection<Member> Members { get; private set; }
        public ReactiveProperty<Member> InputMember { get; private set; }
        public ReactiveProperty<Member> SelectedMember { get; private set; }
        public ReactiveProperty<bool> DeleteEnabled { get; private set; }

        public MaintenanceWindowViewModel()
        {
            this.Members = MembersMaster.Instance.Members
                .ToReadOnlyReactiveCollection(m => m)
                .AddTo(this.CompositeDisposable);

            this.InputMember = new ReactiveProperty<Member>();

            this.SelectedMember = new ReactiveProperty<Member>();

            this.DeleteEnabled = this.SelectedMember
                .Select(x => x != null)
                .ToReactiveProperty()
                .AddTo(this.CompositeDisposable);
        }

        public void Initialize()
        {
        }

        public void New()
        {
            this.InputMember.Value = new Member();
            using (var vm = new EditWindowViewModel(this.InputMember.Value))
            {
                this.Messenger.Raise(new TransitionMessage(vm, "EditWindowOpen"));
            }

            if (string.IsNullOrWhiteSpace(this.InputMember.Value.Number))
            {
                return;
            }

            if (MembersMaster.Instance.Members.Select(m => m).Contains(this.InputMember.Value, new MemberComparer()))
            {
                System.Windows.MessageBox.Show("既に登録されています");
            }
            else
            {
                MembersMaster.Instance.Add(this.InputMember.Value);
            }
        }

        public void Update(ConfirmationMessage message)
        {
            if (message.Response != true) { return; }

            MembersMaster.Instance.Update();
            this.Messenger.Raise(new WindowActionMessage(WindowAction.Close, "Close"));
        }

        public void Delete(ConfirmationMessage message)
        {
            if (message.Response != true) { return; }

            MembersMaster.Instance.Delete(this.SelectedMember.Value);
        }
    }
}
