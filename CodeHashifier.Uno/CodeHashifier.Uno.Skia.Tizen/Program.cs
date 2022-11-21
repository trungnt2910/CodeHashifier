using Tizen.Applications;
using Uno.UI.Runtime.Skia;

namespace CodeHashifier.Uno.Skia.Tizen
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var host = new TizenHost(() => new CodeHashifier.Uno.App());
            host.Run();
        }
    }
}
