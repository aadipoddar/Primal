using System.Windows.Controls;
using System.Windows;

namespace PrimalEditor.GameProject
{
    /// <summary>
    /// Interaction logic for NewProjectView.xaml
    /// </summary>
    public partial class NewProjectView : UserControl
    {
        public NewProjectView()
        {
            InitializeComponent();
        }

        private void On_Create_Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var vm = DataContext as NewProject;
            var projectPath = vm.CreateProject(templateListBox.SelectedItem as ProjectTemplate);
            bool dialogResult = false;
            var win = Window.GetWindow(this);

            if (!string.IsNullOrEmpty(projectPath))
            {
                dialogResult = true;
            }

            win.DialogResult = dialogResult;
            win.Close();
        }
    }
}
