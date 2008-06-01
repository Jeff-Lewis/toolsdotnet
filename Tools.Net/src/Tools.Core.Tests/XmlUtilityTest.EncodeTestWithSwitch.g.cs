using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;

namespace Tools.Core.Tests
{
    public partial class XmlUtilityTest
    {
        [TestMethod]
        [PexGeneratedBy(typeof(XmlUtilityTest))]
        public void EncodeTestWithSwitchChar_20080601_200030_000()
        {
            this.EncodeTestWithSwitch('\0');
        }

        [TestMethod]
        [PexGeneratedBy(typeof(XmlUtilityTest))]
        public void EncodeTestWithSwitchChar_20080601_200031_001()
        {
            this.EncodeTestWithSwitch('\u8000');
        }

        [TestMethod]
        [PexGeneratedBy(typeof(XmlUtilityTest))]
        public void EncodeTestWithSwitchChar_20080601_200031_002()
        {
            this.EncodeTestWithSwitch('\"');
        }

        [TestMethod]
        [PexGeneratedBy(typeof(XmlUtilityTest))]
        public void EncodeTestWithSwitchChar_20080601_200031_003()
        {
            this.EncodeTestWithSwitch('\n');
        }

        [TestMethod]
        [PexGeneratedBy(typeof(XmlUtilityTest))]
        public void EncodeTestWithSwitchChar_20080601_200031_004()
        {
            this.EncodeTestWithSwitch('\r');
        }

        [TestMethod]
        [PexGeneratedBy(typeof(XmlUtilityTest))]
        public void EncodeTestWithSwitchChar_20080601_200031_005()
        {
            this.EncodeTestWithSwitch('&');
        }

        [TestMethod]
        [PexGeneratedBy(typeof(XmlUtilityTest))]
        public void EncodeTestWithSwitchChar_20080601_200031_006()
        {
            this.EncodeTestWithSwitch('<');
        }

        [TestMethod]
        [PexGeneratedBy(typeof(XmlUtilityTest))]
        public void EncodeTestWithSwitchChar_20080601_200031_007()
        {
            this.EncodeTestWithSwitch('\'');
        }

    }
}
