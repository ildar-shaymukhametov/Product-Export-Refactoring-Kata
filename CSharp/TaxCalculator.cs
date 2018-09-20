﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace export
{
    public class TaxCalculator
    {

        public static double CalculateAddedTax(List<Order> orders)
        {
            double tax = 0D;
            foreach (var order in orders) {
            if (order.Date < Util.FromIsoDate("2018-01-01T00:00Z")) {
                tax += 10;
            } else {
                tax += 20;
            }
            foreach (var product in order.Products) {
                if (product.IsEvent()) {
                    tax += product.Price.GetAmountInCurrency("USD")* 0.25;
                } else {
                    tax += product.Price.GetAmountInCurrency("USD")* 0.175;
                }
            }
        }
        return tax;
}
}
}
