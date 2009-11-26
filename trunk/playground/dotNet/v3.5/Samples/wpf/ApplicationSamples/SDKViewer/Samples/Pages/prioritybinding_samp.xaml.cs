using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Diagnostics;
using System.Windows.Media; 
using Microsoft.SdkViewer.Samples;
using System.Threading;

namespace myPriorityBinding //needs to match the .xaml page
{
	public partial class Page1 : Page
	{
        protected internal void OnLoaded(object sender, EventArgs e)
        {
            PageInitalizer.Initialize(this);
        }

    }




  public class AsyncDataProvider
  {
    private string _fastDP;
    private string _slowerDP;
    private string _slowestDP;

    public AsyncDataProvider()
    {
    }

    public string FastDP
    {
      get { return _fastDP; }
      set { _fastDP = value; }
    }

    public string SlowerDP
    {
      get
      {
        // This simulates a lengthy time before the
        // data being bound to is actualy available.
        Thread.Sleep(3000);
        return _slowerDP;
      }
      set { _slowerDP = value; }
    }

    public string SlowestDP
    {
      get
      {
        // This simulates a lengthy time before the
        // data being bound to is actualy available.
        Thread.Sleep(5000);
        return _slowestDP;
      }
      set { _slowestDP = value; }
    }
  }
}