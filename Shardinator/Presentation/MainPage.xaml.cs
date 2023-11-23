using Shardinator.ViewModels;

namespace Shardinator.Presentation;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();
    }

    private void TabBar_SelectionChanged(Uno.Toolkit.UI.TabBar sender, Uno.Toolkit.UI.TabBarSelectionChangedEventArgs args)
    {
        if (this.DataContext != null && this.DataContext is MainViewModel mainViewModel)
        {
            mainViewModel.ActiveRegionChanged();
        }
    }
}
