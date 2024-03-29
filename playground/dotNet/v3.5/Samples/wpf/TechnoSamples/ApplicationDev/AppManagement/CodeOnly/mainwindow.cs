using System;
using System.Windows;
using System.Windows.Controls;

namespace CodeOnlyWindowsApplicationSample {
  
  /// <summary>
  /// MainWindow derives from Window to inherit the ability to show
  /// a window. Developers who derive from Window need to define both
  /// appearance and behavior in code. Appearance is set by filling the
  /// Window.Content property, while behavior is created by handling events,
  /// overriding methods, setting properties, and adding further custom
  /// behavior code.
  /// 
  /// NOTE: Since MainWindow is code-only (no markup) there is no need to
  ///       call the InitializeComponent method eg:
  /// 
  ///       public partial class MainWindow : Window {
  ///          public MainWindow() {
  ///            this.InitializeComponent();
  ///          }
  ///       }
  /// 
  ///       InitializeComponent is a method that is generated by the 
  ///       compiler when markup exists to apply the MainWindow XAML to 
  ///       the actual MainWindow instance, eg to register event handlers. 
  ///       If XAML were used, this class would also need to be a partial 
  ///       class, to merge with the partial class definition that implements 
  ///       the InitializeComponent, method that's generated by the compiler.
  /// </summary>
  public class MainWindow : Window {

    protected override void OnInitialized(EventArgs e) {
      base.OnInitialized(e);
      
      // Create appearance
      Button closeButton = new Button();
      closeButton.Content = "Close";
      this.Content = closeButton;
      
      // Define behavior
      closeButton.Click += delegate {
        this.Close();
      };
    }
  }
}
