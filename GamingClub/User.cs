using System;
using System.Collections.Generic;
using System.Text;

namespace GamingClub
{
    public enum LoyaltyLevel
    {
        Beginner, 
        Pro,     
        Legend    
    }

    public class User : Entity
    {
        private decimal _balance;
        private TimeSpan _bonusTime;

        public string Nickname { get; private set; }
        public LoyaltyLevel Loyalty { get; private set; }

        public decimal Balance
        {
            get => _balance;
            private set => _balance = value < 0 ? 0 : value;
        }

        public TimeSpan BonusTime
        {
            get => _bonusTime;
            private set => _bonusTime = value < TimeSpan.Zero ? TimeSpan.Zero : value;
        }

        public User(string nickname, decimal initialDeposit, LoyaltyLevel loyalty = LoyaltyLevel.Beginner)
        {
            if (string.IsNullOrWhiteSpace(nickname))
                throw new ArgumentException("Нікнейм не може бути порожнім.");

            Nickname = nickname;
            Balance = initialDeposit;
            Loyalty = loyalty;
            BonusTime = TimeSpan.Zero;
        }

        public void TopUpBalance(decimal amount)
        {
            if (amount <= 0) throw new ArgumentException("Сума має бути більшою за 0.");
            Balance += amount;
        }

        public bool TryCharge(decimal amount)
        {
            if (amount < 0 || Balance < amount) return false;
            Balance -= amount;
            return true;
        }

        public void AddBonusHours(double hours)
        {
            if (hours > 0) BonusTime += TimeSpan.FromHours(hours);
        }

        public TimeSpan ConsumeBonusTime(TimeSpan requested)
        {
            if (BonusTime <= TimeSpan.Zero) return TimeSpan.Zero;

            if (BonusTime >= requested)
            {
                BonusTime -= requested;
                return requested;
            }

            TimeSpan used = BonusTime;
            BonusTime = TimeSpan.Zero;
            return used;
        }

        public decimal GetDiscountMultiplier() => Loyalty switch
        {
            LoyaltyLevel.Pro => 0.95m,
            LoyaltyLevel.Legend => 0.90m,
            _ => 1.00m
        };

        public override string GetInfo() => $"{Nickname} [{Loyalty}] | Баланс: {Balance:F2} грн | Бонус: {BonusTime.TotalMinutes:F0} хв";
    }
}
