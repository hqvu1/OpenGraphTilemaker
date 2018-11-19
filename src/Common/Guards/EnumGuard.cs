﻿using System;
using System.ComponentModel;
using System.Linq.Expressions;
using JetBrains.Annotations;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedParameter.Global
// ReSharper disable ParameterOnlyUsedForPreconditionCheck.Global
// ReSharper disable CheckNamespace
#pragma warning disable SA1614 // Element parameter documentation should have text
#pragma warning disable SA1627 // Documentation text should not be empty

namespace Ardalis.GuardClauses
{
    /// <summary>
    ///     EnumGuard.
    /// </summary>
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
                throw new GuardException(new InvalidEnumArgumentException(parameterName, (int)input, enumClass));
        }

        /// <summary>
        ///     Throws an <see cref="InvalidEnumArgumentException" /> if <see cref="input" /> is not a valid value for the defined
        ///     <see cref="enumClass" />.
        /// </summary>
        /// <param name="guardClause"></param>
        /// <param name="input"></param>
        /// <param name="enumClass"></param>
        /// <exception cref="ArgumentException">The <paramref name="input" /> expression is invalid.</exception>
        /// <exception cref="InvalidEnumArgumentException"></exception>
        public static void Enum<T>(this IGuardClause guardClause, [NotNull] Expression<Func<T>> input, Type enumClass) {
            Guard.Against.Enum(input.Compile()(), enumClass, input.MemberExpressionName());
        }
    }
}

#pragma warning restore SA1614 // Element parameter documentation should have text
#pragma warning restore SA1627 // Documentation text should not be empty