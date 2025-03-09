namespace Euphoria.Core;

/// <summary>
/// Represents a semver-compatible version.
/// </summary>
public struct Version
{
    private string _versionString;

    /// <summary>
    /// Create a <see cref="Version"/> from a version string.
    /// </summary>
    /// <param name="versionString">The string to use.</param>
    public Version(string versionString)
    {
        _versionString = versionString;
    }

    /// <summary>
    /// Create a <see cref="Version"/> from a major, minor, and patch version.
    /// </summary>
    /// <param name="major">The major version.</param>
    /// <param name="minor">The minor version.</param>
    /// <param name="patch">The patch version.</param>
    public Version(uint major, uint minor, uint patch = 0)
    {
        _versionString = $"{major}.{minor}.{patch}";
    }

    /// <summary>
    /// Get the version string.
    /// </summary>
    /// <returns>The version string.</returns>
    public override string ToString()
    {
        return _versionString;
    }
}