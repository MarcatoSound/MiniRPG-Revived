using System;
using System.IO;
using System.Runtime.InteropServices;

namespace BasicRPGTest_Mono
{
    public static class Program
    {

        [STAThread]
        static void Main()
        {

            using (var game = new Main())
                game.Run();
        }
    }
}
