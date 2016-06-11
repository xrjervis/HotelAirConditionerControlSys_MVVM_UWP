using System;
using Windows.System.Threading;
using System.Diagnostics;
using Slave.Models;
using Windows.Foundation;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace Slave.Services
{
    public class Threading
    {
        //请求线程
        public static IAsyncAction asyncRequest;
        //自己变化线程
        public static IAsyncAction asyncSelfChange;
        //定时器
        public static ThreadPoolTimer DelayTimer;

        public static void CreatePeriodicRequest(int d)
        {
            //workItem.Status == AsyncStatus.Canceled
            asyncRequest = ThreadPool.RunAsync(
            async (workItem) =>
            {
                while (true)
                {

                    //延迟时间
                    await Task.Delay(d);

                    Debug.WriteLine("进入正常工作线程");
                    double ctemp = SlaveModel.GetInstance().CTemp;
                    double dtemp = SlaveModel.GetInstance().DTemp;
                    string workMode = SlaveModel.GetInstance().WorkMode;
                    string speed = SlaveModel.GetInstance().Speed;



                    if (workMode == "Cooling")
                    {
                        ctemp -= 1.0;
                        if (ctemp < dtemp)
                            ctemp = dtemp;
                    }
                    else
                    {
                        ctemp += 1.0;
                        if (ctemp > dtemp)
                            ctemp = dtemp;
                    }
                    string message = JsonOperation.GetInstance().PraseJson(2, "req", dtemp, ctemp, speed, workMode);
                    await TcpClient.GetInstance().SendToTcpServer(message);

                    //启动5秒定时器
                    CreateTimer(5000);
                    //等待Ack
                    string JsonString = await TcpClient.GetInstance().WaitTcpServer();
                    Debug.WriteLine(JsonOperation.GetInstance().DeserializeJson("ack", JsonString));
                    //如果收到的消息是ack
                    if (JsonOperation.GetInstance().DeserializeJson("ack", JsonString))
                    {
                        //取消定时器
                        DelayTimer.Cancel();

                        //停止自己变化线程
                        if (asyncSelfChange != null)
                        {
                            asyncSelfChange.Cancel();
                            Debug.WriteLine("停止自己变化");
                        }

                        //累加消费金额
                        SlaveModel.GetInstance().Cost += double.Parse(JsonOperation.GetInstance().DeserializeJson(JsonString));
                        //如果当前正在工作，就更新温度，否则就设置为开始工作
                        if (SlaveModel.GetInstance().IsWorking == true)
                        {
                            SlaveModel.GetInstance().CTemp = ctemp;
                            SlaveModel.GetInstance().DTemp = dtemp;
                        }
                        else
                            SlaveModel.GetInstance().IsWorking = true;

                    }
                }
            });
        }

        public static void CreateTimer(int d)
        {
            //定时器
            TimeSpan delay = TimeSpan.FromSeconds(5);
            DelayTimer = ThreadPoolTimer.CreateTimer(
            (source) =>
            {
                Debug.WriteLine("启动定时器任务");
                //从控机进入待机状态
                SlaveModel.GetInstance().IsWorking = false;
                if (asyncSelfChange == null)
                asyncSelfChange = ThreadPool.RunAsync(
                async (selfChange) =>
                {                     
                    while (true)
                    {
                        //如果工作项还没有开始就调用了 workItem.Cancel 方法，工作项
                        //就会被终止。工作项如果开始工作了，那么它就会一直运行到结束，除非它
                        //支持“取消”操作。
                        //如果让工作项支持“取消”操作，这个工作项应该检查 IAsyncAction.Status 
                        //是否为取消操作，并且当取消的时候执行退出操作
                        Debug.WriteLine("selfChange Status:" + selfChange.Status);
                        Debug.WriteLine("asyncSelfChange Status:" + asyncSelfChange.Status);

                        if (SlaveModel.GetInstance().IsWorking == false)
                        {
                            if (SlaveModel.GetInstance().WorkMode == "Cooling")
                            {
                                if (SlaveModel.GetInstance().CTemp + 0.5 > SlaveModel.GetInstance().InitDTemp)
                                    SlaveModel.GetInstance().CTemp = SlaveModel.GetInstance().InitDTemp;
                                else
                                    SlaveModel.GetInstance().CTemp += 0.5;
                            }
                            else
                            {
                                if (SlaveModel.GetInstance().CTemp - 0.5 < 18)
                                    SlaveModel.GetInstance().CTemp = 18.0;
                                else
                                    SlaveModel.GetInstance().CTemp -= 0.5;
                            }
                        }
                        await Task.Delay(d);
                    }
                });
            }, delay,
             (source) =>
            {
                Debug.WriteLine("定时器完成");
            });
        }

        // A reference to the work item is cached so that we can trigger a
        // cancellation when the user presses the Cancel button.
        //m_workItem = asyncAction;


        #region zhiqian
        /*
        private static ThreadPoolTimer PeriodicTimer;
        public static ThreadPoolTimer GetInstance()
        {
            return PeriodicTimer;
        }

        public static void CreatePeriodicWorkItem(int period)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(period);
            if (PeriodicTimer == null)
            {
                PeriodicTimer = ThreadPoolTimer.CreatePeriodicTimer(
                async (source) =>
                {
                    // 
                    // 判断是否工作
                    // 
                    Debug.WriteLine("进入县城");
                    if (SlaveModel.GetInstance().IsWorking == false)
                    {
                        if (SlaveModel.GetInstance().WorkMode == "Cooling")
                        {
                            if (SlaveModel.GetInstance().CTemp + 0.5 > SlaveModel.GetInstance().InitDTemp)
                                SlaveModel.GetInstance().CTemp = SlaveModel.GetInstance().InitDTemp;
                            else
                                SlaveModel.GetInstance().CTemp += 0.5;
                        }
                        else
                        {
                            if (SlaveModel.GetInstance().CTemp - 0.5 < 18)
                                SlaveModel.GetInstance().CTemp = 18.0;
                            else
                                SlaveModel.GetInstance().CTemp -= 0.5;
                        }
                    }

                    double ctemp = SlaveModel.GetInstance().CTemp;
                    double dtemp = SlaveModel.GetInstance().DTemp;
                    string workMode = SlaveModel.GetInstance().WorkMode;
                    string speed = SlaveModel.GetInstance().Speed;

                    if (speed == "High")
                    {
                        if (workMode == "Cooling")
                            ctemp -= 3.0;
                        else
                            ctemp += 3.0;
                    }
                    else if (speed == "Medium")
                    {
                        if (workMode == "Cooling")
                            ctemp -= 2.0;
                        else
                            ctemp += 2.0;
                    }
                    else if (speed == "Low")
                    {
                        if (workMode == "Cooling")
                            ctemp -= 1.0;
                        else
                            ctemp += 1.0;
                    }

                    if (workMode == "Cooling")
                    {
                        if (ctemp < dtemp)
                            ctemp = dtemp;
                    }
                    else
                    {
                        if (ctemp > dtemp)
                            ctemp = dtemp;
                    }
                    string message = JsonOperation.GetInstance().PraseJson(2, "req", dtemp, ctemp, speed, workMode);
                    await TcpClient.GetInstance().SendToTcpServer(message);
                    //等待Ack
                    while (true)
                    {
                        SlaveModel.GetInstance().IsWorking = false;
                        string JsonString = await TcpClient.GetInstance().WaitTcpServer();
                        Debug.WriteLine(JsonOperation.GetInstance().DeserializeJson("ack", JsonString));
                        //如果收到的消息是ack就跳出循环
                        if (JsonOperation.GetInstance().DeserializeJson("ack", JsonString))
                        {
                            SlaveModel.GetInstance().Cost += double.Parse(JsonOperation.GetInstance().DeserializeJson(JsonString));
                            SlaveModel.GetInstance().CTemp = ctemp;
                            SlaveModel.GetInstance().DTemp = dtemp;
                            SlaveModel.GetInstance().IsWorking = true;
                            break;
                        }
                    }

                }, timeSpan);
            }
            else
                PeriodicTimer.Cancel();
        }
        */
        #endregion
    }
}
