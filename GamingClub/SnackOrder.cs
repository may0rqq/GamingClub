using System;

namespace GamingClub
{
    public class SnackOrder : Entity, IBillable
    {
        public const int MaxItemsPerOrder = 20;

        public string ItemName { get; }
        public decimal Price { get; }
        public int Quantity { get; }

        public SnackOrder(string itemName, decimal price, int quantity = 1)
        {
            if (string.IsNullOrWhiteSpace(itemName))
                throw new ArgumentException("Вкажіть назву товару.");
            if (price < 0 || quantity <= 0)
                throw new ArgumentException("Некоректна ціна або кількість.");
            if (quantity > MaxItemsPerOrder)
                throw new OrderLimitExceededException(quantity, MaxItemsPerOrder);

            ItemName = itemName;
            Price = price;
            Quantity = quantity;
        }

        public decimal CalculateTotal() => Price * Quantity;

        public string GetDescription() => $"Бар: {ItemName} x{Quantity} ({Price:F2} грн/шт)";

        public override string GetInfo() => $"{ItemName} x{Quantity} — {CalculateTotal():F2} грн";
    }
}