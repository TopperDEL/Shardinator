using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Data;

namespace Shardinator.Converter;
public class BooleanToInvertedVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if ((bool)value == true)
        {
            return Visibility.Collapsed;
        }
        else
        {
            return Visibility.Visible;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if ((Visibility)value == Visibility.Collapsed)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
