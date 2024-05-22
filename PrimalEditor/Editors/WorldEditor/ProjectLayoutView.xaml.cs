using PrimalEditor.Components;
using PrimalEditor.GameProject;
using PrimalEditor.Utilities;

using System.Windows;
using System.Windows.Controls;

namespace PrimalEditor.Editors
{
	/// <summary>
	/// Interaction logic for ProjectLayourView.xaml
	/// </summary>
	public partial class ProjectLayoutView : UserControl
	{
		public ProjectLayoutView()
		{
			InitializeComponent();
		}

		private void OnAddGameEntity_Button_Click(object sender, RoutedEventArgs e)
		{
			var btn = sender as Button;
			var vm = btn.DataContext as Scene;
			vm.AddGameEntityCommand.Execute(new GameEntity(vm) { Name = "Empty Game Entity" });
		}

		private void OnGameEntities_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			GameEntityView.Instance.DataContext = null;
			var listBox = sender as ListBox;
			if (e.AddedItems.Count > 0)
			{
				GameEntityView.Instance.DataContext = listBox.SelectedItems[0];
			}

			var newSelection = listBox.SelectedItems.Cast<GameEntity>().ToList();
			var previousSelection = newSelection.Except(e.AddedItems.Cast<GameEntity>()).Concat(e.RemovedItems.Cast<GameEntity>()).ToList();

			Project.UndoRedo.Add(new UndoRedoAction(
				() => // undo action
				{
					listBox.UnselectAll();
					previousSelection.ForEach(x => (listBox.ItemContainerGenerator.ContainerFromItem(x) as ListBoxItem).IsSelected = true);
				},
				() => //redo action
				{
					listBox.UnselectAll();
					newSelection.ForEach(x => (listBox.ItemContainerGenerator.ContainerFromItem(x) as ListBoxItem).IsSelected = true);
				},
				"Selection changed"
				));
		}
	}
}
