﻿using System;
using System.ComponentModel;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedParameter.Global
// ReSharper disable ParameterOnlyUsedForPreconditionCheck.Global
// ReSharper disable CheckNamespace

namespace Ardalis.GuardClauses
{
    public static partial class GuardClauseExtensions
    {
        /// <summary>
        ///     Throws an <see cref="InvalidEnumArgumentException" /> if <see cref="input" /> is not a valid value for the defined
        ///     <see cref="enumClass" />.
        /// </summary>
        /// <param name="guardClause"></param>
        /// <param name="input"></param>
        /// <param name="enumClass"></param>
        /// <param name="parameterName"></param>
        /// <exception cref="InvalidEnumArgumentException"></exception>
        public static void Enum(this IGuardClause guardClause, object input, Type enumClass, string parameterName) {
            if (!System.Enum.IsDefined(enumClass, input))
                throw new GuardException(new InvalidEnumArgumentException(parameterName, (int) input, enumClass));
        }
    }
}