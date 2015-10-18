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
    /// Open location code implementation. Specification: http://openlocationcode.com
    /// </summary>
    public sealed class OpenLocationCode
    {        
        /// <summary>
        /// Open location code default version. http://openlocationcode.com
        /// </summary>
        public const FormatVersion DefaultFormatVersion = FormatVersion.V1;

        private IOpenLocationCode _provider;

        /// <summary>
        /// Current Open location code version
        /// </summary>
        public FormatVersion CurrentVersion { get; }

        /// <summary>
        /// Create open location code with <see cref="DefaultFormatVersion"/>
        /// </summary>
        public OpenLocationCode()
            : this(DefaultFormatVersion)
        {
        }

        /// <summary>
        /// Create open location code with specified format version
        /// </summary>
        /// <param name="formatVersion"></param>
        public OpenLocationCode(FormatVersion formatVersion)
        {
            CurrentVersion = formatVersion;
            switch (formatVersion)
            {
                case FormatVersion.V1:
                    _provider = new Providers.OpenLocationCodeV1();
                    break;
                case FormatVersion.VNext:
                    _provider = new Providers.OpenLocationCodeVNext();
                    break;
                default:
                    throw new NotSupportedException(string.Format(System.Globalization.CultureInfo.CurrentUICulture, Properties.Resources.NotSupportedOLCVersion, formatVersion));
            }
        }

        /// <summary>
        /// Create from open location code
        /// </summary>
        /// <param name="code"></param>
        /// <param name="formatVersion"></param>
        public OpenLocationCode(string code, FormatVersion formatVersion = DefaultFormatVersion)
            : this(formatVersion)
        {
            _code = code;
        }

        /// <summary>
        /// Create from latitude, longitude coordinates
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="formatVersion"></param>
        public OpenLocationCode(decimal latitude, decimal longitude, FormatVersion formatVersion = DefaultFormatVersion)
            : this(latitude, longitude, null, formatVersion)
        {
        }

        /// <summary>
        /// Create from latitude, longitude coordinates with specified code length
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="codeLength">(Optional) The number of significant digits in the output code, not including any separator characters</param>
        /// <param name="formatVersion"></param>
        public OpenLocationCode(decimal latitude, decimal longitude, int? codeLength = default(int?), FormatVersion formatVersion = DefaultFormatVersion)
            : this(formatVersion)
        {
            _latitude = latitude;
            _longitude = longitude;
            _codeLength = codeLength;
        }

        /// <summary>
        /// Representation of open location code.  http://openlocationcode.com
        /// </summary>
        public string Code
        {
            get
            {
                if (_code == null)
                {
                    if (_latitude.HasValue && _longitude.HasValue)
                        _code = _provider.Encode(_latitude.Value, _longitude.Value, _codeLength);
                    else
                        throw new InvalidOperationException(Properties.Resources.CantGetCodeWhenLatitudeOrLongitudeUnset);
                }
                return _code;
            }
        }
        private string _code;

        /// <summary>
        /// Determines if a code is valid.
        /// 
        /// To be valid, all characters must be from the Open Location Code character
        /// set with at most one separator.If the prefix character is present, it
        /// must be the first character. If the separator character is present,
        /// it must be after four characters.
        /// </summary>
        public bool IsValid
        {
            get
            {
                if (_isValid.HasValue == false)
                {
                    _isValid = _provider.IsValid(Code);
                }
                return _isValid.Value;
            }
        }
        private bool? _isValid;

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
        public bool IsShort
        {
            get
            {
                if (_isShort.HasValue == false)
                {
                    _isShort = _provider.IsShort(Code);
                }
                return _isShort.Value;
            }
        }
        private bool? _isShort;

        /// <summary>
        /// Determines if a code is a valid full Open Location Code.
        /// 
        /// Not all possible combinations of Open Location Code characters decode to
        /// valid latitude and longitude values.This checks that a code is valid
        /// and also that the latitude and longitude values are legal.If the prefix
        /// character is present, it must be the first character. If the separator
        /// character is present, it must be after four characters.
        /// </summary>
        public bool IsFull
        {
            get
            {
                if (_isFull.HasValue == false)
                {
                    _isFull = _provider.IsFull(Code);
                }
                return _isFull.Value;
            }
        }
        private bool? _isFull;

        /// <summary>
        /// A latitude, in signed decimal degrees
        /// </summary>
        public decimal Latitude
        {
            get
            {
                if (_latitude.HasValue == false)
                {
                    _codeArea = _provider.Decode(_code);
                    _latitude = _codeArea.LatitudeCenter;
                }
                return _latitude.Value;
            }
        }
        private decimal? _latitude;

        /// <summary>
        /// A longitude, in signed decimal degrees
        /// </summary>
        public decimal Longitude
        {
            get
            {
                if (_longitude.HasValue == false)
                {
                    _codeArea = _provider.Decode(_code);
                    _longitude = _codeArea.LongitudeCenter;
                }
                return _longitude.Value;
            }
        }
        private decimal? _longitude;

        /// <summary>
        /// The number of significant digits in the output code, not including any separator characters
        /// </summary>
        public int CodeLength
        {
            get
            {
                if (_codeLength.HasValue == false)
                {
                    _codeArea = _provider.Decode(Code);
                    _codeLength = _codeArea.CodeLength;
                }
                return _codeLength.Value;
            }
        }
        private int? _codeLength;

        /// <summary>
        /// Decoded Area from Open location code
        /// </summary>
        public CodeArea Area
        {
            get
            {
                if (_codeArea == null)
                {
                    _codeArea = _provider.Decode(_code);
                }
                return _codeArea;
            }
        }
        private CodeArea _codeArea;

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
        public OpenLocationCode RecoverNearest(decimal? referenceLatitude = null, decimal? referenceLongitude = null)
        {
            return new OpenLocationCode(_provider.RecoverNearest(Code, referenceLatitude ?? Latitude, referenceLongitude ?? Longitude), CurrentVersion);
        }

        /// <summary>
        /// Try to remove the first four characters from an OLC code.
        /// 
        /// This uses a reference location to determine if the first four characters
        /// can be removed from the OLC code.The reference location must be within
        /// +/- 0.25 degrees of the code center.This allows the original code to be
        /// recovered using this location, with a safety margin.
        /// </summary>
        /// <param name="latitude"> A latitude, in signed decimal degrees, to use as the reference point.</param>
        /// <param name="longitude">A longitude, in signed decimal degrees, to use as the reference point.</param>
        /// <returns>
        /// The OLC code with the first four characters removed.If the reference
        /// location is not close enough, the passed code is returned unchanged.
        /// </returns>
        /// <remarks>Supported only v1.0 format version</remarks>
        public OpenLocationCode ShortenBy4(decimal? latitude = null, decimal? longitude = null)
        {
            return new OpenLocationCode(_provider.ShortenBy4(Code, latitude ?? Latitude, longitude ?? Longitude), CurrentVersion);
        }

        /// <summary>
        /// Try to remove the first six characters from an OLC code.
        /// 
        /// This uses a reference location to determine if the first six characters
        /// can be removed from the OLC code.The reference location must be within
        /// +/- 0.0125 degrees of the code center.This allows the original code to be
        /// recovered using this location, with a safety margin.
        /// </summary>
        /// <param name="latitude"> A latitude, in signed decimal degrees, to use as the reference point.</param>
        /// <param name="longitude">A longitude, in signed decimal degrees, to use as the reference point.</param>
        /// <returns>
        /// The OLC code with the first six characters removed.If the reference
        /// location is not close enough, the passed code is returned unchanged.
        /// </returns>
        /// <remarks>Supported only v1.0 format version</remarks>
        public OpenLocationCode ShortenBy6(decimal? latitude = null, decimal? longitude = null)
        {
            return new OpenLocationCode(_provider.ShortenBy6(Code, latitude ?? Latitude, longitude ?? Longitude), CurrentVersion);
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
        /// <param name="latitude">A latitude, in signed decimal degrees, to use as the reference point.</param>
        /// <param name="longitude">A longitude, in signed decimal degrees, to use as the reference point.</param>
        /// <returns>Either the original code, if the reference location was not close enough, or the .</returns>
        /// <remarks>Supported only VNext format version</remarks>
        public OpenLocationCode Shorten(decimal? latitude = null, decimal? longitude = null)
        {
            return new OpenLocationCode(_provider.Shorten(Code, latitude ?? Latitude, longitude ?? Longitude), CurrentVersion);
        }

        /// <summary>
        /// Convert Open location code to another version
        /// </summary>
        /// <param name="targetVersion"></param>
        /// <returns></returns>
        public OpenLocationCode ConvertToFormatVersion(FormatVersion targetVersion)
        {
            return new OpenLocationCode(Latitude, Longitude, CodeLength, targetVersion);
        }
    }
}
