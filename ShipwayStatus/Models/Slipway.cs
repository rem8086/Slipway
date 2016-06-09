using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShipwayStatus.Models
{

    public class Slipway
    {
        List<Pad> shipwayList;
        string currentDate;

        public Slipway(List<Pad> shipwayList, string currentDate)
        {
            this.shipwayList = shipwayList;
            this.currentDate = currentDate;
        }

        public string GetCurrentDate()
        {
            return currentDate;
        }

        Pad GetPadStatus(string type, int number)
        {
                Pad pad = (from s in shipwayList
                           where (s.Name == type && s.Axis == number)
                           select s).SingleOrDefault();
            if (pad == null) pad = new Pad();
            return pad;
        }

        public string GetPadPhase(string type, int number)
        {
            return GetPadStatus(type, number).MaxPhase();
        }

        public double GetPadPercent(string type, int number)
        {
            return GetPadStatus(type, number).MaxPhasePercent();
        }

        public double GetPadPercentByPhase(string type, int number, string phase)
        {
            return GetPadStatus(type, number).PhasePercentByPhase(phase);
        }
        
        public List<Pad> GetPads()
        {
            return shipwayList;
        }

    }
}