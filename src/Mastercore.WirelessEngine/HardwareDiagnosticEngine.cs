using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Spectre.Console;

namespace Mastercore.WirelessEngine
{
    public enum HardwareTier
    {
        Tier1_WinRT_Tethering,
        Tier2_Legacy_SoftAP,
        Tier3_MultiCard_Bridge,
        Unsupported
    }

    [SupportedOSPlatform("windows")]
    public class HardwareDiagnosticEngine
    {
        [DllImport("wlanapi.dll", SetLastError = true)]
        private static extern uint WlanOpenHandle(
            uint dwClientVersion,
            IntPtr pReserved,
            out uint pdwNegotiatedVersion,
            out IntPtr phClientHandle);

        [DllImport("wlanapi.dll", SetLastError = true)]
        private static extern uint WlanCloseHandle(IntPtr hClientHandle, IntPtr pReserved);

        [DllImport("wlanapi.dll", SetLastError = true)]
        private static extern uint WlanEnumInterfaces(
            IntPtr hClientHandle,
            IntPtr pReserved,
            out IntPtr ppInterfaceList);

        [DllImport("wlanapi.dll", SetLastError = true)]
        private static extern void WlanFreeMemory(IntPtr pMemory);

        public static HardwareTier EvaluateSystemCapabilities(out int interfaceCount)
        {
            interfaceCount = 0;
            if (!OperatingSystem.IsWindows()) return HardwareTier.Unsupported;

            uint result = WlanOpenHandle(2, IntPtr.Zero, out _, out IntPtr handle);
            if (result == 0)
            {
                try
                {
                    result = WlanEnumInterfaces(handle, IntPtr.Zero, out IntPtr interfaceListPtr);
                    if (result == 0 && interfaceListPtr != IntPtr.Zero)
                    {
                        interfaceCount = Marshal.ReadInt32(interfaceListPtr);
                        WlanFreeMemory(interfaceListPtr);
                    }
                }
                finally
                {
                    WlanCloseHandle(handle, IntPtr.Zero);
                }
            }

            if (interfaceCount >= 2)
            {
                return HardwareTier.Tier3_MultiCard_Bridge;
            }
            else if (interfaceCount == 1)
            {
                return HardwareTier.Tier1_WinRT_Tethering;
            }

            return HardwareTier.Unsupported;
        }

        public static HardwareTier DisplayDiagnosticReport()
        {
            var tier = EvaluateSystemCapabilities(out int count);

            var table = new Table().Border(TableBorder.Rounded);
            table.AddColumn("[bold cyan]Diagnostic Parameter[/]");
            table.AddColumn("[bold cyan]System State[/]");

            table.AddRow("Detected WLAN Adapters", count > 0 ? $"[green]{count}[/]" : "[red]0[/]");

            string tierDescription = tier switch
            {
                HardwareTier.Tier1_WinRT_Tethering => "[green]Tier 1 (WinRT SoftAP / Concurrent DBAC)[/]",
                HardwareTier.Tier2_Legacy_SoftAP => "[yellow]Tier 2 (Legacy NDIS Hosted Network)[/]",
                HardwareTier.Tier3_MultiCard_Bridge => "[cyan]Tier 3 (Dual-Card ICS Bridge Mode)[/]",
                _ => "[red]Unsupported / No Wireless Hardware Found[/]"
            };

            table.AddRow("Optimal Engine HAL Tier", tierDescription);
            AnsiConsole.Write(table);

            return tier;
        }
    }
}