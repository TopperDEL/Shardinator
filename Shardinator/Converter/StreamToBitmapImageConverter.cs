using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;

namespace Shardinator.Converter;
public class StreamToBitmapImageConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, string language)
    {
        var stream = (Stream)value;
        var image = new BitmapImage();
        image.SetSource(stream.AsRandomAccessStream());
        return image;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
