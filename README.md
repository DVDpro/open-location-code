# open-location-code
=======================

[![Build Status](https://travis-ci.org/DVDpro/open-location-code.svg)](https://travis-ci.org/DVDpro/open-location-code)

.Net port of Google Open Location Codes.

Open Location Codes are short, generated codes that can be used like street addresses, for places where street addresses don't exist.

http://openlocationcode.com

Open Location Codes were developed at Google's Zurich engineering office, and then open sourced so that they can be freely used.

Platform
-----
Implementation Open Location Code as portable .NET library. 

Portable targets:
* .NET Framework 4.5
* Windows 8
* Windows Phone 8.1
* ASP.NET Core 5
 

Usage
-----
For released version V1.0 Open Location Code
var myOpenLocationCode = "";
var olc = new ASOL.OpenLocationCode.OpenLocationCode(myOpenLocationCode);
if (olc.IsValid)
{
  var longitude = olc.Longitude;
  var latitude = olc.Latitude;
  
 var olc2 = new ASOL.OpenLocationCode.OpenLocationCode(latitude, longitude);
 var code = olc2.Code;
}

For any next format use optional parameter
* new ASOL.OpenLocationCode.OpenLocationCode(myOpenLocationCode, ASOL.OpenLocationCode.FormatVersion.VNext);

Conversion between two Open Location Code format version
var myOpenLocationCodeVNext = "9F2P2CQH+WWH";
var olcVNext = new ASOL.OpenLocationCode.OpenLocationCode(myOpenLocationCode, ASOL.OpenLocationCode.FormatVersion.VNext);
var olcV1 = olcV1.ConvertToFormatVersion(ASOL.OpenLocationCode.FormatVersion.V1);
if (olcV1.Code != olcVNext.Code) { }


Original code by Google
-----------------------

https://github.com/google/open-location-code
