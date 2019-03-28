using SwashSim_VehControlPoint;
using SwashSim_Network;
using System;



namespace SwashSim_SignalControl
{
    public static class SignalControllerLogic
    {

        //Create virtual conflict monitor (MMU), i.e., a matrix of what movements can/cannot move together?

        public static void UpdateSignalDisplays(SignalControllerPretimed sigController, int timeIndex, double timeStep)  //, List<VehicleControlPointData> controlPoints)
        {
            int NextPhaseIndex = 0;
            int TimingPlanNum = 0;

            foreach (TimingRingData TimingRing in sigController.TimingPlans[TimingPlanNum].TimingRings)
            {
                int TimingRingIndex = TimingRing.Id - 1;

                foreach (byte PhaseNum in sigController.TimingPlans[TimingPlanNum].TimingRings[TimingRingIndex].PhaseSequence)
                {
                    //int PhaseIndex = (PhaseNum / TimingRing.Id);
                    int PhaseIndex = sigController.TimingPlans[TimingPlanNum].TimingRings[TimingRingIndex].PhaseSequence.FindIndex(item => item == PhaseNum) + 1;

                    PhaseTimingData Phase = sigController.TimingPlans[TimingPlanNum].TimingRings[TimingRingIndex].Phases[PhaseIndex];

                    if (Phase.ActiveInterval[timeIndex] == PhaseInterval.MaxGreen)
                    {
                        if (Phase.GreenTimeElapsed < Phase.GreenMax)
                        {
                            Phase.GreenTimeElapsed = (float)Math.Round(Phase.GreenTimeElapsed + timeStep, 1);
                            Phase.IntervalTimeRemaining[timeIndex + 1] = Phase.GreenMax - Phase.GreenTimeElapsed;
                            Phase.Display[timeIndex + 1] = ControlDisplayIndication.Green;
                            Phase.ActiveInterval[timeIndex + 1] = PhaseInterval.MaxGreen;

                            //foreach (VehicleControlPointData ControlPoint in Phase.AssociatedControlPoints)
                            //ControlPoint.DisplayIndication = PhaseDisplay.Green;
                        }
                        else if (Phase.GreenTimeElapsed == Phase.GreenMax)
                        {
                            //reset elapsed green time
                            Phase.GreenTimeElapsed = (float)timeStep;

                            if (Phase.YellowTime > 0)
                            {
                                Phase.Display[timeIndex] = ControlDisplayIndication.Yellow;
                                Phase.Display[timeIndex + 1] = ControlDisplayIndication.Yellow;
                                Phase.ActiveInterval[timeIndex] = PhaseInterval.Yellow;
                                Phase.ActiveInterval[timeIndex + 1] = PhaseInterval.Yellow;

                                //initialize elapsed yellow time
                                Phase.YellowTimeElapsed = (float)timeStep;
                                Phase.IntervalTimeRemaining[timeIndex + 1] = Phase.YellowTime;

                                foreach (VehicleControlPointData ControlPoint in Phase.AssociatedControlPoints)
                                    ControlPoint.DisplayIndication = ControlDisplayIndication.Yellow;
                            }
                            else  //for situation such as ramp metering in which yellow interval may not be used
                            {
                                StartAllRedInterval(timeIndex, timeStep, Phase);
                            }

                            //SetControlPointStatus(sigController, controlPoints, sigController.TimingPlans[TimingPlanNum].TimingRings[TimingRingIndex].Phases[PhaseIndex].AssociatedControlPointIds, PhaseDisplay.Yellow);
                        }
                    }
                    else if (Phase.ActiveInterval[timeIndex] == PhaseInterval.Yellow)
                    {
                        if (Phase.YellowTimeElapsed < Phase.YellowTime)
                        {
                            Phase.YellowTimeElapsed = (float)Math.Round(Phase.YellowTimeElapsed + timeStep, 1);
                            Phase.IntervalTimeRemaining[timeIndex + 1] = Phase.YellowTime - Phase.YellowTimeElapsed;
                            Phase.Display[timeIndex + 1] = ControlDisplayIndication.Yellow;
                            Phase.ActiveInterval[timeIndex + 1] = PhaseInterval.Yellow;
                        }
                        else if (Phase.YellowTimeElapsed == Phase.YellowTime)
                        {
                            //reset elapsed yellow time
                            Phase.YellowTimeElapsed = (float)timeStep;
                            StartAllRedInterval(timeIndex, timeStep, Phase);

                            //SetControlPointStatus(sigController, controlPoints, sigController.TimingPlans[TimingPlanNum].TimingRings[TimingRingIndex].Phases[PhaseIndex].AssociatedControlPointIds, PhaseDisplay.Red);
                        }
                    }
                    else if (Phase.ActiveInterval[timeIndex] == PhaseInterval.AllRed)
                    {
                        //Phase.Display[timeIndex + 1] = PhaseDisplay.Red;

                        if (Phase.AllRedTimeElapsed < Phase.AllRedTime)
                        {
                            Phase.AllRedTimeElapsed = (float)Math.Round(Phase.AllRedTimeElapsed + timeStep, 1);
                            Phase.IntervalTimeRemaining[timeIndex + 1] = Phase.AllRedTime - Phase.AllRedTimeElapsed;
                            Phase.Display[timeIndex + 1] = ControlDisplayIndication.Red;
                            Phase.ActiveInterval[timeIndex + 1] = PhaseInterval.AllRed;
                        }
                        else if (Phase.AllRedTimeElapsed == Phase.AllRedTime || Phase.AllRedTime == 0)
                        {
                            //reset elapsed all-red time
                            Phase.AllRedTimeElapsed = (float)timeStep;

                            Phase.Display[timeIndex] = ControlDisplayIndication.Red;
                            Phase.Display[timeIndex + 1] = ControlDisplayIndication.Red;
                            Phase.ActiveInterval[timeIndex + 1] = PhaseInterval.Red;

                            if (sigController.TimingPlans[TimingPlanNum].TimingRings[TimingRingIndex].ActivePhaseIndex == sigController.TimingPlans[TimingPlanNum].TimingRings[TimingRingIndex].PhaseSequence.Count)
                            {
                                sigController.TimingPlans[TimingPlanNum].TimingRings[TimingRingIndex].ActivePhaseIndex = 1;

                                //------------------------- Cycle and Phase Metrics --------------------------

                                CycleData newCycle = new CycleData();
                                newCycle.Id++;

                                PhaseMetricsData newPhaseMetrics = new PhaseMetricsData();
                                newPhaseMetrics.PhaseId = sigController.TimingPlans[TimingPlanNum].TimingRings[TimingRingIndex].Phases[NextPhaseIndex].Id;
                                newPhaseMetrics.EndGreen = (int)(timeIndex / timeStep);

                                newCycle.PhaseMetrics.Add(newPhaseMetrics);
                                sigController.CycleInfo.Add(newCycle);
                            }
                            else
                            {
                                sigController.TimingPlans[TimingPlanNum].TimingRings[TimingRingIndex].ActivePhaseIndex++;
                            }
                            NextPhaseIndex = sigController.TimingPlans[TimingPlanNum].TimingRings[TimingRingIndex].ActivePhaseIndex;

                            PhaseTimingData NextPhase = sigController.TimingPlans[TimingPlanNum].TimingRings[TimingRingIndex].Phases[NextPhaseIndex];

                            //change signal indications for next phase(s) in sequence to green
                            NextPhase.ActiveInterval[timeIndex] = PhaseInterval.MaxGreen;
                            NextPhase.ActiveInterval[timeIndex + 1] = PhaseInterval.MaxGreen;
                            NextPhase.Display[timeIndex] = ControlDisplayIndication.Green;
                            NextPhase.Display[timeIndex + 1] = ControlDisplayIndication.Green;
                            NextPhase.GreenTimeElapsed = 0;

                            foreach (VehicleControlPointData ControlPoint in NextPhase.AssociatedControlPoints)
                                ControlPoint.DisplayIndication = ControlDisplayIndication.Green;

                            //SetControlPointStatus(sigController, controlPoints, sigController.TimingPlans[TimingPlanNum].TimingRings[TimingRingIndex].Phases[NextPhaseIndex].AssociatedControlPointIds, PhaseDisplay.Green);

                        }
                    }
                    else if (Phase.ActiveInterval[timeIndex] == PhaseInterval.Red)
                    {
                        Phase.Display[timeIndex + 1] = ControlDisplayIndication.Red;
                        Phase.ActiveInterval[timeIndex + 1] = PhaseInterval.Red;

                        foreach (VehicleControlPointData ControlPoint in Phase.AssociatedControlPoints)
                            ControlPoint.DisplayIndication = ControlDisplayIndication.Red;

                    }
                }
            }
        }

        private static void StartAllRedInterval(int timeIndex, double timeStep, PhaseTimingData Phase)
        {
            Phase.Display[timeIndex] = ControlDisplayIndication.Red;
            Phase.Display[timeIndex + 1] = ControlDisplayIndication.Red;
            Phase.ActiveInterval[timeIndex] = PhaseInterval.AllRed;
            Phase.ActiveInterval[timeIndex + 1] = PhaseInterval.AllRed;

            //initialize elapsed all-red time
            Phase.AllRedTimeElapsed = (float)timeStep;
            Phase.IntervalTimeRemaining[timeIndex + 1] = Phase.AllRedTime;

            foreach (VehicleControlPointData ControlPoint in Phase.AssociatedControlPoints)
                ControlPoint.DisplayIndication = ControlDisplayIndication.Red;
        }


        public static void UpdateControlPointDisplay(SignalControllerPretimed sigController, NetworkData Network, int timeIndex)
        {
            int ActiveTimingPlanId = 0;
            int LinkIndex;

            foreach (TimingRingData timingRing in sigController.TimingPlans[ActiveTimingPlanId].TimingRings)
            {
                foreach (PhaseTimingData phase in timingRing.Phases)
                {
                    foreach (VehicleControlPointData PhaseControlPoint in phase.AssociatedControlPoints)
                    {
                        foreach (uint LinkId in sigController.AssociatedLinkIds)
                        {
                            try
                            {
                                LinkIndex = Network.Links.FindIndex(Link => Link.Id.Equals(LinkId));

                                foreach (LaneData Lane in Network.Links[LinkIndex].Lanes)
                                {
                                    foreach (VehicleControlPointData LanecontrolPoint in Lane.ControlPoints)
                                    {
                                        if (LanecontrolPoint.LinkId == PhaseControlPoint.LinkId && LanecontrolPoint.LaneId == Lane.Id && LanecontrolPoint.ControlPhaseId == PhaseControlPoint.ControlPhaseId)
                                        {
                                            LanecontrolPoint.DisplayIndication = phase.Display[timeIndex];
                                        }
                                    }
                                }
                            }
                            catch (System.Exception ex)
                            {
                                throw new System.Exception("UpdateControlPointDisplay Method; Signal Controller Id: " + sigController.Id + " Associated Link Id: " + LinkId.ToString() + " could not be found in network;", ex);
                            }
                        }
                    }
                }
            }
        }



    }
}
