using System.ComponentModel;
using System.Windows;

using PrimalEditor.GameProject;

namespace PrimalEditor
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			Loaded += OnMainWindowLoaded;
			Closing += OnMainWindowClosing;
		}

		private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
		{
			Loaded -= OnMainWindowLoaded;
			OpenProjectBrowserDialog();
		}

		private void OnMainWindowClosing(object sender, CancelEventArgs e)
		{
			Closing -= OnMainWindowClosing;
			Project.Current?.Unload();
		}

		private void OpenProjectBrowserDialog()
		{
			var projectBrowser = new ProjectBrowserDialog();
			if (projectBrowser.ShowDialog() == false || projectBrowser.DataContext == null)
			{
				Application.Current.Shutdown();
			}
			else
			{
				Project.Current?.Unload();
				DataContext = projectBrowser.DataContext;
			}
		}
	}
}