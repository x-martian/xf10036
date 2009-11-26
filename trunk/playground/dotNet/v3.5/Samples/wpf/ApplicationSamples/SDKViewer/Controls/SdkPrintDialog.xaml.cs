//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation, 2005.  All rights reserved.
//
// File: SdkPrintDialog.xaml.cs
//
// Description: define the object for our UI to bind to and interact with for printing
//              NOT a true dialog, since we're going to make this per-instance of the
//              SdkSinglePageViewer.
//
//---------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Printing;
using System.Windows.Xps;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Annotations;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Documents.Serialization;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Input;
using System.Windows.Threading;


namespace Microsoft.Windows.SdkViewer.Controls
{

    public partial class SdkPrintDialog : Control, INotifyPropertyChanged
    {

        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------
        #region Constructors

        public SdkPrintDialog() : base()
        {
            InitializeComponent();

            // make sure that we wire up the references to cached templated elements before we use them
            Loaded += EnsureElementReferences;

            localPrintServer = new LocalPrintServer();
            documentsToPrint = new List<DocumentPaginator>();
            writers = new List<XpsDocumentWriter>();
            currentlyPrintingPages = new ObservableCollection<TileBrush>();
        }

        #endregion Constructors


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------
        #region Public Methods

        /// <summary>
        /// display the printing "dialog"
        /// </summary>
        public void ShowSdkDialog()
        {
            RefreshPrinterUI();
            this.Visibility = Visibility.Visible;
            OnShowDialog();
            isOpen = true;
        }

        /// <summary>
        /// hide the printing "dialog"
        /// </summary>
        public void HideSdkDialog()
        {
            OnHideDialog();
            isOpen = false;
        }

        /// <summary>
        /// cancel the print job
        /// </summary>
        public void CancelPrintJob(object sender, EventArgs args)
        {
            foreach (XpsDocumentWriter docWriter in writers)
            {
                docWriter.CancelAsync();
            }
            EndOrCancelPrintJob(true  /*close dialog*/);
        }

        /// <summary>
        /// routine to call when the dialog is no longer needed
        /// </summary>        
        public void EndOrCancelPrintJob(bool closeDialog)
        {
            // clean up, save any relevant settings, and hide the dialog
            currentQueue = null;
            printProgressOverlay.Visibility = Visibility.Hidden;

            writers.Clear();
            documentsToPrint.Clear();
            
            UpdatePages();

            if (closeDialog)
            {
                HideSdkDialog();
            }
        }


        /// <summary>
        /// print the documents previously passed in
        /// </summary>
        public void StartPrintJob(object sender, EventArgs args)
        {
            printProgressOverlay.Visibility = Visibility.Visible;
            printingDocumentCount = documentsToPrint.Count;

            // verify that at least one document has been passed in at some point for us to print
            if (printingDocumentCount < 1)
            {
                throw new InvalidOperationException("StartPrintJob was called with no available document to print!");
            }

            // verify that at we have a printer queue available to us
            if (currentQueue == null)
            {
                throw new InvalidOperationException("The current print queue is null. Unable to print.");
            }

            foreach (DocumentPaginator currentDocument in documentsToPrint)
            {
                PrintTicket ticket = GetPrintTicket(currentQueue);
                XpsDocumentWriter docWriter = PrintQueue.CreateXpsDocumentWriter(currentQueue);

                try
                {
                    writers.Add(docWriter);
                    docWriter.WritingCompleted += OnPrintingDocumentCompleted;
                    docWriter.WriteAsync(currentDocument, ticket);
                }
                catch (Exception e)
                {
                    // we couldn't use the current queue to write to. Fail gracefully
                    // but don't close the dialog so the user knows what happened. 
                    EndOrCancelPrintJob(false /*close dialog*/);

                    printerDescription = "! a printing error has occurred.";
                    descriptionLine1 = "unable to print to the selected queue";
                    descriptionLine2 = String.Empty;
                    descriptionLine3 = String.Empty;
                    NotifyPropertyChanged("PrinterDescription");
                    NotifyPropertyChanged("DescriptionLine1");
                    NotifyPropertyChanged("DescriptionLine2");
                    NotifyPropertyChanged("DescriptionLine3");

                    if (printButton != null)
                    {
                        printButton.IsEnabled = false;
                    }

                    throw e;
                    //break;
                }
            }
        }


        /// <summary>
        /// notification for a print job status change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void OnPrintingDocumentCompleted(object sender, WritingCompletedEventArgs args)
        {
            // check to see if we're printed the last document
            printingDocumentCount--;

            if (printingDocumentCount <= 0)
            {
                printProgressBar.Value = printProgressBar.Maximum;
                EndOrCancelPrintJob(true  /*close dialog*/);
            }
            else
            {
                // set progress bar to the fraction of documents that have printed
                printProgressBar.Value = ((documentsToPrint.Count - printingDocumentCount) / documentsToPrint.Count) * printProgressBar.Maximum;
            }
        }


        /// <summary>
        /// opens a shell window for configuring printers
        /// </summary>
        public void ConfigurePrinters(object sender, RoutedEventArgs args)
        {
            // the below doesn't actually create a new process - it's part of the
            // shell process, so we can't listen to Process.Exited as an indicator
            // that the printer setup has changed somehow. Instead, we close the
            // dialog and set the flag so that we update printer settings when the
            // dialog is re-opened.
            isPrintingInitialized = false;
            EndOrCancelPrintJob(true  /*close dialog*/);
            
            // launch the Printers & Faxes folder
            Process shellPrintWindow = new System.Diagnostics.Process();
            shellPrintWindow.StartInfo.FileName = @"::{2227A280-3AEA-1069-A2DE-08002B30309D}"; /* Printers and Faxes */
            shellPrintWindow.StartInfo.UseShellExecute = true;
            shellPrintWindow.StartInfo.Verb = "Open";
            shellPrintWindow.Start();
        }



        /// <summary>
        /// add a new document to the batch to print
        /// </summary>
        public void AddDocumentToPrintJob(DocumentViewerBase documentViewer, System.IO.Stream annotationStoreStream) //AnnotationService annotationService)
        {
            // make sure that the output paths are ready to go
            InitializePrinting();

            annotationStoreStream.Flush();
            annotationStoreStream.Position = 0;
            DocumentPaginator documentPaginator = new AnnotationDocumentPaginator(documentViewer.Document.DocumentPaginator, annotationStoreStream);

            if (documentPaginator == null)
            {
                throw new InvalidOperationException("unable to create a copy of the document viewer");
            }

            // force the new copy to do pagination, which will also generate our page visuals
            int pageIndex = 0;
            while (!documentPaginator.IsPageCountValid)
            {
                documentPaginator.GetPage(pageIndex++);
            }

            documentsToPrint.Add(documentPaginator);

            UpdatePages();

            documentPaginator.PagesChanged += OnDocumentPagesChanged;
        }
        #endregion Public Methods

        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------
        #region Public Properties

        /// <summary>
        /// is the print dialog open?
        /// </summary>
        public bool IsOpen
        {
            get { return isOpen; }
        }


        /// <summary>
        /// print queue description text for dialog UI to bind to
        /// </summary>
        public String PrinterDescription
        {
            get { return printerDescription; }
        }

        /// <summary>
        /// general information text for dialog UI to bind to
        /// </summary>
        public String DescriptionLine1
        {
            get { return descriptionLine1; }
        }


        /// <summary>
        /// general information text for dialog UI to bind to
        /// </summary>
        public String DescriptionLine2
        {
            get { return descriptionLine2; }
        }

        /// <summary>
        /// general information text for dialog UI to bind to
        /// </summary>
        public String DescriptionLine3
        {
            get { return descriptionLine3; }
        }


        /// <summary>
        /// print preview of our pages.
        /// </summary>
        public ObservableCollection<TileBrush> CurrentlyPrintingPages
        {
            get { return currentlyPrintingPages;}
        }


        /// <summary>
        /// collection of local and networked PrintQueues
        /// </summary>
        public ObservableCollection<PrintQueue> PrintQueues
        {
            get
            {
                if (outputQueues == null)
                {
                    outputQueues = new ObservableCollection<PrintQueue>();
                }
                return outputQueues;
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
        /// show dialog
        /// </summary>
        public static readonly RoutedEvent ShowDialogEvent = EventManager.RegisterRoutedEvent(
                                                                 "ShowDialog",
                                                                 RoutingStrategy.Bubble,
                                                                 typeof(RoutedEventHandler),
                                                                 typeof(SdkPrintDialog));

        public event RoutedEventHandler ShowDialog
        {
            add { AddHandler(ShowDialogEvent, value); }
            remove { RemoveHandler(ShowDialogEvent, value); }
        }

        public static readonly RoutedEvent HideDialogEvent = EventManager.RegisterRoutedEvent(
                                                                 "HideDialog",
                                                                 RoutingStrategy.Bubble,
                                                                 typeof(RoutedEventHandler),
                                                                 typeof(SdkPrintDialog));
        /// <summary>
        /// hide dialog
        /// </summary>
        public event RoutedEventHandler HideDialog
        {
            add { AddHandler(HideDialogEvent, value); }
            remove { RemoveHandler(HideDialogEvent, value); }
        }

        #endregion Public Events

        //------------------------------------------------------
        //
        //  Interfaces
        //
        //------------------------------------------------------
        #region INotifyPropertyChanged

        /// <summary>
        /// notification setup for the properties exposed on the class that might be databound
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;


        /// <summary>
        /// method for raising the PropertyChanged event
        /// </summary>
        /// <param name="propertyName"></param>
        void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion INotifyPropertyChanged

        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------
        #region Protected Methods

        protected virtual void OnShowDialog()
        {
            RoutedEventArgs newEvent = new RoutedEventArgs();
            newEvent.RoutedEvent = SdkPrintDialog.ShowDialogEvent;
            RaiseEvent(newEvent);
        }


        protected virtual void OnHideDialog()
        {
            RoutedEventArgs newEvent = new RoutedEventArgs();
            newEvent.RoutedEvent = SdkPrintDialog.HideDialogEvent;
            RaiseEvent(newEvent);
        }

        #endregion Protected Methods

        //------------------------------------------------------
        //
        //  Private Methods
        //
        //------------------------------------------------------
        #region Private Methods


        /// <summary>
        /// make sure that any members generated by the template have a local reference,
        /// so that we don't have to call Template.FindName over and over again.
        /// </summary>
        private void EnsureElementReferences(object sender, EventArgs args)
        {
            if (printProgressBar == null)
            {
                printProgressBar = (ProgressBar)Template.FindName("PrintProgressBar", this);
            }
            if (printProgressOverlay == null)
            {
                printProgressOverlay = (Border)Template.FindName("PrintProgressOverlay", this);
            }
            if (printButton == null)
            {
                printButton = (Button)Template.FindName("PrintButton", this);
            }

            if (printProgressBar == null || printProgressOverlay == null || printButton == null)
            {
                throw new ApplicationException("unable to find template UI components!");
            }
        }


        /// <summary>
        /// handler for after the hide dialog animation has finished
        /// </summary>
        private void HideDialogCurrentStateInvalidated(object sender, EventArgs args)
        {
            Clock clock = (Clock)sender;
            if (clock.CurrentState != ClockState.Active)
            {
                Visibility = Visibility.Hidden;
            }
        }



        /// <summary>
        /// notification from the UI that the user has selected a different printer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void PrinterSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            ComboBox printerList = (ComboBox)sender;

            if (printerList.SelectedIndex >= 0)
            {
                currentQueue = (PrintQueue)printerList.Items[printerList.SelectedIndex];
            }
            else
            {
                currentQueue = null;
            }

            RefreshPrinterUI();
        }

        /// <summary>
        /// update the printer properties that the UI is bound to
        /// </summary>
        private void RefreshPrinterUI()
        {
            if (currentQueue != null)
            {
                printerDescription = currentQueue.Name;
                descriptionLine1 = currentQueue.Location;
                descriptionLine3 = currentQueue.Comment;

                if (printButton != null)
                {
                    printButton.IsEnabled = true;
                }
            }
            else
            {
                printerDescription = "Unable to connect to a printer";
                descriptionLine1 = descriptionLine2 = descriptionLine3 = String.Empty;

                if (printButton != null)
                {
                    printButton.IsEnabled = false;
                }
            }

            NotifyPropertyChanged("PrinterDescription");
            NotifyPropertyChanged("DescriptionLine1");
            NotifyPropertyChanged("DescriptionLine2");
            NotifyPropertyChanged("DescriptionLine3");
        }

        /// <summary>
        /// get the system ready for printing...
        /// </summary>
        private void InitializePrinting()
        {
            if (!isPrintingInitialized)
            {
                RetrieveAvailableQueues();
                isPrintingInitialized = true;
            }
        }


        /// <summary>
        /// enumerate the local and networked queues / printers
        /// </summary>
        private void RetrieveAvailableQueues()
        {
            PrintQueues.Clear();

            try
            {
                LocalPrintServer printServer = new LocalPrintServer();

                EnumeratedPrintQueueTypes[] queueTypes = new EnumeratedPrintQueueTypes[]
                {
                    EnumeratedPrintQueueTypes.Local,
                    EnumeratedPrintQueueTypes.Connections
                };

                PrintQueueIndexedProperty[] props = new PrintQueueIndexedProperty[] {
                        PrintQueueIndexedProperty.Name,
                        PrintQueueIndexedProperty.Description,
                        PrintQueueIndexedProperty.QueueAttributes,
                        PrintQueueIndexedProperty.QueueStatus,
                        PrintQueueIndexedProperty.Comment,
                        PrintQueueIndexedProperty.Location,
                        PrintQueueIndexedProperty.NumberOfJobs,
                        PrintQueueIndexedProperty.DefaultPrintTicket,
                        PrintQueueIndexedProperty.UserPrintTicket
                    };

                PrintQueueCollection queueCollection = printServer.GetPrintQueues(props, queueTypes);

                // PrintQueue doesn't know if it's the default or not, so we get the default and compare via Name
                string defaultQueueName = SdkPrintDialog.localPrintServer.DefaultPrintQueue.FullName;

                foreach (PrintQueue queue in queueCollection)
                {
                    PrintQueues.Add(queue);
                    if (defaultQueueName == queue.FullName)
                    {
                        currentQueue = queue;
                    }
                }
            }
            catch (PrintQueueException pqe)
            {
                // catch print queue problems
                Debug.WriteLine("Print Queue Exception:" + pqe.Message);
            }
            catch (PrintServerException pse)
            {
                // catch print server problems
                Debug.WriteLine("Print Server Exception:" + pse.Message);
            }

            NotifyPropertyChanged("PrintQueues");
        }


        /// <summary>
        /// notification whenever pagination changes on the document to print
        /// </summary>
        private void OnDocumentPagesChanged(object sender, EventArgs args)
        {
            UpdatePages();
        }



        /// <summary>
        /// obtain a PrintTicket from a PrintQueue
        /// </summary>
        private PrintTicket GetPrintTicket(PrintQueue printQueue)
        {
            PrintTicket ticket = null;

            // try to get a user ticket
            ticket = printQueue.UserPrintTicket;

            // if failed, try to get a default ticket
            if (ticket == null)
            {
                ticket = printQueue.DefaultPrintTicket;
            }
            // otherwise get a regular ticket
            if (ticket == null)
            {
                ticket = new PrintTicket();
            }

            return ticket;
        }


        /// <summary>
        /// refresh our collection of Visuals representing the pages of the document
        /// </summary>
        private void UpdatePages()
        {
            if (documentsToPrint.Count > 0)
            {
                for (int i = 0; i < documentsToPrint.Count; i++)
                {
                    for (int j = 0; j < documentsToPrint[i].PageCount; j++)
                    {
                        DocumentPage page = documentsToPrint[i].GetPage(j);

                        VisualBrush previewBrush = new VisualBrush(page.Visual);
                        previewBrush.SetValue(TileBrush.StretchProperty, Stretch.Uniform);
                        previewBrush.SetValue(TileBrush.AlignmentXProperty, AlignmentX.Left);
                        previewBrush.SetValue(TileBrush.AlignmentYProperty, AlignmentY.Top);
                        currentlyPrintingPages.Add(previewBrush);
                    }
                }
            }
            else
            {
                currentlyPrintingPages.Clear();
            }

            NotifyPropertyChanged("CurrentlyPrintingPages");
        }


        #endregion Private Methods

        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------
        #region Private Fields

        internal static LocalPrintServer localPrintServer = null;  // print server for local machine
        private PrintQueue currentQueue = null;                    // any kind of output queue, e.g. printer
        private List<DocumentPaginator> documentsToPrint = null;   // one or more documents to print as part of the job
        private List<XpsDocumentWriter> writers = null;            // each document has its own writer
        private int printingDocumentCount = 0;                     // # of documents currently being printed
        private bool isOpen = false;                               // is the print dialog open?
        private Border printProgressOverlay = null;                // UI for printing progress bar container
        private ProgressBar printProgressBar = null;               // UI for printing progress bar
        private Button printButton = null;                         // UI for print button

        private string printerDescription;                         // description of printer for UI
        private string descriptionLine1;                           // description of printer for UI
        private string descriptionLine2;                           // description of printer for UI
        private string descriptionLine3;                           // description of printer for UI

        private static ObservableCollection<PrintQueue> outputQueues = null;       // all available queues
        private ObservableCollection<TileBrush> currentlyPrintingPages = null;     // collection of visual elements for page thumbnails
        private static bool isPrintingInitialized = false;

        #endregion Private Fields

    }

    #region Converter Classes
    /// <summary>
    /// given a list of printers, returns the index of the default one.
    /// </summary>
    public class SelectedPrinterConverter : IValueConverter
    {
        public object Convert(Object obj, Type type, object parameter, System.Globalization.CultureInfo culture)
        {
            object returnValue = Binding.DoNothing;
            ObservableCollection<PrintQueue> queues = (ObservableCollection<PrintQueue>)obj;
            
            // PrintQueue doesn't know if it's the default or not, so we get the default and compare via Name
            try
            {
                string defaultQueueName = SdkPrintDialog.localPrintServer.DefaultPrintQueue.FullName;

                Int32 selectedIndex = 0;
                for (int i = 0; i < queues.Count; i++)
                {
                    if (defaultQueueName == queues[i].FullName)
                    {
                        selectedIndex = i;
                        break;
                    }
                }
                returnValue = selectedIndex;
            }
            catch (PrintServerException pse)
            {
                Debug.WriteLine("unable to access local print server");
                Debug.WriteLine(pse.Message);
            }
            catch (PrintQueueException pqe)
            {
                Debug.WriteLine("unable to access default queue");
                Debug.WriteLine(pqe.Message);
            }

            return returnValue;
        }

        public object ConvertBack(Object obj, Type type, object parameter, System.Globalization.CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    /// <summary>
    /// given a string, returns an appropriate Visibility
    /// </summary>
    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(Object obj, Type type, object parameter, System.Globalization.CultureInfo culture)
        {
            string text = (string)obj;
            if (text != null)
            {
                if (text == String.Empty)
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(Object obj, Type type, object parameter, System.Globalization.CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
    #endregion Converter Classes

}