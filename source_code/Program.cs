using Splash;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Runtime.InteropServices;
using static Splash.Native;
class Program
{

    private static Dictionary<string, Action<string, string>> _commandMap = new Dictionary<string, Action<string, string>>()
    {
        ["help"] = DisplayHelpMessage,
        ["cls"] = ClearConsole,
        ["clear"] = ClearConsole,
        ["kill"] = KillProcess,
        ["suspend"] = SuspendProcess,
        ["unsuspend"] = UnsuspendProcess,
        ["ps"] = ListProcesses,
        ["find"] = FindProcess
    };

    
    public static void Main(string[] args)
    {
        Console.Title = "Splash";

        Utilities.SplashScreen();
        while (true)
        {
            Console.Write(">:>$>: ");
            string? input = Console.ReadLine();
            string?[] commands = input.Split("&");
            foreach (string? commandRaw in commands)
            {
                string?[] commandParts = commandRaw.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                string? command = null;
                string? argument1 = null;
                string? argument2 = null;
                try
                {
                    command = commandParts[0];
                    argument1 = commandParts[1];
                    argument2 = commandParts[2];
                }
                catch { }
                if (!_commandMap.TryGetValue(command, out var commandMethod))
                {
                    Console.WriteLine(command + " is an unknown command, if you need help type 'help' \n");
                }
                else
                {
                    commandMethod(argument1, argument2);
                }
            }
        }

    }

    private static void FindProcess(string argument1, string _)
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
                if (int.TryParse(argument1, out id) && process.Id == id || process.ProcessName == argument1)
                {
                    Console.WriteLine($"{process.Id}:::{process.ProcessName}:::Suspended");
                }
            }
            else
            {
                int id;
                if (int.TryParse(argument1, out id) && process.Id == id || process.ProcessName == argument1)
                {
                    Console.WriteLine($"{process.Id}:::{process.ProcessName}:::Working");
                }
            }

        }
        Console.Write("\n");
    }


    private static void ClearConsole(string _, string __)
    {
        Console.Clear();
        Utilities.SplashScreen();
    }
    private static void ListProcesses(string _, string __)
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

    private static void UnsuspendProcess(string argument1, string _)
    {
        if (argument1 != null)
        {
            bool fon = false;
            foreach (var proc in Process.GetProcesses())
            {
                if (proc.Id.ToString() == argument1)
                {
                    Utilities.ResumeProcess(Int32.Parse(argument1));
                    Console.WriteLine(proc.Id.ToString() + ":::" + proc.ProcessName + " unsuspended");
                    fon = true;
                }
                if (proc.ProcessName == argument1)
                {
                    Utilities.ResumeProcess(proc.Id);
                    Console.WriteLine(proc.Id.ToString() + ":::" + proc.ProcessName + " unsuspended");
                    fon = true;
                }
            }
            if (fon == false)
            {
                Console.WriteLine(argument1 + " not found");
            }
            Console.Write("\n");
        }
        else
        {
            Console.WriteLine("You didn't pass an argument, use the `unsuspend` command like this: `unsuspend <process.name>` or `unsuspend <process.pid> \n");
        }
    }

    private static void SuspendProcess(string argument1, string _)
    {
        if (argument1 != null)
        {
            bool fon = false;
            foreach (var proc in Process.GetProcesses())
            {
                if (proc.Id.ToString() == argument1)
                {
                    Utilities.SuspendProcess(Int32.Parse(argument1));
                    Console.WriteLine(proc.Id.ToString() + ":::" + proc.ProcessName + " suspended");
                    fon = true;
                }
                if (proc.ProcessName == argument1)
                {
                    Utilities.SuspendProcess(proc.Id);
                    Console.WriteLine(proc.Id.ToString() + ":::" + proc.ProcessName + " suspended");
                    fon = true;
                }
            }
            if (fon == false)
            {
                Console.WriteLine(argument1 + " not found");
            }
            Console.Write("\n");
        }

        else
        {
            Console.WriteLine("You didn't pass an argument, use the `suspend` command like this: `suspend <process.name>` or `suspend <process.pid>\n");
        }
    }

    private static void KillProcess(string argument1, string _)
    {
        if (argument1 != null)
        {
            bool fon = false;
            foreach (var proc in Process.GetProcesses())
            {
                if (proc.Id.ToString() == argument1)
                {
                    proc.Kill();
                    Console.WriteLine(proc.Id.ToString() + ":::" + proc.ProcessName + " killed");
                    fon = true;
                }
                if (proc.ProcessName == argument1)
                {
                    proc.Kill();
                    Console.WriteLine(proc.Id.ToString() + ":::" + proc.ProcessName + " killed");
                    fon = true;
                }
            }
            if (fon == false)
            {
                Console.WriteLine(argument1 + " not found");
            }
            Console.Write("\n");
        }
        else
        {
            Console.WriteLine("You didn't pass an argument, use the `kill` command like this: `kill <process.name>` or `kill <process.pid>\n");
        }
    }

    private static void DisplayHelpMessage(string argument1, string argument2)
    {
        Console.WriteLine("help - show help");
        Console.WriteLine("clear or cls - cleans the console");
        Console.WriteLine("kill <process.name> or <process.pid>- kill process");
        Console.WriteLine("suspend <process.name> or <process.pid> - suspend process");
        Console.WriteLine("unsuspend <process.name> or <process.pid> - unsuspend process");
        Console.WriteLine("ps - write process list");
        Console.WriteLine("find <process.name> or <process.pid> - find process by pid or name");
    }
}