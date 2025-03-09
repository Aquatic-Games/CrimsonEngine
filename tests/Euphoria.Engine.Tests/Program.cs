using Euphoria.Core;
using Euphoria.Engine;
using Euphoria.Engine.Tests;
using Version = Euphoria.Core.Version;

Logger.EnableConsole();

AppOptions options = new AppOptions("Euphoria.Engine.Tests", new Version(1, 0));
App.Run(in options, new TestApp());