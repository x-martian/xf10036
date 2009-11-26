using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Diagnostics;
using System.Windows.Media; using Microsoft.SdkViewer.Samples;

namespace myToolTip //needs to match the .xaml page
{
	public partial class Page1 : Page
	{
        protected internal void OnLoaded(object sender, EventArgs e)
        {
            PageInitalizer.Initialize(this);
        }

        
        void OnClick(object sender, RoutedEventArgs e)
        {
            ToolTip tt = new ToolTip();
            tt.Content = "Created with C#";
            btn.ToolTip = tt;
        }
    }
}