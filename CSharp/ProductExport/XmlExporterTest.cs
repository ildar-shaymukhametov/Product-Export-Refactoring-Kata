using System.Collections.Generic;
using System.Text.RegularExpressions;
using ApprovalTests;
using ApprovalTests.Reporters;
using Xunit;

namespace ProductExport
{
    [UseReporter(typeof(DiffReporter))]
    public class XmlExporterTest
    {
        const string Price = "149.99";

        [Fact]
        public void ExportFull()
        {
            var orders = new List<Order>() {SampleModelObjects.RecentOrder, SampleModelObjects.OldOrder};
            var xml = XmlExporter.ExportFull(orders);
            Approvals.VerifyXml(xml);
        }

        [Fact]
        public void Tag_with_one_value_and_attribute()
        {
            var tag = new TagNode("price");
            tag.AddAttribute("currency", "USD");
            tag.AddValue(Price);
            var expected = $"<price currency=\"USD\">{Price}</price>";
            Assert.Equal(expected, tag.ToString());
        }

        [Fact]
        public void ExportTaxDetails()
        {
            var orders = new List<Order>() {SampleModelObjects.RecentOrder, SampleModelObjects.OldOrder};
            var xml = XmlExporter.ExportTaxDetails(orders);
            Approvals.VerifyXml(xml);
        }

        [Fact]
        public void ExportStore()
        {
            var store = SampleModelObjects.FlagshipStore;
            var xml = XmlExporter.ExportStore(store);
            Approvals.VerifyXml(xml);
        }

        [Fact]
        public void ExportHistory()
        {
            var orders = new List<Order>() {SampleModelObjects.RecentOrder, SampleModelObjects.OldOrder};
            var xml = XmlExporter.ExportHistory(orders);
            var regex = "createdAt=\"[^\"]+\"";
            var report = Regex.Replace(xml, regex, "createdAt=\"2018-09-20T00:00Z\"");
            Approvals.VerifyXml(report);
        }
    }
}