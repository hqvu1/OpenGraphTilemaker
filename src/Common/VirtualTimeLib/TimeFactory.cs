﻿using System;
using System.Diagnostics.CodeAnalysis;

// ReSharper disable CheckNamespace

namespace VirtualTimeLib
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Utility class")]
    public static class TimeFactory
    {
        /// <summary>
        ///     Get virtual time from supplied time reference point
        /// </summary>
        /// <param name="startTime">The reference time from which the elapsed time is computed</param>
        /// <param name="speedOfTimePerMs">
        ///     at value more than 1 time goes faster. At less then 1 time goes slower. At 0 time stands still
        /// </param>
        /// <param name="marginOfErrorMs">To what degree is the virtual time correct</param>
        public static ITime ToVirtualTime(this DateTime startTime, double speedOfTimePerMs = 1, int marginOfErrorMs = 10) =>
            new VirtualTime(startTime, speedOfTimePerMs, marginOfErrorMs);

        /// <summary>
        ///     A wrapper around System.DateTime
        /// </summary>
        public static ITime RealTime() => new VirtualTime();
    }
}
