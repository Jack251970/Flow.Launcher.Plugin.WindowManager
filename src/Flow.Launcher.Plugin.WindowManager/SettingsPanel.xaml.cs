using System.Windows.Controls;

namespace Flow.Launcher.Plugin.WindowManager;

public partial class SettingsPanel : UserControl
{
    public Settings Settings { get; }

    public SettingsPanel(Settings settings)
    {
        Settings = settings;
        DataContext = settings;
        InitializeComponent();
    }
}
