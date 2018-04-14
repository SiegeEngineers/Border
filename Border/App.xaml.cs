using Border.Helpers;
using Border.Model;
using Border.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using Border.Properties;
using System.Reflection;
using System.Resources;

namespace Border.View
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public void Initialize(object sender, StartupEventArgs args)
        {
            MainWindow = new MainWindow();
            LoadSettings();
            LoadBOFilesLocal();
            //LoadBOFilesOnline(); // todo verify
            CheckUpdates();
            MainWindow.Show();
            ((MainWindow)MainWindow).SetDefaultBO();

        }

        void LoadSettings()
        {
            if (!File.Exists(FolderNames.SettingsJson))
            {
                Console.WriteLine("No " + FolderNames.SettingsJson + " found. Creating one.");
                Settings = new SettingsData();
                File.WriteAllText(FolderNames.SettingsJson, Settings.ToJson());
            }
            else
            {
                using (StreamReader r = new StreamReader(FolderNames.SettingsJson))
                {
                    string json = r.ReadToEnd();
                    Settings = SettingsData.FromJson(json);
                }
            }
        }
        void LoadBOFilesLocal()
        {
            if (!Directory.Exists(FolderNames.BuildOrderDir))
            {
                return;
            }
            foreach (var file in Directory.GetFiles(FolderNames.BuildOrderDir))
            {
                if (Path.GetExtension(file) == ".json")
                {
                    using (StreamReader r = new StreamReader(file))
                    {
                        string json = r.ReadToEnd();
                        BuildOrderData data = BuildOrderData.FromJson(json);
                        BuildOrderList.AddBuildOrders(data);
                    }
                }
            }
        }

        //void LoadBOFilesOnline()
        //{
        //    foreach (var obo in Settings.OnlineBOs)
        //    {
        //        try
        //        {
        //            WebClient webClient = new WebClient();
        //            webClient.DownloadStringCompleted += delegate (object sender, DownloadStringCompletedEventArgs e)
        //            {
        //                if (e.Cancelled || e.Error != null)
        //                {
        //                    Console.WriteLine(string.Format("Could not download Build Order at {0}.", obo));
        //                    return;
        //                }

        //                BuildOrderData boData = BuildOrderData.FromJson(e.Result);
        //                BuildOrderList.AddBuildOrders(boData);
        //            };
        //            webClient.DownloadStringAsync(new Uri(obo));
        //        }
        //        catch (ArgumentNullException e) { Console.WriteLine(e); }
        //        catch (WebException e) { Console.WriteLine(e); }
        //        catch (NotSupportedException e) { Console.WriteLine(e); }

        //    }
        //}
        public void CheckUpdates(bool notify = false)
        {
            try
            {
                WebClient webClient = new WebClient();
                webClient.DownloadStringCompleted += delegate (object sender, DownloadStringCompletedEventArgs e)
                {
                    if (e.Cancelled || e.Error != null)
                    {
                        if (notify)
                        {
                            ShowUpdate(Strings.Update_ConnectionError, "");
                        }
                        Console.WriteLine("Could not download latest Update Information. Please make sure this link is valid and you are connected to the internet.");
                        return;
                    }
                    UpdateInfo dataOnline = UpdateInfo.FromJson(e.Result);
                    //if (!ResourceExists(FolderNames.UpdateJsonPack))
                    //{
                    //    Console.WriteLine("Your" + FolderNames.UpdateJsonPack + "is missing. Please backup your builds and redownload the app.");
                    //    ShowUpdate(string.Format(Strings.Update_FileError, FolderNames.UpdateJsonPack), dataOnline.Latest.Link);
                    //}

                    using (var r = new StreamReader(GetResourceStream(new Uri(FolderNames.UpdateJsonPack)).Stream))
                    {
                        string jsonLocal = r.ReadToEnd();
                        UpdateInfo dataLocal = UpdateInfo.FromJson(jsonLocal);
                        if (dataLocal.Latest.Id < dataOnline.Latest.Id)
                        {
                            HasLatestUpdate = false;
                            ShowUpdate(string.Format(Strings.Update_NewVersion, dataOnline.Latest.Name, dataOnline.Latest.Description), dataOnline.Latest.Link);
                        }
                        else
                        {
                            if (notify)
                            {
                                ShowUpdate(Strings.Update_UpToDate, "");
                            }
                            Console.WriteLine("You are up-to-date!");
                        }
                    }
                };
                webClient.DownloadStringAsync(new Uri(Settings.UpdateURL));
            }
            catch (ArgumentNullException e) { Console.WriteLine(e); }
            catch (WebException e) { Console.WriteLine(e); }
            catch (NotSupportedException e) { Console.WriteLine(e); }
        }

        private void WebClient_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private readonly object updatedLock = new object();
        private bool _HasLatestUpdate = false;
        public bool HasLatestUpdate
        {
            get
            {
                lock (updatedLock)
                {
                    return _HasLatestUpdate;

                }
            }
            set
            {
                lock (updatedLock)
                {
                    _HasLatestUpdate = value;
                }
            }
        }

        public SettingsData Settings { get; set; }

        public BuildOrderList BuildOrderList { get; set; } = new BuildOrderList();

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            SaveSettings();
        }

        private void SaveSettings()
        {
            string json = Settings.ToJson();
            using (StreamWriter writer = new StreamWriter(FolderNames.SettingsJson, false))
            {
                writer.Write(json);
            }
        }

        public SimpleCommand Quit
        {
            get { return new SimpleCommand(delegate () { Application.Current.Shutdown(); }); }
        }

        About about;

        public void ShowAbout()
        {

            about = new About();
            about.Show();
        }

        Update update;
        public void ShowUpdate(string message, string link)
        {
            update = new Update(message, link);
            update.Show();
        }

        public static bool ResourceExists(string resourcePath)
        {
            var assembly = Assembly.GetExecutingAssembly();

            return ResourceExists(assembly, resourcePath);
        }

        public static bool ResourceExists(Assembly assembly, string resourcePath)
        {
            return GetResourcePaths(assembly)
                .Contains(resourcePath.ToLowerInvariant());
        }

        public static IEnumerable<object> GetResourcePaths(Assembly assembly)
        {
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture;
            var resourceName = assembly.GetName().Name + ".g";
            var resourceManager = new ResourceManager(resourceName, assembly);

            try
            {
                var resourceSet = resourceManager.GetResourceSet(culture, true, true);

                foreach (System.Collections.DictionaryEntry resource in resourceSet)
                {
                    yield return resource.Key;
                }
            }
            finally
            {
                resourceManager.ReleaseAllResources();
            }
        }
    }
}
