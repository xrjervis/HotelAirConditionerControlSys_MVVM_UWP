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
using System.Diagnostics;

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

            string message = JsonOperation.GetInstance().PraseJson(2, "SYN", 18, 25, "高","制冷");
            //建立TCP连接，异步
            await TcpClient.GetInstance().Start();
            //向服务器请求链接
            await TcpClient.GetInstance().SendToTcpServer(message);
            //等待Ack
            while(true)
            {
                string JsonString = await TcpClient.GetInstance().WaitTcpServer();
                Debug.WriteLine(JsonOperation.GetInstance().DeserializeJson("ack", JsonString));
                //如果收到的消息是ack且能够成功初始化就跳出循环
                if (JsonOperation.GetInstance().DeserializeJson("ack", JsonString) &&
                    JsonOperation.GetInstance().InitSlaveFromJson(JsonString))
                {
                    break;
                }
            }


            NavigationService.Navigate(typeof(Views.MainPage));
            await Task.CompletedTask;
        }
    }
}
