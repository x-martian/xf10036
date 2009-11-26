//----------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation, 2005.  All rights reserved.
//
// File: BrowserTabControlAutomationPeer.xaml.cs
//
// Description: This file contains the code for a tabbed browser control. 
//              Its children should be BrowserTabItems.
//
//----------------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Automation;
using System.Collections.Generic;
using System.Windows.Automation.Peers;


namespace Microsoft.Windows.SdkViewer.Controls
{
    public class BrowserTabControlAutomationPeer : TabControlAutomationPeer
    {
        public BrowserTabControlAutomationPeer(BrowserTabControl owner) : base(owner)
        {
        }
        protected override string GetClassNameCore()
        {
            return this.GetType().Name;
        }
        override protected List<AutomationPeer> GetChildrenCore()
        {
            // Return standard FlowDocumentPageViewer children
            // 
            List<AutomationPeer> retVal = base.GetChildrenCore();
            if (retVal == null) { retVal = new List<AutomationPeer>(); }

            // Returns nested buttons
            // 
            BrowserTabControl owner = (BrowserTabControl)Owner;
            Button button = (Button)owner.Template.FindName("CloseTabButton", owner);
            AutomationPeer automationPeer = UIElementAutomationPeer.CreatePeerForElement(button);
            retVal.Add(automationPeer);

            button = (Button)owner.Template.FindName("NewTabButton", owner);
            automationPeer = UIElementAutomationPeer.CreatePeerForElement(button);
            retVal.Add(automationPeer);

            return retVal;
        }
    }
}