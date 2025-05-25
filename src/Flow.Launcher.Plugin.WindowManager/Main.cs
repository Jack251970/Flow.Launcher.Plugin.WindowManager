using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
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

    private readonly static bool _virtualDesktopSupported = OperatingSystem.IsWindowsVersionAtLeast(10, 0, 19041);
    private readonly Exception? _virtualDesktopException = null;
    private static bool _virtualDesktopEnabled = false;

    private readonly List<CommandType> _virtualDesktopTypes = new()
    {
        CommandType.PreviousDesktop,
        CommandType.NextDesktop,
        CommandType.MoveToPreviousDesktop,
        CommandType.MoveToNextDesktop,
        CommandType.ToggleWindowPinDesktops,
        CommandType.ToggleAppPinDesktops
    };

    private readonly List<Command> _commands = new()
    {
        new()
        {
            Type = CommandType.LeftTop,
            TitleKey = "flowlauncher_plugin_windowmanager_lefttop_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_lefttop_subtitle",
            IcoPath = "Images/icon.png",
            CommandAction = () => HandleForForegroundWindowAsync(LeftTop),
            Keyword = "Left top"
        },
        new()
        {
            Type = CommandType.Center,
            TitleKey = "flowlauncher_plugin_windowmanager_center_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_center_subtitle",
            IcoPath = "Images/icon.png",
            CommandAction = () => HandleForForegroundWindowAsync(Center),
            Keyword = "Center"
        },
        new()
        {
            Type = CommandType.Maximize,
            TitleKey = "flowlauncher_plugin_windowmanager_maximize_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_maximize_subtitle",
            IcoPath = "Images/icon.png",
            CommandAction = () => HandleForForegroundWindowAsync(Maximize),
            Keyword = "Maximize"
        },
        new()
        {
            Type = CommandType.Minimize,
            TitleKey = "flowlauncher_plugin_windowmanager_minimize_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_minimize_subtitle",
            IcoPath = "Images/icon.png",
            CommandAction = () => HandleForForegroundWindowAsync(Minimize),
            Keyword = "Minimize"
        },
        new()
        {
            Type = CommandType.Restore,
            TitleKey = "flowlauncher_plugin_windowmanager_restore_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_restore_subtitle",
            IcoPath = "Images/icon.png",
            CommandAction = () => HandleForForegroundWindowAsync(Restore),
            Keyword = "Restore"
        },
        new()
        {
            Type = CommandType.MoveUp,
            TitleKey = "flowlauncher_plugin_windowmanager_moveup_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_moveup_subtitle",
            IcoPath = "Images/icon.png",
            CommandAction = () => HandleForForegroundWindowAsync(MoveUp),
            Keyword = "Move up"
        },
        new()
        {
            Type = CommandType.MoveDown,
            TitleKey = "flowlauncher_plugin_windowmanager_movedown_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_movedown_subtitle",
            IcoPath = "Images/icon.png",
            CommandAction = () => HandleForForegroundWindowAsync(MoveDown),
            Keyword = "Move down"
        },
        new()
        {
            Type = CommandType.MoveLeft,
            TitleKey = "flowlauncher_plugin_windowmanager_moveleft_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_moveleft_subtitle",
            IcoPath = "Images/icon.png",
            CommandAction = () => HandleForForegroundWindowAsync(MoveLeft),
            Keyword = "Move left"
        },
        new()
        {
            Type = CommandType.MoveRight,
            TitleKey = "flowlauncher_plugin_windowmanager_moveright_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_moveright_subtitle",
            IcoPath = "Images/icon.png",
            CommandAction = () => HandleForForegroundWindowAsync(MoveRight),
            Keyword = "Move right"
        },
        new()
        {
            Type = CommandType.MaximizeHeight,
            TitleKey = "flowlauncher_plugin_windowmanager_maximizeheight_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_maximizeheight_subtitle",
            IcoPath = "Images/icon.png",
            CommandAction = () => HandleForForegroundWindowAsync(MaximizeHeight),
            Keyword = "Maximize height"
        },
        new()
        {
            Type = CommandType.MaximizeWidth,
            TitleKey = "flowlauncher_plugin_windowmanager_maximizewidth_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_maximizewidth_subtitle",
            IcoPath = "Images/icon.png",
            CommandAction = () => HandleForForegroundWindowAsync(MaximizeWidth),
            Keyword = "Maximize width"
        },
        new()
        {
            Type = CommandType.MakeSmaller,
            TitleKey = "flowlauncher_plugin_windowmanager_makesmaller_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_makesmaller_subtitle",
            IcoPath = "Images/icon.png",
            CommandAction = () => HandleForForegroundWindowAsync(MakeSmaller),
            Keyword = "Make smaller"
        },
        new()
        {
            Type = CommandType.MakeLarger,
            TitleKey = "flowlauncher_plugin_windowmanager_makelarger_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_makelarger_subtitle",
            IcoPath = "Images/icon.png",
            CommandAction = () => HandleForForegroundWindowAsync(MakeLarger),
            Keyword = "Make larger"
        },
        new()
        {
            Type = CommandType.PreviousDesktop,
            TitleKey = "flowlauncher_plugin_windowmanager_previousdesktop_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_previousdesktop_subtitle",
            IcoPath = "Images/icon.png",
            CommandAction = () => HandleForForegroundWindowAsync(PreviousDesktop),
            Keyword = "Previous desktop"
        },
        new()
        {
            Type = CommandType.NextDesktop,
            TitleKey = "flowlauncher_plugin_windowmanager_nextdesktop_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_nextdesktop_subtitle",
            IcoPath = "Images/icon.png",
            CommandAction = () => HandleForForegroundWindowAsync(NextDesktop),
            Keyword = "Next desktop"
        },
        new()
        {
            Type = CommandType.MoveToPreviousDesktop,
            TitleKey = "flowlauncher_plugin_windowmanager_movetopreviousdesktop_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_movetopreviousdesktop_subtitle",
            IcoPath = "Images/icon.png",
            CommandAction = () => HandleForForegroundWindowAsync(MoveToPreviousDesktop),
            Keyword = "Move to previous desktop"
        },
        new()
        {
            Type = CommandType.MoveToNextDesktop,
            TitleKey = "flowlauncher_plugin_windowmanager_movetonextdesktop_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_movetonextdesktop_subtitle",
            IcoPath = "Images/icon.png",
            CommandAction = () => HandleForForegroundWindowAsync(MoveToNextDesktop),
            Keyword = "Move to next desktop"
        },
        new()
        {
            Type = CommandType.ToggleWindowPinDesktops,
            TitleKey = "flowlauncher_plugin_windowmanager_togglewindowpindesktops_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_togglewindowpindesktops_subtitle",
            IcoPath = "Images/icon.png",
            CommandAction = () => HandleForForegroundWindowAsync(ToggleWindowPinDesktops),
            Keyword = "Toggle window pin desktops"
        },
        new()
        {
            Type = CommandType.ToggleAppPinDesktops,
            TitleKey = "flowlauncher_plugin_windowmanager_toggleapppindesktops_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_toggleapppindesktops_subtitle",
            IcoPath = "Images/icon.png",
            CommandAction = () => HandleForForegroundWindowAsync(ToggleAppPinDesktops),
            Keyword = "Toggle app pin desktops"
        },
        new()
        {
            Type = CommandType.PreviousScreen,
            TitleKey = "flowlauncher_plugin_windowmanager_previousscreen_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_previousscreen_subtitle",
            IcoPath = "Images/icon.png",
            CommandAction = () => HandleForForegroundWindowAsync(PreviousScreen),
            Keyword = "Previous screen"
        },
        new()
        {
            Type = CommandType.NextScreen,
            TitleKey = "flowlauncher_plugin_windowmanager_nextscreen_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_nextscreen_subtitle",
            IcoPath = "Images/icon.png",
            CommandAction = () => HandleForForegroundWindowAsync(NextScreen),
            Keyword = "Next screen"
        },
        new()
        {
            Type = CommandType.TopLeftQuarter,
            TitleKey = "flowlauncher_plugin_windowmanager_topleftquarter_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_topleftquarter_subtitle",
            IcoPath = "Images/icon.png",
            CommandAction = () => HandleForForegroundWindowAsync(TopLeftQuarter),
            Keyword = "Top left quarter"
        },
        new()
        {
            Type = CommandType.TopRightQuarter,
            TitleKey = "flowlauncher_plugin_windowmanager_toprightquarter_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_toprightquarter_subtitle",
            IcoPath = "Images/icon.png",
            CommandAction = () => HandleForForegroundWindowAsync(TopRightQuarter),
            Keyword = "Top right quarter"
        },
        new()
        {
            Type = CommandType.BottomLeftQuarter,
            TitleKey = "flowlauncher_plugin_windowmanager_bottomleftquarter_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_bottomleftquarter_subtitle",
            IcoPath = "Images/icon.png",
            CommandAction = () => HandleForForegroundWindowAsync(BottomLeftQuarter),
            Keyword = "Bottom left quarter"
        },
        new()
        {
            Type = CommandType.BottomRightQuarter,
            TitleKey = "flowlauncher_plugin_windowmanager_bottomrightquarter_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_bottomrightquarter_subtitle",
            IcoPath = "Images/icon.png",
            CommandAction = () => HandleForForegroundWindowAsync(BottomRightQuarter),
            Keyword = "Bottom right quarter"
        },
        new()
        {
            Type = CommandType.LeftHalf,
            TitleKey = "flowlauncher_plugin_windowmanager_lefthalf_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_lefthalf_subtitle",
            IcoPath = "Images/icon.png",
            CommandAction = () => HandleForForegroundWindowAsync(LeftHalf),
            Keyword = "Left half"
        },
        new()
        {
            Type = CommandType.RightHalf,
            TitleKey = "flowlauncher_plugin_windowmanager_righthalf_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_righthalf_subtitle",
            IcoPath = "Images/icon.png",
            CommandAction = () => HandleForForegroundWindowAsync(RightHalf),
            Keyword = "Right half"
        },
        new()
        {
            Type = CommandType.TopHalf,
            TitleKey = "flowlauncher_plugin_windowmanager_tophalf_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_tophalf_subtitle",
            IcoPath = "Images/icon.png",
            CommandAction = () => HandleForForegroundWindowAsync(TopHalf),
            Keyword = "Top half"
        },
        new()
        {
            Type = CommandType.BottomHalf,
            TitleKey = "flowlauncher_plugin_windowmanager_bottomhalf_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_bottomhalf_subtitle",
            IcoPath = "Images/icon.png",
            CommandAction = () => HandleForForegroundWindowAsync(BottomHalf),
            Keyword = "Bottom half"
        }
    };

    #endregion

    #region Constructor

    public WindowManager()
    {
        if (_virtualDesktopSupported)
        {
            // Initialize the Virtual Desktop API
            try
            {
                InitializeComObjects();
                _virtualDesktopEnabled = true;
            }
            catch (Exception ex)
            {
                _virtualDesktopException = ex;
            }
        }
    }

    #endregion

    #region IPlugin Interface

    public List<Result> Query(Query query)
    {
        return QueryList(query);
    }

    public void Init(PluginInitContext context)
    {
        Context = context;

        // Init settings
        Settings = context.API.LoadSettingJsonStorage<Settings>();
        Context.API.LogDebug(ClassName, $"Init: {Settings}");

        // Log debug information
        if (!_virtualDesktopSupported)
        {
            Context.API.LogDebug(ClassName, "Virtual Desktop API is not supported.");
            
        }
        else if(_virtualDesktopException == null)
        {
            Context.API.LogDebug(ClassName, "Virtual Desktop API is supported and initialized.");
        }
        else
        {
            Context.API.LogException(ClassName, "Virtual Desktop API is supported but failed to initialize.", _virtualDesktopException);
        }
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

    #region Move & Resize Actions

    // TODO: Change to Context.API.LogError.
    private static async Task HandleForForegroundWindowAsync(Action action)
    {
        action();

        await Task.CompletedTask;
    }

    private static async Task HandleForForegroundWindowAsync(Action<HWND> action)
    {
        while (Context.API.IsMainWindowVisible())
        {
            await Task.Delay(100);
        }

        var handle = PInvoke.GetForegroundWindow();
        if (handle.IsNull)
        {
            Context.API.LogInfo(ClassName, "Failed to find foreground window");
            return;
        }

        action(handle);
    }

    private static async Task HandleForForegroundWindowAsync(Action<HWND, RECT> action)
    {
        while (Context.API.IsMainWindowVisible())
        {
            await Task.Delay(100);
        }

        var handle = PInvoke.GetForegroundWindow();
        if (handle.IsNull)
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

    private static void MoveLeft(HWND handle, RECT rect)
    {
        var screen = MonitorInfo.GetNearestDisplayMonitor(handle);
        var leftX = (int)screen.RectWork.X;

        if (!PInvoke.SetWindowPos(handle, HWND.Null, leftX, rect.Y, rect.Width, rect.Height,
            SET_WINDOW_POS_FLAGS.SWP_NOSIZE | SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE))
        {
            Context.API.LogInfo(ClassName, "Failed to move left");
        }
    }

    private static void MoveRight(HWND handle, RECT rect)
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

    private static void MoveToPreviousDesktop(HWND handle)
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

    private static void MoveToNextDesktop(HWND handle)
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

    #region Query List

    private List<Result> QueryList(Query query)
    {
        var results = new List<Result>();
        var searchTerm = query.Search;
        if (string.IsNullOrEmpty(searchTerm))
        {
            foreach (var command in _commands)
            {
                if (_virtualDesktopTypes.Contains(command.Type) && !_virtualDesktopEnabled) continue;

                results.Add(new Result
                {
                    Title = Context.API.GetTranslation(command.TitleKey),
                    SubTitle = Context.API.GetTranslation(command.SubtitleKey),
                    IcoPath = command.IcoPath,
                    Score = 0,
                    Action = c =>
                    {
                        _ = Task.Run(async () =>
                        {
                            Context.API.HideMainWindow();
                            await command.CommandAction();
                        });
                        return true;
                    }
                });
            }
            return results;
        }
        else
        {
            foreach (var command in _commands)
            {
                if (_virtualDesktopTypes.Contains(command.Type) && !_virtualDesktopEnabled) continue;

                var match = Context.API.FuzzySearch(searchTerm, command.Keyword);

                if (!match.IsSearchPrecisionScoreMet()) continue;
                results.Add(new Result
                {
                    Title = Context.API.GetTranslation(command.TitleKey),
                    AutoCompleteText = command.Keyword,
                    SubTitle = Context.API.GetTranslation(command.SubtitleKey),
                    IcoPath = command.IcoPath,
                    Score = match.Score,
                    Action = c =>
                    {
                        _ = Task.Run(async () =>
                        {
                            Context.API.HideMainWindow();
                            await command.CommandAction();
                        });
                        return true;
                    }
                });
            }
        }

        return results;
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
