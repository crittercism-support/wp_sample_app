﻿namespace CrittercismSDK
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using CrittercismSDK.DataContracts;
    using System.IO.IsolatedStorage;
#if WINDOWS_PHONE
    using System.Windows;
#endif

    public class Crittercism
    {
        /// <summary>
        /// Gets or sets a queue of messages.
        /// </summary>
        /// <value> A Queue of messages. </value>
        internal static Queue<MessageReport> MessageQueue { get; set; }

        /// <summary>
        /// Gets or sets the current breadcrumbs.
        /// </summary>
        /// <value> The breadcrumbs. </value>
        internal static Breadcrumbs CurrentBreadcrumbs { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the application.
        /// </summary>
        /// <value> The identifier of the application. </value>
        internal static string AppID { get; set; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value> The key. </value>
        internal static string Key { get; set; }

        /// <summary>
        /// Gets or sets the secret.
        /// </summary>
        /// <value> The secret. </value>
        internal static string Secret { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value> The username. </value>
        internal static string Username { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value> The email. </value>
        internal static string Email { get; set; }

        /// <summary>
        /// Gets or sets the gender.
        /// </summary>
        /// <value> The gender. </value>
        internal static string Gender { get; set; }

        /// <summary>
        /// Gets or sets the age.
        /// </summary>
        /// <value> The age. </value>
        internal static int Age { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the device.
        /// </summary>
        /// <value> The identifier of the device. </value>
        internal static string DeviceId { get; set; }

        /// <summary>
        /// Gets or sets the operating system platform.
        /// </summary>
        /// <value> The operating system platform. </value>
        internal static string OSPlatform { get; set; }

        /// <summary>
        /// Gets or sets the arbitrary user metadata.
        /// </summary>
        /// <value> The user metadata. </value>
        internal static Dictionary<string, string> ArbitraryUserMetadata { get; set; }

        /// <summary>
        /// Folder name for the messages files
        /// </summary>
        internal static string FolderName = "CrittercismMessages";

        /// <summary>
        /// Initialises this object.
        /// </summary>
        /// <param name="appID">  Identifier for the application. </param>
        /// <param name="key">    The key. </param>
        /// <param name="secret"> The secret. </param>
        public static void Init(string appID, string key, string secret)
        {
            AppID = appID;
            Key = key;
            Secret = secret;
            CurrentBreadcrumbs = new Breadcrumbs();
            DeviceId = string.Empty;
            OSPlatform = Environment.OSVersion.Platform.ToString();
            MessageQueue = new Queue<MessageReport>();
            LoadQueueFromDisk();

            QueueReader queueReader = new QueueReader();
            ThreadStart threadStart = new ThreadStart(queueReader.ReadQueue);
            Thread readerThread = new Thread(threadStart);
            readerThread.Name = "Crittercism Sender";
            readerThread.Start();

            CreateAppLoadReport();
#if WINDOWS_PHONE
            Application.Current.UnhandledException += new EventHandler<ApplicationUnhandledExceptionEventArgs>(Current_UnhandledException);
#else
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
#endif
        }

        /// <summary>
        /// Sets a username.
        /// </summary>
        /// <param name="username"> The username. </param>
        public static void SetUsername(string username)
        {
            Username = username;
        }

        /// <summary>
        /// Sets an email.
        /// </summary>
        /// <param name="email">    The email. </param>
        public static void SetEmail(string email)
        {
            Email = email;
        }

        /// <summary>
        /// Sets a gender.
        /// </summary>
        /// <param name="gender">   The gender. </param>
        public static void SetGender(string gender)
        {
            Gender = gender;
        }

        /// <summary>
        /// Sets an age.
        /// </summary>
        /// <param name="age">  The age. </param>
        public static void SetAge(int age)
        {
            Age = age;
        }

        /// <summary>
        /// Sets an arbitrary user metadata value.
        /// </summary>
        /// <param name="value">    The value. </param>
        /// <param name="key">      The key. </param>
        public static void SetValue(string value, string key)
        {
            lock (ArbitraryUserMetadata)
            {
                ArbitraryUserMetadata.Add(key, value);
            }
        }

        /// <summary>
        /// Leave breadcrum.
        /// </summary>
        /// <param name="breadcrumb">   The breadcrumb. </param>
        public static void LeaveBreadcrum(string breadcrumb)
        {
            lock (CurrentBreadcrumbs)
            {
                CurrentBreadcrumbs.current_session.Add(new string[] { breadcrumb, DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ssK", System.Globalization.CultureInfo.InvariantCulture) });
            }
        }

        public static void addVote()
        {
        }

        public static void addVote(string eventName)
        {
        }

        public static void SetDisplayer(object displayer)
        {
        }

        public static int getVotes()
        {
            return 0;
        }

        public static void setVotes(int numberOfVotes)
        {
        }

        public static void ConfigurePushNotification()
        {
        }

        public static void ShowCrittercism()
        {
        }

        /// <summary>
        /// Creates a crash report.
        /// </summary>
        /// <param name="currentException"> The current exception. </param>
        private static void CreateCrashReport(Exception currentException)
        {
            Breadcrumbs breadcrumbs = new Breadcrumbs();
            breadcrumbs.current_session = new List<string[]>(CurrentBreadcrumbs.current_session);
            breadcrumbs.previous_session = new List<string[]>(CurrentBreadcrumbs.previous_session);
            Crash crash = new Crash(AppID, OSPlatform, breadcrumbs, DeviceId, currentException.GetType().Name, currentException.Message, "1.0", currentException.StackTrace);
            crash.SaveToDisk();
            MessageQueue.Enqueue(crash);
            CurrentBreadcrumbs.previous_session = new List<string[]>(CurrentBreadcrumbs.current_session);
            CurrentBreadcrumbs.current_session.Clear();
        }

        /// <summary>
        /// Creates the application load report.
        /// </summary>
        private static void CreateAppLoadReport()
        {
            AppLoad appLoad = new AppLoad(AppID, DeviceId, "1.0", OSPlatform);
            appLoad.SaveToDisk();
            MessageQueue.Enqueue(appLoad);
        }

        /// <summary>
        /// Creates the error report.
        /// </summary>
        public static void CreateErrorReport(Exception e)
        {
            AppState appState = new AppState();
            List<ExceptionObject> exceptions = new List<ExceptionObject>() { new ExceptionObject("1.0", e.GetType().Name, e.Message, appState, e.StackTrace) };
            Error error = new Error(AppID, OSPlatform, DeviceId, "1.0", exceptions);
            error.SaveToDisk();
            MessageQueue.Enqueue(error);
        }

        /// <summary>
        /// Loads the messages from disk into the queue.
        /// </summary>
        private static void LoadQueueFromDisk()
        {
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
            if (storage.DirectoryExists(FolderName))
            {
                string[] fileNames = storage.GetFileNames(FolderName + "\\*");
                List<MessageReport> messages = new List<MessageReport>();
                foreach (string file in fileNames)
                {
                    string[] fileSplit = file.Split('_');
                    MessageReport message = null;
                    switch (fileSplit[0])
                    {
                        case "AppLoad":
                            message = new AppLoad();
                            break;

                        case "Error":
                            message = new Error();
                            break;

                        default:
                            message = new Crash();
                            break;
                    }

                    message.Name = file;
                    message.CreationDate = storage.GetCreationTime(FolderName + "\\" + file);
                    message.IsLoaded = false;
                }

                messages.Sort((m1, m2) => m1.CreationDate.CompareTo(m2.CreationDate));
                foreach(MessageReport message in messages)
                {
                    MessageQueue.Enqueue(message);
                }
            }
        }

        /// <summary>
        /// Event handler. Called by CurrentDomain for unhandled exception events.
        /// </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Unhandled exception event information. </param>
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            CreateCrashReport((Exception)e.ExceptionObject);
        }

#if WINDOWS_PHONE
        static void Current_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            CreateCrashReport((Exception)e.ExceptionObject);
            e.Handled = true;
        }
#endif
    }
}