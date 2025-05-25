using System.Windows;
using System.Windows.Interop;
using Windows.Win32.Foundation;

namespace Flow.Launcher.Plugin.WindowManager.Helpers;

public class Win32Helper
{
    #region Window Handle

    internal static HWND GetWindowHandle(Window window, bool ensure = false)
    {
        var windowHelper = new WindowInteropHelper(window);
        if (ensure)
        {
            windowHelper.EnsureHandle();
        }
        return new(windowHelper.Handle);
    }

    #endregion
}
