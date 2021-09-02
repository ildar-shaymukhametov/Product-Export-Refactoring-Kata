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

                foreach (var product in order.Products)
                {
                    result.Add(ToProductTag(product));
                }

                var orderTaxTag = new TagNode("orderTax");
                orderTaxTag.AddAttribute("currency", "USD");
                orderTaxTag.AddValue($"{order.GetTax():N2}%");
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
        }

        public static string ExportStore(Store store)
        {
            var xml = new StringBuilder();
            xml.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            xml.Append(ToStoreTag(store));

            return XmlFormatter.PrettyPrint(xml.ToString());

            static TagNode ToProductTag(Store store, Product product)
            {
                var result = new TagNode("product");
                result.AddAttribute("id", product.Id);
                if (product.IsEvent())
                {
                    result.AddAttribute("location", store.Name);
                }
                else
                {
                    result.AddAttribute("weight", product.Weight);
                }

                result.Add(ToPriceTag(product.Price));
                result.AddValue(product.Name);
                return result;
            }

            static TagNode ToPriceTag(Price price)
            {
                var priceTag = new TagNode("price");
                priceTag.AddAttribute("currency", price.CurrencyCode);
                priceTag.AddValue(price.Amount);
                return priceTag;
            }

            static TagNode ToStoreTag(Store store)
            {
                var result = new TagNode("store");
                result.AddAttribute("name", store.Name);
                foreach (var product in store.Stock)
                {
                    result.Add((ToProductTag(store, product)));
                }
                return result;
            }
        }

        public static string ExportHistory(List<Order> orders)
        {
            var xml = new StringBuilder();
            xml.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            xml.Append(ToOrderHistoryTag(orders));
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
                result.AddAttribute("totalDollars", order.TotalDollars());
                foreach (var product in order.Products)
                {
                    result.Add(ToProductTag(product));
                }
                return result;
            }

            static TagNode ToOrderHistoryTag(List<Order> orders)
            {
                var result = new TagNode("orderHistory");
                result.AddAttribute("createdAt", Util.ToIsoDate(DateTime.Now));
                foreach (var order in orders)
                {
                    result.Add(ToOrderTag(order));
                }
                return result;
            }
        }

        private static string StylistFor(Product product)
        {
            return "Celeste Pulchritudo"; // in future we will look up the name of the stylist from the database
        }
    }
}