using System.Windows;
using System.Windows.Controls;

using PrimalEditor.ContentToolsAPIStructs;
using PrimalEditor.DllWrappers;
using PrimalEditor.Utilities.Controls;
using PrimalEditor.Editors;

namespace PrimalEditor.Content
{
	/// <summary>
	/// Interaction logic for PrimitiveMeshDialog.xaml
	/// </summary>
	public partial class PrimitiveMeshDialog : Window
	{
		private void OnPrimitiveType_ComboBox_SelectionChnaged(object sender, SelectionChangedEventArgs e) => UpdatePrmitive();

		private void OnSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) => UpdatePrmitive();

		private void OnScalarBox_ValueChanged(object sender, RoutedEventArgs e) => UpdatePrmitive();

		private float Value(ScalarBox scalarBox, float min)
		{
			float.TryParse(scalarBox.Value, out var result);
			return Math.Max(result, min);
		}

		private void UpdatePrmitive()
		{
			if (!IsInitialized) return;

			var primitiveType = (PrimitiveMeshType)primTypeComboBox.SelectedItem;
			var info = new PrimitiveInitInfo { Type = primitiveType };

			switch (primitiveType)
			{
				case PrimitiveMeshType.Plane:
					{
						info.SegmentX = (int)xSliderPlane.Value;
						info.SegmentZ = (int)zSliderPlane.Value;
						info.Szie.X = Value(widthScalarBoxPlane, 0.001f);
						info.Szie.Z = Value(lengthScalarBoxPlane, 0.001f);
						break;
					}
				case PrimitiveMeshType.Cube:
					break;
				case PrimitiveMeshType.UvSphere:
					break;
				case PrimitiveMeshType.IcoSpher:
					break;
				case PrimitiveMeshType.Cylinder:
					break;
				case PrimitiveMeshType.Capsule:
					break;
				default:
					break;
			}

			var geometry = new Geometry();
			ContentToolsAPI.CreatePrimitiveMesh(geometry, info);
			(DataContext as GeometryEditor).SetAsset(geometry);
		}

		public PrimitiveMeshDialog()
		{
			InitializeComponent();
			Loaded += (s, e) => UpdatePrmitive();
		}

	}
}
