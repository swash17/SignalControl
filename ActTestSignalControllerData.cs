using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;




//namespace SignalControllerPretimed
//{
//    public class TestSignalControllerData
//    {
//        public List<TimingPlan> TimingPlans;
//        public List<TestRingData> Rings;
//        public uint CurrentTimingPlanID;
//        public bool BarrierConstraint = false;
//        double TimeToBarrier;

//        public TestSignalControllerData(List<TimingPlan> timingPlans)
//        {
//            TimingPlans = timingPlans;
//            TestRingData newRing;
//            foreach (int RingID in Enumerable.Range(1, TimingPlans[0].Rings.Count()).ToList())
//            {
//                newRing = new TestRingData();
//                newRing.ID = RingID;
//                newRing.CurrentPhaseNum = TimingPlans[0].Rings[RingID - 1][0];
//                newRing.CurrentIndication = TestRingData.Indication.Green;
//                newRing.Countdown = TimingPlans[0].Phases[newRing.CurrentPhaseNum - 1].MinGreen;
//                newRing.ElapsedTime = 0;
//                newRing.ReadyForBarrier = false;
//                Rings.Add(newRing);
//            }
//        }

//        public void SignalControlStatus(TestSignalControllerData signal, int timestep, List<DetectorData> detectors, List<VehicleControlPointData> vehicleControlPoints)
//        {

//            if (signal.TimingPlans[TimingPlans.FindIndex(TimingPlan => TimingPlan.Id.Equals(signal.CurrentTimingPlanID))].EndTimeStep==timestep)
//            {
//                NewTimingPlan(signal, timestep);//change timing plans, create new rings, 
//            }
//            int ringIndex=0;
//            foreach (TestRingData Ring in signal.Rings)
//            {
//                if (Ring.CurrentPhaseNum == 255)
//                {

//                    FindNextPhase(Ring, signal, ringIndex, detectors);

//                }

//                else
//                {
//                    TimingPlanPhase phase = signal.TimingPlans[signal.TimingPlans.FindIndex(TimingPlan => TimingPlan.Id.Equals(signal.CurrentTimingPlanID))].Phases[Ring.CurrentPhaseNum - 1]; //current phase

//                    if (Ring.CurrentIndication == TestRingData.Indication.Green)
//                    {
//                        if (Ring.ReadyForBarrier == false)
//                        {
//                            if (Ring.ElapsedTime + Ring.Countdown >= phase.MaxGreen) //check for max out conditions
//                            {
//                                Ring.Countdown = phase.MaxGreen - Ring.ElapsedTime - 0.1;
//                                Ring.ElapsedTime = Ring.ElapsedTime + 0.1;
//                            }
//                            else
//                            {
//                                if (Ring.ElapsedTime + phase.GapTime > phase.MinGreen) //check for extensions
//                                {
//                                    if (CheckDetectorCalls(phase.DetectorIds, detectors))//check detector status
//                                    {
//                                        Ring.Countdown = phase.GapTime;
//                                        Ring.ElapsedTime = Ring.ElapsedTime + 0.1;
//                                    }
//                                    else
//                                    {
//                                        Ring.Countdown = Ring.Countdown - 0.1;
//                                        Ring.ElapsedTime = Ring.ElapsedTime + 0.1;
//                                    }
//                                }
//                                else //still in MinGreen
//                                {
//                                    Ring.Countdown = Ring.Countdown - 0.1;
//                                    Ring.ElapsedTime = Ring.ElapsedTime + 0.1;
//                                }
//                            }
//                        }
//                        else //Ring is ready for barrier
//                        {
//                            //Ring.Countdown = 0.1;
//                            Ring.ElapsedTime = Ring.ElapsedTime + 0.1;
//                        }



//                        if (Ring.Countdown == 0)
//                        {
//                            FindNextPhase(Ring, signal, ringIndex, detectors);
//                        }
//                    }
//                    else
//                    {
//                        Ring.Countdown = Ring.Countdown - 0.1;
//                        Ring.ElapsedTime = Ring.ElapsedTime + 0.1;
//                    }
//                    ringIndex++;
//                }
//            }

            
//            if (signal.BarrierConstraint) //At least one ring ready to cross barrier
//            {
//                bool CrossBarrier = true;
//                TimeToBarrier = 0;
//                foreach (TestRingData Ring in signal.Rings)
//                {
//                    CrossBarrier = CrossBarrier & Ring.ReadyForBarrier;
//                    if (Ring.CurrentIndication == TestRingData.Indication.Green)
//                    {
//                        TimeToBarrier = Math.Max(TimeToBarrier, Ring.Countdown + signal.TimingPlans[signal.TimingPlans.FindIndex(TimingPlan => TimingPlan.Id.Equals(signal.CurrentTimingPlanID))].Phases[Ring.CurrentPhaseNum - 1].Yellow + signal.TimingPlans[signal.TimingPlans.FindIndex(TimingPlan => TimingPlan.Id.Equals(signal.CurrentTimingPlanID))].Phases[Ring.CurrentPhaseNum - 1].AllRed);
//                    }
//                    else
//                    {
//                        if (Ring.CurrentIndication == TestRingData.Indication.Yellow)
//                        {
//                            TimeToBarrier = Math.Max(TimeToBarrier, Ring.Countdown + signal.TimingPlans[signal.TimingPlans.FindIndex(TimingPlan => TimingPlan.Id.Equals(signal.CurrentTimingPlanID))].Phases[Ring.CurrentPhaseNum - 1].AllRed);
//                        }
//                        else
//                        {
//                            TimeToBarrier = Math.Max(TimeToBarrier, Ring.Countdown);
//                        }
//                    }
//                }
//                if (CrossBarrier) //all rings ready to cross barrier
//                {
//                    BarrierConstraintHandler(signal, detectors);

//                }
//                else //not all ready to cross- extend green
//                {

//                }
//            }
//            else
//            {
//                foreach (TestRingData Ring in signal.Rings)
//                {
//                    if (Ring.Countdown == 0)
//                    {
//                        if (Ring.CurrentIndication == TestRingData.Indication.Green)
//                        {
//                            Ring.CurrentIndication = TestRingData.Indication.Yellow;
//                            Ring.Countdown = signal.TimingPlans[signal.TimingPlans.FindIndex(TimingPlan => TimingPlan.Id.Equals(signal.CurrentTimingPlanID))].Phases[Ring.CurrentPhaseNum - 1].Yellow;
//                            Ring.ElapsedTime = 0;
//                        }
//                        else
//                        {
//                            if (Ring.CurrentIndication == TestRingData.Indication.Yellow)
//                            {
//                                Ring.CurrentIndication = TestRingData.Indication.Red;
//                                Ring.Countdown = signal.TimingPlans[signal.TimingPlans.FindIndex(TimingPlan => TimingPlan.Id.Equals(signal.CurrentTimingPlanID))].Phases[Ring.CurrentPhaseNum - 1].AllRed;
//                                Ring.ElapsedTime = 0;
//                            }
//                            else //This should happen on the same timestamp for all rings- cross from red across barrier to next green
//                            {
//                                Ring.CurrentPhaseNum = Ring.NextPhaseNum;
//                                Ring.CurrentPhaseIndex = Ring.NextPhaseIndex;
//                                Ring.CurrentIndication = TestRingData.Indication.Green;
//                                Ring.Countdown = signal.TimingPlans[signal.TimingPlans.FindIndex(TimingPlan => TimingPlan.Id.Equals(signal.CurrentTimingPlanID))].Phases[Ring.CurrentPhaseNum - 1].MinGreen;
//                                signal.BarrierConstraint = false;//Reset 
//                                Ring.ReadyForBarrier = false;
//                            }
//                        }
//                    }
//                }
//            }
//        }

//        void NewTimingPlan(TestSignalControllerData signal, int timestep)
//        {
//            CurrentTimingPlanID = signal.TimingPlans[signal.TimingPlans.FindIndex(TimingPlan => TimingPlan.StartTimeStep.Equals(timestep+1))].Id;
//            int TimingPlanIndex = signal.TimingPlans.FindIndex(TimingPlan => TimingPlan.StartTimeStep.Equals(timestep+1));
//            signal.Rings.Clear();
//            TestRingData newRing;
//            foreach (int RingID in Enumerable.Range(1, signal.TimingPlans[signal.TimingPlans.FindIndex(TimingPlan => TimingPlan.Id.Equals(signal.CurrentTimingPlanID))].Rings.Count()).ToList())
//            {
//                newRing = new TestRingData();
//                newRing.ID = RingID;
//                newRing.CurrentPhaseNum = signal.TimingPlans[TimingPlanIndex].Rings[RingID - 1][0];
//                newRing.CurrentPhaseIndex = 0;
//                newRing.CurrentIndication = TestRingData.Indication.Green;
//                newRing.Countdown = signal.TimingPlans[TimingPlanIndex].Phases[newRing.CurrentPhaseNum -1].MinGreen;
//                newRing.ElapsedTime = 0;
//                newRing.ReadyForBarrier = false;
//                signal.Rings.Add(newRing);
//            }
//        }

//        void FindNextPhase(TestRingData Ring, TestSignalControllerData signal, int ringIndex, List<DetectorData> detectors)
//        {
//            for (int currentPhaseIndex = Ring.CurrentPhaseIndex + 1; currentPhaseIndex <= signal.TimingPlans[signal.TimingPlans.FindIndex(TimingPlan => TimingPlan.Id.Equals(signal.CurrentTimingPlanID))].Rings[ringIndex].Count() + Ring.CurrentPhaseIndex; currentPhaseIndex++)
//            {

//                if (currentPhaseIndex <= signal.TimingPlans[signal.TimingPlans.FindIndex(TimingPlan => TimingPlan.Id.Equals(signal.CurrentTimingPlanID))].Rings[ringIndex].Count() - 1)
//                {
//                    if (signal.TimingPlans[signal.TimingPlans.FindIndex(TimingPlan => TimingPlan.Id.Equals(signal.CurrentTimingPlanID))].Rings[ringIndex][currentPhaseIndex] == 0)
//                    {
//                        Ring.ReadyForBarrier = true;
//                        signal.BarrierConstraint = true;
//                        if (Ring.CurrentIndication == TestRingData.Indication.Green)
//                        {
//                            Ring.NextPhaseNum = signal.TimingPlans[signal.TimingPlans.FindIndex(TimingPlan => TimingPlan.Id.Equals(signal.CurrentTimingPlanID))].Rings[ringIndex][currentPhaseIndex + 1];
//                        }
//                        else
//                        {
//                            Ring.NextPhaseNum = 255;
//                            Ring.CurrentPhaseIndex = Ring.CurrentPhaseIndex + 1;
//                        }
//                        break;

//                    }
//                    else
//                    {
//                        if (CheckDetectorCalls(signal.TimingPlans[signal.TimingPlans.FindIndex(TimingPlan => TimingPlan.Id.Equals(signal.CurrentTimingPlanID))].Phases[signal.TimingPlans[signal.TimingPlans.FindIndex(TimingPlan => TimingPlan.Id.Equals(signal.CurrentTimingPlanID))].Rings[ringIndex][currentPhaseIndex] - 1].DetectorIds, detectors))//check for next phase that has a call
//                        {
//                            Ring.NextPhaseNum = signal.TimingPlans[signal.TimingPlans.FindIndex(TimingPlan => TimingPlan.Id.Equals(signal.CurrentTimingPlanID))].Rings[ringIndex][currentPhaseIndex];
//                            Ring.NextPhaseIndex = currentPhaseIndex;
//                            Ring.ReadyForBarrier = false;
//                            break;
//                        }
//                    }
//                }
//                else
//                {
//                    if (signal.TimingPlans[signal.TimingPlans.FindIndex(TimingPlan => TimingPlan.Id.Equals(signal.CurrentTimingPlanID))].Rings[ringIndex][currentPhaseIndex] == 0)
//                    {
//                        Ring.ReadyForBarrier = true;
//                        signal.BarrierConstraint = true;
//                        if (Ring.CurrentIndication == TestRingData.Indication.Green)
//                        {
//                            Ring.NextPhaseNum = signal.TimingPlans[signal.TimingPlans.FindIndex(TimingPlan => TimingPlan.Id.Equals(signal.CurrentTimingPlanID))].Rings[ringIndex][currentPhaseIndex + 1];
//                        }
//                        else
//                        {
//                            Ring.NextPhaseNum = 255;
//                            Ring.CurrentPhaseIndex = Ring.CurrentPhaseIndex + 1;
//                        }
//                        break;

//                    }
//                    else
//                    {
//                        if (CheckDetectorCalls(signal.TimingPlans[signal.TimingPlans.FindIndex(TimingPlan => TimingPlan.Id.Equals(signal.CurrentTimingPlanID))].Phases[signal.TimingPlans[signal.TimingPlans.FindIndex(TimingPlan => TimingPlan.Id.Equals(signal.CurrentTimingPlanID))].Rings[ringIndex][currentPhaseIndex - signal.TimingPlans[signal.TimingPlans.FindIndex(TimingPlan => TimingPlan.Id.Equals(signal.CurrentTimingPlanID))].Rings[ringIndex].Count()] - 1].DetectorIds, detectors))//check for next phase that has a call (loop through sequence)
//                        {
//                            Ring.NextPhaseNum = signal.TimingPlans[signal.TimingPlans.FindIndex(TimingPlan => TimingPlan.Id.Equals(signal.CurrentTimingPlanID))].Rings[ringIndex][currentPhaseIndex];
//                            Ring.NextPhaseIndex = currentPhaseIndex;
//                            Ring.ReadyForBarrier = false;
//                            break;
//                        }
//                    }
//                }
//            }
//        }

//        void BarrierConstraintHandler(TestSignalControllerData signal, List<DetectorData> detectors)
//        {
//            int ringIndex = 0;
//            foreach (TestRingData Ring in signal.Rings)
//            {



//                if (Ring.CurrentIndication == TestRingData.Indication.Green)
//                {
//                    Ring.Countdown = TimeToBarrier - signal.TimingPlans[signal.TimingPlans.FindIndex(TimingPlan => TimingPlan.Id.Equals(signal.CurrentTimingPlanID))].Phases[Ring.CurrentPhaseNum - 1].Yellow - signal.TimingPlans[signal.TimingPlans.FindIndex(TimingPlan => TimingPlan.Id.Equals(signal.CurrentTimingPlanID))].Phases[Ring.CurrentPhaseNum - 1].AllRed;
//                }
//                else
//                {
//                    if (Ring.CurrentIndication == TestRingData.Indication.Yellow)
//                    {
//                        Ring.Countdown = TimeToBarrier - signal.TimingPlans[signal.TimingPlans.FindIndex(TimingPlan => TimingPlan.Id.Equals(signal.CurrentTimingPlanID))].Phases[Ring.CurrentPhaseNum - 1].AllRed;
//                    }
//                    else
//                    {
//                        Ring.Countdown = TimeToBarrier;
//                    }
//                }


//                if (Ring.Countdown == 0)//Change ring status
//                {

//                    if (Ring.CurrentIndication == TestRingData.Indication.Green)
//                    {
//                        Ring.CurrentIndication = TestRingData.Indication.Yellow;
//                        Ring.Countdown = signal.TimingPlans[signal.TimingPlans.FindIndex(TimingPlan => TimingPlan.Id.Equals(signal.CurrentTimingPlanID))].Phases[Ring.CurrentPhaseNum - 1].Yellow;
//                        Ring.ElapsedTime = 0;
//                    }
//                    else
//                    {
//                        if (Ring.CurrentIndication == TestRingData.Indication.Yellow)
//                        {
//                            Ring.CurrentIndication = TestRingData.Indication.Red;
//                            Ring.Countdown = signal.TimingPlans[signal.TimingPlans.FindIndex(TimingPlan => TimingPlan.Id.Equals(signal.CurrentTimingPlanID))].Phases[Ring.CurrentPhaseNum - 1].AllRed;
//                            Ring.ElapsedTime = 0;
//                        }
//                        else //This should happen on the same timestamp for all rings- cross from red across barrier to next green
//                        {
//                            Ring.CurrentPhaseIndex = Ring.CurrentPhaseIndex + 1;
//                            FindNextPhase(Ring, signal, ringIndex, detectors);
//                            //need to handle no calls until barrier (barrier to barrier)
//                            Ring.CurrentPhaseNum = Ring.NextPhaseNum;
//                            Ring.CurrentPhaseIndex = Ring.NextPhaseIndex;
//                            Ring.CurrentIndication = TestRingData.Indication.Green;
//                            if (Ring.CurrentPhaseNum == 255)
//                            {
//                                Ring.Countdown = 0.1;
//                            }
//                            else
//                            {
//                                Ring.Countdown = signal.TimingPlans[signal.TimingPlans.FindIndex(TimingPlan => TimingPlan.Id.Equals(signal.CurrentTimingPlanID))].Phases[Ring.CurrentPhaseNum - 1].MinGreen;
//                            }
//                            signal.BarrierConstraint = false;//Reset 
//                            Ring.ReadyForBarrier = false;
//                        }
//                    }
//                }
//                ringIndex++;
//            }
//        }

//        bool CheckDetectorCalls(List<byte> detectorIDs, List<DetectorData> detectors)
//        {
//            bool call = false;

//            foreach (byte DetectorID in detectorIDs)
//            {
//                if (detectors[detectors.FindIndex(DetectorData => DetectorData.DetectorId.Equals(DetectorID))].Call)
//                {
//                    call = true;
//                    break;
//                }
//            }

//            return call;
//        }
//    }

//    public class TestRingData
//    {
//        public enum Indication
//        {
//            Green,
//            Yellow,
//            Red
//        }

//        public int ID;
//        public byte CurrentPhaseNum;
//        public int CurrentPhaseIndex;
//        public Indication CurrentIndication;
//        public double Countdown;
//        public double ElapsedTime;
//        public bool ReadyForBarrier;
//        public byte NextPhaseNum;
//        public int NextPhaseIndex;

//    }
//}
