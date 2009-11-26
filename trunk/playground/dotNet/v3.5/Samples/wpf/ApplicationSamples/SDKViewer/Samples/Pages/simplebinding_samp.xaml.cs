using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Diagnostics;
using System.Windows.Media; 
using Microsoft.SdkViewer.Samples;

namespace mySimpleBinding //needs to match the .xaml page
{
    public partial class Page1 : Page
	{
        protected internal void OnLoaded(object sender, EventArgs e)
        {
            PageInitalizer.Initialize(this);
        }

    }

  public class SimpleBinding
  {
    private string _simpleProperty = "This is a string from the data source";
    public SimpleBinding() { }
    public string SimpleProperty
    {
      get
      {
        return _simpleProperty;
      }
    }
  }

}