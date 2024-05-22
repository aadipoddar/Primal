using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace PrimalEditor.Dictionaries
{
	public partial class ControlTemplates : ResourceDictionary
	{
		private void OnTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			var textBox = sender as TextBox;
			var exp = textBox.GetBindingExpression(TextBox.TextProperty);
			if (exp == null) return;

			if (e.Key == Key.Enter)
			{
				if (textBox.Tag is ICommand command && command.CanExecute(textBox.Text))
				{
					command.Execute(textBox.Text);
				}
				else
				{
					exp.UpdateSource();
				}
				Keyboard.ClearFocus();
				e.Handled = true;
			}
			else if (e.Key == Key.Escape)
			{
				exp.UpdateTarget();
				Keyboard.ClearFocus();
			}
		}
	}
}