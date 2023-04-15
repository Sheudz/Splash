using System.Diagnostics;
using System.Runtime.InteropServices;

class Program{
     [Flags]
    public enum ThreadAccess : int
    {
      TERMINATE = (0x0001),
      SUSPEND_RESUME = (0x0002),
      GET_CONTEXT = (0x0008),
      SET_CONTEXT = (0x0010),
      SET_INFORMATION = (0x0020),
      QUERY_INFORMATION = (0x0040),
      SET_THREAD_TOKEN = (0x0080),
      IMPERSONATE = (0x0100),
      DIRECT_IMPERSONATION = (0x0200)
    }
    static void Main(string[] args)
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);
        [DllImport("kernel32.dll")]
        static extern uint SuspendThread(IntPtr hThread);
        [DllImport("kernel32.dll")]
        static extern int ResumeThread(IntPtr hThread);
        [DllImport("kernel32", CharSet = CharSet.Auto,SetLastError = true)]
        static extern bool CloseHandle(IntPtr handle);

        static void SuspendProcess(int pid)
        {
            var process = Process.GetProcessById(pid);

            foreach (ProcessThread pT in process.Threads)
            {
                IntPtr pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);

                if (pOpenThread == IntPtr.Zero)
                {
                continue;
                }

                SuspendThread(pOpenThread);

                CloseHandle(pOpenThread);
            }
        }

        static void ResumeProcess(int pid)
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

        void cmd(string command){
            System.Diagnostics.Process.Start("cmd.exe", "/C " + command);
        }
        cmd("cls");
        Console.Title = "Splash";
        void splash(){
            Thread.Sleep(300);
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
        splash();
        while (true)
        {
            Console.Write(">:>$>: ");
            string? caa = Console.ReadLine();
            string?[] cmnds = caa.Split("&");
            foreach (string? comnd in cmnds)
            {
                string?[] caas = comnd.Split(" ");
                string? cmnd = null;
                string? a1 = null;
                string? a2 = null;
                try
                {
                    cmnd = caas[0];
                    a1 = caas[1];
                    a2 = caas[2];
                }
                catch{}
                if (cmnd == "help")
                {
                    Console.WriteLine("help - show help");
                    Console.WriteLine("clear or cls - cleans the console");
                    Console.WriteLine("kill <process.name> or <process.pid>- kill process");
                    Console.WriteLine("suspend <process.name> or <process.pid> - suspend process");
                    Console.WriteLine("unsuspend <process.name> or <process.pid> - unsuspend process");
                    Console.WriteLine("ps - write process list");
                    Console.WriteLine("find <process.name> or <process.pid> - find process by pid or name");
                }
                else if (cmnd == "clear")
                {
                    cmd("cls");
                    splash();
                }
                else if (cmnd == "cls")
                {
                    cmd("cls");
                    splash();
                }
                else if (cmnd == "kill")
                {   
                    if (a1 != null)
                    {
                        bool fon = false;
                        foreach (var proc in Process.GetProcesses())
                        {
                            if (proc.Id.ToString() == a1)
                            {
                                proc.Kill();
                                Console.WriteLine(proc.Id.ToString() + ":::" + proc.ProcessName + " killed");
                                fon = true;
                            }
                            if (proc.ProcessName == a1)
                            {
                                proc.Kill();
                                Console.WriteLine(proc.Id.ToString() + ":::" + proc.ProcessName + " killed");
                                fon = true;
                            }
                        }
                        if (fon == false)
                        {
                            Console.WriteLine(a1 + " not found");
                        }
                        Console.Write("\n");
                    }
                    else
                    {
                        Console.WriteLine("You didn't pass an argument, use the `kill` command like this: `kill <process.name>` or `kill <process.pid>\n");
                    }
                }
                else if (cmnd == "suspend")
                {   
                    if (a1 != null)
                    {
                        bool fon = false;
                        foreach (var proc in Process.GetProcesses())
                        {
                            if (proc.Id.ToString() == a1)
                            {
                                SuspendProcess(Int32.Parse(a1));
                                Console.WriteLine(proc.Id.ToString() + ":::" + proc.ProcessName + " suspended");
                                fon = true;
                            }
                            if (proc.ProcessName == a1)
                            {
                                SuspendProcess(proc.Id);
                                Console.WriteLine(proc.Id.ToString() + ":::" + proc.ProcessName + " suspended");
                                fon = true;
                            }
                        }
                        if (fon == false)
                        {
                            Console.WriteLine(a1 + " not found");
                        }
                        Console.Write("\n");
                    }

                    else
                    {
                        Console.WriteLine("You didn't pass an argument, use the `suspend` command like this: `suspend <process.name>` or `suspend <process.pid>\n");
                    }
                }
                else if (cmnd == "unsuspend")
                {   
                    if (a1 != null)
                    {
                        bool fon = false;
                        foreach (var proc in Process.GetProcesses())
                        {
                            if (proc.Id.ToString() == a1)
                            {
                                ResumeProcess(Int32.Parse(a1));
                                Console.WriteLine(proc.Id.ToString() + ":::" + proc.ProcessName + " unsuspended");
                                fon = true;                        
                            }
                            if (proc.ProcessName == a1)
                            {
                                ResumeProcess(proc.Id);
                                Console.WriteLine(proc.Id.ToString() + ":::" + proc.ProcessName + " unsuspended");
                                fon = true;
                            }
                        }
                        if (fon == false)
                        {
                            Console.WriteLine(a1 + " not found");
                        }
                        Console.Write("\n");
                    }
                    else
                    {
                        Console.WriteLine("You didn't pass an argument, use the `unsuspend` command like this: `unsuspend <process.name>` or `unsuspend <process.pid> \n");
                    }
                }
                else if (cmnd == "ps")
                {
                    var processes = Process.GetProcesses();

					foreach (var process in processes)
					{
					    var isSuspended = false;

					    foreach (ProcessThread thread in process.Threads)
					    {
					        if (thread.ThreadState == System.Diagnostics.ThreadState.Wait &&
					            thread.WaitReason == ThreadWaitReason.Suspended)
					        {
					            isSuspended = true;
					            break;
					        }
					    }

					    if (isSuspended)
					    {
					        Console.WriteLine($"{process.Id}:::{process.ProcessName}:::Suspended");
					    }
					    else
					    {
					        Console.WriteLine($"{process.Id}:::{process.ProcessName}:::Working");
					    }
					}
					Console.Write("\n");
                }
                else if (cmnd == "find")
                {
                    var processes = Process.GetProcesses();

					foreach (var process in processes)
					{
					    var isSuspended = false;

					    foreach (ProcessThread thread in process.Threads)
					    {
					        if (thread.ThreadState == System.Diagnostics.ThreadState.Wait &&
					            thread.WaitReason == ThreadWaitReason.Suspended)
					        {
					            isSuspended = true;
					            break;
					        }
					    }

					    if (isSuspended)
						{
						    int id;
						    if (int.TryParse(a1, out id) && process.Id == id || process.ProcessName == a1)
						    {
						        Console.WriteLine($"{process.Id}:::{process.ProcessName}:::Suspended");
						    }
						}
						else
						{
						    int id;
						    if (int.TryParse(a1, out id) && process.Id == id || process.ProcessName == a1)
						    {
						        Console.WriteLine($"{process.Id}:::{process.ProcessName}:::Working");
						    }
						}
						
					}
					Console.Write("\n");
                }
                else if (cmnd == "")
                {

                }
                else 
                {
                    Console.WriteLine(cmnd + " is an unknown command, if you need help type 'help' \n");
                }
            }
        }
    }
}