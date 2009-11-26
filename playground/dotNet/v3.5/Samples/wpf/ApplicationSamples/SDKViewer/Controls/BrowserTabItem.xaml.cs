//----------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation, 2005.  All rights reserved.
//
// File: BrowserTabItem.xaml.cs
//
// Description: This file contains the code for BrowserTabControl's TabItem.
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
    public partial class BrowserTabItem : TabItem
    {
        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------
        #region Constructors

        public BrowserTabItem()
        {
            InitializeComponent();

            // Input handlers (for opening new BrowserTabItems)
            AddHandler(System.Windows.Input.Mouse.PreviewMouseDownEvent, new System.Windows.Input.MouseButtonEventHandler(OnMouseButtonDown), true);
            AddHandler(System.Windows.Input.Mouse.PreviewMouseUpEvent, new System.Windows.Input.MouseButtonEventHandler(OnMouseButtonUp), true);

            KeyDown += new KeyEventHandler(OnKeyDown);

            // Navigation eventhandlers for updating the ProgressBar
            InnerFrame.NavigationProgress += new NavigationProgressEventHandler(OnNavigationProgress);
            InnerFrame.Navigating += new NavigatingCancelEventHandler(OnNavigating);
            InnerFrame.LoadCompleted += new LoadCompletedEventHandler(OnNavigationStopped);
            InnerFrame.NavigationStopped += new NavigationStoppedEventHandler(OnNavigationStopped);

            Header = (TextBlock)Resources["HeaderTextBlock"];
        }

        #endregion Constructors

        //------------------------------------------------------
        //
        //  Overriden Methods
        //
        //------------------------------------------------------

        /// <summary>
        /// Create an Automation proxy for this control.
        /// </summary>
        protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer()
        {
            return new BrowserTabItemAutomationPeer(this);
        }


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        /// <summary>
        /// Initializes all BrowserTabItems when selection changes.
        /// 
        /// In this case, BrowserTabItem reloads SdkSinglePageViewer's
        /// annotations so that they are in-sync with other Tabs that are
        /// on the same page.
        /// </summary>
        protected override void OnSelected(RoutedEventArgs args)
        {
            SdkSinglePageViewer spv;
            
            if (InnerFrame != null)
            {
                spv = InnerFrame.Content as SdkSinglePageViewer;
                if (spv != null)
                {
                    spv.EnableAnnotations();
                }
            }

            base.OnSelected(args);
        }

        protected override void OnUnselected(RoutedEventArgs args)
        {
            SdkSinglePageViewer spv;
            
            if (InnerFrame != null)
            {
                spv = InnerFrame.Content as SdkSinglePageViewer;
                if (spv != null)
                {
                    spv.DisableAnnotations();
                }
            }

            base.OnUnselected(args);
        }

        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------
        #region Public Properties

        /// <summary>
        /// The Source property is the unresolved relative URI to a resource. It may begin with "\" .
        /// This property is data bound to the BrowserTabItem.TargetFrame.Source property. This is defined in BrowserTabItem.xaml.
        /// </summary>
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(System.Uri), typeof(BrowserTabItem));
        public Uri Source
        {
            get { return (System.Uri)GetValue(BrowserTabItem.SourceProperty); }
            set { SetValue(BrowserTabItem.SourceProperty, value); }
        }


        /// <summary>
        /// Returns the inner frame in the style tree of this control
        /// </summary>
        public Frame TargetFrame
        {
            get
            {
                return InnerFrame;
            }
        }

        /// <summary>
        /// Returns the header text of this control
        /// </summary>        
        public string HeaderText 
        {
            get 
            {
                return ((TextBlock)Header).Text; 
            }
            set 
            {
                ((TextBlock)Header).Text = value; 
            }
        }        
        
        #endregion Public Properties

        //------------------------------------------------------
        //
        //  Public Events
        //
        //------------------------------------------------------

        //------------------------------------------------------
        //
        //  Private Methods
        //
        //------------------------------------------------------
        #region Private Methods

        /// <summary>
        /// Opens a new TabItem if navigation was caused by a shift-click.
        /// Displays ProgressBar if TabItem does not launch a new BrowserTabItem
        /// </summary>
        private void OnNavigating(object sender, NavigatingCancelEventArgs e)
        {
            if (isShiftClick & e.Uri != null)
            {
                parent = ItemsControl.ItemsControlFromItemContainer(this) as BrowserTabControl;
                parent.AssignToNewTab(e.Uri);
                e.Cancel = true;
            }

            // NavigationProgressBar may be null before Loaded is called when the application first loads
            if ((!e.IsNavigationInitiator || !isShiftClick) && NavigationProgressOverlay != null)
            {
                NavigationProgressOverlay.Visibility = Visibility.Visible;
            }

        }

        /// <summary>
        /// Updates ProgressBar for each TabItem to indicate remaining download time
        /// </summary>
        private void OnNavigationProgress(object sender, NavigationProgressEventArgs e)
        {
            if (NavigationProgressBar != null)
            {
                NavigationProgressBar.Value = (e.BytesRead / e.MaxBytes) * NavigationProgressBar.Maximum;
            }
        }

        /// <summary>
        /// When navigation stops (cancelled or download completes)
        /// we want to replace the ProgressBar with the new Header
        /// </summary>
        private void OnNavigationStopped(object sender, NavigationEventArgs e)
        {
            SdkSinglePageViewer spv = InnerFrame.Content as SdkSinglePageViewer;
            string            title = string.Empty;

            if (spv != null)
            {
                title = spv.DocumentTitle;
            }
            else
            {
                // the frame can contain a Web browser control
                if (InnerFrame.Source != null)
                {
                    // if the current document is a WebOC
                    title = InnerFrame.Source.ToString();
                }
            }

            if (title != string.Empty)
            {
                HeaderText = title;
            }

            if (NavigationProgressOverlay != null)
            {
                NavigationProgressOverlay.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// watch for potential shift+click on the app
        /// </summary>
        private void OnMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            isShiftClick = false;
            clickPoint = new Point(0, 0);

            if (e.LeftButton == MouseButtonState.Pressed &&
                System.Windows.Input.Keyboard.Modifiers == System.Windows.Input.ModifierKeys.Shift)
            {
                clickPoint = e.GetPosition(this);
            }
        }

        /// <summary>
        /// verify a shift+click on the app
        /// </summary>
        private void OnMouseButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1 &&
                e.GetPosition(this) == this.clickPoint &&
                System.Windows.Input.Keyboard.Modifiers == System.Windows.Input.ModifierKeys.Shift)
            {
                isShiftClick = true;
            }
            else
            {
                isShiftClick = false;
            }
        }

        /// <summary>
        /// This handles Ctrl-F4 to remove a tab
        /// </summary>
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.F4) && (System.Windows.Input.Keyboard.Modifiers == System.Windows.Input.ModifierKeys.Control))
            {
                parent = ItemsControl.ItemsControlFromItemContainer(this) as BrowserTabControl;
                parent.RemoveSelectedTab();
            }
        }

        #endregion Private Methods


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------
        #region Private Fields

        private Point               clickPoint;             // point clicked on by the mouse
        private bool                isShiftClick = false;   // track shift+click events
        private BrowserTabControl   parent;                 // container for tab control

        #endregion Private Fields
    }

}