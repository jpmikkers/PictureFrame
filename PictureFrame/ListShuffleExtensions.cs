using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace PictureFrame;

public static class ListShuffleExtensions
{
    public static List<T> ShuffleInPlace<T>(this List<T> list)
        where T : class
    {
        Random.Shared.Shuffle(CollectionsMarshal.AsSpan(list));
        return list;
    }

    public static List<T> ShuffleToNew<T>(this List<T> list)
        where T : class
    {
        return new List<T>(list).ShuffleInPlace();
    }
}
