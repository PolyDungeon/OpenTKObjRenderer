using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Xamarin.Forms;

namespace LearnOpenTK
{
    public static class Program
    {
        private static void Main()
        {

            //string deviceModel = Device.CurrentDevice.Model;
            OperatingSystem os = new OperatingSystem(System.Environment.OSVersion.Platform, new Version());
            Console.WriteLine(os.ToString());
            bool Windows = true;
            bool Android = false;
            bool IOS = false;

            if (Windows)
            {
                var nativeWindowSettings = new NativeWindowSettings()
                {
                    Size = new Vector2i(800, 600),
                    Title = "VertexDungeon",
                    // This is needed to run on macos
                    Flags = ContextFlags.ForwardCompatible,
                };

                using (var window = new Window(GameWindowSettings.Default, nativeWindowSettings))
                {
                    window.Run();
                }
            }

        }
    }
}