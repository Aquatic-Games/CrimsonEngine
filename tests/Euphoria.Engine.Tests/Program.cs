using Euphoria.Core;
using Euphoria.Engine;
using Euphoria.Engine.Launch;

Logger.EnableConsole();

LaunchInfo info = LaunchInfo.Default("EuphoriaTest", new Version(1, 0, 0));

App app = new App(in info);
app.Run();