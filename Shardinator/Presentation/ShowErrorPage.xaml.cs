using Shardinator.ViewModels;

namespace Shardinator.Presentation;

public sealed partial class ShowErrorPage : Page
{
    private string _errorMessage;

    public ShowErrorPage()
    {
        this.InitializeComponent();

        this.DataContextChanged += ShowErrorPage_DataContextChanged;
    }

    private void ShowErrorPage_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
    {
        if (DataContext != null && DataContext is ErrorViewModel errorViewModel)
        {
            errorViewModel.ErrorMessage = _errorMessage;
        }
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        _errorMessage = e.Parameter as string;
    }
}
