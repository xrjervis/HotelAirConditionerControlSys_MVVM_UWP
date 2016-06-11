using Windows.UI.Xaml;
using System.Threading.Tasks;
using Slave.Services.SettingsServices;
using Windows.ApplicationModel.Activation;
using Template10.Controls;
using System;
using Windows.UI.Xaml.Data;
using Slave.Services;
using Slave.Models;
using System.Diagnostics;
using Windows.System.Threading;

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
            //初始化请求消息
            string message = JsonOperation.GetInstance().PraseJson(2, "SYN", 25.0, 29.0, "Medium","Cooling");
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
                    //收到初始化消息时候，约定向中央空调再发送一条状态请求消息
                    message = JsonOperation.GetInstance().PraseJson(2, "req", 25.0, 29.0, "Medium", "Cooling");
                    await TcpClient.GetInstance().SendToTcpServer(message);
                    //启动定期请求线程,参数为间隔时间
                    Threading.CreatePeriodicRequest(5000);
                    break;
                }
            }
            NavigationService.Navigate(typeof(Views.MainPage));
            await Task.CompletedTask;
        }

        
    }
}
