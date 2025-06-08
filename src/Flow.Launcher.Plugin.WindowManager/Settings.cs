using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Flow.Launcher.Plugin.WindowManager;

public class Settings : BaseModel
{
    private double _sizeInterval = 10;
    public double SizeInterval
    {
        get => _sizeInterval;
        set
        {
            if ((value > 0 || value < 100) && _sizeInterval != value)
            {
                _sizeInterval = value;
                OnPropertyChanged();
            }
        }
    }

    private double _reasonableSizeWidth = 85;
    public double ReasonableSizeWidth
    {
        get => _reasonableSizeWidth;
        set
        {
            if ((value > 0 || value < 100) && _reasonableSizeWidth != value)
            {
                _reasonableSizeWidth = value;
                OnPropertyChanged();
            }
        }
    }

    private double _reasonableSizeHeight = 85;
    public double ReasonableSizeHeight
    {
        get => _reasonableSizeHeight;
        set
        {
            if ((value > 0 || value < 100) && _reasonableSizeHeight != value)
            {
                _reasonableSizeHeight = value;
                OnPropertyChanged();
            }
        }
    }

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
