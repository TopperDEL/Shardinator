using Shardinator.ViewModels;

namespace Shardinator.Presentation;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();
    }

    private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (this.DataContext != null && this.DataContext is MainViewModel mainViewModel)
        {
            mainViewModel.Gallery.RefreshCommand.Execute(null);
        }
    }
}
