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
    public sealed class CodeAreaTest
    {
        [Test]
        public void CtorTest()
        {
            var area = new ASOL.OpenLocationCode.CodeArea(1.1m, 2.2m, 3.3m, 4.4m, 5);
            Assert.AreEqual(1.1m, area.LatitudeLow, "Invalid LatitudeLow");
            Assert.AreEqual(2.2m, area.LongitudeLow, "Invalid LongitudeLow");
            Assert.AreEqual(3.3m, area.LatitudeHigh, "Invalid LatitudeHigh");
            Assert.AreEqual(4.4m, area.LongitudeHigh, "Invalid LongitudeHigh");
            Assert.AreEqual(5, area.CodeLength, "Invalid CodeLength");
            Assert.AreEqual(2.2, area.LatitudeCenter, "Invalid LatitudeCenter");
            Assert.AreEqual(3.3, area.LongitudeCenter, "Invalid LongitudeCenter");
        }
    }
}
