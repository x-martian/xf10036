using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Diagnostics;
using System.Windows.Media; using Microsoft.SdkViewer.Samples;
using System.ComponentModel;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Globalization;
using System.Collections.ObjectModel;

namespace myMultiBinding //needs to match the .xaml page
{
	public partial class Page1 : Page
	{
        protected internal void OnLoaded(object sender, EventArgs e)
        {
            PageInitalizer.Initialize(this);
        }


    }



    public class NameConverter : IMultiValueConverter
    {
      public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
      {
        string name;

        switch ((string)parameter)
        {
          case "FormatLastFirst":
            name = values[1] + ", " + values[0];
            break;
          case "FormatNormal":
          default:
            name = values[0] + " " + values[1];
            break;
        }

        return name;
      }

      public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
      {
        string[] splitValues = ((string)value).Split(' ');
        return splitValues;
      }
    }

    public class PersonName
    {
      private string _firstName;
      private string _lastName;

      public PersonName(string first, string last)
      {
        this._firstName = first;
        this._lastName = last;
      }

      public string firstName
      {
        get { return _firstName; }
        set { _firstName = value; }
      }

      public string lastName
      {
        get { return _lastName; }
        set { _lastName = value; }
      }
    }

    public class NameList : ObservableCollection<PersonName>
    {
      public NameList()
        : base()
      {
        Add(new PersonName("Willa", "Cather"));
        Add(new PersonName("Isak", "Dinesen"));
        Add(new PersonName("Victor", "Hugo"));
        Add(new PersonName("Jules", "Verne"));
      }
    }

}