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
using System.Windows.Navigation;
using System.Windows.Shapes;
using FluidKit.Showcase;

namespace AutoComplete
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();

			Loaded += (s, args) =>
			          	{
			          		var source = FindResource("DataSource") as StringCollection;
			          		for (int i = 0; i < 30; i++)
			          		{
			          			source.Add("Item - " + i);
			          		}
			          	};
		}
	}
}
