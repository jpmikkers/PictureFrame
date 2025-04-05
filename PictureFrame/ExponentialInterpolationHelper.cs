using System;

namespace PictureFrame;

internal static class ExponentialInterpolationHelper
{
    public static double ExponentialInterpolation(
        double linearInput,
        double linearInputMin,
        double linearInputMax,
        double exponentialOutputMin,
        double exponentialOutputMax)
    {
        // scale input from input range to 0-1
        var scaledInput = (linearInput - linearInputMin) / (linearInputMax - linearInputMin);
        var lerpedExponent = Double.Lerp(0, Math.Log(1.0 + exponentialOutputMax - exponentialOutputMin), scaledInput);
        return Math.Exp(lerpedExponent) - 1.0 + exponentialOutputMin;
    }

    public static double InverseExponentialInterpolation(
        double exponentialInput,
        double exponentialInputMin,
        double exponentialInputMax,
        double linearOutputMin,
        double linearOutputMax)
    {
        exponentialInput = Double.Clamp(
            exponentialInput,
            exponentialInputMin,
            exponentialInputMax);

        var lerpedExponent = Math.Log(exponentialInput - exponentialInputMin + 1);
        var scaledInput = lerpedExponent / Math.Log(1.0 + exponentialInputMax - exponentialInputMin);
        return scaledInput * (linearOutputMax - linearOutputMin) + linearOutputMin;
    }

    public static double RoundToNearest(double value, double multiple)
    {
        return Math.Round(value / multiple) * multiple;
    }

    public static double IntervalToPromille(TimeSpan interval)
    {
        return ExponentialInterpolationHelper.InverseExponentialInterpolation(interval.TotalSeconds, 0.5, 60 * 60, 0, 1000);
    }

    public static TimeSpan PromilleToInterval(double promille)
    {
        var timeInSeconds = ExponentialInterpolationHelper.ExponentialInterpolation(promille, 0, 1000, 0.5, 60 * 60);

        if (timeInSeconds <= 2.0)
        {
            timeInSeconds = ExponentialInterpolationHelper.RoundToNearest(timeInSeconds, 0.1);
        }
        else if (timeInSeconds < 10.0)
        {
            timeInSeconds = ExponentialInterpolationHelper.RoundToNearest(timeInSeconds, 0.5);
        }
        else if (timeInSeconds < 60.0)
        {
            timeInSeconds = ExponentialInterpolationHelper.RoundToNearest(timeInSeconds, 1);
        }
        else if (timeInSeconds < 600.0)
        {
            timeInSeconds = ExponentialInterpolationHelper.RoundToNearest(timeInSeconds, 5);
        }
        else
        {
            timeInSeconds = ExponentialInterpolationHelper.RoundToNearest(timeInSeconds, 30);
        }
        return TimeSpan.FromSeconds(timeInSeconds);
    }
}