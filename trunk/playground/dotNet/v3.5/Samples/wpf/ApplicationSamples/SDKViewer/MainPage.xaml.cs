//----------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation, 2005.  All rights reserved.
//
// File: MainPage.xaml.cs
//
// Description: This is the code-behind file for MainPage.xaml which
//              is the start page for the SdkViewer app.
//
//----------------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Annotations;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Windows.Media;
using System.Collections;
using System.ComponentModel;
using System.Xml;
using System.Windows.Input;
using Microsoft.Windows.SdkViewer.Controls;
using System.IO;
using System.Globalization;
using System.Diagnostics;

namespace Microsoft.Windows.SdkViewer
{
    public partial class MainPage : Page
    {
        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

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
        
        public static RoutedCommand ToggleKioskModeCommand = new RoutedCommand("ToggleKioskMode", typeof(MainPage), null);
        public static RoutedCommand SelectLocationCommand = new RoutedCommand("SelectLocation", typeof(MainPage), null);
        public static RoutedCommand CreateNewTabCommand = new RoutedCommand("CreateNewTab", typeof(MainPage), null);
        public static RoutedCommand CloseCurrentTabCommand = new RoutedCommand("CloseCurrentTab", typeof(MainPage), null);

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

        private IndexPanel TableOfContents
        {
            get 
            { 
                return (IndexPanel != null ? IndexPanel.Content as IndexPanel : null);
            }
        }

        internal BrowserTabControl MainPanelTabControl
        {
            get 
            { 
                return (TabbedViewer != null ? TabbedViewer.ContentInActiveContainer as BrowserTabControl : null);
            }
        }

        /// <summary>
        /// show documentation in default state
        /// </summary>
        private void OnShowDocumentationClicked(object sender, RoutedEventArgs args)
        {
            TabbedViewer.CurrentState = (TabbedViewer.CurrentState != TearOffPanelState.Docked) ? TearOffPanelState.Docked : TearOffPanelState.Hidden;
        }

        /// <summary>
        /// show table of contents in default state
        /// </summary>
        private void OnShowTableOfContentsClicked(object sender, RoutedEventArgs args)
        {
            IndexPanel.CurrentState  = (IndexPanel.CurrentState != TearOffPanelState.Docked) ? TearOffPanelState.Docked : TearOffPanelState.Hidden;
        }

        /// <summary>
        /// Use Online Content when "IsUsingOnlineContentButton" is Checked
        /// </summary>
        private void OnUseOnlineContent(object sender, RoutedEventArgs args)
        {
            if (TableOfContents == null)
                return;

            Settings.Instance.IsUsingOnlineContent = true;
            TableOfContents.NavigationMapSource = new Uri("pack://siteoforigin:,,,/OnlineContent/wpf_conceptual.hxt", UriKind.RelativeOrAbsolute);
        }

        /// <summary>
        /// Use Local Content when "IsUsingOnlineContentButton" is UnChecked
        /// </summary>
        private void OnUseLocalContent(object sender, RoutedEventArgs args)
        {
            if (TableOfContents == null)
                return;

            Settings.Instance.IsUsingOnlineContent = false;
            // included as a resource to work around this issue. 
            TableOfContents.NavigationMapSource = new Uri("pack://application:,,,/wpf_conceptual.hxt", UriKind.RelativeOrAbsolute);
        }


        /// <summary>
        /// Adjust the layout of the IndexPanel and TabbedViewer TearOffPanels so that
        /// the TabbedViewer always take up whole area of when the IndexPanel is not present
        /// </summary>
        private void OnIndexPanelStateChanged(object sender, RoutedEventArgs args)
        {
            TearOffPanel tp = (TearOffPanel)sender;
            ColumnDefinition IndexPanelColumn = TearOffPanelRoot.ColumnDefinitions[0];
            ColumnDefinition TabbedViewerColumn = TearOffPanelRoot.ColumnDefinitions[1];

            if (tp.CurrentState == TearOffPanelState.Docked)
            {
                IndexPanelColumn.Width = new GridLength(25, GridUnitType.Star);
                TabbedViewerColumn.Width = new GridLength(75, GridUnitType.Star);
            }
            else
            {
                IndexPanelColumn.Width = new GridLength(0, GridUnitType.Pixel);
                TabbedViewerColumn.Width = new GridLength(1, GridUnitType.Star);
            }
        }

        /// <summary>
        /// Update the content of the selected BrowserTabItem.
        /// </summary>
        private void OnIndexPanelSelectedItemChanged(Uri location)
        {
            BrowserTabItem tabItem = MainPanelTabControl.SelectedItem as BrowserTabItem;

            if (tabItem != null)
            {
                tabItem.Source = location;
            }
            else
            {
                MainPanelTabControl.AssignToNewTab(location);
            }

            // change the Location textbox background back to its default color 
            // on successful navigation
            LocationTextBox.Background = Brushes.Transparent;
        }

        /// <summary>
        /// When the selected item inside our BrowserTabControl changes, 
        /// we need to update the LocationTextBox to show the address of our
        /// current tab's frame.
        /// We also want to check to see if the SelectionChanged event fired 
        /// because all the tabs are gone - if that is the case, we set
        /// the Back and Forward buttons to be disabled.
        /// </summary>
        private void OnTabControlSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainPanelTabControl.SelectedItem == null)
            {
                // disable navigation buttons if there are no tabs
                if (MainPanelTabControl.HasItems == false)
                {
                    GoBackButton.IsEnabled = false;
                    GoForwardButton.IsEnabled = false;
                }
            }
        }


        /// <summary>
        /// This makes pressing "Enter" cause a navigation.
        /// </summary>
        #region Journal

        /// <summary>
        /// Navigate the tab item frame's journal back an entry.
        /// </summary>
        private void CurrentTabGoBack(object sender, RoutedEventArgs args)
        {
            if (MainPanelTabControl.SelectedItem != null)
            {
                BrowserTabItem tabItem = MainPanelTabControl.SelectedItem as BrowserTabItem;
                if (tabItem.TargetFrame.CanGoBack)
                {
                    tabItem.TargetFrame.GoBack();
                }
            }
            else
            {
                GoBackButton.IsEnabled = false;
                GoForwardButton.IsEnabled = false;
            }
        }

        /// <summary>
        /// Navigate the tab item frame's journal forward an entry
        /// </summary>
        private void CurrentTabGoForward(object sender, RoutedEventArgs args)
        {
            if (MainPanelTabControl.SelectedItem != null)
            {
                BrowserTabItem tabItem = MainPanelTabControl.SelectedItem as BrowserTabItem;

                if (tabItem.TargetFrame.CanGoForward)
                {
                    tabItem.TargetFrame.GoForward();
                }
            }
            else
            {
                GoBackButton.IsEnabled = false;
                GoForwardButton.IsEnabled = false;
            }
        }
        #endregion


        /// <summary>
        /// This makes pressing "Enter" cause a navigation.
        /// </summary>
        private void OnLocationTextBoxKeyDown(object sender, RoutedEventArgs args)
        {
            KeyEventArgs kea = args as KeyEventArgs;

            if (kea.Key == Key.Enter)
            {
                OnGoClicked(sender, args);
            }
        }

        /// <summary>
        /// attempt to navigate to the location entered by the user
        /// </summary>
        private void OnGoClicked(object sender, RoutedEventArgs args)
        {
            try
            {
                Uri NavigateUri = new Uri(LocationTextBox.Text, UriKind.RelativeOrAbsolute);

                // resources within the application are referenced using an absolute Uri. The Jorunal
                // stores the relative path.
                if (!NavigateUri.IsAbsoluteUri)
                {
                    NavigateUri = new Uri("pack://application:,,,/" + LocationTextBox.Text);
                }

                OnIndexPanelSelectedItemChanged(NavigateUri);
            }
            catch
            {
                // the user-supplied URI may not be valid so give a visual cue on error
                LocationTextBox.Background = new SolidColorBrush(Color.FromScRgb(0x88, 0x88, 0x00, 0x00));
            }
        }

        #region CommandBinding handlers

        /// <summary>
        /// switch between kiosk and regular mode
        /// </summary>
        private void OnToggleKioskModeCommand(object source, ExecutedRoutedEventArgs args)
        {
            Window win = Application.Current.MainWindow;

            if (win != null)
            {
                if (win.WindowState == WindowState.Maximized && win.WindowStyle == WindowStyle.None)
                {
                    // switch from kiosk mode to normal
                    win.WindowState = WindowState.Normal;
                    win.WindowStyle = WindowStyle.SingleBorderWindow;
                }
                else if (win.WindowState == WindowState.Normal && win.WindowStyle == WindowStyle.SingleBorderWindow)
                {
                    // switch from normal to kiosk mode
                    win.WindowState = WindowState.Maximized;
                    win.WindowStyle = WindowStyle.None;
                }
            }
        }

        /// <summary>
        /// highlight/select the current document's location
        /// </summary>
        private void OnSelectLocationCommand(object source, ExecutedRoutedEventArgs args)
        {
            if (LocationTextBox != null)
            {
                LocationTextBox.Focus();
                LocationTextBox.SelectAll();
            }
        }

        /// <summary>
        /// create a new tab
        /// </summary>
        private void OnCreateNewTabCommand(object source, ExecutedRoutedEventArgs args)
        {
            if (MainPanelTabControl != null)
            {
                MainPanelTabControl.CreateDefaultTab();
            }
        }

        /// <summary>
        /// close the selected tab
        /// </summary>
        private void OnCloseCurrentTabCommand(object source, ExecutedRoutedEventArgs args)
        {
            if (MainPanelTabControl != null)
            {
                MainPanelTabControl.RemoveSelectedTab();
            }
        }


        #endregion CommandBinding handlers

        #endregion Private Methods
    }
}