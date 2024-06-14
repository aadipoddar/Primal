using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Xml.Linq;

namespace PrimalEditor.Content
{
	enum PrimitiveMeshType
	{
		Plane,
		Cube,
		UvSphere,
		IcoSpher,
		Cylinder,
		Capsule
	}

	class Mesh : ViewModelBase
	{
		private int _vertexSize;
		public int VertexSize
		{
			get => _vertexSize;
			set
			{
				if (_vertexSize != value)
				{
					_vertexSize = value;
					OnPropertyChanged(nameof(VertexSize));
				}
			}
		}

		private int _vertexCount;
		public int VertexCount
		{
			get => _vertexCount;
			set
			{
				if (_vertexCount != value)
				{
					_vertexCount = value;
					OnPropertyChanged(nameof(VertexCount));
				}
			}
		}

		private int _indexSize;
		public int IndexSize
		{
			get => _indexSize;
			set
			{
				if (_indexSize != value)
				{
					_indexSize = value;
					OnPropertyChanged(nameof(IndexSize));
				}
			}
		}

		private int _indexCount;
		public int IndexCount
		{
			get => _indexCount;
			set
			{
				if (_indexCount != value)
				{
					_indexCount = value;
					OnPropertyChanged(nameof(IndexCount));
				}
			}
		}

		public byte[] Vertices { get; set; }
		public byte[] Indices { get; set; }
	}

	class MeshLOD : ViewModelBase
	{
		private string _name;
		public string Name
		{
			get => _name;
			set
			{
				if (_name != value)
				{
					_name = value;
					OnPropertyChanged(nameof(Name));
				}
			}
		}

		private float _lodThreshold;
		public float LodThreshold
		{
			get => _lodThreshold;
			set
			{
				if (_lodThreshold != value)
				{
					_lodThreshold = value;
					OnPropertyChanged(nameof(LodThreshold));
				}
			}
		}

		public ObservableCollection<Mesh> Meshes { get; } = new ObservableCollection<Mesh>();
	}

	class LODGroup : ViewModelBase
	{
		private string _name;
		public string Name
		{
			get => _name;
			set
			{
				if (_name != value)
				{
					_name = value;
					OnPropertyChanged(nameof(Name));
				}
			}
		}

		public ObservableCollection<MeshLOD> LODs { get; } = new ObservableCollection<MeshLOD>();
	}

	class Geometry : Asset
	{
		public Geometry() : base(AssetType.Mesh) { }

		public void FromRawData(byte[] data)
		{
			Debug.Assert(data?.Length > 0);
		}
	}
}
