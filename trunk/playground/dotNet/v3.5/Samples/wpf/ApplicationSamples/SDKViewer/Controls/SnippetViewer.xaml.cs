//----------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation, 2005.  All rights reserved.
//
// File: SnippetViewer.xaml.cs
//
// Description: SnippetViewer, a control to display and render xaml examples.
//              This is is the control embedded in the document (TabControl 
//              with xaml / preview tab and edit button)
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

    public delegate void EditClickedEventHandler(object sender, object root);

    /// <summary>
    /// A control to display XAML code example and render it. 
    /// </summary>
    public partial class SnippetViewer : ContentControl
    {
        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------
        #region Constructors    
        
        /// <summary>
        /// Constructor. Add eventhandlers to save code changes when the page is loaded/unloaded 
        /// </summary>
        public SnippetViewer() : base()
        {
            InitializeComponent();
            // Add EventHandler to Loaded event to initialize.
            this.Loaded += new RoutedEventHandler(OnLoaded);
        }
        
        #endregion Constructors
        
        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------
        #region Public Methods

        /// <summary>
        /// Direct the engine to serialize or not the resource dictionary
        /// ( Yes hidding the original method via "new" is ugly but the only workaround we have)
        /// </summary>  
        new public bool ShouldSerializeResources()
        {
            return false;
        }

        /// <summary>
        /// Initialize the Snippet.
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            StaticCodeTextBox.Text = SnippetText;
            StaticRenderFrame.Content = XamlEditor.LoadXaml(StaticCodeTextBox.Text);
        }


        /// <summary>
        /// Occurs when user click the button; shows the control and allow for modif of the xaml
        /// </summary>
        private void OnClick_ModifyXaml(object sender, RoutedEventArgs e)
        {
            if (EditClicked != null) 
            {
                XamlEditor xamlEditor = new XamlEditor(StaticCodeTextBox.Text, FileName);
                EditClicked(this, xamlEditor); 
            }
        }
        
        /// <summary>
        /// return a friendly name
        /// </summary>
        public override string ToString()
        {
            return "Xaml Editor: '" + ControlName + "'";
        }        
        
        #endregion Public Methods

        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------
        #region Public Properties
        
        /// <summary>
        /// The DependencyProperty SnippetText.
        /// Default Value:      string.Empty
        /// </summary>
        public static readonly DependencyProperty SnippetTextProperty = DependencyProperty.Register(
            "SnippetText", typeof(string), typeof(SnippetViewer), new FrameworkPropertyMetadata(string.Empty));
        
        /// <summary>
        /// Contents of the xamlTextBox.
        /// </summary>
        public string SnippetText
        {
            get { return (string)GetValue(SnippetTextProperty); }
            set { SetValue(SnippetTextProperty, value); }
        }
        
        /// <summary>
        /// Get the unique ID associated with this Control
        /// </summary>
        public string ControlName
        {
            get { return Name; }
        }
        
        /// <summary>
        /// Get the name of the document hosting this control
        /// </summary>
        public string DocumentName
        {
            get { return (string)Tag; }
        }
        
        #endregion Public Properties
        
        //------------------------------------------------------
        //
        //  Public Events
        //
        //------------------------------------------------------
        #region Public Events
        
        public static event EditClickedEventHandler EditClicked;
        
        #endregion Public Events

        
        //------------------------------------------------------
        //
        //  Private Properties
        //
        //------------------------------------------------------        
        #region Private Properties
        
        /// <summary>
        /// Get the full name of the file for this control.
        /// </summary>
        private string FileName
        {
            get { return System.IO.Path.Combine(Settings.Instance.LocalFileCacheLocation, "SnippetViewer_" + ControlName + ".txt"); }
        }
        
        #endregion Private Properties

    }
}