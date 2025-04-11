using System;
using Microsoft.UI.Xaml.Controls;
using System.Numerics;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Animation;

namespace PictureFrame;


public class TransitionPageFlip : Transition
{
    private int direction = 1;

    public TransitionPageFlip()
    {
        DisplayName = "Page flip";
    }

    private static void SetVisualCenterPoint(Image myImage, Microsoft.UI.Composition.Visual visual) => visual.CenterPoint = new Vector3((float)(myImage.ActualWidth), (float)(myImage.ActualHeight / 2.0), 0);

    private Timeline AnimateIn(Image myImage, TimeSpan TransitionTime)
    {
        // Create the PlaneProjection
        var projection = new PlaneProjection { RotationY = -90, CenterOfRotationX = 0.5 };
        myImage.Projection = projection;

        var animation = new DoubleAnimation
        {
            From = -90,
            To = 0,
            Duration = TransitionTime / 2.0,
            BeginTime = TransitionTime / 2.0,
            RepeatBehavior = new RepeatBehavior(1.0),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };

        Storyboard.SetTarget(animation, projection);
        Storyboard.SetTargetProperty(animation, "RotationY");
        myImage.GetVisualInternal().Opacity = 1.0f;
        return animation;
    }

    private Timeline AnimateOut(Image myImage, TimeSpan TransitionTime)
    {
        // Create the PlaneProjection
        var projection = new PlaneProjection { RotationY = 0, CenterOfRotationX = 0.5 };
        myImage.Projection = projection;

        // Create the animation
        var animation = new DoubleAnimation
        {
            From = 0,
            To = 90,
            Duration = TransitionTime / 2.0,
            RepeatBehavior = new RepeatBehavior(1.0),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
        };

        Storyboard.SetTarget(animation, projection);
        Storyboard.SetTargetProperty(animation, "RotationY");
        myImage.GetVisualInternal().Opacity = 1.0f;
        return animation;
    }

    public override void AnimateInternal(Image newImageControl, Image oldImageControl, TimeSpan transitionTime)
    {
        var storyboard = new Storyboard();
        storyboard.Children.Add(AnimateIn(newImageControl, transitionTime));
        storyboard.Children.Add(AnimateOut(oldImageControl, transitionTime));
        storyboard.Begin();
        direction = -direction; // Toggle direction for next animation
    }
}
