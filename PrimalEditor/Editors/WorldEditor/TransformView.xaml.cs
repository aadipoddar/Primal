using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;

using PrimalEditor.Components;
using PrimalEditor.GameProject;
using PrimalEditor.Utilities;

namespace PrimalEditor.Editors
{
    /// <summary>
    /// Interaction logic for TransformView.xaml
    /// </summary>
    public partial class TransformView : UserControl
    {
        private Action _undoAction = null;
        private bool _propertyChange = false;

        public TransformView()
        {
            InitializeComponent();
            Loaded += OnTransformViewLoaded;
        }

        private void OnTransformViewLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnTransformViewLoaded;
            (DataContext as MSTransform).PropertyChanged += (s, e) => _propertyChange = true;
        }

        private Action GetAction(Func<Transform, (Transform transform, Vector3)> selector,
            Action<(Transform transfrom, Vector3)> forEachAction)
        {
            if (!(DataContext is MSTransform vm))
            {
                _undoAction = null;
                _propertyChange = false;
                return null;
            }

            var selection = vm.SelectedComponents.Select(x => selector(x)).ToList();

            return new Action(() =>
            {
                selection.ForEach(x => forEachAction(x));
                (GameEntityView.Instance.DataContext as MSEntity)?.GetMSComponent<MSTransform>().Refresh();
            });
        }

        private Action GetPositionAction() => GetAction((x) => (x, x.Position), (x) => x.transfrom.Position = x.Item2);
        private Action GetRotationAction() => GetAction((x) => (x, x.Rotation), (x) => x.transfrom.Rotation = x.Item2);
        private Action GetScaleAction() => GetAction((x) => (x, x.Scale), (x) => x.transfrom.Scale = x.Item2);


        private void RecordActions(Action redoAction, string name)
        {
            if (_propertyChange)
            {
                Debug.Assert(_undoAction != null);
                _propertyChange = false;
                Project.UndoRedo.Add(new UndoRedoAction(_undoAction, redoAction, name));
            }
        }

        private void OnPosition_VectorBox_PreviewMouse_LBD(object sender, MouseButtonEventArgs e)
        {
            _propertyChange = false;
            _undoAction = GetPositionAction();
        }

        private void OnPosition_VectorBox_PreviewMouse_LBU(object sender, MouseButtonEventArgs e)
        {
            RecordActions(GetPositionAction(), "Position Changed");
        }

        private void OnPosition_VectorBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (_propertyChange && _undoAction != null)
            {
                OnPosition_VectorBox_PreviewMouse_LBU(sender, null);
            }
        }

        private void OnRotation_VectorBox_PreviewMouse_LBD(object sender, MouseButtonEventArgs e)
        {
            _propertyChange = false;
            _undoAction = GetRotationAction();
        }

        private void OnRotation_VectorBox_PreviewMouse_LBU(object sender, MouseButtonEventArgs e)
        {
            RecordActions(GetRotationAction(), "Rotation Changed");
        }

        private void OnRotation_VectorBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (_propertyChange && _undoAction != null)
            {
                OnRotation_VectorBox_PreviewMouse_LBU(sender, null);
            }
        }

        private void OnScale_VectorBox_PreviewMouse_LBD(object sender, MouseButtonEventArgs e)
        {
            _propertyChange = false;
            _undoAction = GetScaleAction();
        }

        private void OnScale_VectorBox_PreviewMouse_LBU(object sender, MouseButtonEventArgs e)
        {
            RecordActions(GetScaleAction(), "Scale Changed");
        }

        private void OnScale_VectorBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (_propertyChange && _undoAction != null)
            {
                OnScale_VectorBox_PreviewMouse_LBU(sender, null);
            }
        }
    }
}
