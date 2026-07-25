using System;
using System.Runtime.Versioning;
using System.Security.Principal;
using Spectre.Console;

namespace Mastercore.WirelessEngine
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Mastercore Wireless Engine v1.0";

            var banner = new FigletText("MASTERCORE")
            {
                Justification = Justify.Left,
                Color = Color.Green
            };
            AnsiConsole.Write(banner);

            AnsiConsole.MarkupLine("[bold green]====================================================================[/]");
            AnsiConsole.MarkupLine("[bold white] Universal Low-Latency Hotspot Engine & Hardware Diagnostic Control [/]");
            AnsiConsole.MarkupLine("[bold green]====================================================================[/]\n");

            if (!IsAdministrator())
            {
                AnsiConsole.MarkupLine("[bold red][[!]] ERROR: Elevated privileges required.[/]");
                AnsiConsole.MarkupLine("[yellow]Please run terminal or VS Code as Administrator to access Win32 WLAN & COM APIs.[/]");
                return;
            }

            AnsiConsole.MarkupLine("[bold green][[✓]] Administrative rights verified.[/]\n");

            // Execute Hardware Diagnostics
            AnsiConsole.MarkupLine("[bold yellow]Running Hardware Abstraction Layer (HAL) Diagnostics...[/]\n");
            var detectedTier = HardwareDiagnosticEngine.DisplayDiagnosticReport();

            if (detectedTier == HardwareTier.Unsupported)
            {
                AnsiConsole.MarkupLine("\n[bold red][!] Error: No active wireless adapters detected on this host system.[/]");
                return;
            }

            AnsiConsole.MarkupLine("\n[bold green][[✓]] Hardware diagnostics complete. System initialized.[/]");
        }

        [SupportedOSPlatform("windows")]
        private static bool IsAdministrator()
        {
            if (!OperatingSystem.IsWindows()) return false;
            using var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}