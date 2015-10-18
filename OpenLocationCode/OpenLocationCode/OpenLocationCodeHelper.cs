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
    /// <summary>
    /// Some common static properties of Open localtion code
    /// </summary>
    internal static class OpenLocationCodeHelper
    {
        /// <summary>
        /// Needs to same result precision that javascript reference implementation
        /// </summary>
        internal const int DecodeDigitsRound = 11;

        /// <summary>
        /// The character set used to encode the values.
        /// </summary>
        internal static readonly char[] CodeAlphabet = new[] { '2', '3', '4', '5', '6', '7', '8', '9', 'C', 'F', 'G', 'H', 'J', 'M', 'P', 'Q', 'R', 'V', 'W', 'X' };

        /// <summary>
        /// The character indexes used to encode the values.
        /// </summary>
        internal static readonly Dictionary<char, int> CodeAlphabetIndex = new Dictionary<char, int>
        {
            {'2', 0 }, {'3', 1 }, {'4', 2 }, {'5', 3 }, {'6', 4 }, {'7', 5 }, {'8', 6 }, {'9', 7 },
            { 'C', 8}, { 'c', 8},
            { 'F', 9}, { 'f', 9},
            { 'G', 10}, { 'g', 10},
            { 'H', 11}, { 'h', 11},
            { 'J', 12}, { 'j', 12},
            { 'M', 13}, { 'm', 13},
            { 'P', 14}, { 'p', 14},
            { 'Q', 15}, { 'q', 15},
            { 'R', 16}, { 'r', 16},
            { 'V', 17}, { 'v', 17},
            { 'W', 18}, { 'w', 18},
            { 'X', 19}, { 'x', 19},
        };

        /// <summary>
        /// The base to use to convert numbers to/from.
        /// </summary>
        internal static readonly int EncodingBase = CodeAlphabet.Length;

        /// <summary>
        /// The maximum value for latitude in degrees.
        /// </summary>
        internal const int LatitudeMax = 90;

        /// <summary>
        /// The maximum value for longitude in degrees.
        /// </summary>
        internal const int LongitudeMax = 180;

        /// <summary>
        /// Maxiumum code length using lat/lng pair encoding. The area of such a
        /// code is approximately 13x13 meters (at the equator), and should be suitable
        /// for identifying buildings. This excludes prefix and separator characters.
        /// </summary>
        internal const int PairCodeLength = 10;

        /// <summary>
        /// The resolution values in degrees for each position in the lat/lng pair
        /// encoding. These give the place value of each position, and therefore the
        /// dimensions of the resulting area.
        /// </summary>
        internal static readonly decimal[] PairResolutions = { 20.0m, 1.0m, .05m, .0025m, .000125m };

        /// <summary>
        /// Number of columns in the grid refinement method.
        /// </summary>
        internal const int GridColumns = 4;

        /// <summary>
        /// Number of rows in the grid refinement method.
        /// </summary>
        internal const int GridRows = 5;

        /// <summary>
        /// Size of the initial grid in degrees.
        /// </summary>
        internal const decimal GridSizeDegrees = 0.000125m;

        /// <summary>
        /// Clip a latitude into the range -90 to 90.
        /// </summary>
        /// <param name="latitude">A latitude in signed decimal degrees.</param>
        /// <returns></returns>
        internal static decimal ClipLatitude(decimal latitude)
        {
            return Math.Min(90m, Math.Max(-90m, latitude));
        }

        /// <summary>
        /// Normalize a longitude into the range -180 to 180, not including 180.
        /// </summary>
        /// <param name="longitude">A longitude in signed decimal degrees.</param>
        /// <returns></returns>
        internal static decimal NormalizeLongitude(decimal longitude)
        {
            while (longitude < -180)
            {
                longitude = longitude + 360;
            }
            while (longitude >= 180)
            {
                longitude = longitude - 360;
            }
            return longitude;
        }

        /// <summary>
        /// Compute the latitude precision value for a given code length. Lengths &lt;=
        /// 10 have the same precision for latitude and longitude, but lengths &gt; 10
        /// have different precisions due to the grid method having fewer columns than rows.
        /// </summary>
        /// <param name="codeLength"></param>
        /// <returns></returns>
        internal static decimal ComputeLatitudePrecision(int codeLength)
        {
            if (codeLength <= 10)
            {
                return Convert.ToDecimal(Math.Pow(20d, Math.Floor(codeLength / -2d + 2d)));
            }
            return Convert.ToDecimal(Math.Pow(20d, -3) / Math.Pow(GridRows, codeLength - 10d));
        }
    }
}
