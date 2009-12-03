using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FluidKit.Controls;

namespace FluidKit.Showcase.HalfCirclePanel
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		private const int MaxItems = 25;
		private double _currentIndex = 0;
		public Window1()
		{
			InitializeComponent();

			var random = new Random();
			for (int i = 0; i < MaxItems; i++)
			{
				int index = random.Next(11);
				var b = new Image()
				        	{
				        		Source = new BitmapImage(new Uri("covers/" + index + ".jpg", UriKind.Relative)),
				        		Stretch = Stretch.Fill
				        	};
				CircPanel.Children.Add(b);
			}

			Scroller.Maximum = MaxItems - 1;
		}

		private void ChangeOrientation(object sender, System.Windows.RoutedEventArgs e)
		{
			var tag = (string)(sender as RadioButton).Tag;
			CircPanel.Orientation = (tag == "Left") ? PanelOrientation.Left : PanelOrientation.Right;
		}

		private void GoToNextItem(object sender, ExecutedRoutedEventArgs e)
		{
			_currentIndex++;
			Debug.WriteLine(_currentIndex);

			CircPanel.AnimateToOffset(_currentIndex);
		}

		private void GoToPreviousItem(object sender, ExecutedRoutedEventArgs e)
		{
			_currentIndex--;
			Debug.WriteLine(_currentIndex);

			CircPanel.AnimateToOffset(_currentIndex);
		}

		private void CanGoToNextItem(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = _currentIndex < MaxItems - 1;
		}

		private void CanGoToPreviousItem(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = _currentIndex > 0;
		}
	}
}