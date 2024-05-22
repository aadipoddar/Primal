using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

using PrimalEditor.GameProject;

namespace PrimalEditor.Editors
{
	/// <summary>
	/// Interaction logic for WorldEditorView.xaml
	/// </summary>
	public partial class WorldEditorView : UserControl
	{
		public WorldEditorView()
		{
			InitializeComponent();
			Loaded += OnWorldEditorViewLoaded;
		}

		private void OnWorldEditorViewLoaded(object sender, RoutedEventArgs e)
		{
			Loaded -= OnWorldEditorViewLoaded;
			Focus();
			((INotifyCollectionChanged)Project.UndoRedo.UndoList).CollectionChanged += (s, e) => Focus();
		}
	}
}
