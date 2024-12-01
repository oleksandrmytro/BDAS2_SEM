﻿using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace BDAS2_SEM.Converters
{
    public class ByteArrayToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            byte[] imageData = value as byte[];

            if (imageData == null || imageData.Length == 0)
                return null;

            BitmapImage image = new BitmapImage();
            using (MemoryStream mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}