using System.Collections;
using System.Numerics;
using System.Text;
using Crimson.Math;

namespace Crimson.Data;

public class QuickConfig
{
    private Dictionary<string, object> _objectDict;

    public QuickConfig()
    {
        _objectDict = [];
    }

    public void SetOption(string name, string value)
    {
        _objectDict[name] = value;
    }

    public void SetOption(string name, double value)
    {
        _objectDict[name] = value;
    }

    public void SetOption(string name, bool value)
    {
        _objectDict[name] = value;
    }

    public void SetOption(string name, Enum value)
    {
        _objectDict[name] = value;
    }

    public void SetOption<T>(string name, T[] value)
    {
        _objectDict[name] = value;
    }

    public void SetOption<T>(string name, Size<T> size) where T : INumber<T>
        => SetOption<T>(name, [size.Width, size.Height]);

    public string? GetString(string name)
    {
        if (_objectDict.TryGetValue(name, out object value))
            return (string) value;

        return null;
    }
    
    public string? GetString(string name, int index)
    {
        if (_objectDict.TryGetValue(name, out object value))
            return (string) ((object[]) value)[index];

        return null;
    }

    public double? GetDouble(string name)
    {
        if (_objectDict.TryGetValue(name, out object value))
            return (double) value;

        return null;
    }
    
    public double? GetDouble(string name, int index)
    {
        if (_objectDict.TryGetValue(name, out object value))
            return (double) ((object[]) value)[index];

        return null;
    }

    public bool? GetBool(string name)
    {
        if (_objectDict.TryGetValue(name, out object value))
            return (bool) value;

        return null;
    }

    public bool? GetBool(string name, int index)
    {
        if (_objectDict.TryGetValue(name, out object value))
            return (bool) ((object[]) value)[index];

        return null;
    }

    public T? GetEnum<T>(string name) where T : struct
    {
        if (!_objectDict.TryGetValue(name, out object value))
            return null;
        
        return Enum.Parse<T>((string) value);
    }
    
    public T? GetEnum<T>(string name, int index) where T : struct
    {
        if (!_objectDict.TryGetValue(name, out object value))
            return null;
        
        return Enum.Parse<T>((string) ((object[]) value)[index]);
    }

    public Size<T>? GetSize<T>(string name) where T : INumber<T>
    {
        double? width = GetDouble(name, 0);
        double? height = GetDouble(name, 1);

        if (width is { } w && height is { } h)
            return new Size<T>(T.CreateChecked(w), T.CreateChecked(h));

        return null;
    }

    public string Serialize()
    {
        StringBuilder builder = new StringBuilder();
        
        foreach ((string key, object value) in _objectDict)
        {
            builder.Append(key);
            builder.Append('=');

            AppendToStringBuilder(builder, value);

            builder.AppendLine();
        }

        return builder.ToString();
    }

    public override string ToString()
        => Serialize();

    public static QuickConfig Deserialize(string text)
    {
        QuickConfig config = new QuickConfig();

        using StringReader reader = new StringReader(text);

        int lineNum = 0;
        StringBuilder currentValue = new StringBuilder();
        List<object> objList = [];
        
        for (string line; (line = reader.ReadLine()!) != null;)
        {
            lineNum++;
            
            line = line.Trim();
            
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
                continue;
            
            int equalsPos = line.IndexOf('=');

            if (equalsPos == -1)
                throw new Exception($"Malformed text at line {lineNum}. Expected '=', was not found.");

            string key = line[..equalsPos].Trim();
            string value = line[(equalsPos + 1)..].Trim();
            
            bool inString = false;
            objList.Clear();
            currentValue.Clear();
            
            foreach (char c in value)
            {
                switch (c)
                {
                    case '"':
                        inString = !inString;
                        break;
                    
                    case ',' when !inString:
                        objList.Add(currentValue.ToString());
                        currentValue.Clear();
                        break;
                    
                    default:
                        currentValue.Append(c);
                        break;
                }
            }
            
            objList.Add(currentValue.ToString());

            for (int i = 0; i < objList.Count; i++)
            {
                string strValue = (string) objList[i];
                object objValue;

                if (double.TryParse(strValue, out double d))
                    objValue = d;
                else if (bool.TryParse(strValue, out bool b))
                    objValue = b;
                else
                    objValue = strValue;

                objList[i] = objValue;
            }

            if (objList.Count == 1)
                config._objectDict[key] = objList[0];
            else
                config._objectDict[key] = objList.ToArray();
        }
        
        return config;
    }

    private static void AppendToStringBuilder(StringBuilder builder, object? value)
    {
        if (value == null)
            return;
        
        switch (value)
        {
            case string str:
            {
                builder.Append('"');
                builder.Append(str);
                builder.Append('"');
                break;
            }

            case Enum e:
                builder.Append(e);
                break;
                
            case sbyte:
            case byte:
            case short:
            case ushort:
            case int:
            case uint:
            case long:
            case ulong:
            case float:
            case double:
                builder.Append(value);
                break;
                
            case bool b:
                builder.Append(b ? "true" : "false");
                break;

            case Array arr:
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    AppendToStringBuilder(builder, arr.GetValue(i));

                    if (i < arr.Length - 1)
                        builder.Append(',');
                }

                break;
            }
            
            default:
                throw new ArgumentException($"Value is of unsupported type '{value.GetType()}'.");
        }
    }
}