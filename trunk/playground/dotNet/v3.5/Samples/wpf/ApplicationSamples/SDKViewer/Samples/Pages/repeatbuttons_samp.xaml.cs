using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Diagnostics;
using System.Windows.Media; 
using Microsoft.SdkViewer.Samples;
using System.Windows.Shapes;
using System.Windows.Data;

namespace RepeatButtons //needs to match the .xaml page
{
	public partial class Page1 : Page
	{
        protected internal void OnLoaded(object sender, EventArgs e)
        {
            PageInitalizer.Initialize(this);
        }


        
        void Increase(object sender, RoutedEventArgs e)
         {
           Int32 Num = Convert.ToInt32(btn.Content);

           btn.Content = ((Num + 1).ToString());
         }

         void Decrease(object sender, RoutedEventArgs e)
         {
           Int32 Num = Convert.ToInt32(btn.Content);

           btn.Content = ((Num - 1).ToString());
         }
         void Increase2(object sender, RoutedEventArgs e)
         {
           Int32 Num = Convert.ToInt32(btn2.Content);

           btn2.Content = ((Num + 1).ToString());
           if (Num >= 50)
           {
             btn2.Content = 50;
           }
         }

         void Decrease2(object sender, RoutedEventArgs e)
         {
           Int32 Num = Convert.ToInt32(btn2.Content);

           btn2.Content = ((Num - 1).ToString());
           if (Num <= 0)
           {
             btn2.Content = 0;
           }
         }
    }
}