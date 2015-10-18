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
    public sealed class EncodingTest
    {
        private class EncodingDataItem
        {
            public string Code { get; set; }

            public decimal Lat { get; set; }
            public decimal Lng { get; set; }
            public decimal LatLo { get; set; }
            public decimal LngLo { get; set; }
            public decimal LatHi { get; set; }
            public decimal LngHi { get; set; }

            public EncodingDataItem(string line)
            {
                String[] parts = line.Split(new[] { ',' });
                if (parts.Length != 7) throw new Exception("Wrong format of testing data.");
                this.Code = parts[0];
                this.Lat = decimal.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture);
                this.Lng = decimal.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture);
                this.LatLo = decimal.Parse(parts[3], System.Globalization.CultureInfo.InvariantCulture);
                this.LngLo = decimal.Parse(parts[4], System.Globalization.CultureInfo.InvariantCulture);
                this.LatHi = decimal.Parse(parts[5], System.Globalization.CultureInfo.InvariantCulture);
                this.LngHi = decimal.Parse(parts[6], System.Globalization.CultureInfo.InvariantCulture);
            }
        }

        private static readonly List<EncodingDataItem> TestDataV1List = new List<EncodingDataItem>();
        private static readonly List<EncodingDataItem> TestDataVNextList = new List<EncodingDataItem>();

        [TestFixtureSetUp]
        public void Setup()
        {
            var manager = new System.Resources.ResourceManager("OpenLocationCode.Test.g", GetType().Assembly);
            using (var sr = new System.IO.StreamReader(manager.GetStream("data/v1/encodingtests.csv")))
            {
                string line = null;
                do
                {
                    line = sr.ReadLine();
                    if (string.IsNullOrEmpty(line) == false)
                    {
                        if (line.StartsWith("#")) continue;
                        TestDataV1List.Add(new EncodingDataItem(line));
                    }
                }
                while (string.IsNullOrEmpty(line) == false);
            }

            using (var sr = new System.IO.StreamReader(manager.GetStream("data/vnext/encodingtests.csv")))
            {
                string line = null;
                do
                {
                    line = sr.ReadLine();
                    if (string.IsNullOrEmpty(line) == false)
                    {
                        if (line.StartsWith("#")) continue;
                        TestDataVNextList.Add(new EncodingDataItem(line));
                    }
                }
                while (string.IsNullOrEmpty(line) == false);
            }
        }

        [Test]
        public void V1EncodingDecodingTest()
        {
            foreach (var testFields in TestDataV1List)
            {
                var olcFromCode = new ASOL.OpenLocationCode.OpenLocationCode(testFields.Code);
                var codeArea = olcFromCode.Area;
                var olcFromCoors = new ASOL.OpenLocationCode.OpenLocationCode(testFields.Lat, testFields.Lng, codeArea.CodeLength);
                var code = olcFromCoors.Code;

                // Did we get the same code?
                Assert.AreEqual(testFields.Code, code, testFields.Code + ": decode/encode fail");
                // Check that it decoded to the correct coordinates.
                Assert.AreEqual(testFields.LatLo, codeArea.LatitudeLow, codeArea.LatitudeLow + " latitude Lo fail");
                Assert.AreEqual(testFields.LngLo, codeArea.LongitudeLow, codeArea.LongitudeLow + " longitude Lo fail");
                Assert.AreEqual(testFields.LatHi, codeArea.LatitudeHigh, codeArea.LatitudeHigh + " latitude Hi fail");
                Assert.AreEqual(testFields.LngHi, codeArea.LongitudeHigh, codeArea.LongitudeHigh + " longitude Hi fail");
            }
        }

        [Test]
        public void VNextEncodingDecodingTest()
        {
            foreach (var testFields in TestDataVNextList)
            {
                var olcFromCode = new ASOL.OpenLocationCode.OpenLocationCode(testFields.Code, ASOL.OpenLocationCode.FormatVersion.VNext);
                var codeArea = olcFromCode.Area;
                var olcFromCoors = new ASOL.OpenLocationCode.OpenLocationCode(testFields.Lat, testFields.Lng, codeArea.CodeLength, ASOL.OpenLocationCode.FormatVersion.VNext);
                var code = olcFromCoors.Code;

                // Did we get the same code?
                Assert.AreEqual(testFields.Code, code, testFields.Code + ": decode/encode fail");
                // Check that it decoded to the correct coordinates.
                Assert.AreEqual(testFields.LatLo, codeArea.LatitudeLow, codeArea.LatitudeLow + " latitude Lo fail");
                Assert.AreEqual(testFields.LngLo, codeArea.LongitudeLow, codeArea.LongitudeLow + " longitude Lo fail");
                Assert.AreEqual(testFields.LatHi, codeArea.LatitudeHigh, codeArea.LatitudeHigh + " latitude Hi fail");
                Assert.AreEqual(testFields.LngHi, codeArea.LongitudeHigh, codeArea.LongitudeHigh + " longitude Hi fail");
            }
        }


    }
}
