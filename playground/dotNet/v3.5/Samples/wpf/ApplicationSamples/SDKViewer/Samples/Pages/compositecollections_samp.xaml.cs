using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Diagnostics;
using System.Windows.Media; 
using Microsoft.SdkViewer.Samples;
using System.ComponentModel;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Collections.ObjectModel;

namespace CompositeCollections //needs to match the .xaml page
{
	public partial class Page1 : Page
	{
        protected internal void OnLoaded(object sender, EventArgs e)
        {
            PageInitalizer.Initialize(this);
        }


    }



    public class GreekGod
    {
      private string _name = string.Empty;

      public string Name
      {
        get
        {
          return _name;
        }

        set
        {
          _name = value;
        }
      }

      public GreekGod(string name)
      {
        Name = name;
      }
    }

    public class GreekGods : ObservableCollection<GreekGod>
    {
      public GreekGods()
      {
        Add(new GreekGod("Aphrodite"));
        Add(new GreekGod("Apollo"));
        Add(new GreekGod("Ares"));
        Add(new GreekGod("Artemis"));
        Add(new GreekGod("Athena"));
        Add(new GreekGod("Demeter"));
        Add(new GreekGod("Dionysus"));
        Add(new GreekGod("Hephaestus"));
        Add(new GreekGod("Hera"));
        Add(new GreekGod("Hermes"));
        Add(new GreekGod("Poseidon"));
        Add(new GreekGod("Zeus"));
      }

    }
}