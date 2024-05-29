using System.Windows;
using System.Windows.Controls;

using PrimalEditor.GameDev;

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
	}
}
