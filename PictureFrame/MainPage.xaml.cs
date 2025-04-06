using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Numerics;
using PictureFrame.ViewModels;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace PictureFrame;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainPage : Page
{
    private readonly MainPageViewModel viewModel;

    private TimeSpan TransitionTime => viewModel.SlideShowTransitionTimeSpan;

    private bool isEven = false;

    private async Task<string?> ViewSelectFolder()
    {
        FolderPicker folderPicker = new()
        {
            ViewMode = PickerViewMode.Thumbnail
        };

        var windowHandle = WindowNative.GetWindowHandle(App.MainWindow);
        InitializeWithWindow.Initialize(folderPicker, windowHandle);

        return (await folderPicker.PickSingleFolderAsync())?.Path;
    }

    public MainPage()
    {
        InitializeComponent();
        // create the viewmodel and hook it up to the view
        viewModel = new MainPageViewModel
        {
            ViewSelectFolder = ViewSelectFolder,
            ViewShowNextPicture = ViewShowNextPicture,
            ViewFullScreen = ViewFullScreen           
        };
        viewModel.SelectedTransition = viewModel.Transitions[0];

        DataContext = viewModel;
        Loaded += MainPage_Loaded;
        imageCanvas.SizeChanged += ImageCanvas_SizeChanged;
    }


    private async void MainPage_Loaded(object sender, RoutedEventArgs e)
    {
        await viewModel.OnLoaded();
    }

    private void ImageCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        ImageA.Width = e.NewSize.Width;
        ImageA.Height = e.NewSize.Height;
        ImageB.Width = e.NewSize.Width;
        ImageB.Height = e.NewSize.Height;
        OverlaidPanel.Width = e.NewSize.Width;
        OverlaidPanel.Height = e.NewSize.Height;
    }

    private async Task ViewShowNextPicture(BitmapImage bitmapImage, Transition transition)
    {
        var image1 = isEven ? ImageA : ImageB;
        var image2 = !isEven ? ImageA : ImageB;
        isEven = !isEven;
        image1.Source = bitmapImage;
        Canvas.SetZIndex(image1, 1);
        Canvas.SetZIndex(image2, 0);
        transition.Animate(image1, image2, TransitionTime);
        await Task.CompletedTask;
    }

    private async Task ViewFullScreen(bool isFullScreen)
    {
        if (isFullScreen)
        {
            App.MainWindow.AppWindow.SetPresenter(Microsoft.UI.Windowing.AppWindowPresenterKind.FullScreen);
        }
        else
        {
            App.MainWindow.AppWindow.SetPresenter(Microsoft.UI.Windowing.AppWindowPresenterKind.Default);
        }
        await Task.CompletedTask;
    }
}
