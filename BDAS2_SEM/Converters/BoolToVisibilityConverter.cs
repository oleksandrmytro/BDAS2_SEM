﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BDAS2_SEM.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibilityConverter : IValueConverter
    {
        // Převádí hodnotu typu bool na hodnotu typu Visibility.
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isVisible = (bool)value;
            return isVisible ? Visibility.Visible : Visibility.Collapsed; // true → Visible, false → Collapsed
        }

        // Převádí hodnotu typu Visibility zpět na bool.
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Visibility)value == Visibility.Visible;
        }
    }
}
