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
using System.Collections;
using System.IO;
using System.Windows;
using System.Windows.Annotations;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Annotations.Storage;

using System.IO.Packaging;
using System.Windows.Xps.Packaging;
using System.Windows.Xps.Serialization;

using System.Net;


namespace Microsoft.Windows.SdkViewer.Controls
{
    public partial class SdkSinglePageViewer : FlowDocumentPageViewer
    {
        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------
        #region Constructors

        static SdkSinglePageViewer()
        {
             // Force pack protocol to be registered
            WebRequest.RegisterPrefix("pack", new PackWebRequestFactory());
        }

        public SdkSinglePageViewer() : base()
        {
            Loaded += OnLoaded;
            InitializeComponent();
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
            return new SdkSinglePageViewerAutomationPeer(this);
        }


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        /// <summary>
        /// Disables and save annotations on the current document to disk.
        /// </summary>
        public void DisableAnnotations()
        {
            if (currentAnnotationStream != null)
            {
                currentAnnotationStream.Flush();
                currentAnnotationStream.Close();
                currentAnnotationStream = null;
            }

            if (annotationService != null)
            {
                if (annotationService.IsEnabled)
                {
                    annotationService.Disable();
                }
                annotationService = null;
            }
        }

        /// <summary>
        /// This enables AnnotationService on the current document.
        /// </summary>
        public void EnableAnnotations()
        {
            if (annotationService == null && viewAnnotationMenuItem != null && viewAnnotationMenuItem.IsChecked)
            {
                string fileName = System.IO.Path.Combine(Settings.Instance.LocalFileCacheLocation, "Annotation-" + DocumentID + ".xml");

                try
                {
                    // allow multiple instances of the same page to overwrite each other's changes
                    currentAnnotationStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                    annotationService       = new AnnotationService(this);
                    annotationService.Enable(new XmlStreamStore(currentAnnotationStream));
                    annotationService.Store.AutoFlush = true;
                } 
                catch (Exception)
                {
                    // Remove all annotations that cause exceptions and try to create an empty file
                    DisableAnnotations();
                    File.Delete(fileName);

                    currentAnnotationStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                    annotationService       = new AnnotationService(this);
                    annotationService.Enable(new XmlStreamStore(currentAnnotationStream));
                    annotationService.Store.AutoFlush = true;
                }
            }
        }


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------
        #region Public Properties

        /// <summary>
        /// Returns the title of the current document. SdkViewer XAML has the title embedded in the Tag property
        /// </summary>
        public string DocumentTitle
        {
            get
            {
                return Tag as string;
            }
        }

        /// <summary>
        /// Returns the unique document title for each SDK document.
        /// </summary>
        public string DocumentID
        {
            get
            {
                return Name as string;
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
        /// make sure that we wire up the references to cached templated elements before we use them
        /// </summary>
        private void OnLoaded(object sender, EventArgs args)
        {
            printDialog             = (SdkPrintDialog)Template.FindName("printDialog", this);
            viewAnnotationMenuItem  = (MenuItem)Template.FindName("viewAnnotationMenuItem", this);

            if (printDialog == null || viewAnnotationMenuItem == null)
            {
                throw new ApplicationException("unable to find template UI components!");
            }

            EnableAnnotations();
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnViewAnnotationChecked(object s, EventArgs args)
        {
            EnableAnnotations();
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnViewAnnotationUnchecked(object s, EventArgs args)
        {
            DisableAnnotations();
        }


        /// <summary>
        /// print all documents in open tabs
        /// </summary>
        private void PrintAllDocuments(object sender, EventArgs args)
        {
            StartPrintJob(true /*print all documents*/);
        }

        /// <summary>
        /// print the current document tab
        /// </summary>
        private void PrintDocument(object sender, EventArgs args)
        {
            StartPrintJob(false /*print current document*/);
        }

        /// <summary>
        /// 
        /// </summary>
        private void StartPrintJob(bool printAllDocuments)
        {
            if (printDialog.IsOpen)
            {
                printDialog.EndOrCancelPrintJob(true  /*close dialog*/);
            }
            else
            {
                DependencyObject rootElement = (DependencyObject)((Application)Application.Current).MainWindow.Content;
                MainPage mainPage = rootElement as MainPage;
                TabControl MainPanelTabControl = ((MainPage)rootElement).MainPanelTabControl;
//                TabControl MainPanelTabControl = (TabControl)LogicalTreeHelper.FindLogicalNode(rootElement, "MainPanelTabControl");

                if (MainPanelTabControl != null)
                {
                    // check to see if we want to print the current document tab or all open document tabs
                    if (printAllDocuments)
                    {
                        // get the open tabbed documents
                        foreach (BrowserTabItem tabItem in MainPanelTabControl.Items)
                        {
                            // Reload the copy of the document so we can serialize it.
                            // This is needed because having 2 controls modifying the same DocumentPaginator is unsupported (undefined) and may gives unexpected result
                            DocumentViewerBase docViewerBase = CloneDocumentViewer(tabItem.Source);
                            printDialog.AddDocumentToPrintJob(docViewerBase, currentAnnotationStream);
                        }
                    }
                    else
                    {
                        BrowserTabItem tabItem = MainPanelTabControl.SelectedItem as BrowserTabItem;
                        // Reload the copy of the document so we can serialize it.
                        // This is needed because having 2 controls modifying the same DocumentPaginator is unsupported (undefined) and may gives unexpected result
                        DocumentViewerBase docViewerBase = CloneDocumentViewer(tabItem.Source);
                        printDialog.AddDocumentToPrintJob(docViewerBase, currentAnnotationStream);
                    }
                }

                printDialog.ShowSdkDialog();
            }
        }

        /// <summary>
        /// Callback occurs when user click the saveAsButton
        /// </summary>
        private void OnSaveDocumentAs(object sender, RoutedEventArgs args)
        {
            string fileName = string.Empty;

            // Invoke the Winform SaveFileDialog
            using (System.Windows.Forms.SaveFileDialog saveDialog = new System.Windows.Forms.SaveFileDialog())
            {
                saveDialog.AddExtension = true;
                saveDialog.CheckPathExists = true;
                
                // Only support container files
                saveDialog.DefaultExt = "container";
                saveDialog.Filter = "Xps Package file (*.xps)|*.xps";
                saveDialog.FilterIndex = 0;
                saveDialog.ValidateNames = true;

                System.Windows.Forms.DialogResult result = saveDialog.ShowDialog();
                if (result != System.Windows.Forms.DialogResult.OK) { return; }
                fileName = saveDialog.FileName;
            }

            // If the file exists, make sure it's not write protected
            if (File.Exists(fileName))
            {
                //Check if file is readonly
                FileAttributes fileAttributes = File.GetAttributes(fileName);
                
                if ((fileAttributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    // It is a ready only file; might be an important document.
                    // Double check the user really want to overwrite this.
                    MessageBoxResult msgBoxResult = MessageBox.Show("The file is read only.\nAre you sure you want to overwrite this file ?", "Read only file", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                    
                    if (msgBoxResult != MessageBoxResult.Yes) 
                    { 
                        return; 
                    }
                    
                    fileAttributes ^= FileAttributes.ReadOnly;
                    System.IO.File.SetAttributes(fileName, fileAttributes);
                }
            }
            
            // Find the active document.
            DependencyObject rootElement = (DependencyObject)((Application)Application.Current).MainWindow.Content;
            MainPage mainPage = rootElement as MainPage;
            TabControl MainPanelTabControl = mainPage.MainPanelTabControl;
            BrowserTabItem tabItem = MainPanelTabControl.SelectedItem as BrowserTabItem;

            // Serialize the Document to file
            try
            {
                // Reload the copy of the document so we can serialize it.
                // This is needed because having 2 controls modifying the same DocumentPaginator is unsupported (undefined) and may gives unexpected result
                DocumentViewerBase docViewerBase = CloneDocumentViewer(tabItem.Source);
                DocumentPaginator documentPaginator = new AnnotationDocumentPaginator(docViewerBase.Document.DocumentPaginator, annotationService.Store);

                // file might be locked by another process hence the try/catch block
                using (FileStream fileStream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Write))
                {
                    // create the container.
                    using (System.IO.Packaging.Package package = System.IO.Packaging.Package.Open(fileStream, FileMode.Create, FileAccess.ReadWrite))
                    {
                        using (XpsDocument xpsDoc = new XpsDocument(package, CompressionOption.Maximum))
                        {
                            XpsPackagingPolicy packagePolicy = new XpsPackagingPolicy(xpsDoc);
                            XpsSerializationManager serializationMgr = new XpsSerializationManager(packagePolicy, false);
                            serializationMgr.SaveAsXaml(documentPaginator);
                        }
                    }
                }
            }
            catch (IOException e)
            {
                MessageBox.Show(Application.Current.MainWindow, e.Message, "Unable to save", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        private DocumentViewerBase CloneDocumentViewer(Uri uri)
        {
            if (uri.IsAbsoluteUri == false)
            {
                // Only when navigating via hyperlink and with the "pack://application" protocol
                uri = new Uri("pack://application:,,,/" + uri.OriginalString, UriKind.RelativeOrAbsolute);
            }
            
            WebRequest request = WebRequest.Create(uri);
            using (WebResponse response = request.GetResponse())
            {
                Stream stream = response.GetResponseStream();
                return (DocumentViewerBase)System.Windows.Markup.XamlReader.Load(stream);
            }
        }

        #endregion Private Methods
        
        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------
        #region Private Fields

        private Stream              currentAnnotationStream = null;  // annotation storage
        private AnnotationService   annotationService = null;        // annotation service
        private SdkPrintDialog      printDialog = null;              // pseudo-dialog UI for printing
        private MenuItem            viewAnnotationMenuItem = null;   // annotations menu

        #endregion Private Fields

    }
}