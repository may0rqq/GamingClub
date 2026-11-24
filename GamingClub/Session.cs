using System;
using System.Collections.Generic;
using System.Linq;

namespace GamingClub
{
    public class Session : Entity, IBillable
    {
        private readonly List<SnackOrder> _orders = new();

        public User Player { get; }
        public GamingStation Station { get; }
        public Tariff SelectedTariff { get; }
        public DateTime StartTime { get; }
        public DateTime? EndTime { get; private set; }
        public TimeSpan? PlannedDuration { get; }
        public bool IsActive => EndTime == null;

        public IReadOnlyList<SnackOrder> Orders => _orders.AsReadOnly();

        public Session(User player, GamingStation station, Tariff tariff, DateTime? startTime = null, TimeSpan? plannedDuration = null)
        {
            if (tariff is VipTariff && !station.IsVip)
                throw new InvalidOperationException("VIP-тариф доступний лише для VIP-місць.");

            Player = player ?? throw new ArgumentNullException(nameof(player));
            Station = station ?? throw new ArgumentNullException(nameof(station));
            SelectedTariff = tariff ?? throw new ArgumentNullException(nameof(tariff));

            decimal minRequired = SelectedTariff.CalculateCost(TimeSpan.FromMinutes(15), Player);
            if (Player.Balance < minRequired && Player.BonusTime < TimeSpan.FromMinutes(15))
                throw new InsufficientFundsException(minRequired, Player.Balance);

            Station.Occupy();
            StartTime = startTime ?? DateTime.Now;
            PlannedDuration = plannedDuration;
        }

        public void AddOrder(SnackOrder order)
        {
            if (!IsActive) throw new InvalidOperationException("Сесія вже завершена.");

            decimal currentTotal = CalculateTimeCost() + CalculateBarCost() + order.CalculateTotal();
            if (Player.Balance < currentTotal)
                throw new InsufficientFundsException(currentTotal, Player.Balance);

            _orders.Add(order);
        }

        public TimeSpan GetDuration(DateTime? currentTime = null)
        {
            DateTime end = EndTime ?? currentTime ?? DateTime.Now;
            return end > StartTime ? end - StartTime : TimeSpan.Zero;
        }

        public decimal CalculateTimeCost(DateTime? currentTime = null)
        {
            TimeSpan duration = GetDuration(currentTime);
            TimeSpan payable = duration;

            if (!IsActive && Player.BonusTime > TimeSpan.Zero)
            {
                payable -= Player.ConsumeBonusTime(duration);
            }

            return SelectedTariff.CalculateCost(payable, Player);
        }

        public decimal CalculateBarCost() => _orders.Sum(o => o.CalculateTotal());

        public Receipt Close(DateTime? endTime = null)
        {
            if (!IsActive) throw new InvalidOperationException("Сесія вже закрита.");

            EndTime = endTime ?? DateTime.Now;
            Station.Release();

            if (SelectedTariff is IBonusProvider bonusProvider)
            {
                TimeSpan earned = bonusProvider.CalculateBonusEarned(GetDuration());
                Player.AddBonusHours(earned.TotalHours);
            }

            var receipt = new Receipt(this);

            if (!Player.TryCharge(receipt.GrandTotal))
            {
                Console.WriteLine($"[УВАГА] Не вистачає коштів на балансі {Player.Nickname}. Доплата готівкою: {receipt.GrandTotal - Player.Balance:F2} грн");
                Player.TryCharge(Player.Balance);
            }

            return receipt;
        }

        public decimal CalculateTotal() => CalculateTimeCost();

        public string GetDescription()
        {
            var dur = GetDuration();
            return $"Гра ({SelectedTariff.Name}, ПК #{Station.StationNumber}): {dur.Hours} год {dur.Minutes} хв";
        }

        public override string GetInfo() =>
            $"Сесія: {Player.Nickname} на ПК #{Station.StationNumber} ({SelectedTariff.Name}) з {StartTime:HH:mm}";
    }
}