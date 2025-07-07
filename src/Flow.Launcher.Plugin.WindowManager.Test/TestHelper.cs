using Flow.Launcher.Plugin.SharedModels;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;

namespace Flow.Launcher.Plugin.WindowManager.Test;

public class TestHelper
{
    public static void InitPlugin(IAsyncPlugin plugin, MainWindow window)
    {
        plugin.InitAsync(new PluginInitContext(null, new PublicAPIInstance(window))).Wait();
    }

    public static List<Result> Query(IAsyncPlugin plugin)
    {
        return plugin.QueryAsync(new Query(), CancellationToken.None).GetAwaiter().GetResult();
    }

    private class PublicAPIInstance(MainWindow mainWindow) : IPublicAPI
    {
        public event VisibilityChangedEventHandler VisibilityChanged = null!;

        public MainWindow MainWindow { get; } = mainWindow;

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
