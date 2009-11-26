using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Diagnostics;
using System.Windows.Media; using Microsoft.SdkViewer.Samples;
using System.ComponentModel;
using System.Windows.Data;
using System.Collections.ObjectModel;

namespace PropertyChangeNotification //needs to match the .xaml page
{
	public partial class Page1 : Page
	{
        protected internal void OnLoaded(object sender, EventArgs e)
        {
            PageInitalizer.Initialize(this);
        }



    }



    public class Bid : INotifyPropertyChanged
    {
      private string _biditemname = "Unset";
      private decimal _biditemprice = (decimal)0;

      public Bid(string newBidItemName, decimal newBidItemPrice)
      {
        _biditemname = newBidItemName;
        _biditemprice = newBidItemPrice;
      }

      public string BidItemName
      {
        get
        {
          return _biditemname;
        }
        set
        {
          if (_biditemname.Equals(value) == false)
          {
            _biditemname = value;
            // Call OnPropertyChanged whenever the property is updated
            OnPropertyChanged("BidItemName");
          }
        }
      }

      public decimal BidItemPrice
      {
        get
        {
          return _biditemprice;
        }
        set
        {
          if (_biditemprice.Equals(value) == false)
          {
            _biditemprice = value;
            // Call OnPropertyChanged whenever the property is updated
            OnPropertyChanged("BidItemPrice");
          }
        }
      }

      // Declare event
      public event PropertyChangedEventHandler PropertyChanged;
      // OnPropertyChanged event handler to update property value in binding
      private void OnPropertyChanged(string propName)
      {
        if (PropertyChanged != null)
        {
          PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
      }
    }

    public class BidCollection : ObservableCollection<Bid>
    {
      private Bid item1 = new Bid("Perseus Vase", (decimal)24.95);
      private Bid item2 = new Bid("Hercules Statue", (decimal)16.05);
      private Bid item3 = new Bid("Odysseus Painting", (decimal)100.0);

      private void Timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
      {
        item1.BidItemPrice += (decimal)1.25;
        item2.BidItemPrice += (decimal)2.45;
        item3.BidItemPrice += (decimal)10.55;
      }

      private void CreateTimer()
      {
        System.Timers.Timer Timer1 = new System.Timers.Timer();
        Timer1.Enabled = true;
        Timer1.Interval = 2000;
        Timer1.Elapsed += new System.Timers.ElapsedEventHandler(Timer1_Elapsed);
      }

      public BidCollection()
        : base()
      {
        Add(item1);
        Add(item2);
        Add(item3);
        CreateTimer();
      }
    }


}