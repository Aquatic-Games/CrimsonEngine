using Crimson.Content;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Crimson.UI;

[YamlSerializable]
public struct Locale : IContentResource<Locale>
{
    /// <summary>
    /// The ID.
    /// </summary>
    public string ID;

    /// <summary>
    /// The display name.
    /// </summary>
    public string Name;

    /// <summary>
    /// The list of strings present in this locale.
    /// </summary>
    public Dictionary<string, string> Strings;

    /// <summary>
    /// Create a locale.
    /// </summary>
    /// <param name="id">The ID.</param>
    /// <param name="name">The display name.</param>
    /// <param name="strings">The list of strings present in this locale.</param>
    public Locale(string id, string name, Dictionary<string, string> strings)
    {
        ID = id;
        Name = name;
        Strings = strings;
    }

    /// <summary>
    /// Get a string from the locale. If the string is not found in <see cref="Strings"/>, the value passed in will be
    /// returned.
    /// </summary>
    /// <param name="str">The string ID.</param>
    /// <param name="args">Arguments to pass into a formatted string.</param>
    /// <returns>The localized string, if it exists.</returns>
    public string GetString(string str, params ReadOnlySpan<object?> args)
        => string.Format(Strings.GetValueOrDefault(str, str), args);

    private static Dictionary<string, Locale> _locales;

    public static IReadOnlyDictionary<string, Locale> AvailableLocales => _locales;

    public static Locale CurrentLocale;

    static Locale()
    {
        _locales = [];
        CurrentLocale = None;
    }
    
    /// <summary>
    /// Get a currently loaded <see cref="Locale"/>.
    /// </summary>
    /// <param name="id">The locale ID.</param>
    /// <returns>The loaded locale.</returns>
    public static Locale GetLocale(string id)
        => _locales[id];

    /// <summary>
    /// Set the current locale.
    /// </summary>
    /// <param name="id">The locale ID.</param>
    public static void SetCurrentLocale(string id)
        => CurrentLocale = _locales[id];

    /// <summary>
    /// Gets a string from the current locale. If the string is not found in <see cref="Strings"/>, the value passed in
    /// will be returned.
    /// </summary>
    /// <param name="str">The string ID.</param>
    /// <param name="args">Arguments to pass into a formatted string.</param>
    /// <returns>The localized string, if it exists.</returns>
    /// <remarks>This is a shortcut to <see cref="CurrentLocale"/>.<see cref="GetString"/>. </remarks>
    public static string GetLocalizedString(string str, params ReadOnlySpan<object?> args)
        => CurrentLocale.GetString(str, args);

    public static void LoadLocalesFromDirectory(string dir)
    {
        string[] files = Content.Content.GetContentFiles(dir, "*.loc");
        foreach (string file in files)
        {
            Locale locale = LoadResource(file, true);
            _locales.Add(locale.ID, locale);
        }
    }

    /// <summary>
    /// No locale - a pass through. Allows engine systems to use the localization system, but without the application
    /// needing to provide a default locale.
    /// This value is not present in <see cref="AvailableLocales"/>.
    /// </summary>
    public static Locale None => new Locale("none", "None", []);

    public static Locale LoadResource(string fullPath, bool hasExtension)
    {
        LocalizationContext context = new LocalizationContext();
        IDeserializer deserializer = new StaticDeserializerBuilder(context)
            .WithNamingConvention(PascalCaseNamingConvention.Instance)
            .WithCaseInsensitivePropertyMatching()
            .Build();

        string yaml;

        if (hasExtension)
            yaml = File.ReadAllText(fullPath);
        else
        {
            fullPath = Path.ChangeExtension(fullPath, "loc");
            if (File.Exists(fullPath))
                yaml = File.ReadAllText(fullPath);

            throw new FileNotFoundException();
        }
        
        return deserializer.Deserialize<Locale>(yaml);
    }
}