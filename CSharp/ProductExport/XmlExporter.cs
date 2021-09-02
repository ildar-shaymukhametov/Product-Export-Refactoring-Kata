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
                ordersTag.Add(ToOrderNode(order));
            }

            xml.Append(ordersTag);
            return XmlFormatter.PrettyPrint(xml.ToString());

            static TagNode ToOrderNode(Order order)
            {
                var result = new TagNode("order");
                result.AddAttribute("id", order.Id);
                foreach (var product in order.Products)
                {
                    result.Add(ToProductNode(product));
                }
                return result;
            }

            static TagNode ToProductNode(Product product)
            {
                var result = new TagNode("product");
                result.AddAttribute("id", product.Id);
                if (product.IsEvent())
                {
                    result.AddAttribute("stylist", StylistFor(product));
                }

                if (product.Weight > 0)
                {
                    result.AddAttribute("weight", product.Weight);
                }

                result.Add(ToPriceNode(product.Price));
                result.AddValue(product.Name);
                return result;
            }

            static TagNode ToPriceNode(Price price)
            {
                var result = new TagNode("price");
                result.AddAttribute("currency", price.CurrencyCode);
                result.AddValue(price.Amount);
                return result;
            }
        }

        public static string ExportTaxDetails(List<Order> orders)
        {
            var xml = new StringBuilder();
            xml.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            xml.Append(ToOrderTaxTag(orders));
            return XmlFormatter.PrettyPrint(xml.ToString());

            static TagNode ToProductTag(Product product)
            {
                var result = new TagNode("product");
                result.AddAttribute("id", product.Id);
                result.AddValue(product.Name);
                return result;
            }

            static TagNode ToOrderTag(Order order)
            {
                var result = new TagNode("order");
                result.AddAttribute("date", Util.ToIsoDate(order.Date));
                var tax = CalculateTax(order);

                foreach (var product in order.Products)
                {
                    result.Add(ToProductTag(product));
                }

                var orderTaxTag = new TagNode("orderTax");
                orderTaxTag.AddAttribute("currency", "USD");
                orderTaxTag.AddValue($"{tax:N2}%");
                result.Add(orderTaxTag);
                return result;
            }

            static TagNode ToOrderTaxTag(List<Order> orders)
            {
                var result = new TagNode("orderTax");
                foreach (var order in orders)
                {
                    result.Add(ToOrderTag(order));
                }

                var totalTax = TaxCalculator.CalculateAddedTax(orders);
                result.AddValue($"{totalTax:N2}%");
                return result;
            }

            static double CalculateTax(Order order)
            {
                var tax = 0D;
                foreach (var product in order.Products)
                {
                    if (product.IsEvent())
                        tax += product.Price.GetAmountInCurrency("USD") * 0.25;
                    else
                        tax += product.Price.GetAmountInCurrency("USD") * 0.175;
                }
                if (order.Date < Util.FromIsoDate("2018-01-01T00:00Z"))
                    tax += 10;
                else
                    tax += 20;
                return tax;
            }
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