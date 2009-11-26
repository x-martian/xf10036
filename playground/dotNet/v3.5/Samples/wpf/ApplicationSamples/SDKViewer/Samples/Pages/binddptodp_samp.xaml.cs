using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Diagnostics;
using System.Windows.Media; 
using Microsoft.SdkViewer.Samples;

namespace BindDpToDp //needs to match the .xaml page
{
	public partial class Page1 : Page
	{
        protected internal void OnLoaded(object sender, EventArgs e)
        {
            PageInitalizer.Initialize(this);
        }



         static int iCount = 0;

         void btnChangeSource(object sender, RoutedEventArgs e)
         {
           iCount++;
           if ((iCount % 2) == 0)
           {
             Text1.Foreground = Brushes.Green;
             Canvas.SetTop(Canvas1, 10);
           }
           else
           {
             Text1.Foreground = Brushes.Red;
             Canvas.SetTop(Canvas1, 20);
           }
           Text1.Text = string.Format("New Text. Count={0}", iCount);
         }

    }
}