using Euphoria.Core;
using Euphoria.Engine;
using Version = Euphoria.Core.Version;

Logger.UseConsole();

AppOptions options = new AppOptions("Euphoria.Engine.Tests", new Version(1, 0));
App.Run(in options);