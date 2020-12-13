using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Navigation;

namespace ImageCompresser
{
 public   class FileSizeConverter:IValueConverter
    {
        
        private bool IsConvertedToMB(long size)=>(size/1024/1024)>1?true: false;
        private string ConvertToMB(long size) =>
            (size / 1024 / 1024).ToString()+"." + ((size / 1024 % 1024)/10).ToString();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)=>
            IsConvertedToMB((long)value) ? ConvertToMB((long)value)+"мб" :  ((long)value / 1024).ToString()+"кб";
        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)=>
            throw new NotImplementedException();
        
    }
}
