//----------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation, 2005.  All rights reserved.
//
// File: Settings.cs
//
// Description: This is the settings store for the application. This object 
//              is persisted to Isolated Storage for this application in the 
//              file named Settings.xml
//
//----------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Reflection;
using System.Security;
using System.Text;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.ObjectModel;


namespace Microsoft.Windows.SdkViewer
{
    public class Settings : INotifyPropertyChanged
    {
        //------------------------------------------------------
        //
        //  Constructors / Destructor
        //
        //------------------------------------------------------
        #region Constructors

        /// <summary>
        /// public constructor required for data binding
        /// </summary>
        public Settings()
        {
        }

        /// <summary>
        /// guarantee settings are saved
        /// </summary>
        ~Settings()
        {
            Save();
        }

        
        #endregion Constructors

        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------
        #region Public Properties

        /// <summary>
        /// singleton accessor for settings
        /// </summary>
        public static Settings Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Load();
                }

                return instance;
            }
        }

        [XmlIgnore]
        public static readonly string SettingsFile = "Settings.xml";

        /// <summary>
        /// Read-only property to create and get the location to local file data cache. This will be 
        /// in stored in "%UserProfile%\Local Settings\Application Data\SdkViewer.v0.0.0.0\"
        /// </summary>
        [XmlIgnore]
        public string LocalFileCacheLocation
        {
            get
            {
                if (localFileCacheLocation == null)
                {
                    string localDataDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    Assembly currAssembly = Assembly.GetExecutingAssembly();
                    AssemblyName an = currAssembly.GetName();

                    localDataDir = System.IO.Path.Combine(localDataDir, an.Name + ".v" + an.Version);
                    if (!Directory.Exists(localDataDir))
                    {
                        Directory.CreateDirectory(localDataDir);
                    }
                    localFileCacheLocation = localDataDir;
                }

                return localFileCacheLocation;
            }
        }

        /// <summary>
        /// This property notifies the application if the user wants to view content which is
        /// deployed with the application or to retrieve a copy from the server if the user has an
        /// internet connection. 
        /// </summary>
        public bool IsUsingOnlineContent
        {
            get
            {
                return isUsingOnlineContent;
            }

            set
            {
                isUsingOnlineContent = value;
                Save();
                RaisePropertyChanged("IsUsingOnlineContent");
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
        //  Interfaces
        //
        //------------------------------------------------------
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        #endregion INotifyPropertyChanged

        //------------------------------------------------------
        //
        //  Private Methods
        //
        //------------------------------------------------------
        #region Private Methods
        private static Settings Load()
        {
            IsolatedStorageFile file;
            IsolatedStorageFileStream stream;
            Settings settings = null;

            try
            {
                // Use Domain storage.  We have a strong name for the application so our evidence is (strong name, version, assembly name).
                file = IsolatedStorageFile.GetUserStoreForDomain();
                stream = new IsolatedStorageFileStream(SettingsFile, FileMode.Open, FileAccess.Read, FileShare.Read);

                XmlTextReader reader = new XmlTextReader(stream);
                reader.WhitespaceHandling = WhitespaceHandling.Significant;

                reader.Read(); // advance to <xml> tag
                reader.Read(); // advance to <Settings> tag

                settings = (new XmlSerializer(typeof(Settings))).Deserialize(reader) as Settings;

                stream.Close();
                file.Close();
            }
            catch
            {
                // Ignore all load errors
            }


            if (settings == null)
            {
                return new Settings();
            }
            else
            {
                return settings;
            }
        }

        /// <summary>
        /// Persist the current settings out to the file.
        /// </summary>
        private void Save()
        {
            try
            {
                IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForDomain();
                IsolatedStorageFileStream stream = new IsolatedStorageFileStream(SettingsFile, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);

                XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8);
                writer.Formatting = Formatting.Indented;
                writer.WriteStartDocument();

                XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                serializer.Serialize(writer, this);

                writer.Flush();
                stream.Close();
                file.Close();
            }
            catch (IOException)
            {
                // Couldn't save the settings file.
            }
        }

        #endregion Private Methods

        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------
        #region Private Fields

        private string localFileCacheLocation = null;       // location of local cache for online content
        private bool isUsingOnlineContent = true;           // where are we getting content from?
        private static Settings instance;                   // singleton instance

        #endregion Private Fields

    }
}
