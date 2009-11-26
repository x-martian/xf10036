using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Diagnostics;
using System.Windows.Media; 
using Microsoft.SdkViewer.Samples;

namespace ControlProperties //needs to match the .xaml page
{
	public partial class Page1 : Page
	{
        protected internal void OnLoaded(object sender, EventArgs e)
        {
            PageInitalizer.Initialize(this);
        }





         private string str = string.Empty;
         private FontFamily ffamily = null;
         private double fsize = double.NaN;

         // To use PaneLoaded put Loaded="PaneLoaded" in root element of .xaml file.
         // private void PaneLoaded(object sender, EventArgs e) {}
         // Sample event handler:  
         void ChangeBackground(object sender, RoutedEventArgs e)
         {
           if (btn.Background == Brushes.Red)
           {
             btn.Background = new LinearGradientBrush(Colors.LightBlue, Colors.SlateBlue, 90);
             btn.Content = "Background";
           }
           else
           {
             btn.Background = Brushes.Red;
             btn.Content = "Control background changes from red to a blue gradient.";
           }
         }
         void ChangeForeground(object sender, RoutedEventArgs e)
         {
           if (btn1.Foreground == Brushes.Green)
           {
             btn1.Foreground = Brushes.Black;
             btn1.Content = "Foreground";
           }
           else
           {
             btn1.Foreground = Brushes.Green;
             btn1.Content = "Control foreground(text) changes from black to green.";
           }
         }
         void ChangeFontFamily(object sender, RoutedEventArgs e)
         {
           ffamily = btn2.FontFamily;
           str = ffamily.ToString();
           if (str == ("Arial Black"))
           {
             btn2.FontFamily = new FontFamily("Arial");
             btn2.Content = "FontFamily";
           }
           else
           {
             btn2.FontFamily = new FontFamily("Arial Black");
             btn2.Content = "Control font family changes from Arial to Arial Black.";

           }
         }
         void ChangeFontSize(object sender, RoutedEventArgs e)
         {
           fsize = btn3.FontSize;
           if (fsize == 16.0)
           {
             btn3.FontSize = 10.0;
             btn3.Content = "FontSize";
           }
           else
           {
             btn3.FontSize = 16.0;
             btn3.Content = "Control font size changes from 10 to 16.";
           }
         }
         void ChangeFontStyle(object sender, RoutedEventArgs e)
         {
           if (btn4.FontStyle == FontStyles.Italic)
           {
             btn4.FontStyle = FontStyles.Normal;
             btn4.Content = "FontStyle";
           }
           else
           {
             btn4.FontStyle = FontStyles.Italic;
             btn4.Content = "Control font style changes from Normal to Italic.";
           }
         }
         void ChangeFontWeight(object sender, RoutedEventArgs e)
         {
           if (btn5.FontWeight == FontWeights.Bold)
           {
             btn5.FontWeight = FontWeights.Normal;
             btn5.Content = "FontWeight";
           }
           else
           {
             btn5.FontWeight = FontWeights.Bold;
             btn5.Content = "Control font weight changes from Normal to Bold.";
           }
         }
         void ChangeBorderBrush(object sender, RoutedEventArgs e)
         {
           str = ((btn6.BorderBrush).ToString());
           if (btn6.BorderBrush == Brushes.Red)
           {
             btn6.BorderBrush = Brushes.Black;
             btn6.Content = "BorderBrush";

           }
           else
           {
             btn6.BorderBrush = Brushes.Red;
             btn6.Content = "Control border brush changes from red to black.";
           }
         }
	         
    }
}