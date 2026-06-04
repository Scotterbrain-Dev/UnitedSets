namespace UnitedSets.UI.Controls;

[QuickMarkup("""
    <root Padding=5>
        <FluentIconElement Symbol=Apps20 Margin=`new(-2,-6,0,0)` />
    </root>
    """)]
partial class AppLauncherButton : Button
{
    readonly BackdropedFlyout _flyout = new();
    bool _flyoutOpen;
    DateTime _flyoutClosedAt = DateTime.MinValue;

    public AppLauncherButton()
    {
        Init();
        ToolTipService.SetToolTip(this, "App Launcher");
        _flyout.Content = new AppLauncherFlyoutModule();
        _flyout.Opened += (_, _) => _flyoutOpen = true;
        _flyout.Closed += (_, _) =>
        {
            _flyoutOpen = false;
            _flyoutClosedAt = DateTime.Now;
        };
        Click += OnToggleClick;
    }

    void OnToggleClick(object sender, RoutedEventArgs e)
    {
        if (_flyoutOpen)
            _flyout.Hide();
        else if ((DateTime.Now - _flyoutClosedAt).TotalMilliseconds > 200)
            _flyout.ShowAt(this);
        // else: light-dismiss from this button click just closed it; don't reopen
    }
}
