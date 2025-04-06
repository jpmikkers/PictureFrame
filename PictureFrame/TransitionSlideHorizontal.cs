using System;
using Microsoft.UI.Xaml.Controls;
using System.Numerics;

namespace PictureFrame;

public class TransitionSlideHorizontal : Transition
{
    private int direction = 1;

    public TransitionSlideHorizontal()
    {
        DisplayName = "Slide horizontal";
    }

    private void AnimateHorizonal(Image myImage, TimeSpan TransitionTime, float from, float to)
    {
        var visual = myImage.GetVisualInternal();
        var compositor = visual.Compositor;
        var offsetAnimation = compositor.CreateVector3KeyFrameAnimation();
        offsetAnimation.InsertKeyFrame(0f, new Vector3(from, 0, 0));
        offsetAnimation.InsertKeyFrame(1f, new Vector3(to, 0, 0));
        offsetAnimation.Direction = Microsoft.UI.Composition.AnimationDirection.Normal;
        offsetAnimation.Duration = TransitionTime;
        visual.StartAnimation("Offset", offsetAnimation);
    }

    private void AnimateSlideIn(Image myImage, TimeSpan TransitionTime)
    {
        var slideWidth = (float)(myImage.ActualWidth + 1.0);
        AnimateHorizonal(myImage, TransitionTime, direction * slideWidth, 0f);    
        myImage.GetVisualInternal().Opacity = 1.0f;
    }

    private void AnimateSlideOut(Image myImage, TimeSpan TransitionTime)
    {
        var slideWidth = (float)(myImage.ActualWidth + 1.0);
        AnimateHorizonal(myImage, TransitionTime, 0f, -direction * slideWidth);
        myImage.GetVisualInternal().Opacity = 1.0f;
    }

    public override void AnimateInternal(Image newImageControl, Image oldImageControl, TimeSpan transitionTime)
    {
        AnimateSlideIn(newImageControl, transitionTime);
        AnimateSlideOut(oldImageControl, transitionTime);
        direction = -direction; // Toggle direction for next animation
    }
}
