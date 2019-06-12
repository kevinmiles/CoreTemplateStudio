﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Templates.Core.Templates
{
    public static class CasingExtensions
    {
        private static readonly char[] Separators = { ' ', '-', '_' };

        private static Dictionary<CasingType, string> ParameterNames { get; set; } = new Dictionary<CasingType, string>
        {
            { CasingType.Pascal, "wts.sourceName.casing.pascal" },
            { CasingType.Kebab, "wts.sourceName.casing.kebab" },
        };

        public static string GetParameterName(this CasingType casing)
        {
            if (ParameterNames.ContainsKey(casing))
            {
                return ParameterNames[casing].ToString();
            }

            return string.Empty;
        }

        public static string GetTransform(this CasingType casing, string name)
        {
            switch (casing)
            {
                case CasingType.Kebab:
                    return ToKebabCase(name);
                case CasingType.Pascal:
                    return ToPascalCase(name);
                case CasingType.Camel:
                    return ToCamelCase(name);
                default:
                    return name;
            }
        }

        private static string ToKebabCase(string name)
        {
            return Transform(name, '-').ToLower();
        }

        private static string ToPascalCase(string name)
        {
            return Transform(name, null, ToUpperCase);
        }

        private static string ToCamelCase(string name)
        {
            return Transform(name, null, ToUpperCaseExceptFirstLetter);
        }

        private static char ToUpperCase(char c, int i)
        {
            return char.ToUpper(c);
        }

        private static char ToUpperCaseExceptFirstLetter(char c, int i)
        {
            if (i == 0)
            {
                return char.ToLower(c);
            }
            else
            {
                return char.ToUpper(c);
            }
        }

        private static string Transform(string name, char? separator, Func<char, int, char> newWordTreatment = null)
        {
            name = name.Trim();
            var builder = new StringBuilder();
            var lastChar = name[0];

            for (var i = 0; i < name.Length; i++)
            {
                var currentCharacter = name[i];

                if (Separators.Contains(currentCharacter))
                {
                    builder.Append(separator);
                }

                if (char.IsLetterOrDigit(currentCharacter))
                {
                    if (IsNewWord(currentCharacter, lastChar, i))
                    {
                        // new word treatment
                        // do not insert duplicate separators and do not use separators at beginning
                        if (i != 0 && separator.HasValue && !Separators.Contains(lastChar))
                        {
                            builder.Append(separator);
                        }

                        if (newWordTreatment != null)
                        {
                            builder.Append(newWordTreatment(currentCharacter, i));
                        }
                        else
                        {
                            builder.Append(currentCharacter);
                        }
                    }
                    else
                    {
                        builder.Append(currentCharacter);
                    }
                }

                lastChar = name[i];
            }

            return builder.ToString();
        }

        private static bool IsNewWord(char currentCharacter, char lastChar, int i)
        {
            if (i == 0)
            {
                return true;
            }

            if (char.IsUpper(currentCharacter) && char.IsLower(lastChar))
            {
                return true;
            }

            if (char.IsDigit(currentCharacter) && !char.IsDigit(lastChar))
            {
                return true;
            }

            if (Separators.Contains(lastChar))
            {
                return true;
            }

            return false;
        }
    }
}
