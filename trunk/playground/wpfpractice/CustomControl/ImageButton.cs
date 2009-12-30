using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CustomControl
{
	public class ImageButton : Button
	{
		public ImageButton()
		{
		}
		public ImageSource Source
		{
			get { return base.GetValue(SourceProperty) as ImageSource; }
			set { base.SetValue(SourceProperty, value); }
		}
		public static readonly DependencyProperty SourceProperty =
			DependencyProperty.Register("Source", typeof(ImageSource), typeof(ImageButton), null);
	}
}