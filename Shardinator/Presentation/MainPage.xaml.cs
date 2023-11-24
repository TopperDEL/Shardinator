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

    private async void NumberBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        if (this.DataContext != null && this.DataContext is MainViewModel mainViewModel)
        {
            await mainViewModel.SaveShardinationDaysAsync(args.NewValue);
        }
    }

    private void Image_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
    {
        Image senderImage = sender as Image;
        if (this.DataContext != null && this.DataContext is MainViewModel mainViewModel)
        {
            mainViewModel.ShowDetailCommand.Execute(senderImage.Tag as string);
        }
    }
}
