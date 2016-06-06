using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Slave.Models;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace Slave.Services
{
    public class JsonOperation
    {
        private static JsonOperation _JsonOperation;
        public static JsonOperation GetInstance()
        {
            if(_JsonOperation == null)
            {
                _JsonOperation = new JsonOperation();
            }
            return _JsonOperation;
        }

        public string PraseJson(int no, string type, int Dtemp, int Ctemp, string speed, string workmode)
        {
            string text =
            "{\"From\":"+no.ToString()+",\"Type\":\""+type+"\",\"Action\":{\"Dtemp\":\""+Dtemp.ToString()+"\",\"Ctemp\":\""+Ctemp.ToString()+"\",\"speed\":\""+speed+"\",\"workmode\":\""+workmode+"\"}}";
            Debug.WriteLine(text);
            return text;
        }

        public bool DeserializeJson(string compareString, string text)
        {
            JObject ackObject = JObject.Parse(text);
            //Debug.WriteLine(ackObject["Type"].ToString().Equals(compareString));
            if (ackObject["Type"].ToString().Equals(compareString))
                return true;
            else
                return false;
        }

        public string DeserializeJson(string text)
        {
            JObject ackObject = JObject.Parse(text);
            Debug.WriteLine(ackObject["Cost"].ToString());
            return ackObject["Cost"].ToString();
        }

        public bool InitSlaveFromJson(string text)
        {
            try
            {
                JObject initObject = JObject.Parse(text);
                if (initObject["Type"].ToString().Equals("ack"))
                {
                    SlaveModel.GetInstance().DTemp = Int32.Parse(initObject["Action"]["Dtemp"].ToString());
                    SlaveModel.GetInstance().CTemp = Int32.Parse(initObject["Action"]["Ctemp"].ToString());
                    SlaveModel.GetInstance().Speed = initObject["Action"]["speed"].ToString();
                    SlaveModel.GetInstance().WorkMode = initObject["Action"]["workmode"].ToString();
                    SlaveModel.GetInstance().Cost = 0.0f;

                    return true;
                }
                else
                    return false;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
