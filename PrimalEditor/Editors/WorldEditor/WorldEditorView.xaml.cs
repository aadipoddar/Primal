using PrimalEditor.Content;
using PrimalEditor.GameDev;
using PrimalEditor.GameProject;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
        }

        private void OnNewScript_Button_Click(object sender, RoutedEventArgs e)
        {
            new NewScriptDialog().ShowDialog();
        }

        private void OnCreatePrimitiveMesh_Button_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new PrimitiveMeshDialog();
            dlg.ShowDialog();
        }

		private void OnNewProject(object sender, ExecutedRoutedEventArgs e)
		{
			ProjectBrowserDialog.GotoNewProjectTab = true;
			Project.Current?.Unload();
			Application.Current.MainWindow.DataContext = null;
			Application.Current.MainWindow.Close();
		}

		private void OnOpenProject(object sender, ExecutedRoutedEventArgs e)
		{
			Project.Current?.Unload();
			Application.Current.MainWindow.DataContext = null;
			Application.Current.MainWindow.Close();
		}

		private void OnEditorClose(object sender, ExecutedRoutedEventArgs e)
		{
			Application.Current.MainWindow.Close();
		}
	}
}
