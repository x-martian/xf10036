using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Diagnostics;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Windows.Media; 
using Microsoft.SdkViewer.Samples;

namespace Menus //needs to match the .xaml page
{
	public partial class Page1 : Page
	{
        protected internal void OnLoaded(object sender, EventArgs e)
        {
            PageInitalizer.Initialize(this);
        }




         System.Windows.Controls.Menu menu;
         System.Windows.Controls.MenuItem mi, mia, mib, mib1, mib1a;

         void OnClick(object sender, RoutedEventArgs e)
         {
           menu = new Menu();
           menu.Background = Brushes.LightBlue;
           mi = new MenuItem();
           mi.Header = "File";
           menu.Items.Add(mi);
           mia = new MenuItem();
           mia.Header = "New";
           mi.Items.Add(mia);
           mib = new MenuItem();
           mib.Header = "Open";
           mi.Items.Add(mib);
           mib1 = new MenuItem();
           mib1.Header = "Recently Opened";
           mib.Items.Add(mib1);
           mib1a = new MenuItem();
           mib1a.Header = "Text.xaml";
           mib1.Items.Add(mib1a);
           cv2.Children.Add(menu);
         }

    }
}