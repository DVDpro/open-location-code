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
    public sealed class ShortFullTest
    {
        private class ShortFullV1DataItem
        {
            public string FullCode { get; set; }

            public decimal Lat { get; set; }
            public decimal Lng { get; set; }
            public string Trim4 { get; set; }
            public string Trim6 { get; set; }

            public ShortFullV1DataItem(string line)
            {
                String[] parts = line.Split(new[] { ',' });
                if (parts.Length != 5) throw new Exception("Wrong format of testing data.");
                this.FullCode = parts[0];
                this.Lat = decimal.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture);
                this.Lng = decimal.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture);
                this.Trim4 = parts[3];
                this.Trim6 = parts[4];
            }
        }

        private class ShortFullVNextDataItem
        {
            public string FullCode { get; set; }

            public decimal Lat { get; set; }
            public decimal Lng { get; set; }
            public string Shortcode { get; set; }

            public ShortFullVNextDataItem(string line)
            {
                String[] parts = line.Split(new[] { ',' });
                if (parts.Length != 4) throw new Exception("Wrong format of testing data.");
                this.FullCode = parts[0];
                this.Lat = decimal.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture);
                this.Lng = decimal.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture);
                this.Shortcode = parts[3];
            }
        }


        private static readonly List<ShortFullV1DataItem> TestDataV1List = new List<ShortFullV1DataItem>();
        private static readonly List<ShortFullVNextDataItem> TestDataVNextList = new List<ShortFullVNextDataItem>();

        [TestFixtureSetUp]
        public void Setup()
        {
            var manager = new System.Resources.ResourceManager("OpenLocationCode.Test.g", GetType().Assembly);
            using (var sr = new System.IO.StreamReader(manager.GetStream("data/v1/shortcodetests.csv")))
            {
                string line = null;
                do
                {
                    line = sr.ReadLine();
                    if (string.IsNullOrEmpty(line) == false)
                    {
                        if (line.StartsWith("#")) continue;
                        TestDataV1List.Add(new ShortFullV1DataItem(line));
                    }
                }
                while (string.IsNullOrEmpty(line) == false);
            }

            using (var sr = new System.IO.StreamReader(manager.GetStream("data/vnext/shortcodetests.csv")))
            {
                string line = null;
                do
                {
                    line = sr.ReadLine();
                    if (string.IsNullOrEmpty(line) == false)
                    {
                        if (line.StartsWith("#")) continue;
                        TestDataVNextList.Add(new ShortFullVNextDataItem(line));
                    }
                }
                while (string.IsNullOrEmpty(line) == false);
            }
        }

        [Test]
        public void V1ShortFullTest()
        {
            foreach (var testFields in TestDataV1List)
            {
                var olc = new ASOL.OpenLocationCode.OpenLocationCode(testFields.FullCode);
                // Shorten the code.
                var shortenBy4 = olc.ShortenBy4(testFields.Lat, testFields.Lng);
                var shortenBy6 = olc.ShortenBy6(testFields.Lat, testFields.Lng);

                // Confirm we got what we expected.
                var coords = " with " + testFields.Lat + "," + testFields.Lng;
                Assert.AreEqual(testFields.Trim4, shortenBy4.Code, testFields.FullCode + " shortenBy4 " + coords);

                Assert.AreEqual(testFields.Trim6, shortenBy6.Code, testFields.FullCode + " shortenBy6 " + coords);

                // Now try expanding the shortened code.
                var expanded = shortenBy4.RecoverNearest(testFields.Lat, testFields.Lng).Code;
                Assert.AreEqual(testFields.FullCode, expanded, testFields.FullCode + " expanding shortenBy4 " + coords);

                expanded = shortenBy6.RecoverNearest(testFields.Lat, testFields.Lng).Code;
                Assert.AreEqual(testFields.FullCode, expanded, testFields.FullCode + " expanding shortenBy6 " + coords);
            }
        }

        [Test]
        public void VNextShortFullTest()
        {
            foreach (var testFields in TestDataVNextList)
            {
                var olc = new ASOL.OpenLocationCode.OpenLocationCode(testFields.FullCode, ASOL.OpenLocationCode.FormatVersion.VNext);
                // Shorten the code.
                var shorten = olc.Shorten(testFields.Lat, testFields.Lng);

                // Confirm we got what we expected.
                var coords = " with " + testFields.Lat + "," + testFields.Lng;
                Assert.AreEqual(testFields.Shortcode, shorten.Code, testFields.FullCode + " shorten " + coords);

                // Now try expanding the shortened code.
                var expanded = shorten.RecoverNearest(testFields.Lat, testFields.Lng).Code;
                Assert.AreEqual(testFields.FullCode, expanded, testFields.FullCode + " expanding shorten " + coords);
            }
        }

    }
}
