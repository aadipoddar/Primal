using System.Windows;
using System.Windows.Controls;

namespace PrimalEditor.GameProject
{
	/// <summary>
	/// Interaction logic for OpenProjectVIew.xaml
	/// </summary>
	public partial class OpenProjectVIew : UserControl
	{
		public OpenProjectVIew()
		{
			InitializeComponent();
		}

		private void OnOpen_Button_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			OpenSelectedProject();
		}

		private void OnListBoxItem_Mouse_DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			OpenSelectedProject();
		}

		private void OpenSelectedProject()
		{
			var project = OpenProject.Open(projectsListBox.SelectedItem as ProjectData);
			bool dialogResult = false;
			var win = Window.GetWindow(this);
			if (project != null)
			{
				dialogResult = true;
				win.DataContext = project;
			}

			win.DialogResult = dialogResult;
			win.Close();
		}
	}
}
