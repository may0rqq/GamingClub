using System;
using System.Collections.Generic;
using System.Text;
using static System.Collections.Specialized.BitVector32;

namespace GamingClub
{
    public class Receipt : Entity
    {
        public Session ClosedSession { get; }
        public List<IBillable> Billables { get; } = new();
        public decimal TimeCost { get; }
        public decimal BarCost { get; }
        public decimal GrandTotal => TimeCost + BarCost;

        public Receipt(Session session)
        {
            if (session.IsActive)
                throw new InvalidOperationException("Неможливо сформувати чек для активної сесії.");

            ClosedSession = session;
            BillableItems.Add(session);
            BillableItems.AddRange(session.Orders);
            TimeCost = session.CalculateTimeCost();
            BarCost = session.CalculateBarCost();
        }

        public void Print()
        {
            Console.WriteLine("\n===============================");
            Console.WriteLine($" ЧЕК #{Id.ToString()[..6].ToUpper()}");
            Console.WriteLine("===============================");
            Console.WriteLine($"Гравець: {ClosedSession.Player.Nickname}");
            Console.WriteLine($"Місце: ПК #{ClosedSession.Station.StationNumber} ({ClosedSession.SelectedTariff.Name})");
            Console.WriteLine($"Час: {ClosedSession.StartTime:HH:mm} - {ClosedSession.EndTime:HH:mm}");
            Console.WriteLine($"Оплата за гру: {TimeCost:F2} грн");

            if (ClosedSession.Orders.Count > 0)
            {
                Console.WriteLine("Бар:");
                foreach (var order in ClosedSession.Orders)
                    Console.WriteLine($" - {order.GetInfo()}");
            }

            Console.WriteLine("-------------------------------");
            Console.WriteLine($"ВСЬОГО: {GrandTotal:F2} грн");
            Console.WriteLine($"Залишок балансу: {ClosedSession.Player.Balance:F2} грн");
            Console.WriteLine("===============================\n");
        }

        public override string GetInfo() => $"Чек #{Id.ToString()[..6]} на суму {GrandTotal:F2} грн";
    }
}
