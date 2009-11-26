//----------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation, 2005.  All rights reserved.
//
// File: BrowserTabControl.xaml.cs
//
// Description: This file contains the code for a tabbed browser control. 
//              Its children should be BrowserTabItems.
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

    public partial class BrowserTabControl : TabControl
    {
        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------
        #region Constructors
        public BrowserTabControl()
        {
            InitializeComponent();
            SnippetViewer.EditClicked += new EditClickedEventHandler(OnSnippetEditClicked);
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
            return new BrowserTabControlAutomationPeer(this);
        }



        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------
        #region Public Methods

        /// <summary>
        /// Creates a new tab and navigates to the given location.
        /// </summary>
        internal BrowserTabItem CreateNewTab()
        {
            BrowserTabItem tabItem = new BrowserTabItem();
            Items.Add(tabItem);
            SelectedItem = tabItem;
            ((BrowserTabItem)SelectedItem).Focus();

            return tabItem;
        }

        /// <summary>
        /// Creates a new tab and navigates to the given location.
        /// </summary>
        public void AssignToNewTab(Uri location)
        {
            // Resources within the application are referenced using an absolute Uri. Relative
            // Uris should be made absolute.
            if (!location.IsAbsoluteUri)
            {
                location = new Uri("pack://application:,,,/" + location.ToString());
            }
            BrowserTabItem tabItem = CreateNewTab();
            tabItem.Source = location;
        }


        /// <summary>
        /// Add a new tab and navigate to the default location. 
        /// </summary>
        public void CreateDefaultTab()
        {
            BrowserTabItem tabItem = SelectedItem as BrowserTabItem;
            if (tabItem != null)
            {
                if (tabItem.TargetFrame.Source != null)
                {
                    AssignToNewTab(tabItem.TargetFrame.Source);
                }
                else
                {
                    // Not a Uri, Clone the object (in the current implementation it can only be a XamlEditor object)
                    ICloneable clonable = tabItem.InnerFrame as ICloneable;
                    if (clonable != null)
                    {
                        BrowserTabItem newTab = CreateNewTab();
                        newTab.InnerFrame.Content = clonable.Clone();
                        newTab.HeaderText = tabItem.HeaderText;
                    }
                }
            }
            else
            {
                // If there are no tabs, then we should start at a welcome page
                Uri welcomeUri = null;
                if (Settings.Instance.IsUsingOnlineContent)
                {
                    welcomeUri = new Uri("pack://siteoforigin:,,,/OnlineContent/f667bd15-2134-41e9-b4af-5ced6fafab5d.xaml", UriKind.RelativeOrAbsolute);
                }
                else
                {
                    welcomeUri = new Uri("pack://application:,,,/Content/f667bd15-2134-41e9-b4af-5ced6fafab5d.xaml", UriKind.RelativeOrAbsolute);
                }
                AssignToNewTab(welcomeUri);
            }
        }


        /// <summary>
        /// Removes the currently selected TabItem
        /// </summary>
        public void RemoveSelectedTab()
        {
            if (SelectedItem != null)
            {
                Items.Remove(SelectedItem);

                // the next TabItem isn't focused by default
                if (SelectedItem != null)
                {
                    ((BrowserTabItem)SelectedItem).Focus();
                }
            }
        }
        #endregion Public Methods

        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

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
        /// Create and focs on a new tab containing the SelectedItem's Source URI when this is clicked.
        /// Show the Welcome page if there are no active tabs.
        /// </summary>
        private void OnNewTabClicked(object sender, EventArgs args)
        {
            CreateDefaultTab();
        }

        /// <summary>
        /// Removes the current selected tab
        /// </summary>
        private void OnCloseTabClicked(object sender, EventArgs args)
        {
            RemoveSelectedTab();
        }


        /// <summary>
        /// open a sample for editing in a new tab
        /// </summary>        
        private void OnSnippetEditClicked(object sender, object root)
        {
            BrowserTabItem tabItem = CreateNewTab();
            tabItem.InnerFrame.Content = root;
            tabItem.HeaderText = (sender != null) ? sender.ToString() : "New TabItem";
        }

        #endregion Private Methods

    }
}