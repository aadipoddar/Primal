using System.Windows;
using System.Windows.Media;

namespace PrimalEditor.Common
{
	static class VisualExtensions
	{
		public static T FindVisualParent<T>(this DependencyObject depObject) where T : DependencyObject
		{
			if (!(depObject is Visual)) return null;

			var parent = VisualTreeHelper.GetParent(depObject);
			while (parent != null)
			{
				if (parent is T type)
				{
					return type;
				}
				parent = VisualTreeHelper.GetParent(parent);
			}
			return null;
		}
	}
}
