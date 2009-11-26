//----------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation, 2005.  All rights reserved.
//
// File: SdkSinglePageViewer.xaml.cs
//
// Description: This version of FlowDocumentPageViewer encapsulates Annotations, 
//              Printing, and a customized Style.
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
    public class SdkSinglePageViewerAutomationPeer : FlowDocumentPageViewerAutomationPeer
    {
        public SdkSinglePageViewerAutomationPeer(SdkSinglePageViewer owner) : base(owner)
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


            // Returns expected Controls for automation to work (Print, SaveAs, annotation button, ...)
            // 
            SdkSinglePageViewer owner = (SdkSinglePageViewer)Owner;

            Button button = (Button)owner.Template.FindName("DocumentPrint", owner);
            AutomationPeer automationPeer = UIElementAutomationPeer.CreatePeerForElement(button);
            retVal.Add(automationPeer);

            Menu menu = (Menu)owner.Template.FindName("DocumentAnnotations", owner);
            automationPeer = UIElementAutomationPeer.CreatePeerForElement(menu);
            retVal.Add(automationPeer);

            button = (Button)owner.Template.FindName("DocumentSaveAs", owner);
            automationPeer = UIElementAutomationPeer.CreatePeerForElement(button);
            retVal.Add(automationPeer);

            button = (Button)owner.Template.FindName("FirstPageButton", owner);
            automationPeer = UIElementAutomationPeer.CreatePeerForElement(button);
            retVal.Add(automationPeer);


            button = (Button)owner.Template.FindName("PreviousPageButton", owner);
            automationPeer = UIElementAutomationPeer.CreatePeerForElement(button);
            retVal.Add(automationPeer);

            button = (Button)owner.Template.FindName("NextPageButton", owner);
            automationPeer = UIElementAutomationPeer.CreatePeerForElement(button);
            retVal.Add(automationPeer);

            button = (Button)owner.Template.FindName("LastPageButton", owner);
            automationPeer = UIElementAutomationPeer.CreatePeerForElement(button);
            retVal.Add(automationPeer);
            

            return retVal;
        }
    }
}