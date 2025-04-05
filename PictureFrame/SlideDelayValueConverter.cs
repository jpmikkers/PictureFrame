using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureFrame;
using Microsoft.UI.Xaml.Data;

public class SlideDelayValueConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is double sliderValue)
        {
            var timeSpan = ExponentialInterpolationHelper.PromilleToInterval(sliderValue);
            return $"{Math.Floor(timeSpan.TotalMinutes)} m {timeSpan.TotalSeconds % 60.0} s"; // Example: formats value to 2 decimal places
        }
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}