using Crimson.Core;
using Crimson.Engine;
using Crimson.Engine.Tests;
using Version = Crimson.Core.Version;

Logger.EnableConsole();

AppOptions options = new AppOptions("Crimson.Engine.Tests", new Version(1, 0));
App.Run(in options, new TestScene());