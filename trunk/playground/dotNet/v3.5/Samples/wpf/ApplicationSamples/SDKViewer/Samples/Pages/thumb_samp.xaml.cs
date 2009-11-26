using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Diagnostics;
using System.Windows.Media; 
using Microsoft.SdkViewer.Samples;
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;
using System.Windows.Data;

namespace myThumb //needs to match the .xaml page
{
	public partial class Page1 : Page
	{
        protected internal void OnLoaded(object sender, EventArgs e)
        {
            PageInitalizer.Initialize(this);
        }

       
        
        void ShowDelta(object sender, DragDeltaEventArgs e)
        {
            Thumb thumb = sender as Thumb;
            changes.Text = e.HorizontalChange.ToString() + ", " + e.VerticalChange.ToString();
        }
    }
}