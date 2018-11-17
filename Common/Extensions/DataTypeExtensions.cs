﻿using System;
using JetBrains.Annotations;

namespace Common.Extensions
{
    public static class DataTypeExtensions
    {
        public static int ToInt32(this object arg) => Convert.ToInt32(arg);

        [CanBeNull]
        public static int? AsIntOrNull(this string value) => int.TryParse(value, out var result) ? (int?) result : null;

        [CanBeNull]
        public static DateTime? AsDateTimeOrNull(this string value) => DateTime.TryParse(value, out var result) ? (DateTime?) result : null;
    }
}