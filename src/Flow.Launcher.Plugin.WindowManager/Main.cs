using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Controls;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace Flow.Launcher.Plugin.WindowManager;

public class WindowManager : IPlugin, IPluginI18n, ISettingProvider, IDisposable
{
    internal static PluginInitContext Context { get; private set; } = null!;

    internal static Settings Settings { get; private set; } = null!;

    #region Private Fileds

    private readonly static string ClassName = nameof(WindowManager);

    private readonly IList<CommandType> _multipleScreenCommands = new List<CommandType>()
    {
        CommandType.PreviousScreen,
        CommandType.NextScreen
    };

    private readonly List<Command> _commands = new()
    {
        new()
        {
            Type = CommandType.TopLeft,
            TitleKey = "flowlauncher_plugin_windowmanager_topleft_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_topleft_subtitle",
            IcoPath = "Images/top left.png",
            CommandAction = () => HandleForForegroundWindowAsync(TopLeft),
            Keyword = "Top left"
        },
        new()
        {
            Type = CommandType.Center,
            TitleKey = "flowlauncher_plugin_windowmanager_center_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_center_subtitle",
            IcoPath = "Images/center.png",
            CommandAction = () => HandleForForegroundWindowAsync(Center),
            Keyword = "Center"
        },
        new()
        {
            Type = CommandType.Maximize,
            TitleKey = "flowlauncher_plugin_windowmanager_maximize_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_maximize_subtitle",
            IcoPath = "Images/maximize.png",
            CommandAction = () => HandleForForegroundWindowAsync(Maximize),
            Keyword = "Maximize"
        },
        new()
        {
            Type = CommandType.Minimize,
            TitleKey = "flowlauncher_plugin_windowmanager_minimize_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_minimize_subtitle",
            IcoPath = "Images/minimize.png",
            CommandAction = () => HandleForForegroundWindowAsync(Minimize),
            Keyword = "Minimize"
        },
        new()
        {
            Type = CommandType.Restore,
            TitleKey = "flowlauncher_plugin_windowmanager_restore_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_restore_subtitle",
            IcoPath = "Images/restore.png",
            CommandAction = () => HandleForForegroundWindowAsync(Restore),
            Keyword = "Restore"
        },
        new()
        {
            Type = CommandType.Close,
            TitleKey = "flowlauncher_plugin_windowmanager_close_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_close_subtitle",
            IcoPath = "Images/close.png",
            CommandAction = () => HandleForForegroundWindowAsync(Close),
            Keyword = "Close"
        },
        new()
        {
            Type = CommandType.MoveUp,
            TitleKey = "flowlauncher_plugin_windowmanager_moveup_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_moveup_subtitle",
            IcoPath = "Images/move up.png",
            CommandAction = () => HandleForForegroundWindowAsync(MoveUp),
            Keyword = "Move up"
        },
        new()
        {
            Type = CommandType.MoveDown,
            TitleKey = "flowlauncher_plugin_windowmanager_movedown_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_movedown_subtitle",
            IcoPath = "Images/move down.png",
            CommandAction = () => HandleForForegroundWindowAsync(MoveDown),
            Keyword = "Move down"
        },
        new()
        {
            Type = CommandType.MoveLeft,
            TitleKey = "flowlauncher_plugin_windowmanager_moveleft_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_moveleft_subtitle",
            IcoPath = "Images/move left.png",
            CommandAction = () => HandleForForegroundWindowAsync(MoveLeft),
            Keyword = "Move left"
        },
        new()
        {
            Type = CommandType.MoveRight,
            TitleKey = "flowlauncher_plugin_windowmanager_moveright_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_moveright_subtitle",
            IcoPath = "Images/move right.png",
            CommandAction = () => HandleForForegroundWindowAsync(MoveRight),
            Keyword = "Move right"
        },
        new()
        {
            Type = CommandType.MaximizeHeight,
            TitleKey = "flowlauncher_plugin_windowmanager_maximizeheight_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_maximizeheight_subtitle",
            IcoPath = "Images/maximize height.png",
            CommandAction = () => HandleForForegroundWindowAsync(MaximizeHeight),
            Keyword = "Maximize height"
        },
        new()
        {
            Type = CommandType.MaximizeWidth,
            TitleKey = "flowlauncher_plugin_windowmanager_maximizewidth_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_maximizewidth_subtitle",
            IcoPath = "Images/maximize width.png",
            CommandAction = () => HandleForForegroundWindowAsync(MaximizeWidth),
            Keyword = "Maximize width"
        },
        new()
        {
            Type = CommandType.MakeSmaller,
            TitleKey = "flowlauncher_plugin_windowmanager_makesmaller_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_makesmaller_subtitle",
            IcoPath = "Images/make smaller.png",
            CommandAction = () => HandleForForegroundWindowAsync(MakeSmaller),
            Keyword = "Make smaller"
        },
        new()
        {
            Type = CommandType.MakeLarger,
            TitleKey = "flowlauncher_plugin_windowmanager_makelarger_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_makelarger_subtitle",
            IcoPath = "Images/make larger.png",
            CommandAction = () => HandleForForegroundWindowAsync(MakeLarger),
            Keyword = "Make larger"
        },
        new()
        {
            Type = CommandType.PreviousScreen,
            TitleKey = "flowlauncher_plugin_windowmanager_previousscreen_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_previousscreen_subtitle",
            IcoPath = "Images/previous screen.png",
            CommandAction = () => HandleForForegroundWindowAsync(PreviousScreen),
            Keyword = "Previous screen"
        },
        new()
        {
            Type = CommandType.NextScreen,
            TitleKey = "flowlauncher_plugin_windowmanager_nextscreen_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_nextscreen_subtitle",
            IcoPath = "Images/next screen.png",
            CommandAction = () => HandleForForegroundWindowAsync(NextScreen),
            Keyword = "Next screen"
        },
        new()
        {
            Type = CommandType.TopLeftQuarter,
            TitleKey = "flowlauncher_plugin_windowmanager_topleftquarter_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_topleftquarter_subtitle",
            IcoPath = "Images/top left quarter.png",
            CommandAction = () => HandleForForegroundWindowAsync(TopLeftQuarter),
            Keyword = "Top left quarter"
        },
        new()
        {
            Type = CommandType.TopRightQuarter,
            TitleKey = "flowlauncher_plugin_windowmanager_toprightquarter_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_toprightquarter_subtitle",
            IcoPath = "Images/top right quarter.png",
            CommandAction = () => HandleForForegroundWindowAsync(TopRightQuarter),
            Keyword = "Top right quarter"
        },
        new()
        {
            Type = CommandType.BottomLeftQuarter,
            TitleKey = "flowlauncher_plugin_windowmanager_bottomleftquarter_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_bottomleftquarter_subtitle",
            IcoPath = "Images/bottom left quarter.png",
            CommandAction = () => HandleForForegroundWindowAsync(BottomLeftQuarter),
            Keyword = "Bottom left quarter"
        },
        new()
        {
            Type = CommandType.BottomRightQuarter,
            TitleKey = "flowlauncher_plugin_windowmanager_bottomrightquarter_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_bottomrightquarter_subtitle",
            IcoPath = "Images/bottom right quarter.png",
            CommandAction = () => HandleForForegroundWindowAsync(BottomRightQuarter),
            Keyword = "Bottom right quarter"
        },
        new()
        {
            Type = CommandType.LeftHalf,
            TitleKey = "flowlauncher_plugin_windowmanager_lefthalf_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_lefthalf_subtitle",
            IcoPath = "Images/left half.png",
            CommandAction = () => HandleForForegroundWindowAsync(LeftHalf),
            Keyword = "Left half"
        },
        new()
        {
            Type = CommandType.RightHalf,
            TitleKey = "flowlauncher_plugin_windowmanager_righthalf_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_righthalf_subtitle",
            IcoPath = "Images/right half.png",
            CommandAction = () => HandleForForegroundWindowAsync(RightHalf),
            Keyword = "Right half"
        },
        new()
        {
            Type = CommandType.TopHalf,
            TitleKey = "flowlauncher_plugin_windowmanager_tophalf_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_tophalf_subtitle",
            IcoPath = "Images/top half.png",
            CommandAction = () => HandleForForegroundWindowAsync(TopHalf),
            Keyword = "Top half"
        },
        new()
        {
            Type = CommandType.BottomHalf,
            TitleKey = "flowlauncher_plugin_windowmanager_bottomhalf_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_bottomhalf_subtitle",
            IcoPath = "Images/bottom half.png",
            CommandAction = () => HandleForForegroundWindowAsync(BottomHalf),
            Keyword = "Bottom half"
        },
        new()
        {
            Type = CommandType.ReasonableSize,
            TitleKey = "flowlauncher_plugin_windowmanager_reasonablesize_title",
            SubtitleKey = "flowlauncher_plugin_windowmanager_reasonablesize_subtitle",
            IcoPath = "Images/center.png",
            CommandAction = () => HandleForForegroundWindowAsync(ReasonableSize),
            Keyword = "Reasonable size"
        }
    };

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
                if (_multipleScreenCommands.Contains(command.Type) && MonitorInfo.GetDisplayMonitorCount() <= 1)
                {
                    continue; // Skip commands that require multiple screens if only one screen is available
                }

                results.Add(new Result
                {
                    Title = Context.API.GetTranslation(command.TitleKey),
                    ContextData = command,
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
                if (_multipleScreenCommands.Contains(command.Type) && MonitorInfo.GetDisplayMonitorCount() <= 1)
                {
                    continue; // Skip commands that require multiple screens if only one screen is available
                }

                var match = Context.API.FuzzySearch(searchTerm, command.Keyword);

                if (!match.IsSearchPrecisionScoreMet()) continue;
                results.Add(new Result
                {
                    Title = Context.API.GetTranslation(command.TitleKey),
                    ContextData = command,
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

    #region Move & Resize Actions

    private static async Task HandleForForegroundWindowAsync(Action<HWND> action)
    {
        while (Context.API.IsMainWindowVisible())
        {
            await Task.Delay(100);
        }

        var handle = PInvoke.GetForegroundWindow();
        if (handle.IsNull)
        {
            Context.API.LogError(ClassName, "Failed to find foreground window");
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
            Context.API.LogError(ClassName, "Failed to find foreground window");
            return;
        }

        if (!PInvoke.GetWindowRect(handle, out var rect))
        {
            Context.API.LogError(ClassName, "Failed to get window rect");
            return;
        }

        action(handle, rect);
    }

    private static void TopLeft(HWND handle, RECT rect)
    {
        // Do nothing if window is maximized
        if (PInvoke.IsZoomed(handle))
        {
            Context.API.LogInfo(ClassName, "Window is maximized");
            return;
        }

        var screen = MonitorInfo.GetNearestDisplayMonitor(handle);
        var width = rect.Width;
        var height = rect.Height;
        var leftX = (int)screen.RectWork.X;
        var topY = (int)screen.RectWork.Y;

        if (!PInvoke.SetWindowPos(handle, HWND.Null, leftX, topY, width, height,
            SET_WINDOW_POS_FLAGS.SWP_NOSIZE | SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE))
        {
            Context.API.LogError(ClassName, "Failed to top left");
        }
    }

    private static void Center(HWND handle, RECT rect)
    {
        // Do nothing if window is maximized
        if (PInvoke.IsZoomed(handle))
        {
            Context.API.LogInfo(ClassName, "Window is maximized");
            return;
        }

        var screen = MonitorInfo.GetNearestDisplayMonitor(handle);
        var width = rect.Width;
        var height = rect.Height;
        var centerX = (int)Math.Round(screen.RectWork.X + screen.RectWork.Width / 2.0 - width / 2.0);
        var centerY = (int)Math.Round(screen.RectWork.Y + screen.RectWork.Height / 2.0 - height / 2.0);

        if (!PInvoke.SetWindowPos(handle, HWND.Null, centerX, centerY, width, height,
            SET_WINDOW_POS_FLAGS.SWP_NOSIZE | SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE))
        {
            Context.API.LogError(ClassName, "Failed to center");
        }
    }

    private static void Maximize(HWND handle)
    {
        if (!PInvoke.ShowWindow(handle, SHOW_WINDOW_CMD.SW_MAXIMIZE))
        {
            Context.API.LogError(ClassName, "Failed to maximize");
        }
    }

    private static void Minimize(HWND handle)
    {
        if (!PInvoke.ShowWindow(handle, SHOW_WINDOW_CMD.SW_MINIMIZE))
        {
            Context.API.LogError(ClassName, "Failed to minimize");
        }
    }

    private static void Restore(HWND handle)
    {
        if (!PInvoke.ShowWindow(handle, SHOW_WINDOW_CMD.SW_RESTORE))
        {
            Context.API.LogError(ClassName, "Failed to restore");
        }
    }

    private static void Close(HWND handle)
    {
        if (PInvoke.SendMessage(handle, PInvoke.WM_CLOSE, 0, 0) != new LRESULT(0))
        {
            Context.API.LogError(ClassName, "Failed to close");
        }
    }

    private static void MoveUp(HWND handle, RECT rect)
    {
        // Do nothing if window is maximized
        if (PInvoke.IsZoomed(handle))
        {
            Context.API.LogInfo(ClassName, "Window is maximized");
            return;
        }

        var screen = MonitorInfo.GetNearestDisplayMonitor(handle);
        var topY = (int)screen.RectWork.Y;

        if (!PInvoke.SetWindowPos(handle, HWND.Null, rect.X, topY, rect.Width, rect.Height,
            SET_WINDOW_POS_FLAGS.SWP_NOSIZE | SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE))
        {
            Context.API.LogError(ClassName, "Failed to move up");
        }
    }

    private static void MoveDown(HWND handle, RECT rect)
    {
        // Do nothing if window is maximized
        if (PInvoke.IsZoomed(handle))
        {
            Context.API.LogInfo(ClassName, "Window is maximized");
            return;
        }

        var screen = MonitorInfo.GetNearestDisplayMonitor(handle);
        var bottomY = (int)screen.RectWork.Bottom - rect.Height;

        if (!PInvoke.SetWindowPos(handle, HWND.Null, rect.X, bottomY, rect.Width, rect.Height,
            SET_WINDOW_POS_FLAGS.SWP_NOSIZE | SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE))
        {
            Context.API.LogError(ClassName, "Failed to move down");
        }
    }

    private static void MoveLeft(HWND handle, RECT rect)
    {
        // Do nothing if window is maximized
        if (PInvoke.IsZoomed(handle))
        {
            Context.API.LogInfo(ClassName, "Window is maximized");
            return;
        }

        var screen = MonitorInfo.GetNearestDisplayMonitor(handle);
        var leftX = (int)screen.RectWork.X;

        if (!PInvoke.SetWindowPos(handle, HWND.Null, leftX, rect.Y, rect.Width, rect.Height,
            SET_WINDOW_POS_FLAGS.SWP_NOSIZE | SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE))
        {
            Context.API.LogError(ClassName, "Failed to move left");
        }
    }

    private static void MoveRight(HWND handle, RECT rect)
    {
        // Do nothing if window is maximized
        if (PInvoke.IsZoomed(handle))
        {
            Context.API.LogInfo(ClassName, "Window is maximized");
            return;
        }

        var screen = MonitorInfo.GetNearestDisplayMonitor(handle);
        var rightX = (int)screen.RectWork.Right - rect.Width;

        if (!PInvoke.SetWindowPos(handle, HWND.Null, rightX, rect.Y, rect.Width, rect.Height,
            SET_WINDOW_POS_FLAGS.SWP_NOSIZE | SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE))
        {
            Context.API.LogError(ClassName, "Failed to move right");
        }
    }

    private static void MaximizeHeight(HWND handle, RECT rect)
    {
        // Do nothing if window is maximized
        if (PInvoke.IsZoomed(handle))
        {
            Context.API.LogInfo(ClassName, "Window is maximized");
            return;
        }

        var screen = MonitorInfo.GetNearestDisplayMonitor(handle);
        var topY = (int)screen.RectWork.Y;
        var height = (int)screen.RectWork.Height;

        if (!PInvoke.SetWindowPos(handle, HWND.Null, rect.X, topY, rect.Width, height,
            SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE))
        {
            Context.API.LogError(ClassName, "Failed to maximize height");
        }
    }

    private static void MaximizeWidth(HWND handle, RECT rect)
    {
        // Do nothing if window is maximized
        if (PInvoke.IsZoomed(handle))
        {
            Context.API.LogInfo(ClassName, "Window is maximized");
            return;
        }

        var screen = MonitorInfo.GetNearestDisplayMonitor(handle);
        var leftX = (int)screen.RectWork.X;
        var width = (int)screen.RectWork.Width;

        if (!PInvoke.SetWindowPos(handle, HWND.Null, leftX, rect.Y, width, rect.Height,
            SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE))
        {
            Context.API.LogError(ClassName, "Failed to maximize width");
        }
    }

    private static void MakeSmaller(HWND handle, RECT rect)
    {
        // Restore window if window is maximized
        if (PInvoke.IsZoomed(handle))
        {
            if (!PInvoke.ShowWindow(handle, SHOW_WINDOW_CMD.SW_RESTORE))
            {
                Context.API.LogError(ClassName, "Failed to restore window");
            }
            else
            {
                Context.API.LogInfo(ClassName, "Window was restored");
            }
            return;
        }

        var screen = MonitorInfo.GetNearestDisplayMonitor(handle);
        var intervalWidth = (int)Math.Round(screen.RectWork.Width * Settings.SizeInterval / 100.0);
        var intervalHeight = (int)Math.Round(screen.RectWork.Height * Settings.SizeInterval / 100.0);

        var width = rect.Width - intervalWidth;
        var height = rect.Height - intervalHeight;
        width = width < 0 ? 0 : width;
        height = height < 0 ? 0 : height;

        var winX = (int)Math.Round(rect.X + intervalWidth / 2.0);
        var winY = (int)Math.Round(rect.Y + intervalHeight / 2.0);

        if (!PInvoke.SetWindowPos(handle, HWND.Null, winX, winY, width, height,
            SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE))
        {
            Context.API.LogError(ClassName, "Failed to make smaller");
        }
    }

    private static void MakeLarger(HWND handle, RECT rect)
    {
        // Do nothing if window is maximized
        if (PInvoke.IsZoomed(handle))
        {
            Context.API.LogInfo(ClassName, "Window is maximized");
            return;
        }

        var screen = MonitorInfo.GetNearestDisplayMonitor(handle);
        var intervalWidth = (int)Math.Round(screen.RectWork.Width * Settings.SizeInterval / 100.0);
        var intervalHeight = (int)Math.Round(screen.RectWork.Height * Settings.SizeInterval / 100.0);

        var width = rect.Width + intervalWidth;
        var height = rect.Height + intervalHeight;
        width = width > screen.RectWork.Width ? (int)Math.Round(screen.RectWork.Width) : width;
        height = height > screen.RectWork.Height ? (int)Math.Round(screen.RectWork.Height) : height;

        var winX = (int)Math.Round(rect.X - intervalWidth / 2.0);
        var winY = (int)Math.Round(rect.Y - intervalHeight / 2.0);

        if (!PInvoke.SetWindowPos(handle, HWND.Null, winX, winY, width, height,
            SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE))
        {
            Context.API.LogError(ClassName, "Failed to make smaller");
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

        // Must make sure window restored
        if (PInvoke.IsZoomed(handle))
        {
            if (!PInvoke.ShowWindow(handle, SHOW_WINDOW_CMD.SW_RESTORE))
            {
                Context.API.LogError(ClassName, "Failed to restore window");
                return;
            }
            else
            {
                Context.API.LogInfo(ClassName, "Window was restored");
            }
        }

        var screen = MonitorInfo.GetNearestDisplayMonitor(handle);
        var index = screens.IndexOf(screen);
        var prevScreen = index > 0 ? screens[index - 1] : screens[^1];

        if (!PInvoke.SetWindowPos(handle, HWND.Null, (int)prevScreen.RectWork.X, (int)prevScreen.RectWork.Y, rect.Width, rect.Height,
            SET_WINDOW_POS_FLAGS.SWP_NOSIZE | SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE))
        {
            Context.API.LogError(ClassName, "Failed to move to previous screen");
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

        // Must make sure window restored
        if (PInvoke.IsZoomed(handle))
        {
            if (!PInvoke.ShowWindow(handle, SHOW_WINDOW_CMD.SW_RESTORE))
            {
                Context.API.LogError(ClassName, "Failed to restore window");
                return;
            }
            else
            {
                Context.API.LogInfo(ClassName, "Window was restored");
            }
        }

        var screen = MonitorInfo.GetNearestDisplayMonitor(handle);
        var index = screens.IndexOf(screen);
        var nextScreen = index < screens.Count - 1 ? screens[index + 1] : screens[0];

        if (!PInvoke.SetWindowPos(handle, HWND.Null, (int)nextScreen.RectWork.X, (int)nextScreen.RectWork.Y, rect.Width, rect.Height,
            SET_WINDOW_POS_FLAGS.SWP_NOSIZE | SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE))
        {
            Context.API.LogError(ClassName, "Failed to move to next screen");
        }
    }

    private static void TopLeftQuarter(HWND handle)
    {
        // Must make sure window restored
        if (PInvoke.IsZoomed(handle))
        {
            if (!PInvoke.ShowWindow(handle, SHOW_WINDOW_CMD.SW_RESTORE))
            {
                Context.API.LogError(ClassName, "Failed to restore window");
                return;
            }
            else
            {
                Context.API.LogInfo(ClassName, "Window was restored");
            }
        }

        var screen = MonitorInfo.GetNearestDisplayMonitor(handle);
        var width = (int)Math.Round(screen.RectWork.Width / 2.0);
        var height = (int)Math.Round(screen.RectWork.Height / 2.0);
        var topLeftX = (int)screen.RectWork.X;
        var topLeftY = (int)screen.RectWork.Y;

        if (!PInvoke.SetWindowPos(handle, HWND.Null, topLeftX, topLeftY, width, height,
            SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE))
        {
            Context.API.LogError(ClassName, "Failed to move to top left quarter");
        }
    }

    private static void TopRightQuarter(HWND handle)
    {
        // Must make sure window restored
        if (PInvoke.IsZoomed(handle))
        {
            if (!PInvoke.ShowWindow(handle, SHOW_WINDOW_CMD.SW_RESTORE))
            {
                Context.API.LogError(ClassName, "Failed to restore window");
                return;
            }
            else
            {
                Context.API.LogInfo(ClassName, "Window was restored");
            }
        }

        var screen = MonitorInfo.GetNearestDisplayMonitor(handle);
        var width = (int)Math.Round(screen.RectWork.Width / 2.0);
        var height = (int)Math.Round(screen.RectWork.Height / 2.0);
        var topRightX = (int)screen.RectWork.Right - width;
        var topRightY = (int)screen.RectWork.Y;

        if (!PInvoke.SetWindowPos(handle, HWND.Null, topRightX, topRightY, width, height,
            SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE))
        {
            Context.API.LogError(ClassName, "Failed to move to top right quarter");
        }
    }

    private static void BottomLeftQuarter(HWND handle)
    {
        // Must make sure window restored
        if (PInvoke.IsZoomed(handle))
        {
            if (!PInvoke.ShowWindow(handle, SHOW_WINDOW_CMD.SW_RESTORE))
            {
                Context.API.LogError(ClassName, "Failed to restore window");
                return;
            }
            else
            {
                Context.API.LogInfo(ClassName, "Window was restored");
            }
        }

        var screen = MonitorInfo.GetNearestDisplayMonitor(handle);
        var width = (int)Math.Round(screen.RectWork.Width / 2.0);
        var height = (int)Math.Round(screen.RectWork.Height / 2.0);
        var bottomLeftX = (int)screen.RectWork.X;
        var bottomLeftY = (int)screen.RectWork.Bottom - height;

        if (!PInvoke.SetWindowPos(handle, HWND.Null, bottomLeftX, bottomLeftY, width, height,
            SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE))
        {
            Context.API.LogError(ClassName, "Failed to move to bottom left quarter");
        }
    }

    private static void BottomRightQuarter(HWND handle)
    {
        // Must make sure window restored
        if (PInvoke.IsZoomed(handle))
        {
            if (!PInvoke.ShowWindow(handle, SHOW_WINDOW_CMD.SW_RESTORE))
            {
                Context.API.LogError(ClassName, "Failed to restore window");
                return;
            }
            else
            {
                Context.API.LogInfo(ClassName, "Window was restored");
            }
        }

        var screen = MonitorInfo.GetNearestDisplayMonitor(handle);
        var width = (int)Math.Round(screen.RectWork.Width / 2.0);
        var height = (int)Math.Round(screen.RectWork.Height / 2.0);
        var bottomRightX = (int)screen.RectWork.Right - width;
        var bottomRightY = (int)screen.RectWork.Bottom - height;

        if (!PInvoke.SetWindowPos(handle, HWND.Null, bottomRightX, bottomRightY, width, height,
            SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE))
        {
            Context.API.LogError(ClassName, "Failed to move to bottom right quarter");
        }
    }

    private static void LeftHalf(HWND handle)
    {
        // Must make sure window restored
        if (PInvoke.IsZoomed(handle))
        {
            if (!PInvoke.ShowWindow(handle, SHOW_WINDOW_CMD.SW_RESTORE))
            {
                Context.API.LogError(ClassName, "Failed to restore window");
                return;
            }
            else
            {
                Context.API.LogInfo(ClassName, "Window was restored");
            }
        }

        var screen = MonitorInfo.GetNearestDisplayMonitor(handle);
        var width = (int)Math.Round(screen.RectWork.Width / 2.0);
        var height = (int)Math.Round(screen.RectWork.Height);
        var leftX = (int)screen.RectWork.X;
        var topY = (int)screen.RectWork.Y;

        if (!PInvoke.SetWindowPos(handle, HWND.Null, leftX, topY, width, height,
            SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE))
        {
            Context.API.LogError(ClassName, "Failed to move to left half");
        }
    }

    private static void RightHalf(HWND handle)
    {
        // Must make sure window restored
        if (PInvoke.IsZoomed(handle))
        {
            if (!PInvoke.ShowWindow(handle, SHOW_WINDOW_CMD.SW_RESTORE))
            {
                Context.API.LogError(ClassName, "Failed to restore window");
                return;
            }
            else
            {
                Context.API.LogInfo(ClassName, "Window was restored");
            }
        }

        var screen = MonitorInfo.GetNearestDisplayMonitor(handle);
        var width = (int)Math.Round(screen.RectWork.Width / 2.0);
        var height = (int)Math.Round(screen.RectWork.Height);
        var rightX = (int)screen.RectWork.Right - width;
        var topY = (int)screen.RectWork.Y;

        if (!PInvoke.SetWindowPos(handle, HWND.Null, rightX, topY, width, height,
            SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE))
        {
            Context.API.LogError(ClassName, "Failed to move to right half");
        }
    }

    private static void TopHalf(HWND handle)
    {
        // Must make sure window restored
        if (PInvoke.IsZoomed(handle))
        {
            if (!PInvoke.ShowWindow(handle, SHOW_WINDOW_CMD.SW_RESTORE))
            {
                Context.API.LogError(ClassName, "Failed to restore window");
                return;
            }
            else
            {
                Context.API.LogInfo(ClassName, "Window was restored");
            }
        }

        var screen = MonitorInfo.GetNearestDisplayMonitor(handle);
        var width = (int)Math.Round(screen.RectWork.Width);
        var height = (int)Math.Round(screen.RectWork.Height / 2.0);
        var leftX = (int)screen.RectWork.X;
        var topY = (int)screen.RectWork.Y;

        if (!PInvoke.SetWindowPos(handle, HWND.Null, leftX, topY, width, height,
            SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE))
        {
            Context.API.LogError(ClassName, "Failed to move to top half");
        }
    }

    private static void BottomHalf(HWND handle)
    {
        // Must make sure window restored
        if (PInvoke.IsZoomed(handle))
        {
            if (!PInvoke.ShowWindow(handle, SHOW_WINDOW_CMD.SW_RESTORE))
            {
                Context.API.LogError(ClassName, "Failed to restore window");
                return;
            }
            else
            {
                Context.API.LogInfo(ClassName, "Window was restored");
            }
        }

        var screen = MonitorInfo.GetNearestDisplayMonitor(handle);
        var width = (int)Math.Round(screen.RectWork.Width);
        var height = (int)Math.Round(screen.RectWork.Height / 2.0);
        var leftX = (int)screen.RectWork.X;
        var bottomY = (int)screen.RectWork.Bottom - height;

        if (!PInvoke.SetWindowPos(handle, HWND.Null, leftX, bottomY, width, height,
            SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE))
        {
            Context.API.LogError(ClassName, "Failed to move to bottom half");
        }
    }

    private static void ReasonableSize(HWND handle, RECT rect)
    {
        // Do nothing if window is maximized
        if (PInvoke.IsZoomed(handle))
        {
            Context.API.LogInfo(ClassName, "Window is maximized");
            return;
        }

        var screen = MonitorInfo.GetNearestDisplayMonitor(handle);
        var width = (int)Math.Round(screen.RectWork.Width * Settings.ReasonableSizeWidth / 100.0);
        var height = (int)Math.Round(screen.RectWork.Height * Settings.ReasonableSizeHeight / 100.0);
        var winX = (int)Math.Round(rect.X + (rect.Width - width) / 2.0);
        var winY = (int)Math.Round(rect.Y + (rect.Height - height) / 2.0);

        if (!PInvoke.SetWindowPos(handle, HWND.Null, winX, winY, width, height,
            SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE))
        {
            Context.API.LogError(ClassName, "Failed to set reasonable size");
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
        return new SettingsPanel(Settings);
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
