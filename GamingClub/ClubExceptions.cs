using System;

namespace GamingClub
{
    public class GamingClubException : Exception
    {
        public GamingClubException(string message) : base(message) { }
    }

    public class StationOccupiedException : GamingClubException
    {
        public int StationNumber { get; }

        public StationOccupiedException(int stationNumber)
            : base($"ПК #{stationNumber} зараз зайнятий іншим гравцем.")
        {
            StationNumber = stationNumber;
        }
    }

    public class InsufficientFundsException : GamingClubException
    {
        public decimal RequiredAmount { get; }
        public decimal CurrentBalance { get; }

        public InsufficientFundsException(decimal required, decimal current)
            : base($"Недостатньо коштів! Потрібно: {required:F2} грн, доступно: {current:F2} грн.")
        {
            RequiredAmount = required;
            CurrentBalance = current;
        }
    }

    public class OrderLimitExceededException : GamingClubException
    {
        public int RequestedQuantity { get; }
        public int MaxAllowed { get; }

        public OrderLimitExceededException(int requestedQuantity, int maxAllowed = 20)
            : base($"Перевищено ліміт замовлення: запитано {requestedQuantity} шт., максимум за один раз — {maxAllowed} шт.")
        {
            RequestedQuantity = requestedQuantity;
            MaxAllowed = maxAllowed;
        }
    }
}