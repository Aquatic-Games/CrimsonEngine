namespace Euphoria.Core;

public struct Version
{
    private string _versionString;

    public Version(string versionString)
    {
        _versionString = versionString;
    }

    public Version(uint major, uint minor, uint patch = 0)
    {
        _versionString = $"{major}.{minor}.{patch}";
    }

    public override string ToString()
    {
        return _versionString;
    }
}