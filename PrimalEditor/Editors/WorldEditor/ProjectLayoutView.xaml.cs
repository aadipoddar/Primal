using PrimalEditor.Components;
using PrimalEditor.GameProject;

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
			var entity = (sender as ListBox).SelectedItems[0];
			GameEntityView.Instance.DataContext = entity;
		}
	}
}
