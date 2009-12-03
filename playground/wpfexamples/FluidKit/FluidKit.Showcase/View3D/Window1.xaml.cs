using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FluidKit.Controls.View3D;

namespace FluidKit.Showcase.View3D
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();
		}

		private void AddModel(object sender, RoutedEventArgs e)
		{
			Button b = sender as Button;
			MapItemVisual3D model = null;
			switch (b.Name)
			{
				case "_cube":
					model = new Cube();
					//model.FaceBrush = new SolidColorBrush(Colors.LightCoral);
					break;
				case "_cylinder":
					model = new Cylinder();
					//model.FaceBrush = new SolidColorBrush(Colors.Ivory);
					break;
				case "_sphere":
					model = new Sphere();
					//model.FaceBrush = new LinearGradientBrush(Colors.LightBlue, Colors.Orchid, 90);
					break;
				case "_torus":
					model = new Torus();
					//model.FaceBrush = new SolidColorBrush(Colors.Khaki);
					break;
				case "_cone":
					model = new Cone();
					//model.FaceBrush = new LinearGradientBrush(Colors.LightBlue, Colors.Orchid, 90);
					break;
			}
			model.FaceBrush = new LinearGradientBrush(Colors.LightBlue, Colors.Orchid, 90);
			model.FaceBrush.Opacity = 0.5;
			model.EdgePen = new Pen(Brushes.Black, 1);

			_view3D.Children.Add(model);

		}


	}
}