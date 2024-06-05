using System.Windows;
using System.Windows.Controls;

namespace PrimalEditor.Utilities
{
	/// <summary>
	/// Interaction logic for RenderSurfaceView.xaml
	/// </summary>
	public partial class RenderSurfaceView : UserControl, IDisposable
	{
		private RenderSurfaceHost _host = null;

		public RenderSurfaceView()
		{
			InitializeComponent();
			Loaded += OnRenderSurfaceViewLoaded;
		}

		private void OnRenderSurfaceViewLoaded(object sender, RoutedEventArgs e)
		{
			Loaded -= OnRenderSurfaceViewLoaded;

			_host = new RenderSurfaceHost(ActualWidth, ActualHeight);
			Content = _host;
		}


		#region IDisposable Support
		private bool _disposedValue;
		protected virtual void Dispose(bool disposing)
		{
			if (!_disposedValue)
			{
				if (disposing)
				{
					_host.Dispose();
				}

				_disposedValue = true;
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
		#endregion
	}
}
