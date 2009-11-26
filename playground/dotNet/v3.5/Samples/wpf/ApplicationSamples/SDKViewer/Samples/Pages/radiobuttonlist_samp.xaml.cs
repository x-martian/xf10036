using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Diagnostics;
using System.Windows.Media; using Microsoft.SdkViewer.Samples;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Windows.Media.Imaging;


namespace RadioButtonList_wcp //needs to match the .xaml page
{
	public partial class Page1 : Page
	{
        protected internal void OnLoaded(object sender, EventArgs e)
        {
            PageInitalizer.Initialize(this);
        }


        
        
        void WriteText(object sender, RoutedEventArgs e)
         {
           RadioButton li = sender as RadioButton;

           btn.Content = "You clicked " + li.Content.ToString();
         }
         void DrawPet(object sender, RoutedEventArgs e)
         {
           RadioButton li = sender as RadioButton;
           switch (li.Name)
           {
             case "rb4":
               //IB: Irritatingly these three dont work
               Image catImage = new Image();
               catImage.Source = new BitmapImage(new Uri("pack://application:,,,/samples;component/Images/cat.png", UriKind.RelativeOrAbsolute));
               catImage.Width = 60;
               catImage.Height = 60;
               btn2.Content = catImage;
               break;

             case "rb5":

               Image dogImage = new Image();
               dogImage.Source = new BitmapImage(new Uri("pack://application:,,,/samples;component/Images/dog.png", UriKind.RelativeOrAbsolute));
               dogImage.Width = 60;
               dogImage.Height = 60;
               btn2.Content = dogImage;
               break;

             case "rb6":

               Image fishImage = new Image();
               fishImage.Source = new BitmapImage(new Uri("pack://application:,,,/samples;component/Images/fish.png", UriKind.RelativeOrAbsolute));
               fishImage.Width = 60;
               fishImage.Height = 60;
               btn2.Content = fishImage;
               break;
           }
         }

    }
}