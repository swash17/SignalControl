using System;
using System.Collections.Generic;
using SwashSim_VehControlPoint;

namespace SwashSim_SignalControl
{    
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
        Orange = 3
    }

    public class SignalControllerPretimed : SignalController
    {
        List<TimingPlanData> _timingPlans;        

        public SignalControllerPretimed(byte id, SignalControlMode controlMode, string label = "") : base(id, controlMode, label)
        {
                                    
            _timingPlans = new List<TimingPlanData>();          
        }

        public List<TimingPlanData> TimingPlans { get => _timingPlans; set => _timingPlans = value; }
        
        public void SetCoordinationParms(SignalControllerPretimed signal)
        {            
            foreach (TimingPlanData timingPlan in signal.TimingPlans)
            {
                timingPlan.Coordination.ReferencePhaseCycleOffset = 0;

                if (timingPlan.IsRunningCoordination == true)  // && signal.IsMaster == true)
                {
                    foreach (TimingStageData TimingStage in timingPlan.TimingStages)
                    {
                        foreach (int PhaseNum in TimingStage.IncludedPhases)
                        {
                            //Identify timing stage that contains coordination reference phase
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

        public void InitializeTimingStages(TimingPlanData timingPlan, bool isMasterController, float masterSignalRefPhaseOffset)
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
                    timingPlan.TimingRings[TimingRing.Id - 1].CurrentGreenPhaseId = timingPlan.TimingRings[TimingRing.Id - 1].Phases[1].Id;
                    SetControlPointDisplay(timingPlan, TimingRing, 1, ControlDisplayIndication.Green);
                }
            }
            else
            {
                float CoordRefPhaseCycleStartTime = timingPlan.Coordination.ReferencePhaseOffsetRelativeToMasterSeconds + masterSignalRefPhaseOffset;
                float TimeBetweenStartOfRefPhaseAndCycleEnd; // = timingPlan.Coordination.CycleLength - timingPlan.Coordination.ReferencePhaseCycleOffset;

                //float TimeToStartRefPhaseAfterCycleZeroPoint;
                //if (CoordinationCycleTime > TimeBetweenStartOfRefPhaseAndCycleEnd)
                if (CoordRefPhaseCycleStartTime > timingPlan.Coordination.CycleLength)
                    //TimeToStartRefPhaseAfterCycleZeroPoint = Temp1 - TimeBetweenStartOfRefPhaseAndCycleEnd;
                    TimeBetweenStartOfRefPhaseAndCycleEnd = 2 * timingPlan.Coordination.CycleLength - CoordRefPhaseCycleStartTime;
                else
                    //TimeToStartRefPhaseAfterCycleZeroPoint = timingPlan.Coordination.ReferencePhaseCycleOffset + timingPlan.Coordination.ReferencePhaseOffsetRelativeToMasterSeconds;
                    TimeBetweenStartOfRefPhaseAndCycleEnd = timingPlan.Coordination.CycleLength - CoordRefPhaseCycleStartTime;
               
                foreach (TimingRingData TimingRing in timingPlan.TimingRings)
                {
                    int PhaseSequenceIndex = -1;
                    bool StartingPhaseAndIntervalFound = false;
                    float CumulativeCycleTime = CoordRefPhaseCycleStartTime;  // 0;
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
                                    StartingPhaseAndIntervalFound = IdentifyStartingPhaseAndInterval(timingPlan, TimingRing, RingPhaseSequence[PhaseSequenceIndex], PhaseIndex, timeIndex, CumulativeCycleTime, /*TimeToStartRefPhaseAfterCycleZeroPoint*/ TimeBetweenStartOfRefPhaseAndCycleEnd, timingPlan.Coordination.CycleLength);

                                    if (PhaseSequenceIndex < RingPhaseSequence.Count - 1)
                                        PhaseSequenceIndex++;
                                    else
                                        PhaseSequenceIndex = 0;

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

        private bool IdentifyStartingPhaseAndInterval(TimingPlanData timingPlan, TimingRingData TimingRing, byte phaseId, int phaseIndex, int timeIndex, float cumulativeCycleTime, float timeToAdvanceToStartOfCycle, float cycleLen)
        {
            bool PhaseAndIntervalFound = false;
            float PhaseTimeElapsed = 0;

            if (cumulativeCycleTime + timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].GreenMax > cycleLen)  //timeToAdvanceToStartOfCycle
            {
                //if (cumulativeCycleTime + timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].GreenMax /*+ timeToAdvanceToStartOfCycle*/ <= cycleLen)
                //    PhaseTimeElapsed = (cumulativeCycleTime + timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].GreenMax) - timeToAdvanceToStartOfCycle;
                //else
                    //PhaseTimeElapsed = (cumulativeCycleTime + timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].GreenMax + timeToAdvanceToStartOfCycle) - cycleLen;
                    PhaseTimeElapsed = cycleLen - cumulativeCycleTime; // timeToAdvanceToStartOfCycle;   //cycleLen - timeToAdvanceToStartOfCycle;

                timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].Display[timeIndex] = ControlDisplayIndication.Green;
                timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].GreenTimeElapsed = PhaseTimeElapsed;
                timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].ActiveInterval[timeIndex] = PhaseInterval.MaxGreen;
                timingPlan.TimingRings[TimingRing.Id - 1].ActivePhaseIndex = (byte)phaseIndex;
                timingPlan.TimingRings[TimingRing.Id - 1].CurrentGreenPhaseId = phaseId;
                SetControlPointDisplay(timingPlan, TimingRing, phaseIndex, ControlDisplayIndication.Green);
                PhaseAndIntervalFound = true;
            }
            else if (cumulativeCycleTime + timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].GreenMax == cycleLen)  //timeToAdvanceToStartOfCycle
            {
                timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].Display[timeIndex] = ControlDisplayIndication.Yellow;
                timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].YellowTimeElapsed = 0;
                timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].ActiveInterval[timeIndex] = PhaseInterval.Yellow;
                timingPlan.TimingRings[TimingRing.Id - 1].ActivePhaseIndex = (byte)phaseIndex;
                SetControlPointDisplay(timingPlan, TimingRing, phaseIndex, ControlDisplayIndication.Yellow);
                PhaseAndIntervalFound = true;
            }
            else if (cumulativeCycleTime + timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].GreenMax + timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].YellowTime > cycleLen)  //timeToAdvanceToStartOfCycle
            {
                PhaseTimeElapsed = (cumulativeCycleTime + timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].GreenMax + timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].YellowTime) - timeToAdvanceToStartOfCycle;

                timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].Display[timeIndex] = ControlDisplayIndication.Yellow;
                timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].YellowTimeElapsed = PhaseTimeElapsed;
                timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].ActiveInterval[timeIndex] = PhaseInterval.Yellow;
                timingPlan.TimingRings[TimingRing.Id - 1].ActivePhaseIndex = (byte)phaseIndex;
                SetControlPointDisplay(timingPlan, TimingRing, phaseIndex, ControlDisplayIndication.Yellow);
                PhaseAndIntervalFound = true;
            }
            else if (cumulativeCycleTime + timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].GreenMax + timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].YellowTime == cycleLen)  //timeToAdvanceToStartOfCycle
            {
                timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].Display[timeIndex] = ControlDisplayIndication.Red;
                timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].AllRedTimeElapsed = 0;
                timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].ActiveInterval[timeIndex] = PhaseInterval.AllRed;
                timingPlan.TimingRings[TimingRing.Id - 1].ActivePhaseIndex = (byte)phaseIndex;
                SetControlPointDisplay(timingPlan, TimingRing, phaseIndex, ControlDisplayIndication.Red);
                PhaseAndIntervalFound = true;
            }
            else if (cumulativeCycleTime + timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].GreenMax + timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].YellowTime + timingPlan.TimingRings[TimingRing.Id - 1].Phases[phaseIndex].AllRedTime >= cycleLen)  //timeToAdvanceToStartOfCycle
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