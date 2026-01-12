using System.Diagnostics;

namespace Test.Simulation;

internal static class ServerManager
{
    internal static Process? StartServer(int port)
    {
        try
        {
            string serverPath = Path.Combine(
                "..",
                "..",
                "..",
                "..",
                "server",
                "bin",
                "Debug",
                "net10.0",
                "Test.SseServer.dll");

            if (!File.Exists(serverPath))
            {
                Console.WriteLine($"Server not found at {serverPath}. Building server...");
                BuildServer();
            }

            ProcessStartInfo startInfo = new()
            {
                FileName = "dotnet",
                Arguments = $"{serverPath} --urls http://localhost:{port}",
                UseShellExecute = false,
                CreateNoWindow = false,
                RedirectStandardOutput = false,
                RedirectStandardError = false
            };

            Process? process = Process.Start(startInfo);
            Console.WriteLine($"[ServerManager] SSE server started on port {port} (PID: {process?.Id})");
            return process;
        }
        catch (IOException ex)
        {
            Console.WriteLine($"[ServerManager] I/O error starting server: {ex.Message}");
            return null;
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"[ServerManager] Invalid operation: {ex.Message}");
            return null;
        }
    }

    internal static void StopServer(Process? process)
    {
        if (process is not null && !process.HasExited)
        {
            try
            {
                process.Kill(entireProcessTree: true);
                process.WaitForExit(3000);
                Console.WriteLine("[ServerManager] SSE server stopped");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"[ServerManager] Error stopping server: {ex.Message}");
            }
        }
    }

    private static void BuildServer()
    {
        string serverProjectPath = Path.Combine(
            "..",
            "..",
            "..",
            "..",
            "server");

        ProcessStartInfo buildInfo = new()
        {
            FileName = "dotnet",
            Arguments = "build --no-restore",
            WorkingDirectory = serverProjectPath,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        using Process? buildProcess = Process.Start(buildInfo);
        buildProcess?.WaitForExit();

        if (buildProcess?.ExitCode != 0)
        {
            throw new InvalidOperationException("Failed to build SSE server");
        }

        Console.WriteLine("[ServerManager] SSE server built successfully");
    }
}
