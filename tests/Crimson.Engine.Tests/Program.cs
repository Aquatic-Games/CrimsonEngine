using Crimson.Core;
using Crimson.Engine;
using Crimson.Engine.Tests;
using Crimson.Graphics;
using Crimson.Platform;
using Version = Crimson.Core.Version;

Logger.EnableConsole();

AppOptions options = new AppOptions("Crimson.Engine.Tests", new Version(1, 0)/*, AppType.Type2D*/);
options.Window.Resizable = true;
//options.Window.FullScreen = true;
options.UI.DefaultFont = "NotoSansJP-Regular";

App.Run(in options, new TestScene());
//App.Run(in options, new Test2D());