using GamingClub;
using System;
using System.Collections.Generic;
using System.Text;

namespace GamingClub
{
    public abstract class Tariff : Entity
    {
        public string Name { get; protected set; }
        public decimal HourlyRate { get; protected set; }

        protected Tariff(string name, decimal hourlyRate)
        {
            if (hourlyRate < 0) throw new ArgumentException("Ціна не може бути відємною.");
            Name = name;
            HourlyRate = hourlyRate;
        }
        public abstract decimal CalculateCost(TimeSpan duration, User user);
        public override string GetInfo() => $"Тариф «{Name}»(({HourlyRate}) грн/год)";
    }
    public class DayTariff : Tariff
    {
        public DayTariff(decimal hourlyRate = 80m) : base("День", hourlyRate) { }
        public override decimal CalculateCost(TimeSpan duration, User user)
        {
            decimal cost = (decimal)duration.TotalHours * HourlyRate;
            return Math.Round(cost * user.GetDiscountMultiplier(), 2);
        }
    }

    public class NightTariff : Tariff
    {
        private readonly decimal _nightPackagePrice;
        public NightTariff(decimal hourlyRate = 60m, decimal nightPackagePrice = 250m) : base("Ніч", hourlyRate)
        {
            _nightPackagePrice = nightPackagePrice;
        }

        public override decimal CalculateCost(TimeSpan duration, User user)
        {
            // Фіксований пакет, якщо грає 3+ години
            decimal baseCost = duration.TotalHours >= 3.0
            ? _nightPackagePrice
            : (decimal)duration.TotalHours * HourlyRate;

            return Math.Round(baseCost * user.GetDiscountMultiplier(), 2);
        }
    }

    public class VipTariff:Tariff,IBonusProvider
    {
        public VipTariff(decimal hourlyRate = 140m):base("VIP",hourlyRate) { }
        public override decimal CalculateCost(TimeSpan duration, User user)
        {
            // Для VIP знижка лояльності діє наполовину
            decimal discount = 1.0m - ((1.0m - user.GetDiscountMultiplier()) / 2m);
            return Math.Round((decimal)duration.TotalHours * HourlyRate * discount, 2);
        }
        public TimeSpan CalculateBonusEarned(TimeSpan sessionDuration)
        {
            double bonusMinutes = sessionDuration.TotalHours * 10.0;
            return TimeSpan.FromMinutes(bonusMinutes);
        }
    }
}