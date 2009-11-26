//----------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation, 2005.  All rights reserved.
//
// File: IndexPanel.xaml.cs
//
// Description: This is a custom control for an index panel
//
//----------------------------------------------------------------------------

using System;
using System.IO;
using System.Xml;
using System.Windows;
using System.Collections;
using System.Diagnostics;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Input;
using System.Globalization;
using System.ComponentModel;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace Microsoft.Windows.SdkViewer.Controls
{
    /// <summary>
    /// Delegate with a Uri.
    /// </summary>
    /// <param name="uri"></param>
    public delegate void SelectedItemChangedEventHandler(Uri uri);

    /// <summary>
    /// This is the control for SdkViewer, index panel.
    /// </summary>
    public partial class IndexPanel: ContentControl
    {
        //------------------------------------------------------
        //
        //  Private Members
        //
        //------------------------------------------------------
        #region Private Members

        private FilterForTreeView filterForTreeView = null;
        private FilterForEmbededListBox filterForEmbededListBox = null;

        #endregion Private Members

        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------
        #region Constructors
        
        /// <summary>
        /// Constructor
        /// </summary>
        public IndexPanel()
        {
            InitializeComponent();
            filterForTreeView = new FilterForTreeView();
            filterForEmbededListBox = new FilterForEmbededListBox();
        }
        
        #endregion Constructors

        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------
        #region Public Properties
        
        /// <summary>
        /// This property stores the location of the XML navigation map used in this panel's XmlDataSource.
        /// These XML files is used to display the table of contents and provide absolute URIs to the
        /// documents. 
        /// 
        /// The NavigationMapSource URI property is databound to XmlDataProvider.Source. The schema is expected
        /// be the same since the XPath query is static. 
        /// </summary>
        public static readonly DependencyProperty NavigationMapSourceProperty =
            DependencyProperty.Register(
            "NavigationMapSource",
            typeof(Uri),
            typeof(IndexPanel),
            new FrameworkPropertyMetadata(null, new PropertyChangedCallback(NavigationMapSourceInvalidated))
            );

        public Uri NavigationMapSource
        {
            get
            {
                return (Uri)GetValue(IndexPanel.NavigationMapSourceProperty);
            }

            set
            {
                SetValue(IndexPanel.NavigationMapSourceProperty, value);
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
        /// SelectedItemChanged is raised whenever the user selects an item in either the ListBox or TreeView.
        /// </summary>
        public event SelectedItemChangedEventHandler SelectedItemChanged;
        
        #endregion Public Events



        //------------------------------------------------------
        //
        //  Private Methods
        //
        //------------------------------------------------------
        #region Private Methods

        /// <summary>
        /// Update the data filer when the Text property of the TextBox changes.
        /// </summary>
        private void OnFilterTextBoxTextChanged(object sender, TextChangedEventArgs args)
        {
            // Update keyword for search filtering
            FilterForControl.Keyword = FilterTextBox.Text.Trim();

            // Apply filter on both the main listbox and the treeview so the empty group won't be displayed.
            IndexListBox.Items.Filter = new Predicate<object>(ListViewHasTopic);
            IndexTreeView.Items.Filter = new Predicate<object>(TreeViewHasTopic);
        }

        /// <summary>
        /// EventHandler on SelectionChange event of the ListBox. This is raised
        /// when the user selected an item to navigate to.
        /// </summary>
        private void OnListBoxSelectionChanged(object sender, RoutedEventArgs args)
        {
            OnSelectionChanged(sender);
        }

        /// <summary>
        /// EventHandler on SelectionChange event of the TreeView. This is raised
        /// when the user selected an item to navigate to.
        /// Note : this is needed because VB has != signature for both ListBox and TreeView and since
        /// the xaml is shared by the C# and VB project, we need this extra handler.
        /// </summary>
        private void OnTreeViewSelectionChanged(object sender, RoutedEventArgs args)
        {
            OnSelectionChanged(sender);
        }

        private void OnSelectionChanged(object sender)
        {
            XmlElement xmlElement = null;

            if (sender is ListBox)
            {
                xmlElement = ((ListBox)sender).SelectedItem as XmlElement;
            }
            else if (sender is TreeView)
            {
                xmlElement = ((TreeView)sender).SelectedItem as XmlElement;
            }

            if (xmlElement != null)
            {
                string location = GetLocation(xmlElement);
                if (location != string.Empty)   // Should never happen, see GetLocation function
                {
                    RaiseSelectionChanged(new Uri(location, UriKind.RelativeOrAbsolute));
                }
            }
        }



        /// <summary>
        /// Sent out event with Uri, so the other panel can be change accordingly.
        /// </summary>
        private void RaiseSelectionChanged(Uri uri)
        {
            if (null != SelectedItemChanged)
            {
                SelectedItemChanged(uri);
            }
        }

        /// <summary>
        /// Toggle the content display layout between TreeView and ListBox        
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void ToggleView(object sender, RoutedEventArgs args)
        {
            Storyboard sb;

            // elements on the page may be null since the Checked event may be raised before the page is completely Loaded
            if (IndexListBox != null && IndexTreeView != null)
            {
                if ((bool)toggleListBoxButton.IsChecked)
                {
                    sb = FindResource("ShowListBoxAnimation") as Storyboard;
                    sb.CurrentStateInvalidated += new EventHandler(OnShowListBoxAnimationStateChanged);
                    sb.Begin(this, HandoffBehavior.SnapshotAndReplace);
                }
                else if ((bool)toggleTreeViewButton.IsChecked)
                {
                    sb = FindResource("ShowTreeViewAnimation") as Storyboard;
                    sb.CurrentStateInvalidated += new EventHandler(OnShowTreeViewAnimationStateChanged);
                    sb.Begin(this, HandoffBehavior.SnapshotAndReplace);
                }
            }
        }

        /// <summary>
        /// Enable the TreeView once the crossfade Storyboard has completed
        /// </summary>
        private void OnShowTreeViewAnimationStateChanged(object sender, EventArgs args)
        {
            Clock clock = (Clock)sender;

            if (clock.CurrentState != ClockState.Active)
            {
                IndexListBox.IsHitTestVisible   = false;
                IndexTreeView.IsHitTestVisible  = true;
            }
        }

        /// <summary>
        /// Enable the ListBox once the crossfade Storyboard has completed
        /// </summary>
        private void OnShowListBoxAnimationStateChanged(object sender, EventArgs args)
        {
            Clock clock = (Clock)sender;

            if (clock.CurrentState != ClockState.Active)
            {
                IndexListBox.IsHitTestVisible   = true;
                IndexTreeView.IsHitTestVisible  = false;
            }
        }

        /// <summary>
        /// Are there any topics in this group?
        /// </summary>
        private bool TreeViewHasTopic(object groupItem)
        {
            bool        retVal  = false;
            XmlElement  element = groupItem as XmlElement;

            XmlNodeList children = element.SelectNodes("HelpTOCNode");

            foreach (XmlNode child in children)
            {
                // if one of the topic should be displayed, the group/parent should also be displayed.
                retVal |= filterForTreeView.Contains(child);
            }

            return retVal;

        }
        /// <summary>
        /// Are there any topics in this group?
        /// </summary>
        private bool ListViewHasTopic(object groupItem)
        {
            bool retVal         = false;
            XmlElement element = groupItem as XmlElement;
            XmlNodeList children = element.SelectNodes("descendant::HelpTOCNode");

            foreach (XmlNode child in children)
            {
                // if one of the topic should be displayed, the group/parent should also be displayed.
                retVal |= filterForEmbededListBox.Contains(child);
            }

            return retVal;

        }

        /// <summary>
        /// Gets the title attribute of the current XML element
        /// </summary>
        /// <returns>Title of the topic</returns>
        private string GetTitle(XmlElement element)
        {
            if (element.Attributes["Title"] == null) { return string.Empty; } // Should never happen though, an exception might be more appropriate here
            return element.Attributes["Title"].Value;
        }

        /// <summary>
        /// Get the location for this element
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private string GetLocation(XmlElement element)
        {
            if (element.Attributes["Url"] == null) { return string.Empty; } // Should never happen though, an exception might be more appropriate here
            return element.Attributes["Url"].Value;
        }

        /// <summary>
        /// update when the navigation map changes
        /// </summary> 
        private static void NavigationMapSourceInvalidated(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            IndexPanel SourcePanel = sender as IndexPanel;
            if (SourcePanel == null)
                return;

            XmlDataProvider xdp = SourcePanel.Resources["NavigationMapListBox"] as XmlDataProvider;
            if (xdp != null)
            {
                xdp.Source = SourcePanel.NavigationMapSource;
                xdp.Refresh();
            }
            xdp = SourcePanel.Resources["NavigationMapTreeView"] as XmlDataProvider;
            if (xdp != null)
            {
                xdp.Source = SourcePanel.NavigationMapSource;
                xdp.Refresh();
            }
        }

        #endregion Private Methods

    }


    /// <summary>
    /// A Convert add filter
    /// </summary>
    public abstract class FilterForControl : IValueConverter
    {
        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------  
        #region Public Methods

        /// <summary>
        /// Does this topic has a title or a child containing the keyword?
        /// <summary>
        public bool Contains(object de)
        {
            bool retVal = false;
            XmlNode node = de as XmlNode;

            // Ignore non-xmlNode elements
            if (node != null)
            {
                XmlAttribute    titleAttribute  = node.Attributes["Title"];
                // Ignore ChildNodes with no title
                if (null != titleAttribute)
                {
                    string title = titleAttribute.Value;
                    retVal = title.ToLowerInvariant().Contains(Keyword.ToLowerInvariant());
                    if (node.HasChildNodes)
                    {
                        retVal |= ContainChildren(node);
                    }
                }
            }

            return retVal;
        }

        public abstract bool ContainChildren(XmlNode parentNode);


        /// <summary>
        /// add filter for the embeded listboxes.
        /// </summary>
        public object Convert(object o, Type type, object parameter, CultureInfo culture)
        {
            //Find ListBox instance.
            ICollectionView view    = CollectionViewSource.GetDefaultView((IEnumerable)o);

            view.Filter = new Predicate<object>(Contains);

            return view;
        }

        /// <summary>
        /// Convert from Uri to Location, not needed.
        /// </summary>
        public object ConvertBack(object o, Type type, object parameter, CultureInfo culture)
        {
            return null;
        }

        #endregion Public Methods

        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------  
        
        #region Public Properties
        
        /// <summary>
        /// 
        /// </summary>        
        public static string Keyword
        {
            get { return keyWord; }
            set { keyWord = value; }
        }
        
        #endregion Public Properties
        
        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------        
        #region Private Fields
        
        private static string keyWord = string.Empty;           // string for filtering content
        
        #endregion Private Fields
        
    }

    /// <summary>
    /// A Convert add filter for the embeded listboxes.
    /// </summary>
    public class FilterForEmbededListBox : FilterForControl
    {
        /// <summary>
        /// ListView does not need to check for nested children
        /// <summary>
        public override bool ContainChildren(XmlNode node)
        {
            return false;
        }
    }

    /// <summary>
    /// A Convert add filter for the embeded listboxes.
    /// </summary>
    public class FilterForTreeView : FilterForControl
    {
        /// <summary>
        /// If any children contains the keyword, parent must be displayed
        /// <summary>
        public override bool ContainChildren(XmlNode parentNode)
        {
            bool retVal = false;

            // if any of the child contains the keyword, parent must be displayed
            for (int t = 0; t < parentNode.ChildNodes.Count; t++)
            {
                retVal |= Contains(parentNode.ChildNodes[t]);
            }

            return retVal;
        }
    }
}