using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Slave.Services
{
    public class IntToDoubleConverter : IValueConverter
    {
        // Define the Convert method to convert a DateTime value to 
        // a month string.
        public object Convert(object value, Type targetType,
            object parameter, string language)
        {

            return (double)value;
        }

        // ConvertBack is not implemented for a OneWay binding.
        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            return (int)value;
        }
    }
}
