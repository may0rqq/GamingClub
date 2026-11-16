using System;
using System.Collections.Generic;
using System.Text;

namespace GamingClub
{
    public class ClubController
    {
        private readonly List<GamingStation> _stations = new();
        private readonly List<Session> _activeSessions = new();
        private readonly List<Receipt> _receipts = new();

        public void AddStation(GamingStation station) => _stations.Add(station);

        public Session? GetActiveSessionByStation(int stationNumber) =>
        _activeSessions.FirstOrDefault(s => s.Station.StationNumber == stationNumber);

        public Session StartSession(User user, int stationNumber, Tariff tariff, DateTime? startTime = null, TimeSpan? plannedDuration = null)
        {
            var station = _stations.FirstOrDefault(s => s.StationNumber == stationNumber)
                ?? throw new KeyNotFoundException($"ПК #{stationNumber} не знайдено.");

            var session = new Session(user, station, tariff, startTime, plannedDuration);
            _activeSessions.Add(session);
            Console.WriteLine($"[СТАРТ] {session.GetInfo()}");
            return session;
        }

        public Receipt EndSession(Session session, DateTime? endTime = null)
        {
            if (!_activeSessions.Contains(session))
                throw new InvalidOperationException("Активну сесію не знайдено.");

            var receipt = session.Close(endTime);
            _activeSessions.Remove(session);
            _receipts.Add(receipt);
            return receipt;
        }

        public void CheckAutoCompletions(DateTime currentTime)
        {
            var toClose = new List<Session>();

            foreach (var session in _activeSessions)
            {
                bool timeIsUp = session.PlannedDuration.HasValue && session.GetDuration(currentTime) >= session.PlannedDuration.Value;
                bool noMoney = session.Player.Balance <= (session.CalculateTimeCost(currentTime) + session.CalculateBarCost());

                if (timeIsUp || noMoney)
                {
                    Console.WriteLine($"[АВТО-СТОП] Завершення сесії {session.Player.Nickname} (ПК #{session.Station.StationNumber}).");
                    toClose.Add(session);
                }
            }

            foreach (var s in toClose)
            {
                EndSession(s, currentTime).Print();
            }
        }

        public void MonitorHall()
        {
            Console.WriteLine("\n--- СТАН ЗАЛУ ---");
            foreach (var st in _stations.OrderBy(s => s.StationNumber))
            {
                Console.WriteLine(st.GetInfo());
            }
            Console.WriteLine("------------------\n");
        }

        public decimal GetShiftRevenue() => _receipts.Sum(r => r.GrandTotal);
    }
}
