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
            xml.Append(GetBuilder(orders).ToXml());
            return XmlFormatter.PrettyPrint(xml.ToString());

            static void WriteProduct(TagBuilder builder, Product product)
            {
                builder.AddToParent("order", "product");
                builder.AddAttribute("id", product.Id);
                if (product.IsEvent())
                {
                    builder.AddAttribute("stylist", StylistFor(product));
                }

                if (product.Weight > 0)
                {
                    builder.AddAttribute("weight", product.Weight);
                }
                builder.AddValue(product.Name);
                WritePrice(builder, product.Price);
            }

            static void WritePrice(TagBuilder builder, Price price)
            {
                builder.AddToParent("product", "price");
                builder.AddAttribute("currency", price.CurrencyCode);
                builder.AddValue(price.Amount);
            }

            static void WriteOrder(TagBuilder builder, Order order)
            {
                builder.AddToParent("orders", "order");
                builder.AddAttribute("id", order.Id);
                foreach (var product in order.Products)
                {
                    WriteProduct(builder, product);
                }
            }

            static TagBuilder GetBuilder(List<Order> orders)
            {
                var result = new TagBuilder("orders");
                foreach (var order in orders)
                {
                    WriteOrder(result, order);
                }

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

                var totalTax = orders.Sum(x => x.GetTax());
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