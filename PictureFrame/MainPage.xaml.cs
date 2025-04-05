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

    private static void SetVisualCenterPoint(Image myImage, Microsoft.UI.Composition.Visual visual) => visual.CenterPoint = new Vector3((float)(myImage.ActualWidth / 2.0), (float)(myImage.ActualHeight / 2.0), 0);

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


    private static Vector3 GetTargetVector(Image myImage)
    {
        var v = new Vector3(0.8f * (float)Math.Max(myImage.ActualHeight, myImage.ActualWidth), 0, 0);
        Matrix4x4 m = Matrix4x4.CreateRotationZ((float)(Random.Shared.NextDouble() * 2.0 * Math.PI));
        var rotatedVector = Vector3.Transform(v, m);
        return rotatedVector;
    }

    // Method to animate zooming
    private void AnimateSlideIn(Microsoft.UI.Xaml.Controls.Image myImage)
    {
        // Get the Visual of the Image
        var visual = myImage.GetVisualInternal();
        SetVisualCenterPoint(myImage, visual);

        // Access the Compositor
        var compositor = visual.Compositor;

        // Create a Scalar KeyFrame Animation for scaling
        var offsetAnimation = compositor.CreateVector3KeyFrameAnimation();

        var targetVector = GetTargetVector(myImage);

        offsetAnimation.InsertKeyFrame(0f, targetVector);
        offsetAnimation.InsertKeyFrame(1f, Vector3.Zero);
        offsetAnimation.Direction = Microsoft.UI.Composition.AnimationDirection.Normal;
        offsetAnimation.Duration = TransitionTime;
        visual.StartAnimation("Offset", offsetAnimation);

        var opacityAnimation = compositor.CreateScalarKeyFrameAnimation();
        opacityAnimation.InsertKeyFrame(0f, 0f);   // Fully transparent
        opacityAnimation.InsertKeyFrame(1f, 1f);   // Fully visible
        opacityAnimation.Duration = TransitionTime;
        visual.StartAnimation("Opacity", opacityAnimation);

        var scaleAnimation = compositor.CreateVector3KeyFrameAnimation();
        scaleAnimation.InsertKeyFrame(0f, new Vector3(1.5f, 1.5f, 1f)); // Original size
        scaleAnimation.InsertKeyFrame(1f, new Vector3(1f, 1f, 1f)); // Scaled up by 1.5x
        scaleAnimation.Duration = TransitionTime;

        visual.StartAnimation("Scale", scaleAnimation);

        // Create a scalar keyframe animation for rotation
        var rotationAnimation = compositor.CreateScalarKeyFrameAnimation();
        rotationAnimation.InsertKeyFrame(0f, Random.Shared.Next(-45, 45));    // Start angle (0 degrees)
        rotationAnimation.InsertKeyFrame(1f, 0f);
        rotationAnimation.Duration = TransitionTime; // 1 second duration
        //rotationAnimation.IterationBehavior = AnimationIterationBehavior.Forever; // Repeat forever

        // Start the animation
        visual.StartAnimation("RotationAngleInDegrees", rotationAnimation);
    }


    private void AnimateSlideOut(Image myImage)
    {
        // Get the Visual of the Image
        var visual = myImage.GetVisualInternal();
        SetVisualCenterPoint(myImage, visual);

        // Access the Compositor
        var compositor = visual.Compositor;

        // Create a Scalar KeyFrame Animation for scaling
        var offsetAnimation = compositor.CreateVector3KeyFrameAnimation();
        offsetAnimation.InsertKeyFrame(0f, Vector3.Zero);
        offsetAnimation.InsertKeyFrame(1f, GetTargetVector(myImage));
        offsetAnimation.Direction = Microsoft.UI.Composition.AnimationDirection.Normal;
        offsetAnimation.Duration = TransitionTime;
        visual.StartAnimation("Offset", offsetAnimation);

        var opacityAnimation = compositor.CreateScalarKeyFrameAnimation();
        opacityAnimation.InsertKeyFrame(0f, 1f);   // Fully visible
        opacityAnimation.InsertKeyFrame(1f, 0f);   // Fully transparent
        opacityAnimation.Duration = TransitionTime;
        visual.StartAnimation("Opacity", opacityAnimation);

        var scaleAnimation = compositor.CreateVector3KeyFrameAnimation();
        scaleAnimation.InsertKeyFrame(0f, new Vector3(1f, 1f, 1f));
        scaleAnimation.InsertKeyFrame(1f, new Vector3(0.5f, 0.5f, 0.5f));
        scaleAnimation.Duration = TransitionTime;

        visual.StartAnimation("Scale", scaleAnimation);

        // Create a scalar keyframe animation for rotation
        var rotationAnimation = compositor.CreateScalarKeyFrameAnimation();
        rotationAnimation.InsertKeyFrame(0f, 0f);
        rotationAnimation.InsertKeyFrame(1f, Random.Shared.Next(-45, 45));
        rotationAnimation.Duration = TransitionTime; // 1 second duration
        //rotationAnimation.IterationBehavior = AnimationIterationBehavior.Forever; // Repeat forever

        // Start the animation
        visual.StartAnimation("RotationAngleInDegrees", rotationAnimation);
    }

    private async Task ViewShowNextPicture(BitmapImage bitmapImage)
    {
        var image1 = isEven ? ImageA : ImageB;
        var image2 = !isEven ? ImageA : ImageB;
        isEven = !isEven;

        image1.Source = bitmapImage;

        Canvas.SetZIndex(image1, 1);
        Canvas.SetZIndex(image2, 0);
        AnimateSlideIn(image1);
        AnimateSlideOut(image2);
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
