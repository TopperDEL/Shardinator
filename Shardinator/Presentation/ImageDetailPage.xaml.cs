using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.UI.Core;
using Shardinator.ViewModels;
using Shardinator.Services.Shardination;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Shardinator.Presentation
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ImageDetailPage : Page
    {
        private string _imageKey;

        public ImageDetailPage()
        {
            this.InitializeComponent();
            this.DataContextChanged += ImageDetailPage_DataContextChanged;
        }

        private void ImageDetailPage_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (this.DataContext != null && this.DataContext is ImageDetailViewModel imageDetailViewModel)
            {
                imageDetailViewModel.ImageThumbKey = _imageKey;
                imageDetailViewModel.ImageKey = _imageKey.Replace(ShardinatorService.THUMB_PREFIX, "");
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _imageKey = e.Parameter as string;
        }

        private async void scrollViewer_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;
            var doubleTapPoint = e.GetPosition(scrollViewer);

            if (scrollViewer.ZoomFactor != 1)
            {
                scrollViewer.ZoomToFactor(1);
            }
            else if (scrollViewer.ZoomFactor == 1)
            {
                scrollViewer.ZoomToFactor(2);

                //var dispatcher = App.Current.ServiceLocation().Get<IDispatcher>();
                //await dispatcher.TryEnqueue(() =>
                //{
                scrollViewer.ScrollToHorizontalOffset(doubleTapPoint.X);
                scrollViewer.ScrollToVerticalOffset(doubleTapPoint.Y);
                //});
            }
        }
    }
}
