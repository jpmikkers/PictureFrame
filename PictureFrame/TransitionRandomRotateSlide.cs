using System;
using Microsoft.UI.Xaml.Controls;
using System.Numerics;

namespace PictureFrame;

public class TransitionRandomRotateSlide : Transition
{
    public TransitionRandomRotateSlide()
    {
        DisplayName = "Random rotate & slide";
    }

    private static void SetVisualCenterPoint(Image myImage, Microsoft.UI.Composition.Visual visual) => visual.CenterPoint = new Vector3((float)(myImage.ActualWidth / 2.0), (float)(myImage.ActualHeight / 2.0), 0);

    private static Vector3 GetTargetVector(Image myImage)
    {
        var v = new Vector3(0.8f * (float)Math.Max(myImage.ActualHeight, myImage.ActualWidth), 0, 0);
        var m = Matrix4x4.CreateRotationZ((float)(Random.Shared.NextDouble() * 2.0 * Math.PI));
        var rotatedVector = Vector3.Transform(v, m);
        return rotatedVector;
    }

    // Method to animate zooming
    private void AnimateSlideIn(Image myImage, TimeSpan TransitionTime)
    {
        // Get the Visual of the Image
        var visual = myImage.GetVisualInternal();
        SetVisualCenterPoint(myImage, visual);

        // Access the Compositor
        var compositor = visual.Compositor;

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
        scaleAnimation.InsertKeyFrame(0f, new Vector3(1.5f, 1.5f, 1f));
        scaleAnimation.InsertKeyFrame(1f, new Vector3(1f, 1f, 1f));
        scaleAnimation.Duration = TransitionTime;

        visual.StartAnimation("Scale", scaleAnimation);

        // Create a scalar keyframe animation for rotation
        var rotationAnimation = compositor.CreateScalarKeyFrameAnimation();
        rotationAnimation.InsertKeyFrame(0f, Random.Shared.Next(-45, 45));
        rotationAnimation.InsertKeyFrame(1f, 0f);
        rotationAnimation.Duration = TransitionTime;

        // Start the animation
        visual.StartAnimation("RotationAngleInDegrees", rotationAnimation);
    }


    private void AnimateSlideOut(Image myImage, TimeSpan TransitionTime)
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
        rotationAnimation.Duration = TransitionTime;

        // Start the animation
        visual.StartAnimation("RotationAngleInDegrees", rotationAnimation);
    }

    public override void AnimateInternal(Image newImageControl, Image oldImageControl, TimeSpan transitionTime)
    {
        AnimateSlideIn(newImageControl, transitionTime);
        AnimateSlideOut(oldImageControl, transitionTime);
    }
}
