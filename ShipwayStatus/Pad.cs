using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShipwayStatus
{
    public struct PadPhase
    {
        public string Name { get; set; }
        public double Percent { get; set; }
    }

    public class Pad
    {
        public int Axis { get; set; }
        public string Name { get; set; }
        public string Performer { get; set; }
        List<PadPhase> phases;

        public Pad()
        {
            phases = new List<PadPhase>();
        }

        public void AddPhase(PadPhase pp)
        {
            phases.Add(pp);
        }

        int MaxPhaseNum()
        {
            int max = -1;
            for (int i = 0; i < phases.Count; i++)
            {
                if (phases[i].Percent > 0.0) max = i;
            }
            return max;
        }

        public string MaxPhase()
        {
            if (MaxPhaseNum() == -1) return "";
            return phases[MaxPhaseNum()].Name;
        }

        public double MaxPhasePercent()
        {
            if (MaxPhaseNum() == -1) return 0.0;
            return phases[MaxPhaseNum()].Percent;
        }

        public double PhasePercentByPhase(string phase)
        {
            foreach (PadPhase p in phases)
            {
                if (p.Name == phase) return p.Percent;
            }
            return -1.0;
        }

        public override string ToString()
        {
            string str = String.Format("Axis: {0}, Name: {1}, Performer: {2} \n", Axis, Name, Performer);
            foreach (PadPhase p in phases)
            {
                str += String.Format("{0} = {1}%\n",p.Name, p.Percent);
            }
            return str;
        }
    }
}