using System;
using System.Collections.Generic;
using System.Text;

namespace GamingClub
{
    public class Session : Entity
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

            Station.Occupy();
            StartTime = startTime ?? DateTime.Now;
            PlannedDuration = plannedDuration;
        }

        public void AddOrder(SnackOrder order)
        {
            if (!IsActive) throw new InvalidOperationException("Сесія вже завершена.");
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

        public decimal CalculateBarCost() => _orders.Sum(o => o.GetTotalCost());

        public Receipt Close(DateTime? endTime = null)
        {
            if (!IsActive) throw new InvalidOperationException("Сесія вже закрита.");

            EndTime = endTime ?? DateTime.Now;
            Station.Release();

            // Бонус за гру на VIP-тарифі (10 хв за кожну годину)
            if (SelectedTariff is VipTariff)
                Player.AddBonusHours((GetDuration().TotalHours * 10.0) / 60.0);

            var receipt = new Receipt(this);

            if (!Player.TryCharge(receipt.GrandTotal))
            {
                Console.WriteLine($"[УВАГА] Не вистачає коштів на балансі {Player.Nickname}. Доплата: {receipt.GrandTotal - Player.Balance:F2} грн");
                Player.TryCharge(Player.Balance);
            }

            return receipt;
        }

        public override string GetInfo() =>
            $"Сесія: {Player.Nickname} на ПК #{Station.StationNumber} ({SelectedTariff.Name}) з {StartTime:HH:mm}";
    }
}
