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
        ["find"] = FindProcess,
        ["getpath"] = GetPath
    };

    
    public static void Main(string[] args)
    {
        Console.Title = "Splash";

        Utilities.SplashScreen();
        while (true)
        {
            Console.Write(">$>: ");
            string? input = Console.ReadLine();
            string?[] commands = input?.Split("&") ?? Array.Empty<string>();
            foreach (string? commandRaw in commands)
            {
                string[] commandParts = commandRaw?.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
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
                if (!string.IsNullOrEmpty(command))
                {
                    if (!_commandMap.TryGetValue(command, out var commandMethod))
                    {
                        Console.WriteLine(command + " is an unknown command, if you need help type 'help' \n");
                    }
                    else
                    {
                        if (commandMethod != null)
                        {
                            if (argument1 != null && argument2 != null)
                            {
                                commandMethod.Invoke(argument1, argument2);
                            }
                            else if (argument1 != null)
                            {
                                commandMethod.Invoke(argument1, string.Empty);
                            }
                            else if (argument2 != null)
                            {
                                commandMethod.Invoke(string.Empty, argument2);
                            }
                            else
                            {
                                commandMethod.Invoke(string.Empty, string.Empty);
                            }
                        }
                    }
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

    private static void GetPath(string argument1, string __)
    {
        Process[] processes = Process.GetProcesses();

        foreach (Process process in processes)
        {
            if (process.ProcessName == argument1 || process.Id.ToString() == argument1)
            {
                try
                {
                    if (process != null && process.MainModule != null)
                    {
                        Console.WriteLine($"{process.Id}:::{process.ProcessName} > {process.MainModule.FileName}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to access path for process: {process.Id}:::{process.ProcessName} >> {ex.Message}");
                }
            }
        }
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
            bool isFinded = false;
            foreach (var proc in Process.GetProcesses())
            {
                try
                {
                    if (proc.Id.ToString() == argument1)
                    {
                        Utilities.ResumeProcess(Int32.Parse(argument1));
                        Console.WriteLine(proc.Id.ToString() + ":::" + proc.ProcessName + " unsuspended");
                        isFinded = true;
                    }
                    if (proc.ProcessName == argument1)
                    {
                        Utilities.ResumeProcess(proc.Id);
                        Console.WriteLine(proc.Id.ToString() + ":::" + proc.ProcessName + " unsuspended");
                        isFinded = true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to unsuspend process >> {ex.Message}");
                    isFinded = true;
                }
            }
            if (isFinded == false)
            {
                Console.WriteLine(argument1 + " not found");
            }
            Console.Write("\n");
        }
        else
        {
            Console.WriteLine("You didn't pass an argument, use the `unsuspend` command like this: `unsuspend <process.name>` or `unsuspend <process.pid>` \n");
        }
    }

    private static void SuspendProcess(string argument1, string _)
    {
        if (argument1 != null)
        {
            bool isFinded = false;
            foreach (var proc in Process.GetProcesses())
            {
                try
                {
                    if (proc.Id.ToString() == argument1)
                    {
                        Utilities.SuspendProcess(Int32.Parse(argument1));
                        Console.WriteLine(proc.Id.ToString() + ":::" + proc.ProcessName + " suspended");
                        isFinded = true;
                    }
                    if (proc.ProcessName == argument1)
                    {
                        Utilities.SuspendProcess(proc.Id);
                        Console.WriteLine(proc.Id.ToString() + ":::" + proc.ProcessName + " suspended");
                        isFinded = true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to suspend process >> {ex.Message}");
                    isFinded = true;
                }
            }
            if (isFinded == false)
            {
                Console.WriteLine(argument1 + " not found");
            }
            Console.Write("\n");
        }

        else
        {
            Console.WriteLine("You didn't pass an argument, use the `suspend` command like this: `suspend <process.name>` or `suspend <process.pid>` \n");
        }
    }

    private static void KillProcess(string argument1, string _)
    {
        if (argument1 != null)
        {
            bool isFinded = false;
            foreach (var proc in Process.GetProcesses())
            {
                try
                {
                    if (proc.Id.ToString() == argument1)
                    {
                        proc.Kill();
                        Console.WriteLine(proc.Id.ToString() + ":::" + proc.ProcessName + " killed");
                        isFinded = true;
                    }
                    if (proc.ProcessName == argument1)
                    {
                        proc.Kill();
                        Console.WriteLine(proc.Id.ToString() + ":::" + proc.ProcessName + " killed");
                        isFinded = true;
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Failed to kill process >> {ex.Message}");
                    isFinded = true;
                }
            }
            if (isFinded == false)
            {
                Console.WriteLine(argument1 + " not found");
            }
            Console.Write("\n");
        }
        else
        {
            Console.WriteLine("You didn't pass an argument, use the `kill` command like this: `kill <process.name>` or `kill <process.pid>` \n");
        }
    }

    private static void DisplayHelpMessage(string argument1, string argument2)
    {
        Console.WriteLine("help - show help");
        Console.WriteLine("clear or cls - clear the console");
        Console.WriteLine("kill <process.name> or <process.id> - kill a process");
        Console.WriteLine("suspend <process.name> or <process.id> - suspend a process");
        Console.WriteLine("unsuspend <process.name> or <process.id> - unsuspend a process");
        Console.WriteLine("ps - write the process list");
        Console.WriteLine("find <process.name> or <process.id> - find a process by its PID or name");
        Console.WriteLine("getpath <process.name> or <process.id> - output the file path");
    }
}
