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

namespace ASOL.OpenLocationCode
{
    /// <summary>
    /// Representation of open location code.  http://openlocationcode.com
    /// </summary>
    interface IOpenLocationCode
    {
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
        bool IsValid(string code);

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
        bool IsShort(string code);

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
        bool IsFull(string code);

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
        string Encode(decimal latitude, decimal longitude, int? codeLength = default(int?));

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
        CodeArea Decode(string code);

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
        string RecoverNearest(string shortCode, decimal referenceLatitude, decimal referenceLongitude);

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
        string ShortenBy4(string code, decimal latitude, decimal longitude);

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
        string ShortenBy6(string code, decimal latitude, decimal longitude);

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
        /// <param name="fullCode">A full, valid code to shorten.</param>
        /// <param name="latitude">A latitude, in signed decimal degrees, to use as the reference point.</param>
        /// <param name="longitude">A longitude, in signed decimal degrees, to use as the reference point.</param>
        /// <returns>Either the original code, if the reference location was not close enough, or the .</returns>
        string Shorten(string fullCode, decimal latitude, decimal longitude);
    }
}
