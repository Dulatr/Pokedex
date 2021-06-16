using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Installed References
using SQLite;

// Local References
using Pokedex.DAO;


namespace Pokedex
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();
                rootFrame.CornerRadius = new CornerRadius(90);

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }        

            if (DB == null)
            {
                DB = new SQLiteConnection(Path.Combine(LocalDataPath,DB_Name));
            }

            // After launching, always check/start the tables in the DB 
            // if they do/don't exist
            Servicer.InitializeTables();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        /// <summary>
        /// Add single instance of the database for application.
        /// </summary>
        private static SQLiteConnection _db;
        private static SQLiteConnection DB
        {
            get
            {
                if (_db == null)
                {
                    _db = new SQLiteConnection(Path.Combine(LocalDataPath, DB_Name));
                }
                return _db;
            }
            set
            {
                _db = value;
            }
        }

        /// <summary>
        /// This acts as the primary means of accessing the database.
        /// It serves any information requested from the UI. A single instance
        /// is accessible here and nowhere else.
        /// </summary>
        private static Services _servicer;
        public static Services Servicer
        {
            get
            {
                if (_servicer == null)
                    _servicer = new Services(DB);
                return _servicer;
            }
            private set
            {
                _servicer = value;
            }
        }

        #region General Application Properties

        /// <summary>
        /// The AppData Local folder path returned as string.
        /// </summary>
        public static readonly string LocalDataPath = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
        public static readonly string DB_Name = "pokedb.db";
        #endregion
    }
}
