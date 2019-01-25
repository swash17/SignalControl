using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;


namespace SwashSim_SignalControllerActuated
{
    [Serializable]
    public class TimingPlan
    {
        [XmlAttribute("Plan_Number")]
        public uint Id;
        [XmlAttribute("Controller_ID")]
        public uint ControllerId;
        public uint MasterCid;
        public double CycleLength;
        public double Offset;
        public uint YieldPoint;
        public int StartTimeStep;
        public int EndTimeStep;
        public List<List<byte>> Rings = new List<List<byte>>();
        [XmlArrayItem("Phase", typeof(TimingPlanPhase), IsNullable = false)]
        public TimingPlanPhase[] Phases = new TimingPlanPhase[16];
        

        public TimingPlan()
        {
            //for (int i = 1; i <= 16; i++)
            //{
            //    TimingPlanPhase newPhase = new TimingPlanPhase();
            //    newPhase.PhaseNumber = Convert.ToByte(i);
            //    Phases[i - 1] = newPhase;
            //}
            //List<byte> Ring1 = new List<byte>(6) { 2, 1, 0, 3, 4, 0 };
            //Rings.Add(Ring1);
            //List<byte> Ring2 = new List<byte>(6) { 5, 6, 0, 7, 8, 0 };
            //Rings.Add(Ring1);
        }

    }

    [Serializable]
    public class TimingPlanPhase
    {
        public enum ControlIndication
        {
            Green = 1,
            Yellow = 2,
            Red = 3,
            Permitted = 4
        }
        
        [XmlAttribute("Phase_Number")]
        public byte PhaseNumber;
        public double MinGreen;
        public double MaxGreen;
        public double GapTime;
        public double Yellow;
        public double AllRed;
        public double SplitTime;
        public bool PhaseOmit;
        public bool MinRecall;
        public bool MaxRecall;
        public bool SoftRecall;
        public bool GapReduction;
        public double TimeBeforeReduction;
        public double TimeToReduce;
        public double MinimumGap;
        public bool LeadLeft;
        public double PedGreen;
        public double PedClearance;
        public List<uint> AssociatedControlPointIds = new List<uint>();
        public List<byte> AssociatedDetectorIds = new List<byte>();
        //public ControlIndication Indication;

    }
}
