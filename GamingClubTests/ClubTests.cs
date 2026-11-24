using System;
using Xunit;
using GamingClub;

namespace GamingClubTests
{
    public class ClubTests
    {
        // Тест 1: Перевірка власного винятку при спробі зайняти вже зайнятий ПК
        [Fact]
        public void GamingStation_OccupyAlreadyOccupied_ThrowsStationOccupiedException()
        {
            var station = new GamingStation(1, "RTX 4060");
            station.Occupy();

            Assert.Throws<StationOccupiedException>(() => station.Occupy());
        }

        // Тест 2: Перевірка власного винятку при нестачі коштів на старті сесії
        [Fact]
        public void Session_StartWithInsufficientFunds_ThrowsInsufficientFundsException()
        {
            var poorUser = new User("BrokeGamer", 5m); // На балансі 5 грн, мінімум треба 20 грн
            var station = new GamingStation(1, "RTX 4060");
            var tariff = new DayTariff(80m);

            Assert.Throws<InsufficientFundsException>(() =>
                new Session(poorUser, station, tariff));
        }

        // Тест 3: Перевірка ліміту замовлення з бару (не більше 20 шт.)
        [Fact]
        public void SnackOrder_MoreThan20Items_ThrowsOrderLimitExceededException()
        {
            Assert.Throws<OrderLimitExceededException>(() =>
                new SnackOrder("Red Bull", 70m, quantity: 21));
        }

        // Тест 4: Перевірка пакетного нічного тарифу + знижка лояльності Legend (10%)
        [Fact]
        public void NightTariff_OverThreeHours_AppliesFixedPackageWithLoyaltyDiscount()
        {
            var legendUser = new User("Pro", 500m, LoyaltyLevel.Legend);
            var nightTariff = new NightTariff(hourlyRate: 60m, nightPackagePrice: 250m);

            decimal cost = nightTariff.CalculateCost(TimeSpan.FromHours(4), legendUser);

            // Пакет 250 грн - 10% = 225 грн
            Assert.Equal(225m, cost);
        }

        // Тест 5: Перевірка автозавершення сесії, коли закінчуються гроші на балансі
        [Fact]
        public void ClubController_AutoCompletion_ClosesSessionWhenBalanceRunsOut()
        {
            var club = new ClubController(new FileLogger("test_logs.log"));
            club.AddStation(new GamingStation(1, "RTX 4060"));
            var user = new User("Gamer", 100m); // Вистачить на 1 год
            var tariff = new DayTariff(100m);

            DateTime start = new DateTime(2026, 11, 24, 14, 0, 0);
            club.StartSession(user, 1, tariff, start);

            // Перемотуємо на 1.5 години (вартість 150 грн > баланс 100 грн)
            club.CheckAutoCompletions(start.AddHours(1.5));

            Assert.Equal(100m, club.GetShiftRevenue());
        }
    }
}