using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Flow.Launcher.Plugin.WindowManager;

public class Settings
{
    public void RestoreToDefault()
    {
        var defaultSettings = new Settings();
        var type = GetType();
        var props = type.GetProperties();
        foreach (var prop in props)
        {
            if (CheckJsonIgnoredOrKeyAttribute(prop))
            {
                continue;
            }
            var defaultValue = prop.GetValue(defaultSettings);
            prop.SetValue(this, defaultValue);
        }
    }

    public override string ToString()
    {
        var type = GetType();
        var props = type.GetProperties();
        var s = props.Aggregate(
            "Settings(\n",
            (current, prop) =>
            {
                if (CheckJsonIgnoredOrKeyAttribute(prop))
                {
                    return current;
                }
                return current + $"\t{prop.Name}: {prop.GetValue(this)}\n";
            }
        );
        s += ")";
        return s;
    }

    private static bool CheckJsonIgnoredOrKeyAttribute(PropertyInfo prop)
    {
        return
            // JsonIgnored
            prop.GetCustomAttribute<JsonIgnoreAttribute>() != null;
    }
}
