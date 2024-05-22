using System.Windows.Controls;

namespace PrimalEditor.Editors
{
	/// <summary>
	/// Interaction logic for GameEntityVIew.xaml
	/// </summary>
	public partial class GameEntityView : UserControl
	{
		public static GameEntityView Instance { get; private set; }
		public GameEntityView()
		{
			InitializeComponent();
			DataContext = null;
			Instance = this;
		}
	}
}
