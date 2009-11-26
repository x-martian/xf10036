namespace Microsoft.SdkViewer.Samples
{
    using System;
    using System.Windows;
    using System.Diagnostics;
    using System.Windows.Media;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Navigation;


    internal static class PageInitalizer
    {
        // Will disapear when changing on click for WebOC navigation
        private const string ABSOLUTE_PATH = "http://winfx.msdn.microsoft.com/library/default.asp?url=/library/en-us/cpref/html/";//"http://winfx.msdn.microsoft.com/library/en-us/cpref/html/";

        /// <summary>
        /// Perform the sample page initialization ( hide empty tabs, set frame to external web resources, ... )
        /// </summary>
        /// <param name="page"></param>
        /// <param name="frame"></param>
        /// <param name="sampleType"></param>
        internal static void Initialize(Page page)
        {
            TabControl tabControl = page.Content as TabControl;
            DisableEmptyTabControl(tabControl);
        }


        internal static void Clicked(TabItem sampleDocTab, string relativePath)
        {
            Frame frame = new Frame();
            frame.Source = PathToUri(relativePath);
            sampleDocTab.Content = frame;
        }


        /// <summary>
        /// Get the online help associated with this type
        /// </summary>
        /// <param name="sampleType">The type of the element to get info for</param>
        /// <returns>Returns a Uri pointing to the web documentation for this control</returns>
        private static Uri PathToUri(string relativePath)
        {
            return new Uri(ABSOLUTE_PATH + relativePath + ".asp?frame=true");
        }

        /// <summary>
        /// Look recursively into all item of a tab control to make sure they are not empty.
        /// If all are, don't show the tab
        /// </summary>
        /// <param name="tabControl">The TabControl to parse</param>
        /// <returns>true if is has been collapsed (empty)</returns>
        private static bool DisableEmptyTabControl(TabControl tabControl)
        {
            bool retVal = false;
            if (tabControl != null)
            {
                int collapsedItem = 0;
                for (int t = 0; t < tabControl.Items.Count; t++)
                {
                    TabItem tabItem = tabControl.Items[t] as TabItem;
                    if (tabItem != null)
                    {
                        if (DisableEmptyTabItem(tabItem)) { collapsedItem++; }
                    }
                }
                if (collapsedItem == tabControl.Items.Count)
                {
                    tabControl.Visibility = Visibility.Collapsed;
                    retVal = true;
                }
            }
            return retVal;
        }

        /// <summary>
        /// Analyze the content of a tab item to determin if it's empty or not.
        /// If it is empty (or contains a TabControl that is empty), don't show the item.
        /// </summary>
        /// <param name="tabItem">The TabItem to parse</param>
        /// <returns>true if is has been collapsed (empty)</returns>
        private static bool DisableEmptyTabItem(TabItem tabItem)
        {
            bool retVal = false;
            if (tabItem != null)
            {
                if (tabItem.Content == null)
                {
                    tabItem.Visibility = Visibility.Collapsed;
                    retVal = true;
                }
                else
                {
                    TabControl subTabControl = tabItem.Content as TabControl;
                    if (subTabControl != null)
                    {
                        retVal = DisableEmptyTabControl(subTabControl);
                        if (retVal) { tabItem.Visibility = Visibility.Collapsed; }
                    }
                }
            }
            return retVal;
        }

    }

}