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
    /// Representation of open location code version 1.0 (released 4 Nov 2014).  http://openlocationcode.com
    /// </summary>
    internal sealed class OpenLocationCodeV1 : IOpenLocationCode
    {
        /// <summary>
        /// The prefix char. Used to help disambiguate OLC codes from postcodes.
        /// </summary>
        private const char Prefix = '+';

        /// <summary>
        /// A separator used to break the code into two parts to aid memorability.
        /// </summary>
        private const char Separator = '.';

        /// <summary>
        /// The number of characters to place before the separator.
        /// </summary>
        private const int SeparatorPosition = 4;

        /// <summary>
        /// Minimum length of a short code.
        /// </summary>
        private const int MinShortCodeLen = 4;

        /// <summary>
        /// Maximum length of a short code.
        /// </summary>
        private const int MaxShortCodeLen = 7;

        /// <summary>
        /// Minimum length of a code that can be shortened.
        /// </summary>
        private const int MinTrimmableCodeLen = 10;

        /// <summary>
        /// Maximum length of a code that can be shortened.
        /// </summary>
        private const int MAX_TRIMMABLE_CODE_LEN_ = 11;


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
            // If the code includes more than one prefix character, it is not valid.
            if (code.IndexOf(Prefix) != code.LastIndexOf(Prefix))
            {
                return false;
            }
            // If the code includes the prefix character but not in the first position,
            // it is not valid.
            if (code.IndexOf(Prefix) > 0)
            {
                return false;
            }
            // Strip off the prefix if it was provided.
            code = code.RemoveCharacter(Prefix);
            // If the code includes more than one separator, it is not valid.
            if (code.IndexOf(Separator) >= 0)
            {
                if (code.IndexOf(Separator) != code.LastIndexOf(Separator))
                {
                    return false;
                }
                // If there is a separator, and it is in a position != SEPARATOR_POSITION,
                // the code is not valid.
                if (code.IndexOf(Separator) != SeparatorPosition)
                {
                    return false;
                }
            }
            // Check the code contains only valid characters.
            foreach (var character in code)
            {
                if (character != Separator && OpenLocationCodeHelper.CodeAlphabetIndex.ContainsKey(character) == false)
                {
                    return false;
                }
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
            if (!IsValid(code))
            {
                return false;
            }
            if (code.IndexOf(Separator) != -1)
            {
                return false;
            }
            // Strip off the prefix if it was provided.
            code = code.RemoveCharacter(Prefix);
            if (code.Length < MinShortCodeLen)
            {
                return false;
            }
            if (code.Length > MaxShortCodeLen)
            {
                return false;
            }
            return true;
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
            // Strip off the prefix if it was provided.
            code = code.RemoveCharacter(Prefix);

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
            if (codeLength.Value < 2)
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
            var code = Prefix + encodePairs(latitude, longitude, Math.Min(codeLength.Value, OpenLocationCodeHelper.PairCodeLength));
            // If the requested length indicates we want grid refined codes.
            if (codeLength > OpenLocationCodeHelper.PairCodeLength)
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
            // Strip off the prefix if it was provided.
            code = code.RemoveCharacter(Prefix);
            // Strip out separator character (we've already established the code is
            // valid so the maximum is one) and convert to upper case.
            code = code.RemoveCharacter(Separator).ToUpper();
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
            // Strip off the prefix if it was provided.
            shortCode = shortCode.RemoveCharacter(Prefix);

            // Compute padding length and adjust for odd-length short codes.
            var paddingLength = OpenLocationCodeHelper.PairCodeLength - shortCode.Length;
            if (shortCode.Length % 2 == 1)
            {
                paddingLength += 1;
            }
            // The resolution (height and width) of the padded area in degrees.
            var resolution = MathExtension.Pow(20, 2 - (paddingLength / 2));

            // Distance from the center to an edge (in degrees).
            var areaToEdge = resolution / 2.0m;

            // Now round down the reference latitude and longitude to the resolution.
            var roundedLatitude = MathExtension.Floor(referenceLatitude / resolution) * resolution;
            var roundedLongitude = MathExtension.Floor(referenceLongitude / resolution) * resolution;

            // Pad the short code with the rounded reference location.
            var codeArea = Decode(Encode(roundedLatitude, roundedLongitude, paddingLength) + shortCode);
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
            return shortenBy(4, code, latitude, longitude, 0.25m);
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
            return shortenBy(6, code, latitude, longitude, 0.0125m);
        }

        public string Shorten(string fullCode, decimal latitude, decimal longitude)
        {
            throw new InvalidOperationException("Format version v1.0 not supported Shorten method. Use ShortenBy4 or ShortenBy6.");
        }

        #region helper methods

        /// <summary>
        /// Try to remove the first few characters from an OLC code.
        /// 
        /// This uses a reference location to determine if the first few characters
        /// can be removed from the OLC code.The reference location must be within
        /// the passed range(in degrees) of the code center.This allows the original
        /// code to be recovered using this location, with a safety margin.
        /// </summary>
        /// <param name="trimLength">The number of characters to try to remove.</param>
        /// <param name="code">A full, valid code to shorten.</param>
        /// <param name="latitude">A latitude, in signed decimal degrees, to use as the reference point.</param>
        /// <param name="longitude">A longitude, in signed decimal degrees, to use as the reference point.</param>
        /// <param name="range">The maximum acceptable difference in either latitude or longitude.</param>
        /// <returns>The OLC code with leading characters removed. If the reference location is not close enough, the passed code is returned unchanged.</returns>
        private string shortenBy(int trimLength, string code, decimal latitude, decimal longitude, decimal range)
        {
            if (!IsFull(code))
            {
                throw new ArgumentException(string.Format(System.Globalization.CultureInfo.CurrentUICulture, Properties.Resources.PassedOLCIsNotFull, code), "code");
            }
            var codeArea = Decode(code);
            if (codeArea.CodeLength < MinTrimmableCodeLen ||
                codeArea.CodeLength > MAX_TRIMMABLE_CODE_LEN_)
            {
                throw new FormatException(string.Format(System.Globalization.CultureInfo.CurrentUICulture, Properties.Resources.InvalidCodeLength, MinTrimmableCodeLen, MAX_TRIMMABLE_CODE_LEN_));
            }
            // Ensure that latitude and longitude are valid.
            latitude = OpenLocationCodeHelper.ClipLatitude(latitude);
            longitude = OpenLocationCodeHelper.NormalizeLongitude(longitude);
            // Are the latitude and longitude close enough?
            if (Math.Abs(codeArea.LatitudeCenter - latitude) > range || Math.Abs(codeArea.LongitudeCenter - longitude) > range)
            {// No they're not, so return the original code.
                return code;
            }
            // They are, so we can trim the required number of characters from the
            // code. But first we strip the prefix and separator and convert to upper
            // case.
            var newCode = code.RemoveCharacter(Prefix).RemoveCharacter(Separator).ToUpper();
            // And trim the characters, adding one to avoid the prefix.
            return Prefix + newCode.Substring(trimLength);
        }

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
                if (digitCount == codeLength)
                {
                    break;
                }
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
