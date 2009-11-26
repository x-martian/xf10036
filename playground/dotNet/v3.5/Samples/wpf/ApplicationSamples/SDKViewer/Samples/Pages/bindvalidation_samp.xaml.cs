using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Diagnostics;
using System.Windows.Media; 
using Microsoft.SdkViewer.Samples;
using System.Globalization;
using System.Collections.ObjectModel;

namespace BindValidation //needs to match the .xaml page
{
	public partial class Page1 : Page
	{
        protected internal void OnLoaded(object sender, EventArgs e)
        {
            PageInitalizer.Initialize(this);
        }

    }





    public class MyDataProvider
    {
      private string _name;
      private int _age;

      public MyDataProvider()
      {
        Name = "";
        Age = 0;
      }

      public string Name
      {
        get { return _name; }
        set { _name = value; }
      }

      public int Age
      {
        get { return _age; }
        set { _age = value; }
      }
    }

    public class LengthRangeRule : ValidationRule
    {
      private int _min;
      private int _max;

      public LengthRangeRule()
      {
      }

      public int Min
      {
        get { return _min; }
        set { _min = value; }
      }


      public int Max
      {
        get { return _max; }
        set { _max = value; }
      }

      public override ValidationResult Validate(object value, CultureInfo cultureInfo)
      {
        string strInserted = value.ToString();
        int actualLength = strInserted.Length;

        if ((actualLength < Min) || (actualLength > Max))
        {
          return new ValidationResult(false,
            "You entered " + actualLength + " characters. Please enter " + Min + " - " + Max + " characters.");
        }
        return new ValidationResult(true, null);
      }
    }

    public class AgeRangeRule : ValidationRule
    {
      private int _min;
      private int _max;

      public AgeRangeRule()
      {
      }

      public int Min
      {
        get { return _min; }
        set { _min = value; }
      }

      public int Max
      {
        get { return _max; }
        set { _max = value; }
      }

      public override ValidationResult Validate(object value, CultureInfo cultureInfo)
      {
        int age = 0;

        try
        {
          if (((string)value).Length > 0)
            age = Int32.Parse((String)value);
        }
        catch (Exception e)
        {
          return new ValidationResult(false, "Illegal characters or " + e.Message);
        }

        if ((age < Min) || (age > Max))
        {
          return new ValidationResult(false,
            "Please enter an age in the range: " + Min + " - " + Max + ".");
        }

        return new ValidationResult(true, null);
      }
    }

}