using System;
using System.Numerics;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;

namespace PictureFrame;

public class TransitionNone : Transition
{
    public TransitionNone()
    {
        DisplayName = "None";
    }

    public override void AnimateInternal(Image newImage, Image oldImage, TimeSpan transitionTime)
    {
        newImage.GetVisualInternal().Opacity = 1.0f;
        oldImage.GetVisualInternal().Opacity = 0.0f;
    }
}
