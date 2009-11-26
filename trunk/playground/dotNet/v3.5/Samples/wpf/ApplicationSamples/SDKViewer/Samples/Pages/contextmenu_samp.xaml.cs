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


namespace mysamp_Samp //needs to match the .xaml page
{
	public partial class Page1 : Page
	{
        protected internal void OnLoaded(object sender, EventArgs e)
        {
            PageInitalizer.Initialize(this);
        }





        //This is a list of commonly used namespaces for a pane.

        System.Windows.Controls.ContextMenu contextmenu;
        System.Windows.Controls.MenuItem mi, mia, mib, mib1, mib1a;
        System.Windows.Controls.Button btn;

        void OnClick(object sender, RoutedEventArgs e)
		{
		        btn = new Button();
                        btn.Content = "Create with C#";
                        contextmenu = new ContextMenu();
                        btn.ContextMenu = contextmenu;
                        mi = new MenuItem();
			mi.Header = "File";
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
                        contextmenu.Items.Add(mi);
			cv2.Children.Add(btn);
		}


    }
}