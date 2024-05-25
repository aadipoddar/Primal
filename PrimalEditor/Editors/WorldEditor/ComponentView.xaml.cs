using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows;

namespace PrimalEditor.Editors
{
	[ContentProperty("ComponentContent")]
	public partial class ComponentView : UserControl
	{
		public string Header
		{
			get { return (string)GetValue(HeaderProperty); }
			set { SetValue(HeaderProperty, value); }
		}

		public static readonly DependencyProperty HeaderProperty =
			DependencyProperty.Register(nameof(Header), typeof(string), typeof(ComponentView));

		public FrameworkElement ComponentContent
		{
			get { return (FrameworkElement)GetValue(ComponentContentProperty); }
			set { SetValue(ComponentContentProperty, value); }
		}

		public static readonly DependencyProperty ComponentContentProperty =
			DependencyProperty.Register(nameof(ComponentContent), typeof(FrameworkElement), typeof(ComponentView));

		public ComponentView()
		{
			InitializeComponent();
		}
	}
}