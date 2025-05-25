using System;
using System.Threading.Tasks;

namespace Flow.Launcher.Plugin.WindowManager.Models;

public class Command
{
    public required CommandType Type { get; init; }

    public required string TitleKey { get; init; }

    public required string SubtitleKey { get; init; }

    public required string IcoPath { get; init; }

    public required Func<Task> CommandAction { get; init; }

    public required string Keyword { get; set; }
}

public enum CommandType
{
    LeftTop,
    Center,
    Maximize,
    Minimize,
    Restore,
    MoveUp,
    MoveDown,
    MoveLeft,
    MoveRight,
    MaximizeHeight,
    MaximizeWidth,
    MakeSmaller,
    MakeLarger,
    PreviousScreen,
    NextScreen,
    TopLeftQuarter,
    TopRightQuarter,
    BottomLeftQuarter,
    BottomRightQuarter,
    LeftHalf,
    RightHalf,
    TopHalf,
    BottomHalf
}
