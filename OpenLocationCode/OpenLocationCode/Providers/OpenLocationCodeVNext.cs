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
using System.Text;

namespace ASOL.OpenLocationCode.Providers
{
    /// <summary>
    /// Representation of open location code latest unreleased version.  http://openlocationcode.com
    /// </summary>
    internal sealed class OpenLocationCodeVNext : IOpenLocationCode
    {
        /// <summary>
        /// A separator used to break the code into two parts to aid memorability.
        /// </summary>
        private const char Separator = '+';

        /// <summary>
        /// The number of characters to place before the separator. 
        /// </summary>
        private const int SeparatorPosition = 8;

        /// <summary>
        /// The character used to pad codes.
        /// </summary>
        private const char PaddingCharacter = '0';

        /// <summary>
        /// Minimum length of a code that can be shortened.
        /// </summary>
        private const int MinTrimmableCodeLen = 6;

        private static readonly System.Text.RegularExpressions.Regex PaddingCharacterRegex = new System.Text.RegularExpressions.Regex("(" + PaddingCharacter + "+)");
        private static readonly System.Text.RegularExpressions.Regex PaddingCharacterRegexReplace = new System.Text.RegularExpressions.Regex(PaddingCharacter + "+");
        private static readonly System.Text.RegularExpressions.Regex SeparatorRegex = new System.Text.RegularExpressions.Regex("\\" + Separator + "+");

        /// <summary>
        /// Determines if a code is valid.
        /// 
        /// To be valid, all characters must be from the Open Location Code character
        /// set with at most one separator.If the prefix character is present, it
        /// must be the first character. If the separator character is present,
        /// it must be after four characters.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool IsValid(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return false;
            }
            // The separator is required.
            if (code.IndexOf(Separator) == -1)
            {
                return false;
            }
            if (code.IndexOf(Separator) != code.LastIndexOf(Separator))
            {
                return false;
            }
            // Is it the only character?
            if (code.Length == 1)
            {
                return false;
            }
            // Is it in an illegal position?
            if (code.IndexOf(Separator) > SeparatorPosition || code.IndexOf(Separator) % 2 == 1)
            {
                return false;
            }
            // We can have an even number of padding characters before the separator,
            // but then it must be the final character.
            if (code.IndexOf(PaddingCharacter) > -1)
            {
                // Not allowed to start with them!
                if (code.IndexOf(PaddingCharacter) == 0)
                {
                    return false;
                }
                // There can only be one group and it must have even length.
                var padMatch = PaddingCharacterRegex.Matches(code);
                if (padMatch.Count > 1 || padMatch[0].Length % 2 == 1 || padMatch[0].Length > SeparatorPosition - 2)
                {
                    return false;
                }
                // If the code is long enough to end with a separator, make sure it does.
                if (code[code.Length - 1] != Separator)
                {
                    return false;
                }
            }
            // If there are characters after the separator, make sure there isn't just
            // one of them (not legal).
            if (code.Length - code.IndexOf(Separator) - 1 == 1)
            {
                return false;
            }

            // Strip the separator and any padding characters.
            code = SeparatorRegex.Replace(code, string.Empty);
            code = PaddingCharacterRegexReplace.Replace(code, string.Empty);
            // Check the code contains only valid characters.
            foreach (var character in code)
            {
                if (character != Separator && OpenLocationCodeHelper.CodeAlphabetIndex.ContainsKey(character) == false)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Determines if a code is a valid short code.
        /// 
        /// A short Open Location Code is a sequence created by removing the first
        /// four or six characters from a full Open Location Code.
        /// 
        /// A code must be a possible sub-string of a generated Open Location Code, at
        /// least four and at most seven characters long and not include a separator
        /// character.If the prefix character is present, it must be the first character.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool IsShort(string code)
        {
            // Check it's valid.
            if (!IsValid(code))
            {
                return false;
            }
            // If there are less characters than expected before the SEPARATOR.
            if (code.IndexOf(Separator) >= 0 &&
                code.IndexOf(Separator) < SeparatorPosition)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Determines if a code is a valid full Open Location Code.
        /// 
        /// Not all possible combinations of Open Location Code characters decode to
        /// valid latitude and longitude values.This checks that a code is valid
        /// and also that the latitude and longitude values are legal.If the prefix
        /// character is present, it must be the first character. If the separator
        /// character is present, it must be after four characters.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool IsFull(string code)
        {
            if (!IsValid(code))
            {
                return false;
            }
            // If it's short, it's not full.
            if (IsShort(code))
            {
                return false;
            }

            // Work out what the first latitude character indicates for latitude.
            var firstLatValue = OpenLocationCodeHelper.CodeAlphabetIndex[code[0]] * OpenLocationCodeHelper.EncodingBase;
            if (firstLatValue >= OpenLocationCodeHelper.LatitudeMax * 2)
            {
                // The code would decode to a latitude of >= 90 degrees.
                return false;
            }
            if (code.Length > 1)
            {
                // Work out what the first longitude character indicates for longitude.
                var firstLngValue = OpenLocationCodeHelper.CodeAlphabetIndex[code[1]] * OpenLocationCodeHelper.EncodingBase;
                if (firstLngValue >= OpenLocationCodeHelper.LongitudeMax * 2)
                {
                    // The code would decode to a longitude of >= 180 degrees.
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Encode a location into an Open Location Code.
        /// 
        /// Produces a code of the specified length, or the default length if no length is provided.
        /// 
        /// The length determines the accuracy of the code.The default length is
        /// 10 characters, returning a code of approximately 13.5x13.5 meters.Longer
        /// codes represent smaller areas, but lengths > 14 are sub-centimetre and so
        /// 11 or 12 are probably the limit of useful codes.
        /// </summary>
        /// <param name="latitude">A latitude in signed decimal degrees. Will be clipped to the range -90 to 90.</param>
        /// <param name="longitude">A longitude in signed decimal degrees. Will be normalised to the range -180 to 180.</param>
        /// <param name="codeLength">(optional) The number of significant digits in the output code, not including any separator characters.</param>
        /// <returns></returns>
        public string Encode(decimal latitude, decimal longitude, int? codeLength = default(int?))
        {
            if (codeLength.HasValue == false)
            {
                codeLength = OpenLocationCodeHelper.PairCodeLength;
            }
            if (codeLength.Value < 2 || (codeLength.Value < SeparatorPosition && codeLength.Value % 2 == 1))
            {
                throw new ArgumentException(Properties.Resources.InvalidOLCLength, "codeLength");
            }
            // Ensure that latitude and longitude are valid.
            latitude = OpenLocationCodeHelper.ClipLatitude(latitude);
            longitude = OpenLocationCodeHelper.NormalizeLongitude(longitude);
            // Latitude 90 needs to be adjusted to be just less, so the returned code
            // can also be decoded.
            if (latitude == 90)
            {
                latitude = latitude - OpenLocationCodeHelper.ComputeLatitudePrecision(codeLength.Value);
            }
            var code = encodePairs(latitude, longitude, Math.Min(codeLength.Value, OpenLocationCodeHelper.PairCodeLength));
            // If the requested length indicates we want grid refined codes.
            if (codeLength.Value > OpenLocationCodeHelper.PairCodeLength)
            {
                code += encodeGrid(latitude, longitude, codeLength.Value - OpenLocationCodeHelper.PairCodeLength);
            }
            return code;
        }

        /// <summary>
        /// Decodes an Open Location Code into the location coordinates.
        /// 
        /// Returns a CodeArea object that includes the coordinates of the bounding
        /// box - the lower left, center and upper right.
        /// </summary>
        /// <param name="code">The Open Location Code to decode.</param>
        /// <returns>
        /// A CodeArea object that provides the latitude and longitude of two of the
        /// corners of the area, the center, and the length of the original code.
        /// </returns>
        public CodeArea Decode(string code)
        {
            if (!IsFull(code))
            {
                throw new ArgumentException(string.Format(System.Globalization.CultureInfo.CurrentUICulture, Properties.Resources.PassedOLCIsNotFull, code));
            }
            // Strip out separator character (we've already established the code is
            // valid so the maximum is one), padding characters and convert to upper
            // case.
            code = code.RemoveCharacter(Separator);
            code = PaddingCharacterRegexReplace.Replace(code, string.Empty);
            code = code.ToUpper();
            // Decode the lat/lng pair component.
            var codeArea = decodePairs(code.Substring(0, Math.Min(code.Length, OpenLocationCodeHelper.PairCodeLength)));
            // If there is a grid refinement component, decode that.
            if (code.Length <= OpenLocationCodeHelper.PairCodeLength)
            {
                return codeArea;
            }
            var gridArea = decodeGrid(code.Substring(OpenLocationCodeHelper.PairCodeLength));
            return new CodeArea(
              Math.Round(codeArea.LatitudeLow + gridArea.LatitudeLow, OpenLocationCodeHelper.DecodeDigitsRound, MidpointRounding.AwayFromZero),
              Math.Round(codeArea.LongitudeLow + gridArea.LongitudeLow, OpenLocationCodeHelper.DecodeDigitsRound, MidpointRounding.AwayFromZero),
              Math.Round(codeArea.LatitudeLow + gridArea.LatitudeHigh, OpenLocationCodeHelper.DecodeDigitsRound, MidpointRounding.AwayFromZero),
              Math.Round(codeArea.LongitudeLow + gridArea.LongitudeHigh, OpenLocationCodeHelper.DecodeDigitsRound, MidpointRounding.AwayFromZero),
              codeArea.CodeLength + gridArea.CodeLength);
        }

        /// <summary>
        /// Recover the nearest matching code to a specified location.
        /// 
        /// Given a short Open Location Code of between four and seven characters,
        /// this recovers the nearest matching full code to the specified location.
        /// 
        /// The number of characters that will be prepended to the short code, where S
        /// is the supplied short code and R are the computed characters, are:
        /// SSSS    -> RRRR.RRSSSS
        /// SSSSS   -> RRRR.RRSSSSS
        /// SSSSSS  -> RRRR.SSSSSS
        /// SSSSSSS -> RRRR.SSSSSSS
        /// Note that short codes with an odd number of characters will have their
        /// last character decoded using the grid refinement algorithm.
        /// </summary>
        /// <param name="shortCode">A valid short OLC character sequence.</param>
        /// <param name="referenceLatitude">The latitude (in signed decimal degrees) to use to find the nearest matching full code.</param>
        /// <param name="referenceLongitude">The longitude(in signed decimal degrees) to use to find the nearest matching full code.</param>
        /// <returns>
        /// The nearest full Open Location Code to the reference location that matches
        /// the short code.Note that the returned code may not have the same
        /// computed characters as the reference location.This is because it returns
        /// the nearest match, not necessarily the match within the same cell. If the
        /// passed code was not a valid short code, but was a valid full code, it is
        /// returned unchanged.
        /// </returns>
        public string RecoverNearest(string shortCode, decimal referenceLatitude, decimal referenceLongitude)
        {
            if (!IsShort(shortCode))
            {
                if (IsFull(shortCode))
                {
                    return shortCode;
                }
                else
                {
                    throw new ArgumentException(string.Format(System.Globalization.CultureInfo.CurrentUICulture, Properties.Resources.PassedShortCodeIsNotValid, shortCode), "shortCode");
                }
            }
            // Ensure that latitude and longitude are valid.
            referenceLatitude = OpenLocationCodeHelper.ClipLatitude(referenceLatitude);
            referenceLongitude = OpenLocationCodeHelper.NormalizeLongitude(referenceLongitude);

            // Clean up the passed code.
            shortCode = shortCode.ToUpper();
            // Compute the number of digits we need to recover.
            var paddingLength = SeparatorPosition - shortCode.IndexOf(Separator);
            // The resolution (height and width) of the padded area in degrees.
            var resolution = MathExtension.Pow(20, 2 - (paddingLength / 2));
            // Distance from the center to an edge (in degrees).
            var areaToEdge = resolution / 2.0m;

            // Now round down the reference latitude and longitude to the resolution.
            var roundedLatitude = MathExtension.Floor(referenceLatitude / resolution) * resolution;
            var roundedLongitude = MathExtension.Floor(referenceLongitude / resolution) * resolution;

            // Use the reference location to pad the supplied short code and decode it.
            var codeArea = Decode(Encode(roundedLatitude, roundedLongitude).Substring(0, paddingLength) + shortCode);
            // How many degrees latitude is the code from the reference? If it is more
            // than half the resolution, we need to move it east or west.
            var degreesDifference = codeArea.LatitudeCenter - referenceLatitude;
            if (degreesDifference > areaToEdge)
            {
                // If the center of the short code is more than half a cell east,
                // then the best match will be one position west.
                codeArea.LatitudeCenter -= resolution;
            }
            else if (degreesDifference < -areaToEdge)
            {
                // If the center of the short code is more than half a cell west,
                // then the best match will be one position east.
                codeArea.LatitudeCenter += resolution;
            }

            // How many degrees longitude is the code from the reference?
            degreesDifference = codeArea.LongitudeCenter - referenceLongitude;
            if (degreesDifference > areaToEdge)
            {
                codeArea.LongitudeCenter -= resolution;
            }
            else if (degreesDifference < -areaToEdge)
            {
                codeArea.LongitudeCenter += resolution;
            }

            return Encode(codeArea.LatitudeCenter, codeArea.LongitudeCenter, codeArea.CodeLength);
        }

        /// <summary>
        /// Try to remove the first four characters from an OLC code.
        /// 
        /// This uses a reference location to determine if the first four characters
        /// can be removed from the OLC code.The reference location must be within
        /// +/- 0.25 degrees of the code center.This allows the original code to be
        /// recovered using this location, with a safety margin.
        /// </summary>
        /// <param name="code">A full, valid code to shorten.</param>
        /// <param name="latitude"> A latitude, in signed decimal degrees, to use as the reference point.</param>
        /// <param name="longitude">A longitude, in signed decimal degrees, to use as the reference point.</param>
        /// <returns>
        /// The OLC code with the first four characters removed.If the reference
        /// location is not close enough, the passed code is returned unchanged.
        /// </returns>
        public string ShortenBy4(string code, decimal latitude, decimal longitude)
        {
            throw new InvalidOperationException("Format version vNext not supported ShortenBy4 method. Use shorten.");
        }

        /// <summary>
        /// Try to remove the first six characters from an OLC code.
        /// 
        /// This uses a reference location to determine if the first six characters
        /// can be removed from the OLC code.The reference location must be within
        /// +/- 0.0125 degrees of the code center.This allows the original code to be
        /// recovered using this location, with a safety margin.
        /// </summary>
        /// <param name="code">A full, valid code to shorten.</param>
        /// <param name="latitude"> A latitude, in signed decimal degrees, to use as the reference point.</param>
        /// <param name="longitude">A longitude, in signed decimal degrees, to use as the reference point.</param>
        /// <returns>
        /// The OLC code with the first six characters removed.If the reference
        /// location is not close enough, the passed code is returned unchanged.
        /// </returns>
        public string ShortenBy6(string code, decimal latitude, decimal longitude)
        {
            throw new InvalidOperationException("Format version vNext not supported ShortenBy6 method. Use shorten.");
        }

        /// <summary>
        /// Remove characters from the start of an OLC code.
        /// 
        /// This uses a reference location to determine how many initial characters
        /// can be removed from the OLC code.The number of characters that can be
        /// removed depends on the distance between the code center and the reference
        /// location.
        /// 
        /// The minimum number of characters that will be removed is four.If more than
        /// four characters can be removed, the additional characters will be replaced
        /// with the padding character. At most eight characters will be removed.
        /// The reference location must be within 50% of the maximum range. This ensures
        /// that the shortened code will be able to be recovered using slightly different
        /// </summary>
        /// <param name="code">A full, valid code to shorten.</param>
        /// <param name="latitude">A latitude, in signed decimal degrees, to use as the reference point.</param>
        /// <param name="longitude">A longitude, in signed decimal degrees, to use as the reference point.</param>
        /// <returns>Either the original code, if the reference location was not close enough, or the .</returns>
        public string Shorten(string code, decimal latitude, decimal longitude)
        {
            if (!IsFull(code))
            {
                throw new ArgumentException(string.Format(System.Globalization.CultureInfo.CurrentUICulture, Properties.Resources.PassedOLCIsNotFull, code));
            }
            if (code.IndexOf(PaddingCharacter) != -1)
            {
                throw new ArgumentException(string.Format(System.Globalization.CultureInfo.CurrentUICulture, Properties.Resources.CantShortenPaddedCodes, code));
            }
            code = code.ToUpper();
            var codeArea = Decode(code);
            if (codeArea.CodeLength < MinTrimmableCodeLen)
            {
                throw new ArgumentException(string.Format(System.Globalization.CultureInfo.CurrentUICulture, Properties.Resources.CodeLengthMust, MinTrimmableCodeLen));
            }
            // Ensure that latitude and longitude are valid.
            latitude = OpenLocationCodeHelper.ClipLatitude(latitude);
            longitude = OpenLocationCodeHelper.NormalizeLongitude(longitude);
            // How close are the latitude and longitude to the code center.
            var range = Math.Max(Math.Abs(codeArea.LatitudeCenter - latitude), Math.Abs(codeArea.LongitudeCenter - longitude));
            for (var i = OpenLocationCodeHelper.PairResolutions.Length - 2; i >= 1; i--)
            {
                // Check if we're close enough to shorten. The range must be less than 1/2
                // the resolution to shorten at all, and we want to allow some safety, so
                // use 0.3 instead of 0.5 as a multiplier.
                if (range < (OpenLocationCodeHelper.PairResolutions[i] * 0.3m))
                {
                    // Trim it.
                    return code.Substring((i + 1) * 2);
                }
            }
            return code;
        }

        #region helper methods
        /// <summary>
        /// Encode a location into a sequence of OLC lat/lng pairs.
        /// 
        /// This uses pairs of characters(longitude and latitude in that order) to
        /// represent each step in a 20x20 grid.Each code, therefore, has 1/400th
        /// the area of the previous code.
        /// </summary>
        /// <param name="latitude">A latitude in signed decimal degrees.</param>
        /// <param name="longitude">A longitude in signed decimal degrees.</param>
        /// <param name="codeLength">The number of significant digits in the output code, not including any separator characters.</param>
        /// <returns></returns>
        private static string encodePairs(decimal latitude, decimal longitude, int codeLength)
        {
            var code = new StringBuilder();
            // Adjust latitude and longitude so they fall into positive ranges.
            var adjustedLatitude = latitude + OpenLocationCodeHelper.LatitudeMax;
            var adjustedLongitude = longitude + OpenLocationCodeHelper.LongitudeMax;
            // Count digits - can't use string length because it may include a separator
            // character.
            var digitCount = 0;
            while (digitCount < codeLength)
            {
                // Provides the value of digits in this place in decimal degrees.
                var placeValue = OpenLocationCodeHelper.PairResolutions[MathExtension.FloorInt(digitCount / 2m)];
                // Do the latitude - gets the digit for this place and subtracts that for
                // the next digit.
                var digitValue = MathExtension.FloorInt(adjustedLatitude / placeValue);
                adjustedLatitude -= digitValue * placeValue;
                code.Append(OpenLocationCodeHelper.CodeAlphabet[digitValue]);
                digitCount += 1;
                // And do the longitude - gets the digit for this place and subtracts that
                // for the next digit.
                digitValue = MathExtension.FloorInt(adjustedLongitude / placeValue);
                adjustedLongitude -= digitValue * placeValue;
                code.Append(OpenLocationCodeHelper.CodeAlphabet[digitValue]);
                digitCount += 1;
                // Should we add a separator here?
                if (digitCount == SeparatorPosition && digitCount < codeLength)
                {
                    code.Append(Separator);
                }
            }
            if (code.Length < SeparatorPosition)
            {
                code.Append(new string(PaddingCharacter, SeparatorPosition - code.Length));
            }
            if (code.Length == SeparatorPosition)
            {
                code.Append(Separator);
            }
            return code.ToString();
        }

        /// <summary>
        /// Encode a location using the grid refinement method into an OLC string.
        /// 
        /// The grid refinement method divides the area into a grid of 4x5, and uses a
        /// single character to refine the area.This allows default accuracy OLC codes
        /// to be refined with just a single character.
        /// </summary>
        /// <param name="latitude">A latitude in signed decimal degrees.</param>
        /// <param name="longitude">A longitude in signed decimal degrees.</param>
        /// <param name="codeLength">The number of characters required.</param>
        /// <returns></returns>
        private static string encodeGrid(decimal latitude, decimal longitude, int codeLength)
        {
            var code = string.Empty;
            var latPlaceValue = OpenLocationCodeHelper.GridSizeDegrees;
            var lngPlaceValue = OpenLocationCodeHelper.GridSizeDegrees;
            // Adjust latitude and longitude so they fall into positive ranges and
            // get the offset for the required places.
            var adjustedLatitude = (latitude + OpenLocationCodeHelper.LatitudeMax) % latPlaceValue;
            var adjustedLongitude = (longitude + OpenLocationCodeHelper.LongitudeMax) % lngPlaceValue;
            for (var i = 0; i < codeLength; i++)
            {
                // Work out the row and column.
                var row = MathExtension.FloorInt(adjustedLatitude / (latPlaceValue / OpenLocationCodeHelper.GridRows));
                var col = MathExtension.FloorInt(adjustedLongitude / (lngPlaceValue / OpenLocationCodeHelper.GridColumns));
                latPlaceValue /= OpenLocationCodeHelper.GridRows;
                lngPlaceValue /= OpenLocationCodeHelper.GridColumns;
                adjustedLatitude -= row * latPlaceValue;
                adjustedLongitude -= col * lngPlaceValue;
                code += OpenLocationCodeHelper.CodeAlphabet[row * OpenLocationCodeHelper.GridColumns + col];
            }
            return code;
        }

        /// <summary>
        /// Decode an OLC code made up of lat/lng pairs.
        /// 
        /// This decodes an OLC code made up of alternating latitude and longitude
        /// characters, encoded using base 20.
        /// </summary>
        /// <param name="code">A valid OLC code, presumed to be full, but with the separator removed.</param>
        /// <returns></returns>
        private static CodeArea decodePairs(string code)
        {
            // Get the latitude and longitude values. These will need correcting from
            // positive ranges.
            var latitude = decodePairsSequence(code, 0);
            var longitude = decodePairsSequence(code, 1);
            // Correct the values and set them into the CodeArea object.
            return new CodeArea(
                latitude[0] - OpenLocationCodeHelper.LatitudeMax,
                longitude[0] - OpenLocationCodeHelper.LongitudeMax,
                latitude[1] - OpenLocationCodeHelper.LatitudeMax,
                longitude[1] - OpenLocationCodeHelper.LongitudeMax,
                code.Length);
        }

        /// <summary>
        /// Decode either a latitude or longitude sequence.
        /// 
        /// This decodes the latitude or longitude sequence of a lat/lng pair encoding.
        /// Starting at the character at position offset, every second character is
        /// decoded and the value returned.
        /// </summary>
        /// <param name="code">A valid OLC code, presumed to be full, with the separator removed.</param>
        /// <param name="offset">The character to start from.</param>
        /// <returns>
        /// A pair of the low and high values. The low value comes from decoding the
        /// characters.The high value is the low value plus the resolution of the
        /// last position.Both values are offset into positive ranges and will need
        /// to be corrected before use.
        /// </returns>
        private static decimal[] decodePairsSequence(string code, int offset)
        {
            var i = 0;
            var value = 0m;
            while (i * 2 + offset < code.Length)
            {
                value += OpenLocationCodeHelper.CodeAlphabetIndex[code[i * 2 + offset]] * OpenLocationCodeHelper.PairResolutions[i];
                i += 1;
            }
            return new[] { value, value + OpenLocationCodeHelper.PairResolutions[i - 1] };
        }

        /// <summary>
        /// Decode the grid refinement portion of an OLC code.
        /// 
        /// This decodes an OLC code using the grid refinement method.
        /// </summary>
        /// <param name="code">A valid OLC code sequence that is only the grid refinement portion.This is the portion of a code starting at position 11.</param>
        /// <returns></returns>
        private static CodeArea decodeGrid(string code)
        {
            var latitudeLo = 0.0m;
            var longitudeLo = 0.0m;
            var latPlaceValue = OpenLocationCodeHelper.GridSizeDegrees;
            var lngPlaceValue = OpenLocationCodeHelper.GridSizeDegrees;
            var i = 0;
            while (i < code.Length)
            {
                var codeIndex = OpenLocationCodeHelper.CodeAlphabetIndex[code[i]];
                var row = MathExtension.Floor(codeIndex / OpenLocationCodeHelper.GridColumns);
                var col = codeIndex % OpenLocationCodeHelper.GridColumns;

                latPlaceValue /= OpenLocationCodeHelper.GridRows;
                lngPlaceValue /= OpenLocationCodeHelper.GridColumns;

                latitudeLo += row * latPlaceValue;
                longitudeLo += col * lngPlaceValue;
                i += 1;
            }
            return new CodeArea(
                latitudeLo, longitudeLo, latitudeLo + latPlaceValue,
                longitudeLo + lngPlaceValue, code.Length);
        }

        #endregion
    }
}
