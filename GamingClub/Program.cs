using GamingClub;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GamingClub
{
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            var club = new ClubController();
            var users = new List<User>();

            // Початкова ініціалізація залу (5 ПК)
            club.AddStation(new GamingStation(1, "RTX 4060 / i5-13400F / 16GB", isVip: false));
            club.AddStation(new GamingStation(2, "RTX 4060 / i5-13400F / 16GB", isVip: false));
            club.AddStation(new GamingStation(3, "RTX 4070 / i7-13700F / 32GB", isVip: false));
            club.AddStation(new GamingStation(4, "RTX 4090 / i9-14900K / 64GB / 360Hz", isVip: true));
            club.AddStation(new GamingStation(5, "RTX 4090 / i9-14900K / 64GB / 360Hz", isVip: true));

            // Додамо кілька тестових гравців для швидкого старту
            users.Add(new User("S1mple", 1000m, LoyaltyLevel.Legend));
            users.Add(new User("B1t", 300m, LoyaltyLevel.Pro));

            // Використовуємо віртуальний годинник клубу, щоб зручно перемотувати час на захисті
            DateTime clubTime = DateTime.Now;

            while (true)
            {
                Console.WriteLine($"\n================ КІБЕР-АРЕНА [{clubTime:dd.MM.yyyy HH:mm}] ================");
                Console.WriteLine("1. Моніторинг залу (стан ПК)");
                Console.WriteLine("2. Зареєструвати нового гравця");
                Console.WriteLine("3. Поповнити баланс гравця");
                Console.WriteLine("4. Посадити гравця за ПК (Старт сесії)");
                Console.WriteLine("5. Замовити їжу/напої з бару до місця");
                Console.WriteLine("6. Завершити сесію та видати чек");
                Console.WriteLine("7. Перемотати час вперед (+1 год) та перевірити автозавершення");
                Console.WriteLine("8. Переглянути список гравців та виручку за зміну");
                Console.WriteLine("0. Вихід");
                Console.Write("Оберіть дію: ");

                string choice = Console.ReadLine()?.Trim() ?? "";

                try
                {
                    switch (choice)
                    {
                        case "1":
                            club.MonitorHall();
                            break;

                        case "2":
                            RegisterUserMenu(users);
                            break;

                        case "3":
                            TopUpBalanceMenu(users);
                            break;

                        case "4":
                            StartSessionMenu(club, users, clubTime);
                            break;

                        case "5":
                            AddSnackMenu(club);
                            break;

                        case "6":
                            EndSessionMenu(club, clubTime);
                            break;

                        case "7":
                            clubTime = clubTime.AddHours(1);
                            Console.WriteLine($"\n[ЧАС] Минула 1 година. Поточний час клубу: {clubTime:HH:mm}");
                            club.CheckAutoCompletions(clubTime);
                            break;

                        case "8":
                            Console.WriteLine("\n--- ЗАРЕЄСТРОВАНІ ГРАВЦІ ---");
                            foreach (var u in users)
                                Console.WriteLine(u.GetInfo());
                            Console.WriteLine($"\nЗагальна виручка за зміну: {club.GetShiftRevenue():F2} грн");
                            break;

                        case "0":
                            Console.WriteLine("Роботу завершено.");
                            return;

                        default:
                            Console.WriteLine("Невідома команда. Введіть число від 0 до 8.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    // Перехоплюємо всі винятки валідації (зайнятий ПК, VIP-тариф на звичайному ПК тощо)
                    Console.WriteLine($"\n[ПОМИЛКА] {ex.Message}");
                }
            }
        }

        static void RegisterUserMenu(List<User> users)
        {
            Console.Write("Введіть нікнейм гравця: ");
            string nick = Console.ReadLine()?.Trim() ?? "";

            if (users.Any(u => u.Nickname.Equals(nick, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Гравець із таким нікнеймом вже існує.");

            decimal deposit = ReadDecimal("Початковий депозит (грн): ");

            Console.WriteLine("Рівень лояльності: 0 - Beginner (0%), 1 - Pro (5%), 2 - Legend (10%)");
            int lvl = ReadInt("Оберіть рівень (0-2): ");
            LoyaltyLevel loyalty = lvl switch
            {
                1 => LoyaltyLevel.Pro,
                2 => LoyaltyLevel.Legend,
                _ => LoyaltyLevel.Beginner
            };

            var newUser = new User(nick, deposit, loyalty);
            users.Add(newUser);
            Console.WriteLine($"[ОК] Гравця зареєстровано: {newUser.GetInfo()}");
        }

        static void TopUpBalanceMenu(List<User> users)
        {
            var user = SelectUser(users);
            if (user == null) return;

            decimal amount = ReadDecimal("Сума поповнення (грн): ");
            user.TopUpBalance(amount);
            Console.WriteLine($"[ОК] Баланс оновлено: {user.GetInfo()}");
        }

        static void StartSessionMenu(ClubController club, List<User> users, DateTime currentTime)
        {
            var user = SelectUser(users);
            if (user == null) return;

            club.MonitorHall();
            int stationNum = ReadInt("Введіть номер вільного ПК: ");

            Console.WriteLine("Оберіть тариф:");
            Console.WriteLine("1. День (80 грн/год)");
            Console.WriteLine("2. Ніч (60 грн/год або фікс. пакет 250 грн від 3 год)");
            Console.WriteLine("3. VIP (140 грн/год + бонусні хвилини)");
            int tariffChoice = ReadInt("Ваш вибір (1-3): ");

            Tariff tariff = tariffChoice switch
            {
                2 => new NightTariff(),
                3 => new VipTariff(),
                _ => new DayTariff()
            };

            Console.Write("Вкажіть заброньований час у годинах (або 0 для безлімітної гри по балансу): ");
            double hours = (double)ReadDecimal("");
            TimeSpan? planned = hours > 0 ? TimeSpan.FromHours(hours) : null;

            club.StartSession(user, stationNum, tariff, currentTime, planned);
        }

        static void AddSnackMenu(ClubController club)
        {
            int stationNum = ReadInt("Введіть номер ПК, куди принести замовлення: ");
            var session = club.GetActiveSessionByStation(stationNum);

            if (session == null)
                throw new InvalidOperationException($"За ПК #{stationNum} зараз ніхто не грає.");

            Console.Write("Назва позиції (CocaCola, Red Bull, Бургер, Fanta, Sprite, Сендвіч, Вода): ");
            string item = Console.ReadLine()?.Trim() ?? "";
            decimal price = ReadDecimal("Ціна за одиницю (грн): ");
            int qty = ReadInt("Кількість: ");

            session.AddOrder(new SnackOrder(item, price, qty));
            Console.WriteLine($"[ОК] Замовлення додано до сесії гравця {session.Player.Nickname}.");
        }

        static void EndSessionMenu(ClubController club, DateTime currentTime)
        {
            int stationNum = ReadInt("Введіть номер ПК для завершення сесії: ");
            var session = club.GetActiveSessionByStation(stationNum);

            if (session == null)
                throw new InvalidOperationException($"На ПК #{stationNum} немає активної сесії.");

            var receipt = club.EndSession(session, currentTime);
            receipt.Print();
        }

        static User? SelectUser(List<User> users)
        {
            if (users.Count == 0)
            {
                Console.WriteLine("Список гравців порожній. Спочатку зареєструйте гравця.");
                return null;
            }

            Console.WriteLine("\nОберіть гравця:");
            for (int i = 0; i < users.Count; i++)
                Console.WriteLine($"{i + 1}. {users[i].GetInfo()}");

            int index = ReadInt("Номер гравця: ") - 1;
            if (index < 0 || index >= users.Count)
            {
                Console.WriteLine("Невірний номер гравця.");
                return null;
            }
            return users[index];
        }

        static int ReadInt(string prompt)
        {
            while (true)
            {
                if (!string.IsNullOrEmpty(prompt)) Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out int result))
                    return result;
                Console.Write("Помилка! Введіть ціле число: ");
            }
        }

        static decimal ReadDecimal(string prompt)
        {
            while (true)
            {
                if (!string.IsNullOrEmpty(prompt)) Console.Write(prompt);
                if (decimal.TryParse(Console.ReadLine(), out decimal result) && result >= 0)
                    return result;
                Console.Write("Помилка! Введіть коректне число: ");
            }
        }
    }
}