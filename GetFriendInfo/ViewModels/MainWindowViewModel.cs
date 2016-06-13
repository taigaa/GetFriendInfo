using GetFriendInfo.Models;
using Livet;
using Livet.Messaging;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace GetFriendInfo.ViewModels
{
    class MainWindowViewModel: ViewModel
    {
        private ObservableCollection<string> HtmlTrSource;
        public ReactiveProperty<string> HtmlTable { get; set; }
        public ReactiveProperty<string> HtmlToDisplay { get; private set; }
        public ReactiveCommand ReLoad { get; private set; }
        public ReactiveCommand Maintenance { get; private set; }

        public MainWindowViewModel()
        {
            this.HtmlTrSource = new ObservableCollection<string>();
            this.HtmlTrSource.ObserveAddChanged().Subscribe(a => this.HtmlTable.Value += a);

            this.HtmlTable = new ReactiveProperty<string>();
            this.HtmlToDisplay = this.HtmlTable
                .Select(t => Properties.Settings.Default.HtmlHeader + t + Properties.Settings.Default.HtmlFooter)
                .ToReactiveProperty();

            this.ReLoad = new ReactiveCommand();
            this.ReLoad.Subscribe(_ => this.ReloadAsync());

            this.Maintenance = new ReactiveCommand();
            this.Maintenance.Subscribe(_ => this.MaintenanceMember());

            MembersMaster.Instance.Members
                .CollectionChangedAsObservable()
                    .Subscribe(x =>
                    {
                        //Console.WriteLine($"{x.Action}が実行されました");
                        this.ReloadAsync();
                    });
        }

        public void Initialize()
        {
            if (MembersMaster.Instance.Members.Count() == 0)
            {
                System.Windows.MessageBox.Show("表示したいメンバーを登録してください");
                this.MaintenanceMember();
            }
            else
            {
                this.ReloadAsync();
            }
        }

        /// <summary>
        /// リンクがクリックされたらページ遷移はせずにデフォルトブラウザーを開く
        /// </summary>
        public void OpenBrowser()
        {
            // ToDo
        }

        /// <summary>
        /// 部署ごとに非同期にページへアクセスして情報を取得し表示し直す
        /// </summary>
        private async void ReloadAsync()
        {
            this.HtmlTrSource.Clear();
            this.HtmlTable.Value = "";
            var boards = MembersMaster.Instance.Members.Select(m => m.Board).Distinct();

            await BoardHtmlBuilder.GetTableAsync(boards, MembersMaster.Instance.Members, this.HtmlTrSource);
        }

        /// <summary>
        /// 表示する社員を設定する画面を開く
        /// </summary>
        private void MaintenanceMember()
        {
            this.Messenger.Raise(new TransitionMessage("MaintenanceWindowOpen"));
        }
    }
}
