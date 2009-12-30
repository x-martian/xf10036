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
using System.ComponentModel;

namespace CustomControl
{
	[Description("Represents a custom button control that responds to a Click event.  Displays an image using a custom Source property if the Source property is bound to an Image in the template.")]
	public class ImageButton : Button
	{
		public ImageButton()
		{
		}
		[Description("The image displayed in the button if there is an Image Control in the template whose Source property is template-bound to the ImageButton Source property."), Category("Common Properties")]
		public ImageSource Source
		{
			get { return base.GetValue(SourceProperty) as ImageSource; }
			set { base.SetValue(SourceProperty, value); }
		}
		public static readonly DependencyProperty SourceProperty =
			DependencyProperty.Register("Source", typeof(ImageSource), typeof(ImageButton), null);
	}
}