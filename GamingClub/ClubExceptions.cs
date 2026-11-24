using System;
using System.Collections.Generic;
using System.Text;

namespace GamingClub
{
    // Базовий виняток клубу
    public class CyberArenaException : Exception
    {
        public CyberArenaException(string message) : base(message)
        {
        }
        // 1. Помилка: ПК вже зайнятий
        public class StationOccupiedException : CyberArenaException
        {

            public int StationNumber { get; }

            public StationOccupiedException(int StationNumber) : base($"ПК #{stationNumber}зараз зайнято іншим гравцем.")
            {
                StationNumber = StationNumber;
            }


        }

        // 2. Помилка: Недостатньо коштів на балансі
        public class InsufficientFundsException : CyberArenaException
        {
            public decimal RequiredAmount { get; }
            public decimal CurrentBalance { get; }

            public InsufficientFundsException(decimal required, decimal current) : base($"Недостатньо коштів!Потрібно:{required:F2}грн,доступно:{current:F2}грн.")
            {
                RequiredAmount = required;
                CurrentBalance = current;
            }

        }
        // 3. Помилка: Замовлення понад 20 одиниць товару за один раз
        public class OrderLimitExceededException : CyberArenaException
        {
            public int RequestedQuantity { get; }
            public int MaxAllowed { get; }

            public OrderLimitExceededException(int requestedQuantity, int maxAllowed = 20) : base($"Перевищено ліміт замовлення: запитано{requestedQuantity}шт.,максимум за 1 раз - {maxAllowed}шт.")
            {
                RequestedQuantity = requestedQuantity;
                MaxAllowed = maxAllowed;
            }

        }
    }
}