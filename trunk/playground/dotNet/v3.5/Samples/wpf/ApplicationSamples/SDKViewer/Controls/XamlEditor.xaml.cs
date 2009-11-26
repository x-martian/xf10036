//----------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation, 2005.  All rights reserved.
//
// File: XamlEditor.xaml.cs
//
// Description: XamlEditor, a control to display xaml examples. This is 
//              the control that opens in a new tab, and the xaml can 
//              be edited and refreshed for immediate rendering
//
//----------------------------------------------------------------------------

namespace Microsoft.Windows.SdkViewer.Controls
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Windows;
    using System.Diagnostics;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using System.Windows.Controls;
    using System.Windows.Threading;
    using System.Windows.Documents;
    using System.Windows.Navigation;
    using System.Windows.Markup;
    using System.Windows.Media.Animation;

    /// <summary>
    /// A control to display XAML code example. It has descrption, code 
    /// editor and a area for runtime displaying. There is also a 
    /// Reset button to reset the code to the original one. When application
    /// is navigating from the current page, the changes made will be 
    /// serialized (saved in local file) so the changes will not be lost.
    /// </summary>
    public partial class XamlEditor : ContentControl, ICloneable
    {

        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------
        #region Constructors    

        /// <summary>
        /// 
        /// </summary>
        public XamlEditor() : base()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Add eventhandlers to save code changes when the page is loaded/unloaded 
        /// </summary>
        public XamlEditor(string originalCodeSnippet, string saveFileName) : this()
        {
            if (originalCodeSnippet == null) 
            { 
                originalCodeSnippet = string.Empty; 
            }
            
            originalText = originalCodeSnippet;
            CodeSnippet = originalCodeSnippet;
            xamlTextBox.Text = originalCodeSnippet;
            
            if (saveFileName == null) 
            { 
                saveFileName = string.Empty; 
            }
            
            fileName = saveFileName;

            // Add EventHandler to Loaded event to initialize.
            this.Loaded += new RoutedEventHandler(OnLoaded);
            
            // Add EventHandler to save changes.
            this.Unloaded += new RoutedEventHandler(OnUnloaded);
        }
    
        #endregion Constructors
        
        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------
        #region Public Methods
        
        /// <summary>
        ///
        /// </summary>
        internal static object LoadXaml(string xamlText)
        {
            object root = null;
            byte[] buffer = System.Text.UTF8Encoding.UTF8.GetBytes(xamlText);
            using (MemoryStream memoryStream = new MemoryStream(buffer, 0, buffer.Length, true, true))
            {
                root = System.Windows.Markup.XamlReader.Load(memoryStream);
                FixedDocument fixedDoc = root as FixedDocument;
                if (fixedDoc != null) { root = fixedDoc.Pages[0].GetPageRoot(false); }
            }

            return root;
        }
        
        #endregion Public Methods

        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------
        #region Public Properties

        /// <summary>
        /// Contents of the xamlTextBox.
        /// </summary>
        public string CodeSnippet
        {
            get { return codeSnippet; }
            set { codeSnippet = value; }
        }
        
        #endregion Public Properties
        
        //------------------------------------------------------
        //
        //  Public Events
        //
        //------------------------------------------------------

        //------------------------------------------------------
        //
        //  Interfaces
        //
        //------------------------------------------------------
        #region IClonable

        public object Clone()
        {
            return new XamlEditor(originalText, fileName);
        }

        #endregion IClonable

        //------------------------------------------------------
        //
        //  Private Methods
        //
        //------------------------------------------------------
        #region Private Methods
        
        /// <summary>
        /// Initialize the Snippet.
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Check to see whether there are any files that matchs.
            if (fileName != String.Empty && File.Exists(fileName))
            {
                using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (StreamReader reader = new StreamReader(fileStream, System.Text.UTF8Encoding.UTF8))
                    {
                        CodeSnippet = reader.ReadToEnd();
                        xamlTextBox.Text = CodeSnippet;
                    }
                }
            }
        }
        
        /// <summary>
        /// When customer navigates away from this page, save the snippet.
        /// </summary>
        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            // Save the snippet text to a file.
            if (xamlTextBox.Text.Trim() == string.Empty) 
            { 
                return; 
            }
            
            SaveCode(xamlTextBox.Text);
        }
        
        /// <summary>
        /// Text has been changed by user.
        /// </summary>
        private void HandleTextChanged(object sender, TextChangedEventArgs me)
        {
            PostTextChanged();
        }
        
        /// <summary>
        /// Reset the snippet to the original one.
        /// </summary>
        private void OnReset(object sender, RoutedEventArgs args) 
        {
            xamlTextBox.Text = originalText;
            SaveCode(originalText);  // in case navigation occurs before "TextChange" timer gets called
        }        
        
        /// <summary>
        /// Save the snippet text to a file.
        /// </summary>        
        private void SaveCode(string code)
        { 
            
            if (fileName != String.Empty)
            {
                if (File.Exists(fileName))
                {
                    // If user chaged attribute to be readonly, remove it and overwrite (after all this is proprietary data)
                    FileAttributes fileAttributes = File.GetAttributes(fileName);
                    if ((fileAttributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        fileAttributes ^= FileAttributes.ReadOnly;
                    }
                }

                // Create file
                try
                {
                    using (FileStream fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read))
                    {
                        // Save changes
                        using (StreamWriter writer = new StreamWriter(fileStream, System.Text.UTF8Encoding.UTF8))
                        {
                            writer.Write(code);
                        }
                    }
                }
                catch (IOException)
                { 
                    // Can happen if multiple tab with same control, ignore.
                }
            }

        }

        /// <summary>
        /// When the text changes, an update of the view should be performed.
        /// </summary>
        private void PostTextChanged()
        {
            ClearDispatcherTimer();

            TimeSpan delay = new TimeSpan(0, 0, 1);
            dispatcherTimer = new DispatcherTimer(delay, DispatcherPriority.ApplicationIdle, new EventHandler(AttemptUpdate), Dispatcher.CurrentDispatcher);
        }

        /// <summary>
        /// Stop and clean up DispatcherTimer.
        /// </summary>
        private void ClearDispatcherTimer()
        {
            if (dispatcherTimer != null)
            {
                dispatcherTimer.Stop();
                dispatcherTimer = null;
            }
        }

        /// <summary>
        /// Code changes, thus try to update.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void AttemptUpdate(object sender, EventArgs args)
        {
            ClearDispatcherTimer();


            try
            {
                object root = XamlEditor.LoadXaml(xamlTextBox.Text);
           
                if (root is Window)
                {
                    // Since we cannot open a new window in Partial trust, just give an error message.
                    FramedContent.Content = XamlEditor.LoadXaml(CANNOTDISPLAYWINDOW_MESSAGE);
                }
                else
                {
                    // Show the content in the displaying panel
                    FramedContent.Content = root;
                }

                StatusText.Text = "Done.";
                ((UIElement)GoToErrorHyperlink).Visibility = Visibility.Collapsed;
                
                // Reset text to be black (previous error will set it to red)
                xamlTextBox.Foreground = Brushes.Black;
            }
            catch(XamlParseException parserException)
            {
                // Make red and return
                xamlTextBox.Foreground = Brushes.Red;
                StatusText.Text = parserException.Message;
                int errorLineNumber = parserException.LineNumber;
                int errorPosition = parserException.LinePosition;
                ((UIElement)GoToErrorHyperlink).Visibility = Visibility.Visible;
                GoToErrorHyperlink.Content = "Jump To: line " + errorLineNumber + " col " + errorPosition;
            }
            catch
            {
                ((UIElement)GoToErrorHyperlink).Visibility = Visibility.Collapsed;
                GoToErrorHyperlink.Content = string.Empty;
            }

        }        
        
        #endregion Private Methods

        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------
        #region Private Fields

        private const string CANNOTDISPLAYWINDOW_MESSAGE = "<TextBlock xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" ><Italic>Cannot display a Window. </Italic></TextBlock>";

        private DispatcherTimer dispatcherTimer = null; // periodically update with changes
        private string originalText = string.Empty;     // original code 
        private string codeSnippet = string.Empty;      // edited code
        private string fileName = string.Empty;         // location of saved snippet

        #endregion Private Fields

    }
}