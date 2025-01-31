using Euphoria.Engine;
using Euphoria.Engine.Launch;

LaunchInfo info = LaunchInfo.Default("EuphoriaTest", new Version(1, 0, 0));

App app = new App(in info);
app.Run();