using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Flow.Launcher.Plugin.WindowManager;

public class WindowManager : IAsyncPlugin, IPluginI18n, ISettingProvider, IDisposable
{
    internal static PluginInitContext Context { get; private set; } = null!;

    internal static Settings Settings { get; private set; } = null!;

    #region Private Fileds

    private readonly static string ClassName = nameof(WindowManager);

    #endregion

    #region IAsyncPlugin Interface

    public async Task<List<Result>> QueryAsync(Query query, CancellationToken token)
    {
        return await Task.Run(() => Query(query, token), token);
    }

    public async Task<List<Result>> Query(Query query, CancellationToken token)
    {
        var searchTerm = query.Search;
        var results = new List<Result>();

        await Task.CompletedTask;

        return results;
    }

    public async Task InitAsync(PluginInitContext context)
    {
        Context = context;

        // Init settings
        Settings = context.API.LoadSettingJsonStorage<Settings>();
        Context.API.LogDebug(ClassName, $"Init: {Settings}");

        await Task.CompletedTask;
    }

    #endregion

    #region IPluginI18n Interface

    public string GetTranslatedPluginTitle()
    {
        return Context.API.GetTranslation("flowlauncher_plugin_windowmanager_plugin_name");
    }

    public string GetTranslatedPluginDescription()
    {
        return Context.API.GetTranslation("flowlauncher_plugin_windowmanager_plugin_description");
    }

    public void OnCultureInfoChanged(CultureInfo cultureInfo)
    {

    }

    #endregion

    #region ISettingProvider Interface

    public Control CreateSettingPanel()
    {
        Context.API.LogDebug(ClassName, $"Settings Panel: {Settings}");
        return null!;
    }

    #endregion

    #region IDisposable

    private bool _disposed;

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _disposed = true;
        }
    }

    #endregion
}
