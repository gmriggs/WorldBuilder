using System;
using System.Globalization;
using System.Numerics;
using Avalonia.Data.Converters;

namespace WorldBuilder.Converters {
    public class PositionToCoordConverter : IValueConverter {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
            if (value is Vector3 position) {
                var lbX = (int)Math.Max(0, position.X / 192f);
                var lbY = (int)Math.Max(0, position.Y / 192f);
                lbX = Math.Clamp(lbX, 0, 253);
                lbY = Math.Clamp(lbY, 0, 253);
                return $"({lbX},{lbY})";
            }
            return value;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
