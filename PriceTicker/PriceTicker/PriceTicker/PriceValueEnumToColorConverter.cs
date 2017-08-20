using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PriceTicker
{
    public class PriceValueEnumToColorConverter : IValueConverter
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var priceValueEnum = (PriceValueEnum)value;
                switch (priceValueEnum)
                {
                    case PriceValueEnum.Higher:
                        return new SolidColorBrush(Colors.Blue);

                    case PriceValueEnum.Neutral:
                        return new SolidColorBrush(Colors.Transparent);

                    case PriceValueEnum.Lower:
                        return new SolidColorBrush(Colors.Red);

                    default:
                        return new SolidColorBrush(Colors.Transparent);
                }
            }
            catch (Exception)
            {
                return new SolidColorBrush(Colors.Transparent);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}