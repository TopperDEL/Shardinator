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
using Shardinator.ViewModels;
using Shardinator.Services.Shardination;
using Windows.Media.Core;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Shardinator.Presentation
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class VideoDetailPage : Page
	{
        private string _videoKey;

        public VideoDetailPage()
		{
			this.InitializeComponent();
            this.DataContextChanged += VideoDetailPage_DataContextChanged;
        }

        private async void VideoDetailPage_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (this.DataContext != null && this.DataContext is VideoDetailViewModel videoDetailViewModel)
            {
                videoDetailViewModel.VideoThumbKey = _videoKey;
                videoDetailViewModel.VideoKey = _videoKey.Replace(ShardinatorService.THUMB_PREFIX, "");

                var stream = await videoDetailViewModel.InitStreamAsync();
                mediaControl.Source = MediaSource.CreateFromStream(stream.AsRandomAccessStream(), "video/mp4");
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _videoKey = e.Parameter as string;
        }
    }
}
