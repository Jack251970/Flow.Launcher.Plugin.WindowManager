using Flow.Launcher.Plugin.WindowManager.Models;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Flow.Launcher.Plugin.WindowManager.Test;

public partial class MainWindow : Window
{
    private readonly WindowManager windowManager = new();
    public bool Visible { get; set; } = true;

    public MainWindow()
    {
        InitializeComponent();
        TestHelper.InitPlugin(windowManager, this);
        var results = TestHelper.Query(windowManager);
        foreach (var result in results)
        {
            if (result.ContextData is Command command)
            {
                var item = new Button
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Height = 30,
                    Content = command.Keyword,
                    Margin = new Thickness(4)
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
}
