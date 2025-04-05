using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using Windows.Storage;

namespace PictureFrame.ViewModels;

public partial class MainPageViewModel : ObservableObject
{
    private readonly DispatcherTimer dispatcherTimer = new();

    public Func<Task<string?>> ViewSelectFolder
    {
        get; set;
    } = () => Task.FromResult<string?>(null);

    public Func<BitmapImage,Task> ViewShowNextPicture
    {
        get; set;
    } = _ => Task.CompletedTask;

    public Func<bool,Task> ViewFullScreen
    {
        get; set;
    } = _ => Task.CompletedTask;

    public List<String> Playlist
    {
        get;
        set;
    } = [];

    public int PlayIndex
    {
        get;set;
    }

    [ObservableProperty]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "MVVMTK0045:Using [ObservableProperty] on fields is not AOT compatible for WinRT", Justification = "<Pending>")]
    private bool _isExpanded;

    [ObservableProperty]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "MVVMTK0045:Using [ObservableProperty] on fields is not AOT compatible for WinRT", Justification = "<Pending>")]
    private bool _isFullScreen;

    [ObservableProperty]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "MVVMTK0045:Using [ObservableProperty] on fields is not AOT compatible for WinRT", Justification = "<Pending>")]
    private double _slideShowDelayPromille = ExponentialInterpolationHelper.IntervalToPromille(TimeSpan.FromSeconds(10.0));

    [ObservableProperty]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "MVVMTK0045:Using [ObservableProperty] on fields is not AOT compatible for WinRT", Justification = "<Pending>")]
    private double _slideShowTransitionTime = 2.0;

    public TimeSpan SlideShowTotalIntervalTimeSpan => ExponentialInterpolationHelper.PromilleToInterval(SlideShowDelayPromille) + SlideShowTransitionTimeSpan;
    public TimeSpan SlideShowTransitionTimeSpan => TimeSpan.FromSeconds(SlideShowTransitionTime);

    [RelayCommand]
    private async Task SelectPicturesFolder()
    {
        var folder = await ViewSelectFolder();

        if (!string.IsNullOrEmpty(folder))
        {
            dispatcherTimer.Stop();
            await BuildPlayList(folder);
            await HandleTick();
        }
    }

    partial void OnSlideShowDelayPromilleChanged(double value)
    {
        dispatcherTimer.Interval = SlideShowTotalIntervalTimeSpan;
    }

    partial void OnSlideShowTransitionTimeChanged(double value)
    {
        dispatcherTimer.Interval = SlideShowTotalIntervalTimeSpan;
    }

    private bool IsImage(string path)
    {
        var ext = Path.GetExtension(path).ToLowerInvariant();
        return ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".bmp";
    }

    private async Task BuildPlayList(string folder)
    {
        Playlist = Directory.GetFiles(folder, "*.*",
            new EnumerationOptions
            {
                RecurseSubdirectories = true,
                IgnoreInaccessible = true,
            }).Where(x => IsImage(x)).ToList().ShuffleInPlace();
        PlayIndex = 0;
        await Task.CompletedTask;
    }

    public async Task OnLoaded()
    {
        dispatcherTimer.Interval = SlideShowTotalIntervalTimeSpan;
        dispatcherTimer.Tick += DispatcherTimer_Tick;
        dispatcherTimer.Start();
        await Task.CompletedTask;
    }

    private async void DispatcherTimer_Tick(object? sender, object e)
    {
        await HandleTick();
    }

    private async Task HandleTick()
    {
        dispatcherTimer.Stop();

        try
        {
            await ShowNextPicture();
        }
        finally
        {
            dispatcherTimer.Start();
        }
    }

    private async Task ShowNextPicture()
    {
        if (PlayIndex < Playlist.Count)
        {
            var image = await LoadImageFromDiskAsync(Playlist[PlayIndex]);
            await ViewShowNextPicture(image);

            PlayIndex++;
            if (PlayIndex >= Playlist.Count)
            {
                PlayIndex = 0;
            }
        }
    }

    // Method to load an image
    private async Task<BitmapImage> LoadImageFromDiskAsync(string filePath)
    {
        // Get the file from the specified path
        StorageFile file = await StorageFile.GetFileFromPathAsync(filePath);

        // Open the file as a stream
        using IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read);

        // Create a BitmapImage and set its source
        var bitmapImage = new BitmapImage();
        await bitmapImage.SetSourceAsync(fileStream);
        return bitmapImage;
    }

    [RelayCommand]
    private async Task FullScreen()
    {
        await ViewFullScreen(IsFullScreen);
    }
}
