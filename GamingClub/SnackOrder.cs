using System;
using System.Collections.Generic;
using System.Text;

namespace GamingClub
{
    public class SnackOrder : Entity
    {
        public string ItemName { get; }
        public decimal Price { get; }
        public int Quantity { get; }

        public SnackOrder(string itemName, decimal price, int quantity = 1)
        {
            if (string.IsNullOrWhiteSpace(itemName)) throw new ArgumentException("Вкажіть назву.");
            if (price < 0 || quantity <= 0) throw new ArgumentException("Некоректна ціна або кількість.");

            ItemName = itemName;
            Price = price;
            Quantity = quantity;
        }

        public decimal GetTotalCost() => Price * Quantity;

        public override string GetInfo() => $"{ItemName} x{Quantity} — {GetTotalCost():F2} грн";
    }
}
