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
    public sealed class ValidationTest
    {
        private class ValidationDataItem
        {
            public string Code { get; set; }

            public bool IsValid { get; set; }

            public bool IsShort { get; set; }

            public bool IsFull { get; set; }

            public ValidationDataItem(string line)
            {
                String[] parts = line.Split(new[] { ',' });
                if (parts.Length != 4) throw new Exception("Wrong format of testing data.");
                this.Code = parts[0];
                this.IsValid = bool.Parse(parts[1]);
                this.IsShort = bool.Parse(parts[2]);
                this.IsFull = bool.Parse(parts[3]);
            }
        }

        private static readonly List<ValidationDataItem> TestDataV1List = new List<ValidationDataItem>();
        private static readonly List<ValidationDataItem> TestDataVNextList = new List<ValidationDataItem>();

        [TestFixtureSetUp]
        public void Setup()
        {
            var manager = new System.Resources.ResourceManager("OpenLocationCode.Test.g", GetType().Assembly);
            using (var sr = new System.IO.StreamReader(manager.GetStream("data/v1/validitytests.csv")))
            {
                string line = null;
                do
                {
                    line = sr.ReadLine();
                    if (string.IsNullOrEmpty(line) == false)
                    {
                        if (line.StartsWith("#")) continue;
                        TestDataV1List.Add(new ValidationDataItem(line));
                    }                    
                }
                while (string.IsNullOrEmpty(line) == false);
            }

            using (var sr = new System.IO.StreamReader(manager.GetStream("data/vnext/validitytests.csv")))
            {
                string line = null;
                do
                {
                    line = sr.ReadLine();
                    if (string.IsNullOrEmpty(line) == false)
                    {
                        if (line.StartsWith("#")) continue;
                        TestDataVNextList.Add(new ValidationDataItem(line));
                    }
                }
                while (string.IsNullOrEmpty(line) == false);
            }
        }

        [Test]
        public void IsValidV1Test()
        {
            foreach (var validData in TestDataV1List)
            {
                var olc = new ASOL.OpenLocationCode.OpenLocationCode(validData.Code);
                Assert.AreEqual(validData.IsValid, olc.IsValid, validData.Code);
            }
        }

        [Test]
        public void IsShortV1Test()
        {
            foreach (var validData in TestDataV1List)
            {
                var olc = new ASOL.OpenLocationCode.OpenLocationCode(validData.Code);
                Assert.AreEqual(validData.IsShort, olc.IsShort, validData.Code);
            }
        }

        [Test]
        public void IsFullV1Test()
        {
            foreach (var validData in TestDataV1List)
            {
                var olc = new ASOL.OpenLocationCode.OpenLocationCode(validData.Code);
                Assert.AreEqual(validData.IsFull, olc.IsFull, validData.Code);
            }
        }

        [Test]
        public void IsValidVNextTest()
        {
            foreach (var validData in TestDataVNextList)
            {
                var olc = new ASOL.OpenLocationCode.OpenLocationCode(validData.Code, ASOL.OpenLocationCode.FormatVersion.VNext);
                Assert.AreEqual(validData.IsValid, olc.IsValid, validData.Code);
            }
        }

        [Test]
        public void IsShortVNext1Test()
        {
            foreach (var validData in TestDataVNextList)
            {
                var olc = new ASOL.OpenLocationCode.OpenLocationCode(validData.Code, ASOL.OpenLocationCode.FormatVersion.VNext);
                Assert.AreEqual(validData.IsShort, olc.IsShort, validData.Code);
            }
        }

        [Test]
        public void IsFullVNextTest()
        {
            foreach (var validData in TestDataVNextList)
            {
                var olc = new ASOL.OpenLocationCode.OpenLocationCode(validData.Code, ASOL.OpenLocationCode.FormatVersion.VNext);
                Assert.AreEqual(validData.IsFull, olc.IsFull, validData.Code);
            }
        }
    }
}
