using System;
using Xamarin.Forms;

namespace MvxForms.Platform.Forms.Converter
{
	public class SpacingConverter:IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			//var items = value as IEnumerable<HomeViewModel>;

			//var collection = new ColumnDefinitionCollection();
			//foreach (var item in items)
			//{
			//	collection.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
			//}
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
