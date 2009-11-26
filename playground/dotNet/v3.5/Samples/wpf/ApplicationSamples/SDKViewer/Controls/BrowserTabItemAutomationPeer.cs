//----------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation, 2005.  All rights reserved.
//
// File: BrowserTabItemAutomationPeer.xaml.cs
//
// Description: This file contains the code for BrowserTabControl's TabItem.
//
//----------------------------------------------------------------------------



using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Collections.Generic;

namespace Microsoft.Windows.SdkViewer.Controls
{

    public class BrowserTabItemAutomationPeer : TabItemWrapperAutomationPeer 
    {
        public BrowserTabItemAutomationPeer(BrowserTabItem owner) : base(owner)
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

            // Returns nested frame
            // 
            BrowserTabItem owner = (BrowserTabItem)Owner;
            Frame frame = owner.InnerFrame;
            AutomationPeer automationPeer = UIElementAutomationPeer.CreatePeerForElement(frame);
            retVal.Add(automationPeer);

            return retVal;
        }
    }
}