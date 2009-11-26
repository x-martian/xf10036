//----------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation, 2005.  All rights reserved.
//
// File: TearOffPanel.xaml.cs
//
// Description: This is the code-behind file for TearOffPanel.xaml. 
//              The TearOffPanel panel supports multiple window states.
//
//----------------------------------------------------------------------------



using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Threading;

namespace Microsoft.Windows.SdkViewer.Controls
{
    //Enum to define the various states of the TearOffPanel or the embedded window object.
    public enum TearOffPanelState
    {
        /// <summary>
        /// Default: TearOffPanel is fully visible and floatingWindow is closed
        /// </summary>
        Docked,

        /// <summary>
        /// TearOffPanel is hidden and floatingWindow is closed
        /// </summary>
        Hidden,

        /// <summary>
        /// TearOffPanel takes up no space and its contents are in floatingWindow
        /// </summary>
        Floating
    }


    public partial class TearOffPanel : ContentControl
    {
        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------
        #region Constructors
        
        public TearOffPanel() : base()
        {
            InitializeComponent();

            // Set the style of the TearOffPanel. This style is also shared with the floating Window.
            this.Style = (Style)this.Resources["TearOffPanelStyle"];
        }
        #endregion Constructors

        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------
        #region Public Properties

        /// <summary>
        /// Dependency Property to store the current state of the TearOffPanel. The default value is Docked.
        /// </summary>
        public static readonly DependencyProperty CurrentStateProperty =
            DependencyProperty.Register(
                    "CurrentState",
                    typeof(TearOffPanelState),
                    typeof(TearOffPanel),
                    new PropertyMetadata(TearOffPanelState.Docked, new PropertyChangedCallback(OnCurrentStateInvalidated))
                );
        
        /// <summary>
        /// get / set the panel state
        /// </summary>
        public TearOffPanelState CurrentState
        {
            get { return (TearOffPanelState)GetValue(TearOffPanel.CurrentStateProperty); }
            set { SetValue(TearOffPanel.CurrentStateProperty, value); }
        }

        /// <summary>
        /// Returns the control's Content given the CurrentState
        /// </summary>
        public object ContentInActiveContainer
        {
            get
            {
                if (TearOffPanelState.Floating == CurrentState)
                {
                    return floatingWindow.Content;
                }
                else
                {
                    return Content;
                }
            }
        }

        #endregion Public Properties

        //------------------------------------------------------
        //
        //  Public Events
        //
        //------------------------------------------------------
        #region Public Events
        /// <summary>
        /// StateChanged event declaration. This is raised whenever CurrentState changes
        /// </summary>
        public static readonly RoutedEvent StateChangedEvent = EventManager.RegisterRoutedEvent("StateChanged", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(TearOffPanel));
        public event RoutedEventHandler StateChanged
        {
            add { AddHandler(StateChangedEvent, value); }
            remove { RemoveHandler(StateChangedEvent, value); }
        }

        #endregion Public Events


        //------------------------------------------------------
        //
        //  Private Methods
        //
        //------------------------------------------------------
        #region Private Methods

        /// <summary>
        /// Show or hide the window or panel appropriately when the CurrentState property changes
        /// </summary>
        /// <param name="sender">this</param>
        private static void OnCurrentStateInvalidated(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            TearOffPanel SourcePanel = sender as TearOffPanel;

            if (SourcePanel == null)
                return;

            switch (SourcePanel.CurrentState)
            {
                case TearOffPanelState.Floating:
                    if (!SourcePanel.IsWindowFloating)
                    {
                        SourcePanel.IsWindowFloating = true;
                    }
                    SourcePanel.Visibility = Visibility.Collapsed;
                    break;

                case TearOffPanelState.Docked:
                    if (SourcePanel.IsWindowFloating)
                    {
                        SourcePanel.IsWindowFloating = false;
                    }
                    SourcePanel.Visibility = Visibility.Visible;
                    break;

                case TearOffPanelState.Hidden:
                    if (SourcePanel.IsWindowFloating)
                    {
                        SourcePanel.IsWindowFloating = false;
                    }
                    SourcePanel.Visibility = Visibility.Collapsed;
                    break;

                default:
                    throw new InvalidOperationException("Invalid TearOffPanel state: " + SourcePanel.CurrentState);
            }

            SourcePanel.RaiseEvent(new RoutedEventArgs(TearOffPanel.StateChangedEvent, SourcePanel));
        }

        /// <summary>
        /// toggle window docking
        /// </summary>
        private void OnDockOrUnDockWindow(object sender, RoutedEventArgs e)
        {
            if (CurrentState == TearOffPanelState.Floating)
            {
                CurrentState = TearOffPanelState.Docked;
            }
            else if (CurrentState == TearOffPanelState.Docked)
            {
                CurrentState = TearOffPanelState.Floating;
            }
        }
        
        /// <summary>
        /// close the panel
        /// </summary>
        private void OnCloseWindow(object sender, RoutedEventArgs e)
        {
            CurrentState = TearOffPanelState.Hidden;
        }

        /// <summary>
        /// toggle between maximize and normal states
        /// </summary>
        private void OnMaximizeWindow(object sender, RoutedEventArgs e)
        {
            if (CurrentState == TearOffPanelState.Floating)
            {
                if (floatingWindow.WindowState == WindowState.Maximized)
                    floatingWindow.WindowState = WindowState.Normal;
                else
                    floatingWindow.WindowState = WindowState.Maximized;
            }
        }

        /// <summary>
        /// handle a double-click on the panel & change the state accordingly
        /// </summary>
        private void OnDragAreaMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (CurrentState == TearOffPanelState.Floating)
                {
                    if (floatingWindow.WindowState == WindowState.Normal)
                        floatingWindow.WindowState = WindowState.Maximized;
                    else
                        floatingWindow.WindowState = WindowState.Normal;

                }
            }
        }

        /// <summary>
        /// allow the user to drag the panel with the mouse
        /// </summary>
        private void OnDragAreaMouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                CurrentState = TearOffPanelState.Floating;
                floatingWindow.DragMove();
            }
        }
    

        #endregion Private Methods

        //------------------------------------------------------
        //
        //  Private Properties
        //
        //------------------------------------------------------
        #region Private Properties
    
        /// <summary>
        /// Shows or hides the floating window and TearOffPanel
        /// </summary>
        private bool IsWindowFloating
        {
            get { return isWindowFloating; }
            set
            {
                isWindowFloating = value;
                if (isWindowFloating && floatingWindow == null)
                {
                    floatingWindow = new Window();
                    floatingWindow.WindowStyle = WindowStyle.None;
                    floatingWindow.ShowInTaskbar = false;
                    
                    // use the same style as TearOffPanel so not to duplicate the template
                    floatingWindow.Background = Brushes.White;
                    floatingWindow.Style = new Style(floatingWindow.GetType(), this.Style); 

                    // this is the title text of the Panel
                    floatingWindow.Tag = this.Tag; 
                    floatingWindow.Owner = Application.Current.MainWindow;
                }

                if (value)
                {
                    // position the window relative to where the panel was is in the application
                    Grid PanelRoot = (Grid)this.Template.FindName("PanelRoot", this);
                    
                    Transform t = (Transform) PanelRoot.TransformToAncestor(Application.Current.MainWindow);
                    Matrix m;

                    m = t.Value;
                    Point point = new Point(0, 0) * m;

                    floatingWindow.WindowState = WindowState.Normal;
                    floatingWindow.Top = point.Y + Application.Current.MainWindow.Top;
                    floatingWindow.Left = point.X + Application.Current.MainWindow.Left;
                    floatingWindow.Height = this.ActualHeight;
                    floatingWindow.Width = this.ActualWidth;

                    // show the panel's contents in the window and hide the panel
                    object PanelContents = this.Content;
                    this.Content = null;

                    floatingWindow.Content = PanelContents;
                    floatingWindow.Show();
                }
                else
                {
                    // show the window's contents in the panel and hide the window
                    object WindowContents = floatingWindow.Content;

                    floatingWindow.Content = null;
                    this.Content = WindowContents;
                    floatingWindow.Hide();
                }
            }
        }

        #endregion Private Properties

        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------
        #region Private Fields
        private Window floatingWindow;      // TearOffPanel container for the Floating state
        private bool isWindowFloating;      // is the window floating?
        #endregion Private Fields

    }
}