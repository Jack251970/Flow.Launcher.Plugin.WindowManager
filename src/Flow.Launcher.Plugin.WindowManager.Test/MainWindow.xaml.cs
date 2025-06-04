using Flow.Launcher.Plugin.SharedModels;
using Flow.Launcher.Plugin.WindowManager.Models;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Flow.Launcher.Plugin.WindowManager.Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly WindowManager windowManager = new();
        public bool Visible { get; set; } = true;

        public MainWindow()
        {
            InitializeComponent();
            windowManager.Init(new PluginInitContext(null, new PublicAPIInstance() { MainWindow = this }));
            var results = windowManager.Query(new Query());
            foreach (var result in results)
            {
                if (result.ContextData is Command command)
                {
                    var item = new Button
                    {
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        Height = 30,
                        Content = command.Keyword
                    };
                    item.Click += async (s, e) =>
                    {
                        Debug.WriteLine($"Executing command: {command.Keyword}");
                        if (result.Action != null)
                        {
                            result.Action.Invoke(new ActionContext());
                        }
                        else if (result.AsyncAction != null)
                        {
                            await result.AsyncAction.Invoke(new ActionContext());
                        }
                        else
                        {
                            Debug.WriteLine($"No action defined for command: {command.Keyword}");
                            throw new NotImplementedException($"No action defined for command: {command.Keyword}");
                        }
                    };
                    MainPanel.Children.Add(item);
                }
            }
        }

        private class PublicAPIInstance : IPublicAPI
        {
            public event VisibilityChangedEventHandler VisibilityChanged = null!;

            public MainWindow MainWindow { get; set; } = null!;

            public void RegisterGlobalKeyboardCallback(Func<int, int, SpecialKeyState, bool> callback)
            {
                throw new NotImplementedException();
            }

            public void RemoveGlobalKeyboardCallback(Func<int, int, SpecialKeyState, bool> callback)
            {
                throw new NotImplementedException();
            }

            public Task HttpDownloadAsync(string url, string filePath, CancellationToken token = default)
            {
                throw new NotImplementedException();
            }

            public void ReQuery(bool reselect = true)
            {
                throw new NotImplementedException();
            }

            public void ChangeQuery(string query, bool requery = false)
            {
                throw new NotImplementedException();
            }

            public void RestartApp()
            {
                throw new NotImplementedException();
            }

            public void ShellRun(string cmd, string filename = "cmd.exe")
            {
                throw new NotImplementedException();
            }

            public void CopyToClipboard(string text, bool directCopy = false, bool showDefaultNotification = true)
            {
                throw new NotImplementedException();
            }

            public void SaveAppAllSettings()
            {
                throw new NotImplementedException();
            }

            public Task ReloadAllPluginData()
            {
                throw new NotImplementedException();
            }

            public void CheckForNewUpdate()
            {
                throw new NotImplementedException();
            }

            public void ShowMsgError(string title, string subTitle = "")
            {
                throw new NotImplementedException();
            }

            public void ShowMainWindow()
            {
                Application.Current.Dispatcher.Invoke(async () =>
                {
                    MainWindow.WindowState = WindowState.Normal;
                    await Task.Delay(200); // Allow time for the window to minimize
                    MainWindow.Visible = true;
                });
            }

            public void HideMainWindow()
            {
                Application.Current.Dispatcher.Invoke(async () =>
                {
                    MainWindow.WindowState = WindowState.Minimized;
                    await Task.Delay(200); // Allow time for the window to minimize
                    MainWindow.Visible = false;
                });
            }

            public bool IsMainWindowVisible()
            {
                return MainWindow.Visible;
            }

            public void ShowMsg(string title, string subTitle = "", string iconPath = "")
            {
                throw new NotImplementedException();
            }

            public void ShowMsg(string title, string subTitle, string iconPath, bool useMainWindowAsOwner = true)
            {
                throw new NotImplementedException();
            }

            public void OpenSettingDialog()
            {
                throw new NotImplementedException();
            }

            public string GetTranslation(string key)
            {
                return key;
            }

            public List<PluginPair> GetAllPlugins()
            {
                throw new NotImplementedException();
            }

            public MatchResult FuzzySearch(string query, string stringToCompare)
            {
                throw new NotImplementedException();
            }

            public Task<string> HttpGetStringAsync(string url, CancellationToken token = default)
            {
                throw new NotImplementedException();
            }

            public Task<Stream> HttpGetStreamAsync(string url, CancellationToken token = default)
            {
                throw new NotImplementedException();
            }

            public void AddActionKeyword(string pluginId, string newActionKeyword)
            {
                throw new NotImplementedException();
            }

            public void RemoveActionKeyword(string pluginId, string oldActionKeyword)
            {
                throw new NotImplementedException();
            }

            public bool ActionKeywordAssigned(string actionKeyword)
            {
                throw new NotImplementedException();
            }

            public void SavePluginSettings()
            {
                throw new NotImplementedException();
            }

            public void LogDebug(string className, string message, [CallerMemberName] string methodName = "")
            {
                Debug.WriteLine($"DEBUG: [{className}.{methodName}] {message}");
            }

            public void LogInfo(string className, string message, [CallerMemberName] string methodName = "")
            {
                Debug.WriteLine($"INFO: [{className}.{methodName}] {message}");
            }

            public void LogWarn(string className, string message, [CallerMemberName] string methodName = "")
            {
                Debug.WriteLine($"WARN: [{className}.{methodName}] {message}");
            }

            void IPublicAPI.LogError(string className, string message, string methodName)
            {
                Debug.WriteLine($"ERROR: [{className}.{methodName}] {message}");
            }

            public void LogException(string className, string message, Exception e, [CallerMemberName] string methodName = "")
            {
                Debug.WriteLine($"EXCEPTION: [{className}.{methodName}] {message}\n{e}");
            }

            public T LoadSettingJsonStorage<T>() where T : new()
            {
                return new T();
            }

            public void SaveSettingJsonStorage<T>() where T : new()
            {
                throw new NotImplementedException();
            }

            public void OpenDirectory(string DirectoryPath, string FileNameOrFilePath = null!)
            {
                throw new NotImplementedException();
            }

            public void OpenUrl(Uri url, bool? inPrivate = null)
            {
                throw new NotImplementedException();
            }

            public void OpenUrl(string url, bool? inPrivate = null)
            {
                throw new NotImplementedException();
            }

            public void OpenAppUri(Uri appUri)
            {
                throw new NotImplementedException();
            }

            public void OpenAppUri(string appUri)
            {
                throw new NotImplementedException();
            }

            public void ToggleGameMode()
            {
                throw new NotImplementedException();
            }

            public void SetGameMode(bool value)
            {
                throw new NotImplementedException();
            }

            public bool IsGameModeOn()
            {
                throw new NotImplementedException();
            }

            void IPublicAPI.FocusQueryTextBox()
            {
                throw new NotImplementedException();
            }

            Task IPublicAPI.HttpDownloadAsync(string url, string filePath, Action<double> reportProgress, CancellationToken token)
            {
                throw new NotImplementedException();
            }

            void IPublicAPI.BackToQueryResults()
            {
                throw new NotImplementedException();
            }

            MessageBoxResult IPublicAPI.ShowMsgBox(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult)
            {
                throw new NotImplementedException();
            }

            Task IPublicAPI.ShowProgressBoxAsync(string caption, Func<Action<double>, Task> reportProgressAsync, Action cancelProgress)
            {
                throw new NotImplementedException();
            }

            void IPublicAPI.StartLoadingBar()
            {
                throw new NotImplementedException();
            }

            void IPublicAPI.StopLoadingBar()
            {
                throw new NotImplementedException();
            }

            List<ThemeData> IPublicAPI.GetAvailableThemes()
            {
                throw new NotImplementedException();
            }

            ThemeData IPublicAPI.GetCurrentTheme()
            {
                throw new NotImplementedException();
            }

            bool IPublicAPI.SetCurrentTheme(ThemeData theme)
            {
                throw new NotImplementedException();
            }

            void IPublicAPI.SavePluginCaches()
            {
                throw new NotImplementedException();
            }

            Task<T> IPublicAPI.LoadCacheBinaryStorageAsync<T>(string cacheName, string cacheDirectory, T defaultData)
            {
                throw new NotImplementedException();
            }

            Task IPublicAPI.SaveCacheBinaryStorageAsync<T>(string cacheName, string cacheDirectory)
            {
                throw new NotImplementedException();
            }

            ValueTask<ImageSource> IPublicAPI.LoadImageAsync(string path, bool loadFullImage, bool cacheImage)
            {
                throw new NotImplementedException();
            }

            Task<bool> IPublicAPI.UpdatePluginManifestAsync(bool usePrimaryUrlOnly, CancellationToken token)
            {
                throw new NotImplementedException();
            }

            IReadOnlyList<UserPlugin> IPublicAPI.GetPluginManifest()
            {
                throw new NotImplementedException();
            }

            bool IPublicAPI.PluginModified(string id)
            {
                throw new NotImplementedException();
            }

            Task IPublicAPI.UpdatePluginAsync(PluginMetadata pluginMetadata, UserPlugin plugin, string zipFilePath)
            {
                throw new NotImplementedException();
            }

            void IPublicAPI.InstallPlugin(UserPlugin plugin, string zipFilePath)
            {
                throw new NotImplementedException();
            }

            Task IPublicAPI.UninstallPluginAsync(PluginMetadata pluginMetadata, bool removePluginSettings)
            {
                throw new NotImplementedException();
            }

            long IPublicAPI.StopwatchLogDebug(string className, string message, Action action, string methodName)
            {
                throw new NotImplementedException();
            }

            Task<long> IPublicAPI.StopwatchLogDebugAsync(string className, string message, Func<Task> action, string methodName)
            {
                throw new NotImplementedException();
            }

            long IPublicAPI.StopwatchLogInfo(string className, string message, Action action, string methodName)
            {
                throw new NotImplementedException();
            }

            Task<long> IPublicAPI.StopwatchLogInfoAsync(string className, string message, Func<Task> action, string methodName)
            {
                throw new NotImplementedException();
            }
        }
    }
}
