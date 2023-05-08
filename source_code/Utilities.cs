using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Splash.Native;

namespace Splash
{
    internal class Utilities
    {
        internal static void SplashScreen()
        {
            Console.Write("\n");
            Console.WriteLine("#######################################################################################################");
            Console.WriteLine("#  _####            _#######=_       ##                     ####                _####         ##      #");
            Console.WriteLine("# ##-   #           #        #       ##                    ##  ##              ##-   #        ##      #");
            Console.WriteLine("# ##                #        #       ##                   ##    ##             ##             #####   #");
            Console.WriteLine("#   ###=_           #========J       ##                  ##      ##             ###=_         ######  #");
            Console.WriteLine("#       ##          ##               ##                 ##========##                ##        ##   ## #");
            Console.WriteLine("#       ##          #                ###               ##          ##               ##        ##   ## #");
            Console.WriteLine("#  ######           #                #######          ##            ##         ######         ##   ## #");
            Console.WriteLine("####################################################################################################### \n");
        }

        internal static void SuspendProcess(int pid)
        {
            var process = Process.GetProcessById(pid);

            foreach (ProcessThread pT in process.Threads)
            {
                IntPtr openThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);

                if (openThread == IntPtr.Zero)
                {
                    continue;
                }

                SuspendThread(openThread);

                CloseHandle(openThread);
            }
        }

        internal static void ResumeProcess(int pid)
        {
            var process = Process.GetProcessById(pid);

            if (process.ProcessName == string.Empty)
                return;

            foreach (ProcessThread pT in process.Threads)
            {
                IntPtr pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);

                if (pOpenThread == IntPtr.Zero)
                {
                    continue;
                }

                var suspendCount = 0;
                do
                {
                    suspendCount = ResumeThread(pOpenThread);
                } while (suspendCount > 0);

                CloseHandle(pOpenThread);
            }
        }
    }
}
