using System.Diagnostics;
using Windows.Storage.Pickers;
using UnitedSets.Tabs;

namespace UnitedSets.UI.FlyoutModules;

public sealed partial class AppLauncherFlyoutModule : Grid
{
    public AppLauncherFlyoutModule()
    {
        InitializeComponent();
    }

    private async void LaunchExeClick(object sender, RoutedEventArgs e)
    {
        LaunchExeButton.IsEnabled = false;
        try
        {
            var picker = new FileOpenPicker();
            WinRT.Interop.InitializeWithWindow.Initialize(
                picker,
                WinRT.Interop.WindowNative.GetWindowHandle(UnitedSetsApp.Current.MainWindow));
            picker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            picker.FileTypeFilter.Add(".exe");

            var file = await picker.PickSingleFileAsync();
            if (file is null) return;

            Process? proc;
            try
            {
                proc = Process.Start(new ProcessStartInfo(file.Path) { UseShellExecute = true });
            }
            catch
            {
                return;
            }
            if (proc is null) return;

            // Poll for the main window handle with a 30-second timeout
            var deadline = DateTime.UtcNow.AddSeconds(30);
            while (DateTime.UtcNow < deadline)
            {
                proc.Refresh();
                if (proc.MainWindowHandle != IntPtr.Zero) break;
                await Task.Delay(300);
            }
            proc.Refresh();
            if (proc.MainWindowHandle == IntPtr.Zero) return;

            // Brief settle delay so the window is fully rendered before capture
            await Task.Delay(400);

            var window = WindowEx.FromWindowHandle(proc.MainWindowHandle);
            var tab = WindowHostTab.Create(window);
            if (tab is null) return;

            UnitedSetsApp.Current.Tabs.Add(tab);
            UnitedSetsApp.Current.SelectedTab = tab;
        }
        finally
        {
            LaunchExeButton.IsEnabled = true;
        }
    }
}
