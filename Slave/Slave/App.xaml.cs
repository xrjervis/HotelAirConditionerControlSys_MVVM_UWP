using Windows.UI.Xaml;
using System.Threading.Tasks;
using Slave.Services.SettingsServices;
using Windows.ApplicationModel.Activation;
using Template10.Controls;
using Template10.Common;
using System;
using System.Linq;
using Windows.UI.Xaml.Data;
using Slave.Services;
using Slave.Models;
using System.Diagnostics;
using Windows.System.Threading;
using Windows.UI.Core;

namespace Slave
{
    /// Documentation on APIs used in this page:
    /// https://github.com/Windows-XAML/Template10/wiki

    [Bindable]
    public sealed partial class App : Template10.Common.BootStrapper
    {
        public App()
        {
            InitializeComponent();

            SplashFactory = (e) => new Views.Splash(e);

            #region App settings

            var _settings = SettingsService.Instance;
            RequestedTheme = _settings.AppTheme;
            CacheMaxDuration = _settings.CacheMaxDuration;
            ShowShellBackButton = _settings.UseShellBackButton;

            #endregion
        }

        public override async Task OnInitializeAsync(IActivatedEventArgs args)
        {
            if (Window.Current.Content as ModalDialog == null)
            {
                // create a new frame 
                var nav = NavigationServiceFactory(BackButton.Attach, ExistingContent.Include);

                // create modal root
                Window.Current.Content = new ModalDialog
                {
                    DisableBackButtonWhenModal = true,
                    Content = new Views.Shell(nav),
                    ModalContent = new Views.Busy(),
                };
            }
            await Task.CompletedTask;
        }

        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            // long-running startup tasks go here
            await Task.Delay(3000);

            string message = JsonOperation.GetInstance().PraseJson(2, "SYN", 25, 29, "Medium","Cooling");
            //建立TCP连接，异步
            await TcpClient.GetInstance().Start();
            //向服务器请求链接
            await TcpClient.GetInstance().SendToTcpServer(message);
            //等待Ack
            while(true)
            {
                string JsonString = await TcpClient.GetInstance().WaitTcpServer();
                //Debug.WriteLine(JsonOperation.GetInstance().InitSlaveFromJson(JsonString));
                //如果收到的消息是ack且能够成功初始化就跳出循环
                if (JsonOperation.GetInstance().InitSlaveFromJson(JsonString))
                {
                    message = JsonOperation.GetInstance().PraseJson(2, "req", 25, 29, "Medium", "Cooling");
                    await TcpClient.GetInstance().SendToTcpServer(message);
                    //启动定期工作项目
                    CreatePeriodicWorkItem(15);
                    break;
                }
            }
            NavigationService.Navigate(typeof(Views.MainPage));
            await Task.CompletedTask;
        }

        public void CreatePeriodicWorkItem(int period)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(period);

            ThreadPoolTimer PeriodicTimer = ThreadPoolTimer.CreatePeriodicTimer(
                async (source) =>
                {
                    // 
                    // TODO: Work
                    // 
                    int ctemp = SlaveModel.GetInstance().CTemp;
                    int dtemp = SlaveModel.GetInstance().DTemp;
                    bool isWorking = SlaveModel.GetInstance().IsWorking;
                    string workMode = SlaveModel.GetInstance().WorkMode;
                    string speed = SlaveModel.GetInstance().Speed;

                    if(isWorking)
                    {
                        if (speed == "High")
                        {
                            if (workMode == "Cooling")
                                ctemp -= 3;
                            else
                                ctemp += 3;
                        }
                        else if (speed == "Medium")
                        {
                            if (workMode == "Cooling")
                                ctemp -= 2;
                            else
                                ctemp += 2;
                        }
                        else if (speed == "Low")
                        {
                            if (workMode == "Cooling")
                                ctemp -= 1;
                            else
                                ctemp += 1;
                        }
                    }
                    else
                    {
                        if (workMode == "Cooling")
                            ctemp += 1;
                        else
                            ctemp -= 1;
                    }

                    if (workMode == "Cooling" && ctemp <= dtemp)
                    {
                        isWorking = false;
                        ctemp = dtemp;
                    }
                        
                    else if (workMode == "Heating" && ctemp >= dtemp)
                    {
                        isWorking = false;
                        ctemp = dtemp;
                    }   
                    else
                        isWorking = true;

                    string message = JsonOperation.GetInstance().PraseJson(2, "req", dtemp, ctemp, speed, workMode);
                    await TcpClient.GetInstance().SendToTcpServer(message);
                    //等待Ack
                    while (true)
                    {
                        string JsonString = await TcpClient.GetInstance().WaitTcpServer();
                        Debug.WriteLine(JsonOperation.GetInstance().DeserializeJson("ack", JsonString));
                        //如果收到的消息是ack就跳出循环
                        if (JsonOperation.GetInstance().DeserializeJson("ack", JsonString))
                        {
                            SlaveModel.GetInstance().Cost = float.Parse(JsonOperation.GetInstance().DeserializeJson(JsonString));
                            SlaveModel.GetInstance().CTemp = ctemp;
                            SlaveModel.GetInstance().DTemp = dtemp;
                            SlaveModel.GetInstance().IsWorking = isWorking;
                            break;
                        }
                    }

                }, timeSpan);
        }
    }
}
