using System;
using System.Collections.Generic;
using System.Text;

namespace GamingClub
{
    // 1. Контракт для будь-якої платної позиції у чеку (гра, бар тощо)

    public interface IBillable
    {
        string GetDescription();
        decimal CalculateTotal();
    }

    // 2. Контракт для об'єктів, які можуть приносити або нараховувати бонуси

    public interface IBonusProvider
    {
        TimeSpan CalculateBonusEarned(TimeSpan sessionDuration);
    }
    // 3. Контракт для системи логування (його реалізуєш ти у FileLogger.cs)
    public interface IClubLogger
    {
        void LogInfo(string message);
        void LogWarning(string message);
        void LogError(string message);
    }
}
