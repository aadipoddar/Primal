﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace PrimalEditor.Utilities
{
	/// <summary>
	/// Interaction logic for RenderSurfaceView.xaml
	/// </summary>
	public partial class RenderSurfaceView : UserControl, IDisposable
	{
		private enum Win32Msg
		{
			WM_SIZING = 0x0214,
			WM_ENTERSIZEMOVE = 0x0231,
			WM_EXITSIZEMOVE = 0x0232,
			WM_SIZE = 0x0005,
		}

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
			_host.MessageHook += new HwndSourceHook(HostMsgFilter);
			Content = _host;
		}

		private IntPtr HostMsgFilter(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			switch ((Win32Msg)msg)
			{
				case Win32Msg.WM_SIZING: throw new Exception();
				case Win32Msg.WM_ENTERSIZEMOVE: throw new Exception();
				case Win32Msg.WM_EXITSIZEMOVE: throw new Exception();
				case Win32Msg.WM_SIZE:
					break;
				default:
					break;
			}

			return IntPtr.Zero;
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
