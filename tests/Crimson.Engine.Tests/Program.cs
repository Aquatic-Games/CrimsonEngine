using Crimson.Core;
using Crimson.Engine;
using Crimson.Engine.Tests;

Logger.LogToConsole = true;
Logger.OutputTraceLogs = true;

AppOptions options = new AppOptions("Crimson.Engine.Tests", "1.0.0"/*, AppType.Type2D*/);
options.Window.Resizable = true;
//options.Window.FullScreen = true;
options.UI.DefaultFont = "NotoSansJP-Regular";

App.Run(in options, new TestScene());
//App.Run(in options, new Test2D());
//App.Run(in options, new TestDDS());
//App.Run(in options, new TestAudio());