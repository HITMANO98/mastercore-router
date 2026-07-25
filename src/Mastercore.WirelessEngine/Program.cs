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
            
            // Render Header Banner
            var banner = new FigletText("MASTERCORE")
            {
                Justification = Justify.Left,
                Color = Color.Green
            };
            AnsiConsole.Write(banner);

            AnsiConsole.MarkupLine("[bold green]====================================================================[/]");
            AnsiConsole.MarkupLine("[bold white] Universal Low-Latency Hotspot Engine & DFS Bypass [/]");
            AnsiConsole.MarkupLine("[bold green]====================================================================[/]\n");

            // Admin Privilege Check
            if (!IsAdministrator())
            {
                AnsiConsole.MarkupLine("[bold red][[!]] ERROR: Elevated privileges required.[/]");
                AnsiConsole.MarkupLine("[yellow]Please restart VS Code or Terminal as Administrator to access Win32 WLAN & COM Sharing APIs.[/]");
                return;
            }

            // Escaped brackets using [[✓]] so Spectre doesn't parse it as a style name
            AnsiConsole.MarkupLine("[bold green][[✓]] Administrative rights verified.[/]\n");

            // Dashboard Status Panel Layout
            var table = new Table().Border(TableBorder.Rounded);
            table.AddColumn("[bold cyan]Pipeline Layer[/]");
            table.AddColumn("[bold cyan]Interface / Config[/]");
            table.AddColumn("[bold cyan]Status[/]");

            table.AddRow("Inbound Link", "5GHz (Channel 60 - DFS)", "[green]Connected[/]");
            table.AddRow("Outbound Hotspot", "MediaTek 2.4GHz (Channel 1)", "[yellow]Standby[/]");
            table.AddRow("Routing Mode", "NAT Firewall", "[green]Isolated[/]");
            table.AddRow("QoS Gaming Priority", "Multimedia SystemProfile", "[green]Active[/]");

            AnsiConsole.Write(table);

            AnsiConsole.MarkupLine("\n[bold gray]Press Ctrl+C to stop engine loop...[/]");
        }

        [SupportedOSPlatform("windows")]
        private static bool IsAdministrator()
        {
            if (!OperatingSystem.IsWindows())
                return false;

            using var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}