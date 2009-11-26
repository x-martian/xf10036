using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Diagnostics;
using System.Windows.Media; 
using Microsoft.SdkViewer.Samples;

namespace checkboxelement //needs to match the .xaml page
{
	public partial class Page1 : Page
	{
        protected internal void OnLoaded(object sender, EventArgs e)
        {
            PageInitalizer.Initialize(this);
        }



        
        
        private void HandleChange(object sender, RoutedEventArgs e)
         {
           cb1.Content = "You clicked the check box";
         }

         private void HandleChange1(object sender, RoutedEventArgs e)
         {
           txt.FontSize = 16;
           txt.Text = "I took this photo yesterday.";
         }
         private void HandleChange2(object sender, RoutedEventArgs e)
         {
           txt3.FontSize = 16;
           txt3.Text = "My favorite photo.";
         }
    }
}