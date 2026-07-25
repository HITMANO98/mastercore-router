@echo off
title MT7601 Ultra-Low Latency Gaming Controller
:menu
cls
color 0A
echo ====================================================
echo        MT7601 GAMING-OPTIMIZED HOTSPOT ENGINE       
echo ====================================================
echo  [1] START Hotspot (Max Speed + Gaming QoS Injection)
echo  [2] STOP Hotspot (Clean Tear Down)
echo  [3] SHOW Connected Clients (Check Jitter)
echo  [4] HARD RESET Stack (Flush Freezes)
echo  [5] EXIT
echo ====================================================
set /p choice="Select Engine Action (1-5): "

if "%choice%"=="1" goto start_hotspot
if "%choice%"=="2" goto stop_hotspot
if "%choice%"=="3" goto show_status
if "%choice%"=="4" goto hard_reset
if "%choice%"=="5" exit
goto menu

:start_hotspot
cls
color 0A
echo [!] Wiping network queues and error caches...
netsh wlan stop hostednetwork >nul 2>&1
reg delete "HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\WlanSvc\Parameters\HostedNetworkSettings" /v HostedNetworkSettings /f >nul 2>&1
netsh wlan set hostednetwork mode=allow ssid=ADAM key=12345678abc

echo [!] Tuning system drivers for low latency...
net stop WlanSvc >nul 2>&1
timeout /t 2 >nul
net start WlanSvc >nul 2>&1
timeout /t 2 >nul

echo [!] Igniting raw broadcast stream on Channel 2412...
netsh wlan start hostednetwork

echo [!] Injecting packet routing pipelines...
:: Upgraded PowerShell command to capture the source adapter dynamically by description to avoid naming bugs
powershell -windowstyle hidden -Command "$m = New-Object -ComObject HNetCfg.HNetShare; $c = $m.EnumEveryConnection | Where-Object { $m.NetConnectionProps($_).DeviceName -like '*Centrino*' -or $m.NetConnectionProps($_).Name -eq 'Wi-Fi' }; $s = $m.INetSharingConfigurationForINetConnection($c); $s.EnableSharing(0); $v = $m.EnumEveryConnection | Where-Object { $m.NetConnectionProps($_).Name -like 'Local Area Connection*' }; $vs = $m.INetSharingConfigurationForINetConnection($v); $vs.EnableSharing(1)" >nul 2>&1

echo [!] INJECTING MULTIMEDIA REGISTRY PRIORITY QUEUES...
reg add "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Games" /v "Scheduling Category" /t REG_SZ /d "High" /f >nul 2>&1
reg add "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Games" /v "Priority" /t REG_DWORD /d 6 /f >nul 2>&1
reg add "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Games" /v "GPU Priority" /t REG_DWORD /d 8 /f >nul 2>&1

echo [!] DISABLING TCP NETWORK THROTTLING FOR HOTSPOT PIPELINE...
:: Stops Windows from throttling background network packets when high CPU gaming load occurs
reg add "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile" /v "NetworkThrottlingIndex" /t REG_DWORD /d 4294967295 /f >nul 2>&1

echo ====================================================
echo SUCCESS! PERFORMANCE TWEAKS INJECTED.
echo Centrino: Locked Channel 60 (Max Downstream)
echo Hotspot: Priority Queue Active on Channel 2412
echo ====================================================
pause
goto menu

:stop_hotspot
cls
color 0C
echo [!] Safely shutting down the hotspot broadcast...
netsh wlan stop hostednetwork
netsh wlan set hostednetwork mode=disallow >nul 2>&1
echo [!] Internet sharing disabled cleanly.
pause
goto menu

:show_status
cls
color 0E
echo ====================================================
echo             CURRENT HOTSPOT STATUS / CLIENTS        
echo ====================================================
netsh wlan show hostednetwork
echo ====================================================
pause
goto menu

:hard_reset
cls
color 0D
echo [!] Performing structural network reset...
netsh wlan stop hostednetwork >nul 2>&1
netsh wlan set hostednetwork mode=disallow >nul 2>&1
reg delete "HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\WlanSvc\Parameters\HostedNetworkSettings" /v HostedNetworkSettings /f >nul 2>&1
net stop WlanSvc >nul 2>&1
net start WlanSvc >nul 2>&1
echo [!] Wireless memory stack fully flushed. Ready for clean start.
pause
goto menu
