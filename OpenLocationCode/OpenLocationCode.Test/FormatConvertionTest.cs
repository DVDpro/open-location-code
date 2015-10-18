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

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLocationCode.Test
{
    [TestFixture]
    public sealed class FormatConvertionTest
    {
        [Test]
        public void ForwardTest()
        {
            var longitude = 14.4298583m;
            var latitude = 50.0398061m;
            var codeV1 = new ASOL.OpenLocationCode.OpenLocationCode(latitude, longitude, ASOL.OpenLocationCode.FormatVersion.V1);
            var codeVNext = codeV1.ConvertToFormatVersion(ASOL.OpenLocationCode.FormatVersion.VNext);
            Assert.AreEqual(codeV1.Longitude, codeVNext.Longitude, "longitude");
            Assert.AreEqual(codeV1.Latitude, codeVNext.Latitude, "latitude");
            Assert.AreNotEqual(codeV1.Code, codeVNext.Code, "code");
        }

        [Test]
        public void BackwardTest()
        {
            var longitude = 14.4298583m;
            var latitude = 50.0398061m;
            var codeVNext = new ASOL.OpenLocationCode.OpenLocationCode(latitude, longitude, ASOL.OpenLocationCode.FormatVersion.VNext);
            var codeV1 = codeVNext.ConvertToFormatVersion(ASOL.OpenLocationCode.FormatVersion.V1);
            Assert.AreEqual(codeV1.Longitude, codeVNext.Longitude, "longitude");
            Assert.AreEqual(codeV1.Latitude, codeVNext.Latitude, "latitude");
            Assert.AreNotEqual(codeV1.Code, codeVNext.Code, "code");
        }

    }
}
