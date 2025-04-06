using System;
using Microsoft.UI.Xaml.Controls;
using System.Numerics;

namespace PictureFrame;

public class TransitionFade : Transition
{
    public TransitionFade()
    {
        DisplayName = "Fade";
    }

    private void AnimateFade(Image myImage, TimeSpan TransitionTime, float startOpacity, float endOpacity)
    {
        // Get the Visual of the Image
        var visual = myImage.GetVisualInternal();
        var compositor = visual.Compositor;

        var opacityAnimation = compositor.CreateScalarKeyFrameAnimation();
        opacityAnimation.InsertKeyFrame(0f, startOpacity);
        opacityAnimation.InsertKeyFrame(1f, endOpacity);
        opacityAnimation.Duration = TransitionTime;
        visual.StartAnimation("Opacity", opacityAnimation);
    }

    public override void AnimateInternal(Image newImageControl, Image oldImageControl, TimeSpan transitionTime)
    {
        AnimateFade(newImageControl, transitionTime, 0.0f, 1.0f);
        AnimateFade(oldImageControl, transitionTime, 1.0f, 0.0f);
    }
}
