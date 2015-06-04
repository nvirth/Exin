using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using Common.Log;
using Common.Utils.Helpers;

/// -------------------------------------------------------------------------------------------------
/// <summary> Application Running Helper. </summary>
/// -------------------------------------------------------------------------------------------------
public static class ApplicationRunningHelper
{
    [DllImport("user32.dll")]
    private static extern
        bool SetForegroundWindow(IntPtr hWnd);
    [DllImport("user32.dll")]
    private static extern
        bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
    [DllImport("user32.dll")]
    private static extern
        bool IsIconic(IntPtr hWnd);

    /// -------------------------------------------------------------------------------------------------
    /// <summary> check if current process already running. if running, set focus to existing process and 
    ///           returns <see langword="true"/> otherwise returns <see langword="false"/>. </summary>
    /// <returns> <see langword="true"/> if it succeeds, <see langword="false"/> if it fails. </returns>
    /// -------------------------------------------------------------------------------------------------
    public static void SwitchToRunningInstanceIfExists()
    {
        /*
        const int SW_HIDE = 0;
        const int SW_SHOWNORMAL = 1;
        const int SW_SHOWMINIMIZED = 2;
        const int SW_SHOWMAXIMIZED = 3;
        const int SW_SHOWNOACTIVATE = 4;
        const int SW_RESTORE = 9;
        const int SW_SHOWDEFAULT = 10;
        */
        const int swRestore = 9;

        var currentProcess = Process.GetCurrentProcess();
        var runningInstances = Process.GetProcessesByName(currentProcess.ProcessName);

		// Apply these when we need to select the "real" process from the running instances,
		// which has the GUI with windows and so. We can determine this from the
		// used memory size
		//
		//var otherRunningInstances = runningInstances.Where(process => process.Id != currentProcess.Id).ToList();
		//if(otherRunningInstances.Count == 0)
		//	return;
		//
		//var runningInstance = runningInstances
		//	.Where(process => process.Id != currentProcess.Id) // Aggr: Max by VirtualMemorySize64
		//	.Aggregate(otherRunningInstances[0], (accMax, process) => process.VirtualMemorySize64 > accMax.VirtualMemorySize64 ? process : accMax);

		var runningInstance = runningInstances.FirstOrDefault(process => process.Id != currentProcess.Id);
        if (runningInstance != null)
        {
            var runningInstanceHandle = runningInstance.MainWindowHandle;

            // If iconic, we need to restore the window
            if (IsIconic(runningInstanceHandle))
                ShowWindowAsync(runningInstanceHandle, swRestore);

            // Bring it to the foreground
            SetForegroundWindow(runningInstanceHandle);

            // Close this instance (hard and fast)
            currentProcess.Kill();

            // Close this instance (soft and slow)
            // Note, that we don't need anything to do by shutdown in this state,
            // so we can just kill the process instead.
            // Note, that this way this 2. process' shutdown needs time
            //Application.Current.Shutdown();
        }
    }
}