using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Diagnostics;
using System.Windows.Media;
using Microsoft.SdkViewer.Samples;

namespace BindNonTextProperty //needs to match the .xaml page
{
	public partial class Page1 : Page
	{
        protected internal void OnLoaded(object sender, EventArgs e)
        {
            PageInitalizer.Initialize(this);
        }

    }


    public partial class MyData
    {
      private string _data = "Red";

      public string BoundColor
      {
        get
        {
          return _data;
        }
      }
    }
}