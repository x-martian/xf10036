using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Diagnostics;
using System.Windows.Media;
using Microsoft.SdkViewer.Samples;

namespace ButtonColor //needs to match the .xaml page
{
	public partial class Page1 : Page
	{
        protected internal void OnLoaded(object sender, EventArgs e)
        {
            PageInitalizer.Initialize(this);
        }





         static int newcolor = 0;
         Button btncsharp;
         void OnClick1(object sender, RoutedEventArgs e)
         {
           btn1.Background = Brushes.LightBlue;
         }

         void OnClick2(object sender, RoutedEventArgs e)
         {
           btn2.Background = Brushes.Pink;
         }

         void OnClick3(object sender, RoutedEventArgs e)
         {
           btn1.Background = Brushes.Pink;
           btn2.Background = Brushes.LightBlue;
         }
         void OnClick4(object sender, RoutedEventArgs e)
         {
           switch (newcolor)
           {
             case 0:
               btn4.Background = Brushes.Red;
               btn4.Foreground = Brushes.Beige;
               btn4.Content = "Font Size 10";
               btn4.FontSize = 10;
               break;

             case 1:
               btn4.Background = Brushes.CadetBlue;
               btn4.Foreground = Brushes.LemonChiffon;
               btn4.Content = "Font Size 12";
               btn4.FontSize = 12;
               break;

             case 2:
               btn4.Background = Brushes.Purple;
               btn4.Foreground = Brushes.PeachPuff;
               btn4.Content = "Font Size 14";
               btn4.FontSize = 14;
               break;

             case 3:
               btn4.Background = Brushes.BlanchedAlmond;
               btn4.Foreground = Brushes.DarkRed;
               btn4.Content = "Font Size 16";
               btn4.FontSize = 16;
               break;

             case 4:
               btn4.Background = Brushes.Green;
               btn4.Foreground = Brushes.White;
               btn4.Content = "Font Size 18";
               btn4.FontSize = 18;
               break;
           }
           newcolor = newcolor + 1;
           if (newcolor > 4)
           {
             newcolor = 0;
           }
         }
         void OnClick5(object sender, RoutedEventArgs e)
         {
           btn6.FontSize = 16;
           btn6.Content = "This is my favorite photo.";
           btn6.Background = Brushes.Red;
         }
         void OnClick6(object sender, RoutedEventArgs e)
         {
           btn7.FontSize = 16;
           txt.Text = "You clicked the button.";
           btn7.Background = Brushes.Yellow;
         }
         void OnClick7(object sender, RoutedEventArgs e)
         {
           btncsharp = new Button();
           btncsharp.Content = "Created with C# code.";
           btncsharp.Background = SystemColors.ControlDarkDarkBrush;
           btncsharp.FontSize = SystemFonts.CaptionFontSize;
           cv2.Children.Add(btncsharp);
         }

    }
}