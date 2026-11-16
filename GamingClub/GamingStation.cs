using System;
using System.Collections.Generic;
using System.Text;

namespace GamingClub
{
 public enum StationStatus
    {
        Free,
        Occupied
    }
 public class GamingStation:Entity
    {
        public int StationNumber { get; }
        public string HardwareSpecs {  get; private set; }
        public bool IsVip { get; }
        public StationStatus Status { get; private set; }
        public GamingStation(int stationNumber,string hardwareSpecs,bool isVip=false)
        {
            StationNumber = stationNumber;
            HardwareSpecs = hardwareSpecs;
            IsVip = isVip;
            Status = StationStatus.Free;

        }
        public void Occupy()
        {
            if (Status == StationStatus.Occupied)
                throw new InvalidOperationException($"ПК #{StationNumber} вже зайнятий.");
            Status = StationStatus.Occupied;
        }
        public void Release() => Status = StationStatus.Free;
        public override string GetInfo()=>
        $"ПК #{StationNumber} [{(IsVip ? "VIP" : "Standard")}] ({HardwareSpecs}) — {Status}";
    }
}

