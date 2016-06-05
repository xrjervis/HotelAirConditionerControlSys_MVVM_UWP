using Template10.Mvvm;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using Slave;
using Slave.Models;
using Slave.Services;
using Windows.UI.Xaml.Data;

namespace Slave.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public SlaveModel Slave
        {
            get { return SlaveModel.GetInstance(); }
        }


        public MainPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                Value = "Designtime value";
            }
        }
        
        #region 属性
        string _Value ="从控机";
        public string Value { get { return _Value; } set { Set(ref _Value, value); } }
        string _BusyText = "请稍后......";
        public string BusyText
        {
            get { return _BusyText; }
            set
            {
                Set(ref _BusyText, value);
                _ChangeSpeedCommand.RaiseCanExecuteChanged();
            }
        }
        #endregion

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            if (suspensionState.Any())
            {
                Value = suspensionState[nameof(Value)]?.ToString();
            }
            await Task.CompletedTask;
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            if (suspending)
            {
                suspensionState[nameof(Value)] = Value;
            }
            await Task.CompletedTask;
        }

        public override async Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            args.Cancel = false;
            await Task.CompletedTask;
        }

        public void GotoSettings() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 0);

        public void GotoPrivacy() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 1);

        public void GotoAbout() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 2);

        #region ChangeSpeedCommand:改变风速命令
        DelegateCommand _ChangeSpeedCommand;
        public DelegateCommand ChangeSpeedCommand
            => _ChangeSpeedCommand ?? (_ChangeSpeedCommand = new DelegateCommand(async () =>
            {
                Views.Busy.SetBusy(true, _BusyText);

                string speed = SlaveModel.GetInstance().Speed;
                int dtemp = SlaveModel.GetInstance().DTemp;
                int ctemp = SlaveModel.GetInstance().CTemp;
                string workmode = SlaveModel.GetInstance().WorkMode;

                switch (speed)
                {
                    case "低":
                        speed = "中";
                        break;
                    case "中":
                        speed = "高";
                        break;
                    case "高":
                        speed = "低";
                        break;
                }

                string message = JsonOperation.GetInstance().PraseJson(2, "req", dtemp, ctemp, speed, workmode);
                //向服务器请求链接
                await TcpClient.GetInstance().SendToTcpServer(message);
                //等待Ack
                while (true)
                {
                    string JsonString = await TcpClient.GetInstance().WaitTcpServer();
                    if (JsonOperation.GetInstance().DeserializeJson("ack", JsonString))
                    {
                        SlaveModel.GetInstance().Cost = float.Parse(JsonOperation.GetInstance().DeserializeJson(JsonString));
                        SlaveModel.GetInstance().Speed = speed;
                        break;
                    }
                }
                Views.Busy.SetBusy(false);
            }, () => !string.IsNullOrEmpty(BusyText)));
        #endregion

        #region ChangeWorkModeCommand:改变工作模式命令
        DelegateCommand _ChangeWorkModeCommand;
        public DelegateCommand ChangeWorkModeCommand
            => _ChangeWorkModeCommand ?? (_ChangeWorkModeCommand = new DelegateCommand(async () =>
            {
                Views.Busy.SetBusy(true, _BusyText);

                string speed = SlaveModel.GetInstance().Speed;
                int dtemp = SlaveModel.GetInstance().DTemp;
                int ctemp = SlaveModel.GetInstance().CTemp;
                string workmode = SlaveModel.GetInstance().WorkMode;

                switch (workmode)
                {
                    case "制冷":
                        workmode = "制热";
                        break;
                    case "制热":
                        workmode = "制冷";
                        break;
                }

                string message = JsonOperation.GetInstance().PraseJson(2, "req", dtemp, ctemp, speed, workmode);
                //向服务器请求链接
                await TcpClient.GetInstance().SendToTcpServer(message);
                //等待Ack
                while (true)
                {
                    string JsonString = await TcpClient.GetInstance().WaitTcpServer();
                    if (JsonOperation.GetInstance().DeserializeJson("ack", JsonString))
                    {
                        SlaveModel.GetInstance().Cost = float.Parse(JsonOperation.GetInstance().DeserializeJson(JsonString));
                        SlaveModel.GetInstance().WorkMode = workmode;
                        break;
                    }
                }
                Views.Busy.SetBusy(false);
            }, () => !string.IsNullOrEmpty(BusyText)));
        #endregion
    }
}

