﻿using PrimalEditor.Content;
using PrimalEditor.GameProject;

using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace PrimalEditor
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public static string PrimalPath { get; private set; }

		public MainWindow()
		{
			InitializeComponent();
			Loaded += OnMainWindowLoaded;
			Closing += OnMainWindowClosing;
		}

		private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
		{
			Loaded -= OnMainWindowLoaded;
			GetEnginePath();
			OpenProjectBrowserDialog();
		}

		private void GetEnginePath()
		{
			var primalPath = Environment.GetEnvironmentVariable("PRIMAL_ENGINE", EnvironmentVariableTarget.User);
			if (primalPath == null || !Directory.Exists(Path.Combine(primalPath, @"Engine\EngineAPI")))
			{
				var dlg = new EnginePathDialog();
				if (dlg.ShowDialog() == true)
				{
					PrimalPath = dlg.PrimalPath;
					Environment.SetEnvironmentVariable("PRIMAL_ENGINE", PrimalPath.ToUpper(), EnvironmentVariableTarget.User);
				}
				else
				{
					Application.Current.Shutdown();
				}
			}
			else
			{
				PrimalPath = primalPath;
			}
		}

		private void OnMainWindowClosing(object sender, CancelEventArgs e)
		{
			if (DataContext == null)
			{
				e.Cancel = true;
				Application.Current.MainWindow.Hide();
				OpenProjectBrowserDialog();
				if (DataContext != null)
				{
					Application.Current.MainWindow.Show();
				}
			}
			else
			{
				Closing -= OnMainWindowClosing;
				Project.Current?.Unload();
				DataContext = null;
			}
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
				var project = projectBrowser.DataContext as Project;
				Debug.Assert(project != null);
				AssetRegistry.Reset(project.ContentPath);
				DataContext = projectBrowser.DataContext;
			}
		}
	}
}