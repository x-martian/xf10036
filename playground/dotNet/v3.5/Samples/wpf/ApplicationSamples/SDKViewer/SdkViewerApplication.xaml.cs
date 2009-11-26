//----------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation, 2005.  All rights reserved.
//
// File: SdkViewerApplication.xaml.cs
//
// Description: This is the code-behind file for SdkViewerApplication.xaml which
//              defines the application object.
//
//----------------------------------------------------------------------------


using System;
using System.IO;
using System.Xml;
using System.Data;
using System.Net;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Reflection;
using System.Configuration;
using System.Windows.Threading;
using System.Windows.Navigation;
using System.Text.RegularExpressions;
using Microsoft.Windows.SdkViewer.Controls;

namespace Microsoft.Windows.SdkViewer
{
    public partial class SdkViewerApplication : Application
    {
        private IntPtr      MainWindowHwnd;
        private HwndSource  MainWindowHwndSource;

        /// <summary>
        /// Application startp: listen for all unhandled exceptions on this thread's dispatcher
        /// </summary>
        private void OnStartingUp(object sender, StartupEventArgs args)
        {
            DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(DispatcherExceptionHandler);
        }

        /// <summary>
        /// Set DWM when MainWindow and page has been loaded.
        /// </summary>
        private void OnLoaded(object sender, NavigationEventArgs args)
        {
            // Get the HWND of the main application window
            WindowInteropHelper wih = new WindowInteropHelper(Application.Current.MainWindow);
            MainWindowHwnd = wih.Handle;

            ApplyGlass(MainWindowHwnd);

            // Add an event handler to receive window messages
            MainWindowHwndSource = PresentationSource.FromVisual(Application.Current.MainWindow) as HwndSource;
            MainWindowHwndSource.AddHook(new HwndSourceHook(MainWindowHwndMessageFilter));
        }


        /// <summary>
        /// Window message handler for the application main window. The application needs to handle DWM messages to 
        /// set re-apply window blur and effects if the DMW is disabled and re-enabled.
        /// </summary>
        IntPtr MainWindowHwndMessageFilter(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case DwmApi.WM_DWMCOMPOSITIONCHANGED: // clients must manually re-apply on display changes
                    ApplyGlass(MainWindowHwnd);
                    handled = true;
                    break;
                default:
                    handled = false;
                    break;
            }

            return IntPtr.Zero;
        }        


        /// <summary>
        /// Enable glass and blur using the Desktop Window Manager APIs which is available on Vista.
        /// </summary>
        /// <param name="hwnd">target window to apply effects on</param>
        private void ApplyGlass(IntPtr hwnd)
        {
            try
            {
                bool IsCompositionEnabled = false;

                DwmApi.DwmIsCompositionEnabled(ref IsCompositionEnabled);
                if (IsCompositionEnabled)
                {
                    Application.Current.MainWindow.Background = Brushes.Transparent;

                    // Enabled window in the transparent areas of the Window
                    Win32.RECT r = new Win32.RECT();
                    Win32.GetClientRect(hwnd, ref r);

                    DwmApi.DWM_BLURBEHIND bb = new DwmApi.DWM_BLURBEHIND();
                    bb.dwFlags = DwmApi.DWM_BB_ENABLE | DwmApi.DWM_BB_BLURREGION;
                    bb.fEnable = true;
                    bb.hRgnBlur = IntPtr.Zero; // blur the entire client area of the window
                    DwmApi.DwmEnableBlurBehindWindow(hwnd, ref bb);

                    // Extend glass frame into client area
                    // Note that the defauly desktop DPI is 96dpi. The user should always convert pixels 
                    // to the current system DPI like below
                    DwmApi.MARGINS margins = new DwmApi.MARGINS();
                    margins.cxLeftWidth     = Convert.ToInt32(5 * (DesktopDpiX/96));
                    margins.cxRightWidth    = Convert.ToInt32(5 * (DesktopDpiX/96));
                    margins.cyTopHeight     = Convert.ToInt32(55 * (DesktopDpiY/96)); // this is the size of the MainPage.TopToolBar 
                    margins.cyBottomHeight  = Convert.ToInt32(5 * (DesktopDpiY/96));

                    HwndSource source = HwndSource.FromHwnd(hwnd);
                    source.CompositionTarget.BackgroundColor = Color.FromArgb(0, 0, 0, 0);

                    int hr = DwmApi.DwmExtendFrameIntoClientArea(source.Handle, ref margins);
                    if (hr < 0)
                    {
                        throw new ApplicationException("DwmExtendFrameIntoClientArea: Failed with " + hr);
                    }
                }
                else
                {
                    // the original Window background color is white
                    Application.Current.MainWindow.Background = Brushes.White;
                }
            }
            catch (ApplicationException)
            {
                Application.Current.MainWindow.Background = Brushes.White;
                throw;
            }
            catch (DllNotFoundException)
            {
                Application.Current.MainWindow.Background = Brushes.White;
            }
        }

        /// <summary>
        /// This handles all uncaught exceptions for the application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void DispatcherExceptionHandler(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            System.Diagnostics.Trace.WriteLine(args.Exception.Message);
            System.Diagnostics.Trace.WriteLine(args.Exception.StackTrace);

                // set the application to use Local content if the user has connectivity problems
                if (args.Exception is WebException)
                {
                    MessageBox.Show(
                        args.Exception.Message,
                        "Page not found",
                        MessageBoxButton.OK,
                        MessageBoxImage.Exclamation);

                    // This property is data-bound to "IsUsingOnlineContentButton" on MainPage.xaml
                    Settings.Instance.IsUsingOnlineContent = false;
                    args.Handled = true;
                }
        }

        /// <summary>
        /// Gets the DPI for the height of the desktop
        /// </summary>
        private float _DesktopDpiY = 0.0f;
        private float DesktopDpiY
        {
            get
            {
                if (MainWindowHwnd != null && _DesktopDpiY <= 0.0f)
                {
                    System.Drawing.Graphics desktop = System.Drawing.Graphics.FromHwnd(MainWindowHwnd);
                    _DesktopDpiY = desktop.DpiY;
                }

                return _DesktopDpiY;
            }
        }

        /// <summary>
        /// Gets the DPI for the width of the desktop
        /// </summary>
        private float _DesktopDpiX = 0.0f;
        private float DesktopDpiX
        {
            get
            {
                if (MainWindowHwnd != null && _DesktopDpiX <= 0.0f)
                {
                    System.Drawing.Graphics desktop = System.Drawing.Graphics.FromHwnd(MainWindowHwnd);
                    _DesktopDpiX = desktop.DpiX;
                }
                
                return _DesktopDpiX;
            }
        }
    }
}
