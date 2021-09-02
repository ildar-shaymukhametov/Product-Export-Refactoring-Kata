using System;
using System.Collections.Generic;
using System.Text;

namespace ProductExport
{
    public class XmlExporter
    {
        public static string ExportFull(List<Order> orders)
        {
            var xml = new StringBuilder();
            xml.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            var ordersTag = new TagNode("orders");
            foreach (var order in orders)
            {
                WriteOrder(ordersTag, order);
            }

            xml.Append(ordersTag);
            return XmlFormatter.PrettyPrint(xml.ToString());

            static void WriteOrder(TagNode parent, Order order)
            {
                var orderTag = new TagNode("order");
                orderTag.AddAttribute("id", order.Id);
                foreach (var product in order.Products)
                {
                    WriteProduct(orderTag, product);
                }
                parent.Add(orderTag);
            }

            static void WriteProduct(TagNode parent, Product product)
            {
                var productTag = new TagNode("product");
                productTag.AddAttribute("id", product.Id);
                if (product.IsEvent())
                {
                    productTag.AddAttribute("stylist", StylistFor(product));
                }

                if (product.Weight > 0)
                {
                    productTag.AddAttribute("weight", product.Weight);
                }

                WritePrice(productTag, product.Price);
                productTag.AddValue(product.Name);
                parent.Add(productTag);
            }

            static void WritePrice(TagNode parent, Price price)
            {
                var tagNode = new TagNode("price");
                tagNode.AddAttribute("currency", price.CurrencyCode);
                tagNode.AddValue(price.Amount);
                parent.Add(tagNode);
            }
        }

        public static string ExportTaxDetails(List<Order> orders)
        {
            var xml = new StringBuilder();
            xml.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            xml.Append("<orderTax>");
            foreach (var order in orders)
            {
                xml.Append("<order");
                xml.Append(" date='");
                xml.Append(Util.ToIsoDate(order.Date));
                xml.Append("'");
                xml.Append(">");
                var tax = 0D;
                foreach (var product in order.Products)
                {
                    xml.Append("<product");
                    xml.Append(" id='");
                    xml.Append(product.Id);
                    xml.Append("'");
                    xml.Append(">");
                    xml.Append(product.Name);
                    xml.Append("</product>");
                    if (product.IsEvent())
                        tax += product.Price.GetAmountInCurrency("USD") * 0.25;
                    else
                        tax += product.Price.GetAmountInCurrency("USD") * 0.175;
                }

                xml.Append("<orderTax currency='USD'>");
                if (order.Date < Util.FromIsoDate("2018-01-01T00:00Z"))
                    tax += 10;
                else
                    tax += 20;
                xml.Append($"{tax:N2}%");
                xml.Append("</orderTax>");
                xml.Append("</order>");
            }

            var totalTax = TaxCalculator.CalculateAddedTax(orders);
            xml.Append($"{totalTax:N2}%");
            xml.Append("</orderTax>");
            return XmlFormatter.PrettyPrint(xml.ToString());
        }

        public static string ExportStore(Store store)
        {
            var xml = new StringBuilder();
            xml.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");

            xml.Append("<store");
            xml.Append(" name='");
            xml.Append(store.Name);
            xml.Append("'");
            xml.Append(">");
            foreach (var product in store.Stock)
            {
                xml.Append("<product");
                xml.Append(" id='");
                xml.Append(product.Id);
                xml.Append("'");
                if (product.IsEvent())
                {
                    xml.Append(" location='");
                    xml.Append(store.Name);
                    xml.Append("'");
                }
                else
                {
                    xml.Append(" weight='");
                    xml.Append(product.Weight);
                    xml.Append("'");
                }

                xml.Append(">");
                xml.Append("<price");
                xml.Append(" currency='");
                xml.Append(product.Price.CurrencyCode);
                xml.Append("'>");
                xml.Append(product.Price.Amount);
                xml.Append("</price>");
                xml.Append(product.Name);
                xml.Append("</product>");
            }

            xml.Append("</store>");

            return XmlFormatter.PrettyPrint(xml.ToString());
        }

        public static string ExportHistory(List<Order> orders)
        {
            var xml = new StringBuilder();
            xml.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            xml.Append("<orderHistory");
            xml.Append(" createdAt='");
            var now = DateTime.Now;
            xml.Append(Util.ToIsoDate(now));
            xml.Append("'");
            xml.Append(">");
            foreach (var order in orders)
            {
                xml.Append("<order");
                xml.Append(" date='");
                xml.Append(Util.ToIsoDate(order.Date));
                xml.Append("'");
                xml.Append(" totalDollars='");
                xml.Append(order.TotalDollars());
                xml.Append("'>");
                foreach (var product in order.Products)
                {
                    xml.Append("<product");
                    xml.Append(" id='");
                    xml.Append(product.Id);
                    xml.Append("'");
                    xml.Append(">");
                    xml.Append(product.Name);
                    xml.Append("</product>");
                }

                xml.Append("</order>");
            }

            xml.Append("</orderHistory>");
            return XmlFormatter.PrettyPrint(xml.ToString());
        }

        private static string StylistFor(Product product)
        {
            return "Celeste Pulchritudo"; // in future we will look up the name of the stylist from the database
        }
    }
}