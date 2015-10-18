// Copyright 2015 Asseco Solutions All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the 'License');
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an 'AS IS' BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// .Net port of Google Open Location Codes.
// Open Location Codes are short, generated codes that can be used like street addresses, for places where street addresses don't exist.
// Open Location Codes were developed at Google's Zurich engineering office, and then open sourced so that they can be freely used.
// http://openlocationcode.com. 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOL.OpenLocationCode
{
    #region helper extension for portability support
    internal static class MathExtension
    {
        public static decimal Floor(decimal value)
        {
            return Convert.ToDecimal(Math.Floor(Convert.ToDouble(value)));
        }

        public static int FloorInt(decimal value)
        {
            return Convert.ToInt32(Math.Floor(Convert.ToDouble(value)));
        }

        public static decimal Pow(decimal x, decimal y)
        {
            return Convert.ToDecimal(Math.Pow(Convert.ToDouble(x), Convert.ToDouble(y)));
        }
    }

    internal static class StringExtension
    {
        public static string RemoveCharacter(this string value, char character)
        {
            return value.Replace(character.ToString(), string.Empty);
        }
    }
    #endregion
}
