using System;
using System.Collections.Generic;
using SwashSim_VehControlPoint;


namespace SwashSim_SignalControl
{

    public enum SignalControlMode
    {
        None,
        Pretimed,
        Actuated,
        Pedestrian
    }

    public enum StopBarIndication
    {
        None,
        Stop,
        Go
    }

    public enum SignalDisplayColor
    {
        Green = 0,
        Yellow = 1,
        Red = 2,
        Orange = 3,
    }


    public class SignalController
    {
        private UInt16 _id;
        string _label;
        SignalControlMode _controlMode;
        List<CycleData> _cycleInfo;

        bool _isMaster;
        float _localClockCurrentTimeSeconds;
        //float _masterClockMaxTimeSeconds;
        List<TimingPlanData> _timingPlans;
        List<uint> _associatedLinkIds;
        string _associatedLinkIdsString;


        public SignalController(UInt16 id, SignalControlMode controlMode, string label = "")
        {
            _id = id;
            _label = label;
            _controlMode = controlMode;

            _cycleInfo = new List<CycleData>();
            _timingPlans = new List<TimingPlanData>();
            _associatedLinkIds = new List<uint>();

            if (_controlMode == SignalControlMode.Pretimed)
            {

            }
        }

        public string ConvertLinkIdListToString(List<UInt32> linkIds)
        {
            //Create string of vehicle control point IDs using comma to separate
            System.Text.StringBuilder linksLIst = new System.Text.StringBuilder();

            int TotalLinks = linkIds.Count;
            int NumLinks = 0;

            foreach (UInt32 linkID in linkIds)
            {
                linksLIst.Append(linkID);
                NumLinks++;

                if (NumLinks < TotalLinks)
                    linksLIst.Append(",");
            }

            string LinkIDsString = linksLIst.ToString();
            return LinkIDsString;
        }



        public UInt16 Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Label
        {
            get { return _label; }
            set { _label = value; }
        }

        public SignalControlMode ControlMode
        {
            get { return _controlMode; }
            set { _controlMode = value; }
        }

        //private TimingStageData _phasePlan;
        //public TimingStageData PhasePlan
        //{
        //    get { return _phasePlan; }
        //    set { _phasePlan = value; }
        //}

        //private List<TimingStageData> _phasePlans = new List<TimingStageData>();
        //public List<TimingStageData> PhasePlans
        //{
        //    get { return _phasePlans; }
        //    set { _phasePlans = value; }
        //}

        public List<TimingPlanData> TimingPlans
        {
            get { return _timingPlans; }
            set { _timingPlans = value; }
        }
        public List<CycleData> CycleInfo
        {
            get { return _cycleInfo; }
            set { _cycleInfo = value; }
        }

        public List<uint> AssociatedLinkIds
        {
            get { return _associatedLinkIds; }
            set { _associatedLinkIds = value; }
        }

        public string AssociatedLinkIdsString
        {
            get { return _associatedLinkIdsString; }
            set { _associatedLinkIdsString = value; }
        }


        public float LocalClockCurrentTimeSeconds { get => _localClockCurrentTimeSeconds; set => _localClockCurrentTimeSeconds = value; }
        public bool IsMaster { get => _isMaster; set => _isMaster = value; }


        //public float MasterClockMaxTimeSeconds { get => _masterClockMaxTimeSeconds; set => _masterClockMaxTimeSeconds = value; }

        public void SetCoordinationParms(SignalController signal)  //List<SignalController> signalsPretimed)
        {
            //bool IsReferencePhaseIdFound = false;

            //foreach (SignalController signal in signalsPretimed)
            //{
            foreach (TimingPlanData timingPlan in signal.TimingPlans)
            {
                timingPlan.Coordination.ReferencePhaseCycleOffset = 0;

                if (timingPlan.IsRunningCoordination == true)  // && signal.IsMaster == true)
                {
                    foreach (TimingStageData TimingStage in timingPlan.TimingStages)
                    {
                        foreach (int PhaseNum in TimingStage.IncludedPhases)
                        {
                            if (PhaseNum == timingPlan.Coordination.OffsetReferencePhaseId)
                            {
                                timingPlan.Coordination.ReferenceTimingStageIndex = TimingStage.Id;
                                return;
                            }
                        }
                        if (TimingStage.Id > 0)
                            timingPlan.Coordination.ReferencePhaseCycleOffset += TimingStage.GreenMax + TimingStage.YellowTime + TimingStage.AllRedTime;
                    }
                }
            }

            //foreach (TimingPlanData timingPlan in signal.TimingPlans)
            //{
            //    foreach (TimingRingData ring in timingPlan.TimingRings)
            //    {
            //        foreach (PhaseTimingData phase in ring.Phases)
            //        {
            //        }
            //    }
            //}
            //}
        }


        //public void PhaseInitialization(TimingPlanData timingPlan, bool isMasterController) //, List<VehicleControlPointData> ControlPoints)
        //{                        
        //}



        public void InitializeTimingStages(TimingPlanData timingPlan, bool isMasterController)
        {
            int timeIndex = 0;

            //first set all phases to red, then determine which timing stage and phase interval should be active at t=0

            foreach (TimingRingData TimingRing in timingPlan.TimingRings)
            {
                foreach (int PhaseNum in timingPlan.TimingRings[TimingRing.Id - 1].PhaseSequence)
                {
                    int PhaseIndex = timingPlan.TimingRings[TimingRing.Id - 1].Phases.FindIndex(item => item.Id == PhaseNum);

                    timingPlan.TimingRings[TimingRing.Id - 1].Phases[PhaseIndex].Display[timeIndex] = ControlDisplayIndication.Red;
                    timingPlan.TimingRings[TimingRing.Id - 1].Phases[PhaseIndex].ActiveInterval[timeIndex] = PhaseInterval.Red;
                    SetControlPointDisplay(timingPlan, TimingRing, PhaseIndex, ControlDisplayIndication.Red);
                }
            }

            if (timingPlan.IsRunningCoordination == false || (timingPlan.IsRunningCoordination == true && isMasterController == true))
            {
                foreach (TimingRingData TimingRing in timingPlan.TimingRings)
                {
                    timingPlan.TimingRings[TimingRing.Id - 1].Phases[1].Display[timeIndex] = ControlDisplayIndication.Green;
                    timingPlan.TimingRings[TimingRing.Id - 1].Phases[1].GreenTimeElapsed = 0;
                    timingPlan.TimingRings[TimingRing.Id - 1].Phases[1].ActiveInterval[timeIndex] = PhaseInterval.MaxGreen;
                    timingPlan.TimingRings[TimingRing.Id - 1].CurrentGreenPhaseId = (byte)timingPlan.TimingRings[TimingRing.Id - 1].Phases[1].Id;
                    SetControlPointDisplay(timingPlan, TimingRing, 1, ControlDisplayIndication.Green);
                }
            }
            else
            {
                float IntersectionRefPhaseOffset = timingPlan.Coordination.ReferencePhaseOffsetRelativeToMasterSeconds + timingPlan.Coordination.ReferencePhaseCycleOffset;

                float timeToAdvanceToStartOfCycle = timingPlan.Coordination.CycleLength - IntersectionRefPhaseOffset;                               

                foreach (TimingRingData TimingRing in timingPlan.TimingRings)
                {
                    int PhaseSequenceIndex = -1;
                    bool StartingPhaseAndIntervalFound = false;
                    float CumulativeCycleTime = 0;
                    List<byte> RingPhaseSequence = timingPlan.TimingRings[TimingRing.Id - 1].PhaseSequence;

                    foreach (int PhaseNum in RingPhaseSequence)
                    {
                        PhaseSequenceIndex++;

                        //if (timingPlan.TimingRings[TimingRing.Id - 1].Phases[PhaseIndex].Id == PhaseNum)
                        //int PhaseIndex = timingPlan.TimingRings[TimingRing.Id - 1].Phases.FindIndex(item => item.Id == PhaseNum);

                        foreach (int PhaseId in timingPlan.TimingStages[timingPlan.Coordination.ReferenceTimingStageIndex].IncludedPhases)
                        {
                            if (PhaseId == PhaseNum)
                            {
                                do
                                {
                                    int PhaseIndex = timingPlan.TimingRings[TimingRing.Id - 1].Phases.FindIndex(item => item.Id == RingPhaseSequence[PhaseSequenceIndex]);

                                    //need to pass in cumulative phase time
                                    StartingPhaseAndIntervalFound = IdentifyStartingPhaseAndInterval(timingPlan, TimingRing, RingPhaseSequence[PhaseSequenceIndex], PhaseIndex, timeIndex, CumulativeCycleTime, timeToAdvanceToStartOfCycle);

                                    PhaseSequenceIndex++;

                                    CumulativeCycleTime += (timingPlan.TimingRings[TimingRing.Id - 1].Phases[PhaseIndex].GreenMax + timingPlan.TimingRings[TimingRing.Id - 1].Phases[PhaseIndex].YellowTime + timingPlan.TimingRings[TimingRing.Id - 1].Phases[PhaseIndex].AllRedTime);
                                }
                                while (StartingPhaseAndIntervalFound == false);
                            }
                        }

                        //int TimeCounter = 0;
                        //for (int TimeIndex = 0; TimeIndex <= timeToAdvanceToStartOfCycle * 10; TimeIndex++)  //use tenths of a second
                        //{
                        //    TimeCounter += TimeIndex;
                        //    if (timingPlan.TimingRings[TimingRing.Id - 1].Phases[PhaseIndex].GreenMax == TimeCounter)
                        //    {
                        //        TotalPhaseTime = TimeCounter;
                        //    }
                        //}

                    }
                }
            }
        }


        private bool IdentifyStartingPhaseAndInterval(TimingPlanData timingPlan, TimingRingData TimingRing, int phaseId, int phaseIndex, int timeIndex, float cumulativeCycleTime, float timeToAdvanceToStartOfCycle)
        {
            bool PhaseAndIntervalFound = false;
            float PhaseTimeElapsed = 0;

            if (cumulativeCycleTime + timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].GreenMax > timeToAdvanceToStartOfCycle)
            {
                PhaseTimeElapsed = (cumulativeCycleTime + timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].GreenMax) - timeToAdvanceToStartOfCycle;

                timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].Display[timeIndex] = ControlDisplayIndication.Green;
                timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].GreenTimeElapsed = PhaseTimeElapsed;
                timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].ActiveInterval[timeIndex] = PhaseInterval.MaxGreen;
                timingPlan.TimingRings[TimingRing.Id - 1].ActivePhaseIndex = (byte)phaseIndex;
                timingPlan.TimingRings[TimingRing.Id - 1].CurrentGreenPhaseId = (byte)phaseId;
                SetControlPointDisplay(timingPlan, TimingRing, phaseIndex, ControlDisplayIndication.Green);
                PhaseAndIntervalFound = true;
            }
            else if (cumulativeCycleTime + timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].GreenMax == timeToAdvanceToStartOfCycle)
            {
                timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].Display[timeIndex] = ControlDisplayIndication.Yellow;
                timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].YellowTimeElapsed = 0;
                timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].ActiveInterval[timeIndex] = PhaseInterval.Yellow;
                timingPlan.TimingRings[TimingRing.Id - 1].ActivePhaseIndex = (byte)phaseIndex;
                SetControlPointDisplay(timingPlan, TimingRing, phaseIndex, ControlDisplayIndication.Yellow);
                PhaseAndIntervalFound = true;
            }
            else if (cumulativeCycleTime + timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].GreenMax + timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].YellowTime > timeToAdvanceToStartOfCycle)
            {
                PhaseTimeElapsed = (cumulativeCycleTime + timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].GreenMax + timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].YellowTime) - timeToAdvanceToStartOfCycle;

                timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].Display[timeIndex] = ControlDisplayIndication.Yellow;
                timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].YellowTimeElapsed = PhaseTimeElapsed;
                timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].ActiveInterval[timeIndex] = PhaseInterval.Yellow;
                timingPlan.TimingRings[TimingRing.Id - 1].ActivePhaseIndex = (byte)phaseIndex;
                SetControlPointDisplay(timingPlan, TimingRing, phaseIndex, ControlDisplayIndication.Yellow);
                PhaseAndIntervalFound = true;
            }
            else if (cumulativeCycleTime + timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].GreenMax + timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].YellowTime == timeToAdvanceToStartOfCycle)
            {
                timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].Display[timeIndex] = ControlDisplayIndication.Red;
                timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].AllRedTimeElapsed = 0;
                timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].ActiveInterval[timeIndex] = PhaseInterval.AllRed;
                timingPlan.TimingRings[TimingRing.Id - 1].ActivePhaseIndex = (byte)phaseIndex;
                SetControlPointDisplay(timingPlan, TimingRing, phaseIndex, ControlDisplayIndication.Red);
                PhaseAndIntervalFound = true;
            }
            else if (cumulativeCycleTime + timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].GreenMax + timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].YellowTime + timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].AllRedTime >= timeToAdvanceToStartOfCycle)
            {
                PhaseTimeElapsed = (cumulativeCycleTime + timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].GreenMax + timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].YellowTime + timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].AllRedTime) - timeToAdvanceToStartOfCycle;

                timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].Display[timeIndex] = ControlDisplayIndication.Red;
                timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].AllRedTimeElapsed = PhaseTimeElapsed;
                timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].ActiveInterval[timeIndex] = PhaseInterval.AllRed;
                timingPlan.TimingRings[TimingRing.Id - 1].ActivePhaseIndex = (byte)phaseIndex;
                SetControlPointDisplay(timingPlan, TimingRing, phaseIndex, ControlDisplayIndication.Red);
                PhaseAndIntervalFound = true;
            }
            //else if (cumulativeCycleTime + timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].GreenMax + timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].YellowTime + timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].AllRedTime >= timeToAdvanceToStartOfCycle)
            //{
            //    PhaseTimeElapsed = (cumulativeCycleTime + timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].GreenMax + timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].YellowTime + timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].AllRedTime) - timeToAdvanceToStartOfCycle;

            //    timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].Display[timeIndex] = ControlDisplayIndication.Red;
            //    timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].AllRedTimeElapsed = PhaseTimeElapsed;
            //    timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].ActiveInterval[timeIndex] = PhaseInterval.AllRed;
            //    //timingPlan.TimingRings[TimingRing.Id - 1].CurrentGreenPhaseId = (byte)PhaseNum;
            //    SetControlPointDisplay(timingPlan, TimingRing, phaseIndex, ControlDisplayIndication.Red);
            //    PhaseAndIntervalFound = true;
            //}

            return PhaseAndIntervalFound;
        }


        private void SetControlPointDisplay(TimingPlanData timingPlan, TimingRingData TimingRing, int PhaseIndex, ControlDisplayIndication displayInterval)
        {
            foreach (VehicleControlPointData ControlPoint in timingPlan.TimingRings[TimingRing.Id - 1].Phases[PhaseIndex].AssociatedControlPoints)
            {
                ControlPoint.DisplayIndication = displayInterval;
            }
        }


        




    }
}