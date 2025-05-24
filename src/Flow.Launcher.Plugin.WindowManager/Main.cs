using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;
using WindowsDesktop;

namespace Flow.Launcher.Plugin.WindowManager;

public class WindowManager : IPlugin, IPluginI18n, ISettingProvider, IDisposable
{
    internal static PluginInitContext Context { get; private set; } = null!;

    internal static Settings Settings { get; private set; } = null!;

    #region Private Fileds

    private readonly static string ClassName = nameof(WindowManager);

    private static HWND _mainHandle = HWND.Null;

    #endregion

    #region Constructor

    public WindowManager()
    {
        // Initialize the Virtual Desktop API
        InitializeComObjects();
    }

    #endregion

    #region IPlugin Interface

    public List<Result> Query(Query query)
    {
        var searchTerm = query.Search;
        var results = new List<Result>();

        return results;
    }

    public void Init(PluginInitContext context)
    {
        Context = context;

        // Init settings
        Settings = context.API.LoadSettingJsonStorage<Settings>();
        Context.API.LogDebug(ClassName, $"Init: {Settings}");

        // Init main handle
        _mainHandle = new HWND(Application.Current.MainWindow.GetHandle());
    }

    #endregion

    #region Initialization

    private static void InitializeComObjects()
    {
        VirtualDesktop.Configure();

        VirtualDesktop.Created += (_, desktop) =>
        {
            Context.API.LogDebug(ClassName, $"Created: {desktop.Name}");
        };

        VirtualDesktop.CurrentChanged += (_, args) =>
        {
            Context.API.LogDebug(ClassName, $"Switched: {args.OldDesktop.Name} -> {args.NewDesktop.Name}");
        };

        VirtualDesktop.Moved += (_, args) =>
        {
            Context.API.LogDebug(ClassName, $"Moved: {args.OldIndex} -> {args.NewIndex}, {args.Desktop}");
        };

        VirtualDesktop.Destroyed += (_, args) =>
        {
            Context.API.LogDebug(ClassName, $"Destroyed: {args.Destroyed}");   
        };

        VirtualDesktop.Renamed += (_, args) =>
        {
            Context.API.LogDebug(ClassName, $"Renamed: {args.Desktop}");
        };

        VirtualDesktop.WallpaperChanged += (_, args) =>
        {
            Context.API.LogDebug(ClassName, $"Wallpaper changed: {args.Desktop}, {args.Path}");
        };

        var currentId = VirtualDesktop.Current.Id;
        foreach (var desktop in VirtualDesktop.GetDesktops())
        {
            if (desktop.Id == currentId)
            {
                Context.API.LogDebug(ClassName, $"Current Desktop: {desktop.Name}, ID: {desktop.Id}");
            }
            else
            {
                Context.API.LogDebug(ClassName, $"Desktop: {desktop.Name}, ID: {desktop.Id}");
            }
        }
    }

    #endregion

    #region Move & Resize

    // TODO: Change to Context.API.LogError.
    private static void HandleForForegroundWindow(Action<HWND> action)
    {
        var timeOut = !SpinWait.SpinUntil(() => PInvoke.GetForegroundWindow() != _mainHandle, 1200);
        if (timeOut)
        {
            Context.API.LogInfo(ClassName, "Timeout waiting for foreground window to change from main handle");
            return;
        }

        var handle = PInvoke.GetForegroundWindow();
        if (handle == HWND.Null)
        {
            Context.API.LogInfo(ClassName, "Failed to find foreground window");
            return;
        }

        action(handle);
    }

    private static void HandleForForegroundWindow(Action<HWND, RECT> action)
    {
        var timeOut = !SpinWait.SpinUntil(() => PInvoke.GetForegroundWindow() != _mainHandle, 1200);
        if (timeOut)
        {
            Context.API.LogInfo(ClassName, "Timeout waiting for foreground window to change from main handle");
            return;
        }

        var handle = PInvoke.GetForegroundWindow();
        if (handle == HWND.Null)
        {
            Context.API.LogInfo(ClassName, "Failed to find foreground window");
            return;
        }

        if (!PInvoke.GetWindowRect(handle, out var rect))
        {
            Context.API.LogInfo(ClassName, "Failed to get window rect");
            return;
        }

        action(handle, rect);
    }

    private static void LeftTop(HWND handle, RECT rect)
    {
        var screen = MonitorInfo.GetNearestDisplayMonitor(handle);
        var width = rect.Width;
        var height = rect.Height;
        var leftX = (int)screen.RectWork.X;
        var topY = (int)screen.RectWork.Y;

        if (PInvoke.SetWindowPos(handle, HWND.Null, leftX, topY, width, height,
            SET_WINDOW_POS_FLAGS.SWP_NOSIZE | SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE))
        {
            Context.API.LogInfo(ClassName, "Failed to left top");
        }
    }

    private static void Center(HWND handle, RECT rect)
    {
        var screen = MonitorInfo.GetNearestDisplayMonitor(handle);
        var width = rect.Width;
        var height = rect.Height;
        var centerX = (int)Math.Round(screen.RectWork.X + screen.RectWork.Width / 2.0 - width / 2.0);
        var centerY = (int)Math.Round(screen.RectWork.Y + screen.RectWork.Height / 2.0 - height / 2.0);

        if (!PInvoke.SetWindowPos(handle, HWND.Null, centerX, centerY, width, height,
            SET_WINDOW_POS_FLAGS.SWP_NOSIZE | SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE))
        {
            Context.API.LogInfo(ClassName, "Failed to center");
        }
    }

    private static void Maximize(HWND handle)
    {
        if (!PInvoke.ShowWindow(handle, SHOW_WINDOW_CMD.SW_MAXIMIZE))
        {
            Context.API.LogInfo(ClassName, "Failed to maximize");
        }
    }

    private static void Minimize(HWND handle)
    {
        if (!PInvoke.ShowWindow(handle, SHOW_WINDOW_CMD.SW_MINIMIZE))
        {
            Context.API.LogInfo(ClassName, "Failed to minimize");
        }
    }

    private static void Restore(HWND handle)
    {
        if (!PInvoke.ShowWindow(handle, SHOW_WINDOW_CMD.SW_RESTORE))
        {
            Context.API.LogInfo(ClassName, "Failed to restore");
        }
    }

    private static void MoveUp(HWND handle, RECT rect)
    {
        var screen = MonitorInfo.GetNearestDisplayMonitor(handle);
        var topY = (int)screen.RectWork.Y;

        if (!PInvoke.SetWindowPos(handle, HWND.Null, rect.X, topY, rect.Width, rect.Height,
            SET_WINDOW_POS_FLAGS.SWP_NOSIZE | SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE))
        {
            Context.API.LogInfo(ClassName, "Failed to move up");
        }
    }

    private static void MoveDown(HWND handle, RECT rect)
    {
        var screen = MonitorInfo.GetNearestDisplayMonitor(handle);
        var bottomY = (int)screen.RectWork.Bottom - rect.Height;

        if (!PInvoke.SetWindowPos(handle, HWND.Null, rect.X, bottomY, rect.Width, rect.Height,
            SET_WINDOW_POS_FLAGS.SWP_NOSIZE | SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE))
        {
            Context.API.LogInfo(ClassName, "Failed to move down");
        }
    }

    private static void MoveLeftInScreen(HWND handle, RECT rect)
    {
        var screen = MonitorInfo.GetNearestDisplayMonitor(handle);
        var leftX = (int)screen.RectWork.X;

        if (!PInvoke.SetWindowPos(handle, HWND.Null, leftX, rect.Y, rect.Width, rect.Height,
            SET_WINDOW_POS_FLAGS.SWP_NOSIZE | SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE))
        {
            Context.API.LogInfo(ClassName, "Failed to move left");
        }
    }

    private static void MoveRightInScreen(HWND handle, RECT rect)
    {
        var screen = MonitorInfo.GetNearestDisplayMonitor(handle);
        var rightX = (int)screen.RectWork.Right - rect.Width;

        if (!PInvoke.SetWindowPos(handle, HWND.Null, rightX, rect.Y, rect.Width, rect.Height,
            SET_WINDOW_POS_FLAGS.SWP_NOSIZE | SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE))
        {
            Context.API.LogInfo(ClassName, "Failed to move right");
        }
    }

    private static void MaximizeHeight(HWND handle, RECT rect)
    {
        var screen = MonitorInfo.GetNearestDisplayMonitor(handle);
        var topY = (int)screen.RectWork.Y;
        var height = (int)screen.RectWork.Height;

        if (!PInvoke.SetWindowPos(handle, HWND.Null, rect.X, topY, rect.Width, height,
            SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE))
        {
            Context.API.LogInfo(ClassName, "Failed to maximize height");
        }
    }

    private static void MaximizeWidth(HWND handle, RECT rect)
    {
        var screen = MonitorInfo.GetNearestDisplayMonitor(handle);
        var leftX = (int)screen.RectWork.X;
        var width = (int)screen.RectWork.Width;

        if (!PInvoke.SetWindowPos(handle, HWND.Null, leftX, rect.Y, width, rect.Height,
            SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE))
        {
            Context.API.LogInfo(ClassName, "Failed to maximize width");
        }
    }

    private static void MakeSmaller(HWND handle, RECT rect)
    {
        var screen = MonitorInfo.GetNearestDisplayMonitor(handle);
        var intervalWidth = (int)Math.Round(screen.RectWork.Width * Settings.SizeIntervalPercentage / 100.0);
        var intervalHeight = (int)Math.Round(screen.RectWork.Height * Settings.SizeIntervalPercentage / 100.0);

        var width = rect.Width - intervalWidth;
        var height = rect.Height - intervalHeight;
        width = width < 0 ? 0 : width;
        height = height < 0 ? 0 : height;

        var winX = (int)Math.Round(rect.X + intervalWidth / 2.0);
        var winY = (int)Math.Round(rect.Y + intervalHeight / 2.0);

        if (!PInvoke.SetWindowPos(handle, HWND.Null, winX, winY, width, height,
            SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE))
        {
            Context.API.LogInfo(ClassName, "Failed to make smaller");
        }
    }

    private static void MakeLarger(HWND handle, RECT rect)
    {
        var screen = MonitorInfo.GetNearestDisplayMonitor(handle);
        var intervalWidth = (int)Math.Round(screen.RectWork.Width * Settings.SizeIntervalPercentage / 100.0);
        var intervalHeight = (int)Math.Round(screen.RectWork.Height * Settings.SizeIntervalPercentage / 100.0);

        var width = rect.Width + intervalWidth;
        var height = rect.Height + intervalHeight;
        width = width > screen.RectWork.Width ? (int)Math.Round(screen.RectWork.Width) : width;
        height = height > screen.RectWork.Height ? (int)Math.Round(screen.RectWork.Height) : height;

        var winX = (int)Math.Round(rect.X - intervalWidth / 2.0);
        var winY = (int)Math.Round(rect.Y - intervalHeight / 2.0);

        if (!PInvoke.SetWindowPos(handle, HWND.Null, winX, winY, width, height,
            SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE))
        {
            Context.API.LogInfo(ClassName, "Failed to make smaller");
        }
    }

    private static void PreviousDesktop()
    {
        var desktops = VirtualDesktop.GetDesktops();
        if (desktops.Length <= 1)
        {
            Context.API.LogInfo(ClassName, "No other desktops available to move to");
            return;
        }

        var index = Array.IndexOf(desktops, VirtualDesktop.Current);
        var prevDesktop = index > 0 ? desktops[index - 1] : desktops[^1];

        prevDesktop.Switch();
    }

    private static void NextDesktop()
    {
        var desktops = VirtualDesktop.GetDesktops();
        if (desktops.Length <= 1)
        {
            Context.API.LogInfo(ClassName, "No other desktops available to move to");
            return;
        }

        var index = Array.IndexOf(desktops, VirtualDesktop.Current);
        var nextDesktop = index < desktops.Length - 1 ? desktops[index + 1] : desktops[0];

        nextDesktop.Switch();
    }

    private static void MovePreviousDesktop(HWND handle)
    {
        var desktops = VirtualDesktop.GetDesktops();
        if (desktops.Length <= 1)
        {
            Context.API.LogInfo(ClassName, "No other desktops available to move to");
            return;
        }

        var index = Array.IndexOf(desktops, VirtualDesktop.Current);
        var prevDesktop = index > 0 ? desktops[index - 1] : desktops[^1];

        if (VirtualDesktop.IsPinnedWindow(handle) == false)
        {
            VirtualDesktop.MoveToDesktop(handle, prevDesktop);
        }
        prevDesktop.Switch();
    }

    private static void MoveNextDesktop(HWND handle)
    {
        var desktops = VirtualDesktop.GetDesktops();
        if (desktops.Length <= 1)
        {
            Context.API.LogInfo(ClassName, "No other desktops available to move to");
            return;
        }

        var index = Array.IndexOf(desktops, VirtualDesktop.Current);
        var nextDesktop = index < desktops.Length - 1 ? desktops[index + 1] : desktops[0];

        if (VirtualDesktop.IsPinnedWindow(handle) == false)
        {
            VirtualDesktop.MoveToDesktop(handle, nextDesktop);
        }
        nextDesktop.Switch();
    }

    private static void ToggleWindowPinDesktops(HWND handle)
    {
        (VirtualDesktop.IsPinnedWindow(handle) ?
            VirtualDesktop.UnpinWindow :
            (Func<IntPtr, bool>)VirtualDesktop.PinWindow)(handle);
    }

    private static void ToggleAppPinDesktops(HWND handle)
    {
        if (VirtualDesktop.TryGetAppUserModelId(handle, out var appId))
        {
            (VirtualDesktop.IsPinnedApplication(appId) ?
                VirtualDesktop.UnpinApplication :
                (Func<string, bool>)VirtualDesktop.PinApplication)(appId);
        }
    }

    private static void PreviousScreen(HWND handle, RECT rect)
    {
        var screens = MonitorInfo.GetDisplayMonitors();
        if (screens.Count <= 1)
        {
            Context.API.LogInfo(ClassName, "No other screens available to move to");
            return;
        }

        var screen = MonitorInfo.GetNearestDisplayMonitor(handle);
        var index = screens.IndexOf(screen);
        var prevScreen = index > 0 ? screens[index - 1] : screens[^1];

        if (!PInvoke.SetWindowPos(handle, HWND.Null, (int)prevScreen.RectWork.X, (int)prevScreen.RectWork.Y, rect.Width, rect.Height,
            SET_WINDOW_POS_FLAGS.SWP_NOSIZE | SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE))
        {
            Context.API.LogInfo(ClassName, "Failed to move to previous screen");
        }
    }

    private static void NextScreen(HWND handle, RECT rect)
    {
        var screens = MonitorInfo.GetDisplayMonitors();
        if (screens.Count <= 1)
        {
            Context.API.LogInfo(ClassName, "No other screens available to move to");
            return;
        }

        var screen = MonitorInfo.GetNearestDisplayMonitor(handle);
        var index = screens.IndexOf(screen);
        var nextScreen = index < screens.Count - 1 ? screens[index + 1] : screens[0];

        if (!PInvoke.SetWindowPos(handle, HWND.Null, (int)nextScreen.RectWork.X, (int)nextScreen.RectWork.Y, rect.Width, rect.Height,
            SET_WINDOW_POS_FLAGS.SWP_NOSIZE | SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE))
        {
            Context.API.LogInfo(ClassName, "Failed to move to next screen");
        }
    }

    private static void TopLeftQuarter(HWND handle)
    {
        var screen = MonitorInfo.GetNearestDisplayMonitor(handle);
        var width = (int)Math.Round(screen.RectWork.Width / 2.0);
        var height = (int)Math.Round(screen.RectWork.Height / 2.0);
        var topLeftX = (int)screen.RectWork.X;
        var topLeftY = (int)screen.RectWork.Y;

        if (!PInvoke.SetWindowPos(handle, HWND.Null, topLeftX, topLeftY, width, height,
            SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE))
        {
            Context.API.LogInfo(ClassName, "Failed to move to top left quarter");
        }
    }

    private static void TopRightQuarter(HWND handle)
    {
        var screen = MonitorInfo.GetNearestDisplayMonitor(handle);
        var width = (int)Math.Round(screen.RectWork.Width / 2.0);
        var height = (int)Math.Round(screen.RectWork.Height / 2.0);
        var topRightX = (int)screen.RectWork.Right - width;
        var topRightY = (int)screen.RectWork.Y;

        if (!PInvoke.SetWindowPos(handle, HWND.Null, topRightX, topRightY, width, height,
            SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE))
        {
            Context.API.LogInfo(ClassName, "Failed to move to top right quarter");
        }
    }

    private static void BottomLeftQuarter(HWND handle)
    {
        var screen = MonitorInfo.GetNearestDisplayMonitor(handle);
        var width = (int)Math.Round(screen.RectWork.Width / 2.0);
        var height = (int)Math.Round(screen.RectWork.Height / 2.0);
        var bottomLeftX = (int)screen.RectWork.X;
        var bottomLeftY = (int)screen.RectWork.Bottom - height;

        if (!PInvoke.SetWindowPos(handle, HWND.Null, bottomLeftX, bottomLeftY, width, height,
            SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE))
        {
            Context.API.LogInfo(ClassName, "Failed to move to bottom left quarter");
        }
    }

    private static void BottomRightQuarter(HWND handle)
    {
        var screen = MonitorInfo.GetNearestDisplayMonitor(handle);
        var width = (int)Math.Round(screen.RectWork.Width / 2.0);
        var height = (int)Math.Round(screen.RectWork.Height / 2.0);
        var bottomRightX = (int)screen.RectWork.Right - width;
        var bottomRightY = (int)screen.RectWork.Bottom - height;

        if (!PInvoke.SetWindowPos(handle, HWND.Null, bottomRightX, bottomRightY, width, height,
            SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE))
        {
            Context.API.LogInfo(ClassName, "Failed to move to bottom right quarter");
        }
    }

    private static void LeftHalf(HWND handle)
    {
        var screen = MonitorInfo.GetNearestDisplayMonitor(handle);
        var width = (int)Math.Round(screen.RectWork.Width / 2.0);
        var height = (int)Math.Round(screen.RectWork.Height);
        var leftX = (int)screen.RectWork.X;
        var topY = (int)screen.RectWork.Y;

        if (!PInvoke.SetWindowPos(handle, HWND.Null, leftX, topY, width, height,
            SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE))
        {
            Context.API.LogInfo(ClassName, "Failed to move to left half");
        }
    }

    private static void RightHalf(HWND handle)
    {
        var screen = MonitorInfo.GetNearestDisplayMonitor(handle);
        var width = (int)Math.Round(screen.RectWork.Width / 2.0);
        var height = (int)Math.Round(screen.RectWork.Height);
        var rightX = (int)screen.RectWork.Right - width;
        var topY = (int)screen.RectWork.Y;

        if (!PInvoke.SetWindowPos(handle, HWND.Null, rightX, topY, width, height,
            SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE))
        {
            Context.API.LogInfo(ClassName, "Failed to move to right half");
        }
    }

    private static void TopHalf(HWND handle)
    {
        var screen = MonitorInfo.GetNearestDisplayMonitor(handle);
        var width = (int)Math.Round(screen.RectWork.Width);
        var height = (int)Math.Round(screen.RectWork.Height / 2.0);
        var leftX = (int)screen.RectWork.X;
        var topY = (int)screen.RectWork.Y;

        if (!PInvoke.SetWindowPos(handle, HWND.Null, leftX, topY, width, height,
            SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE))
        {
            Context.API.LogInfo(ClassName, "Failed to move to top half");
        }
    }

    private static void BottomHalf(HWND handle)
    {
        var screen = MonitorInfo.GetNearestDisplayMonitor(handle);
        var width = (int)Math.Round(screen.RectWork.Width);
        var height = (int)Math.Round(screen.RectWork.Height / 2.0);
        var leftX = (int)screen.RectWork.X;
        var bottomY = (int)screen.RectWork.Bottom - height;

        if (!PInvoke.SetWindowPos(handle, HWND.Null, leftX, bottomY, width, height,
            SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE))
        {
            Context.API.LogInfo(ClassName, "Failed to move to bottom half");
        }
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
