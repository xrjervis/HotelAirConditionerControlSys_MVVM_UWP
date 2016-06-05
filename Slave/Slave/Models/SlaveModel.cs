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
        private int _dTemp;
        public int DTemp
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
        private int _cTemp;
        public int CTemp
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
        private float _cost;
        public float Cost
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
        /// 是否正忙
        /// </summary>
        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                Set(ref _isBusy, value);
            }
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
            _cost = 0.0f;
            _cTemp = 25;
            _dTemp = 18;
            _speed = "低";
            _workMode = "制冷";
        }
    }
}
