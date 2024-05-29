using PrimalEditor.DllWrappers;
using PrimalEditor.GameDev;
using PrimalEditor.Utilities;

using System.IO;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Input;

namespace PrimalEditor.GameProject
{
	enum BuildConfiguration
	{
		Debug,
		DebugEditor,
		Release,
		ReleaseEditor,
	}

	[DataContract(Name = "Game")]
	class Project : ViewModelBase
	{
		public static string Extension { get; } = ".primal";
		[DataMember]
		public string Name { get; private set; } = "New Project";
		[DataMember]
		public string Path { get; private set; }
		public string FullPath => $@"{Path}{Name}{Extension}";
		public string Solution => $@"{Path}{Name}.sln";

		private static readonly string[] _buildConfigurationNames = new string[] { "Debug", "DebugEditor", "Release", "ReleaseEditor" };

		private int _buildConfig;
		[DataMember]
		public int BuildConfig
		{
			get => _buildConfig;
			set
			{
				if (_buildConfig != value)
				{
					_buildConfig = value;
					OnPropertyChanged(nameof(BuildConfig));
				}
			}
		}

		public BuildConfiguration StandAloneBuildConfig => BuildConfig == 0 ? BuildConfiguration.Debug : BuildConfiguration.Release;
		public BuildConfiguration DllBuildConfig => BuildConfig == 0 ? BuildConfiguration.DebugEditor : BuildConfiguration.ReleaseEditor;

		[DataMember(Name = "Scenes")]
		private ObservableCollection<Scene> _scenes = new ObservableCollection<Scene>();
		public ReadOnlyObservableCollection<Scene> Scenes
		{ get; private set; }

		private Scene _activeScene;
		public Scene ActiveScene
		{
			get => _activeScene;
			set
			{
				if (_activeScene != value)
				{
					_activeScene = value;
					OnPropertyChanged(nameof(ActiveScene));
				}
			}
		}

		public static Project Current => Application.Current.MainWindow.DataContext as Project;

		public static UndoRedo UndoRedo { get; } = new UndoRedo();

		public ICommand UndoCommand { get; private set; }
		public ICommand RedoCommand { get; private set; }

		public ICommand AddSceneCommand { get; private set; }
		public ICommand RemoveSceneCommand { get; private set; }
		public ICommand SaveCommand { get; private set; }
		public ICommand BuildCommand { get; private set; }

		private void SetCommands()
		{
			AddSceneCommand = new RelayCommand<object>(x =>
			{
				AddScene($"New Scene {_scenes.Count}");
				var newScene = _scenes.Last();
				var sceneIndex = _scenes.Count - 1;

				UndoRedo.Add(new UndoRedoAction(
					() => RemoveScene(newScene),
					() => _scenes.Insert(sceneIndex, newScene),
					$"Add {newScene.Name}"));
			});

			RemoveSceneCommand = new RelayCommand<Scene>(x =>
			{
				var sceneIndex = _scenes.IndexOf(x);
				RemoveScene(x);

				UndoRedo.Add(new UndoRedoAction(
					() => _scenes.Insert(sceneIndex, x),
					() => RemoveScene(x),
					$"Remove {x.Name}"));
			}, x => !x.IsActive);

			UndoCommand = new RelayCommand<object>(x => UndoRedo.Undo(), x => UndoRedo.UndoList.Any());
			RedoCommand = new RelayCommand<object>(x => UndoRedo.Redo(), x => UndoRedo.RedoList.Any());
			SaveCommand = new RelayCommand<object>(x => Save(this));
			BuildCommand = new RelayCommand<bool>(async x => await BuildGameCodeDll(x), x => !VisualStudio.IsDebugging() && VisualStudio.BuildDone);

			OnPropertyChanged(nameof(AddSceneCommand));
			OnPropertyChanged(nameof(RemoveSceneCommand));
			OnPropertyChanged(nameof(UndoCommand));
			OnPropertyChanged(nameof(RedoCommand));
			OnPropertyChanged(nameof(SaveCommand));
			OnPropertyChanged(nameof(BuildCommand));
		}

		private static string GetConfigurationName(BuildConfiguration config) => _buildConfigurationNames[(int)config];

		private void AddScene(string sceneName)
		{
			Debug.Assert(!string.IsNullOrEmpty(sceneName.Trim()));
			_scenes.Add(new Scene(this, sceneName));
		}

		private void RemoveScene(Scene scene)
		{
			Debug.Assert(_scenes.Contains(scene));
			_scenes.Remove(scene);
		}

		public static Project Load(string file)
		{
			Debug.Assert(File.Exists(file));
			return Serializer.FromFile<Project>(file);
		}

		public void Unload()
		{
			VisualStudio.CloseVisualStudio();
			UndoRedo.Reset();
		}

		public static void Save(Project project)
		{
			Serializer.ToFile(project, project.FullPath);
			Logger.Log(MessageType.Info, $"Project saved to {project.FullPath}");
		}

		private async Task BuildGameCodeDll(bool showWindow = true)
		{
			try
			{
				UnloadGameCodeDll();
				await Task.Run(() => VisualStudio.BuildSolution(this, GetConfigurationName(DllBuildConfig), showWindow));
				if (VisualStudio.BuildSucceeded)
				{
					LoadGameCodeDll();
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
				throw;
			}
		}

		private void LoadGameCodeDll()
		{
			var configName = GetConfigurationName(DllBuildConfig);
			var dll = $@"{Path}x64\{configName}\{Name}.dll";
			if (File.Exists(dll) && EngineAPI.LoadGameCodeDll(dll) != 0)
			{
				Logger.Log(MessageType.Info, "Game code DLL loaded successfully.");
			}
			else
			{
				Logger.Log(MessageType.Warning, "Failed to load game code DLL file. Try to build the project first.");
			}
		}

		private void UnloadGameCodeDll()
		{
			if (EngineAPI.UnloadGameCodeDll() != 0)
			{
				Logger.Log(MessageType.Info, "Game code DLL unloaded");
			}
		}

		[OnDeserialized]
		private async void OnDeserialized(StreamingContext context)
		{
			if (_scenes != null)
			{
				Scenes = new ReadOnlyObservableCollection<Scene>(_scenes);
				OnPropertyChanged(nameof(Scenes));
			}
			ActiveScene = Scenes.FirstOrDefault(x => x.IsActive);

			await BuildGameCodeDll(false);

			SetCommands();
		}

		public Project(string name, string path)
		{
			Name = name;
			Path = path;

			OnDeserialized(new StreamingContext());
		}
	}
}