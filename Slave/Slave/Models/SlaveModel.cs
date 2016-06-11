using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;

namespace Slave.Models
{
    public class SlaveModel : ViewModelBase
    {
        #region 属性
        /// <summary>
        /// 目标温度
        /// </summary>
        private double _dTemp;
        public double DTemp
        {
            get { return _dTemp; }
            set
            {
                Set(ref _dTemp, value);
            }
        }
        /// <summary>
        /// 当前温度
        /// </summary>
        private double _cTemp;
        public double CTemp
        {
            get { return _cTemp; }
            set
            {
                Set(ref _cTemp, value);
            }
        }
        /// <summary>
        /// 当前费用
        /// </summary>
        private double _cost;
        public double Cost
        {
            get { return _cost; }
            set
            {
                Set(ref _cost, value);
            }
        }
        /// <summary>
        /// 当前风速
        /// </summary>
        private string _speed;
        public string Speed
        {
            get { return _speed; }
            set
            {
                Set(ref _speed, value);
            }
        }
        /// <summary>
        /// 当前工作模式
        /// </summary>
        private string _workMode;
        public string WorkMode
        {
            get { return _workMode; }
            set
            {
                Set(ref _workMode, value);
            }
        }
        /// <summary>
        /// 是否工作
        /// </summary>
        private bool _isWorking;
        public bool IsWorking
        {
            get { return _isWorking; }
            set
            {
                Set(ref _isWorking, value);
            }
        }
        /// <summary>
        /// 初始化目标温度
        /// </summary>
        private double _InitDTemp;
        public double InitDTemp
        {
            get { return _InitDTemp; }
        }
        #endregion

        private static SlaveModel _slaveModel;
        public static SlaveModel GetInstance()
        {
            if(_slaveModel == null)
            {
                _slaveModel = new SlaveModel();
            }
            return _slaveModel;
        }

        public SlaveModel()
        {
            _cost = 0.0;
            _cTemp = 29.0;
            _dTemp = 25.0;
            _InitDTemp = 29.0;
            _speed = "Medium";
            _workMode = "Cooling";
            _isWorking = true;
        }
    }
}
