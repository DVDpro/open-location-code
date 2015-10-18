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

namespace ASOL.OpenLocationCode
{
    /// <summary>
    /// Coordinates of a decoded Open Location Code.
    /// The coordinates include the latitude and longitude of the lower left and
    /// upper right corners and the center of the bounding box for the area the
    /// code represents.
    /// </summary>
    public sealed class CodeArea
    {
        /// <summary>
        /// The latitude of the SW corner in degrees. 
        /// </summary>
        public decimal LatitudeLow { get; private set; }

        /// <summary>
        /// The longitude of the SW corner in degrees. 
        /// </summary>
        public decimal LongitudeLow { get; private set; }

        /// <summary>
        /// The latitude of the NE corner in degrees. 
        /// </summary>
        public decimal LatitudeHigh { get; private set; }

        /// <summary>
        /// The longitude of the NE corner in degrees.
        /// </summary>
        public decimal LongitudeHigh { get; private set; }

        /// <summary>
        /// The number of significant characters that were in the code. This excludes the separator. 
        /// </summary>
        public int CodeLength { get; private set; }

        /// <summary>
        /// The latitude of the center in degrees. 
        /// </summary>
        public decimal LatitudeCenter { get; internal set; }

        /// <summary>
        /// The longitude of the center in degrees. 
        /// </summary>
        public decimal LongitudeCenter { get; internal set; }

        /// <summary>
        /// Coordinates of a decoded Open Location Code.
        /// </summary>
        /// <param name="latitudeLow">The latitude of the SW corner in degrees.</param>
        /// <param name="longitudeLow">The longitude of the SW corner in degrees.</param>
        /// <param name="latitudeHigh">The latitude of the NE corner in degrees.</param>
        /// <param name="longitudeHigh">The longitude of the NE corner in degrees.</param>
        /// <param name="codeLength"></param>
        public CodeArea(decimal latitudeLow, decimal longitudeLow, decimal latitudeHigh, decimal longitudeHigh, int codeLength)
        {
            LatitudeLow = latitudeLow;
            LongitudeLow = longitudeLow;
            LatitudeHigh = latitudeHigh;
            LongitudeHigh = longitudeHigh;
            CodeLength = codeLength;
            LatitudeCenter = Math.Min(latitudeLow + (latitudeHigh - latitudeLow) / 2, OpenLocationCodeHelper.LatitudeMax);
            LongitudeCenter = Math.Min(longitudeLow + (longitudeHigh - longitudeLow) / 2, OpenLocationCodeHelper.LongitudeMax);
        }
    }    
}
