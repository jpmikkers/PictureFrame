using System;
using System.Numerics;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml.Controls;

namespace PictureFrame;

public abstract class Transition
{
    public string DisplayName { get; set; } = string.Empty;

    public abstract void AnimateInternal(Image newImage, Image oldImage, TimeSpan transitionTime);

    private static void ResetAnimation(Image image, float opacity)
    {
        var visual = image.GetVisualInternal();
        visual.StopAnimation("Opacity");
        visual.StopAnimation("Offset");
        visual.StopAnimation("RotationAngleInDegrees");
        visual.StopAnimation("Scale");
        visual.Offset = Vector3.Zero;
        visual.RotationAngleInDegrees = 0.0f;
        visual.Scale = Vector3.One;
        visual.Opacity = opacity;
    }

    public void Animate(Image newImage, Image oldImage, TimeSpan transitionTime)
    {
        ResetAnimation(newImage, 0.0f);
        ResetAnimation(oldImage, 1.0f);
        AnimateInternal(newImage, oldImage, transitionTime);
    }
}
