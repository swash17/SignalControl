using System.Collections.Generic;



namespace SwashSim_SignalControl
{

    public static class PhasingInputs
    {

        public static bool[] IsRing1Active = new bool[9];
        public static bool[] IsRing2Active = new bool[9];
        public static bool[] IsConcurGroup1Active = new bool[9];
        public static bool[] IsConcurGroup2Active = new bool[9];



        public static void InitializePhaseStatus(List<TimingStageData> timingStages)
        {
            foreach (TimingStageData TimingStage in timingStages)
            {
                foreach (int PhaseNum in TimingStage.IncludedPhases)
                {
                    switch (PhaseNum)
                    {
                        case 1:
                            IsRing1Active[TimingStage.Id] = true;
                            IsConcurGroup1Active[TimingStage.Id] = true;
                            break;
                        case 2:
                            IsRing1Active[TimingStage.Id] = true;
                            IsConcurGroup1Active[TimingStage.Id] = true;
                            break;
                        case 3:
                            IsRing1Active[TimingStage.Id] = true;
                            IsConcurGroup2Active[TimingStage.Id] = true;
                            break;
                        case 4:
                            IsRing1Active[TimingStage.Id] = true;
                            IsConcurGroup2Active[TimingStage.Id] = true;
                            break;
                        case 5:
                            IsRing2Active[TimingStage.Id] = true;
                            IsConcurGroup1Active[TimingStage.Id] = true;
                            break;
                        case 6:
                            IsRing2Active[TimingStage.Id] = true;
                            IsConcurGroup1Active[TimingStage.Id] = true;
                            break;
                        case 7:
                            IsRing2Active[TimingStage.Id] = true;
                            IsConcurGroup2Active[TimingStage.Id] = true;
                            break;
                        case 8:
                            IsRing2Active[TimingStage.Id] = true;
                            IsConcurGroup2Active[TimingStage.Id] = true;
                            break;
                    }
                }
            }
        }


        public static void SetPhaseStatus(TimingStageData timingStage, int phaseNum, int activeStage)  // ref bool[,] IsPhaseActive)
        {
            //List<byte> PhaseNumsToCheck = new List<byte>();
                        
            bool IsPhaseNumInList = timingStage.IncludedPhases.Exists(item => item == phaseNum);

            if (IsPhaseNumInList == true)
            {
                int PhaseNumIndex = timingStage.IncludedPhases.FindIndex(item => item == phaseNum);
                timingStage.IncludedPhases.RemoveAt(PhaseNumIndex);  //user has clicked on previously active phase in the stage, now remove it
                PhaseBeingRemovedFromStage(timingStage, phaseNum, activeStage);
            }
            else
            {
                AddPhaseToStageIfNoConflict(timingStage, phaseNum, activeStage);
            }
        }

        public static void PhaseBeingRemovedFromStage(TimingStageData timingStage, int phaseNum, int activeStage)
        {
            bool IsPhaseNumInList2;

            switch (phaseNum)
            {
                case 1:
                    IsPhaseNumInList2 = CheckForPhasesPresentInTimingStage(timingStage.IncludedPhases, 2, 3, 4);
                    if (IsPhaseNumInList2 == false)
                        IsRing1Active[activeStage] = false;

                    IsPhaseNumInList2 = CheckForPhasesPresentInTimingStage(timingStage.IncludedPhases, 2, 5, 6);
                    if (IsPhaseNumInList2 == false)
                        IsConcurGroup1Active[activeStage] = false;
                    break;
                case 2:
                    IsPhaseNumInList2 = CheckForPhasesPresentInTimingStage(timingStage.IncludedPhases, 1, 3, 4);
                    if (IsPhaseNumInList2 == false)
                        IsRing1Active[activeStage] = false;

                    IsPhaseNumInList2 = CheckForPhasesPresentInTimingStage(timingStage.IncludedPhases, 1, 5, 6);
                    if (IsPhaseNumInList2 == false)
                        IsConcurGroup1Active[activeStage] = false;
                    break;
                case 3:
                    IsPhaseNumInList2 = CheckForPhasesPresentInTimingStage(timingStage.IncludedPhases, 1, 2, 4);
                    if (IsPhaseNumInList2 == false)
                        IsRing1Active[activeStage] = false;

                    IsPhaseNumInList2 = CheckForPhasesPresentInTimingStage(timingStage.IncludedPhases, 4, 7, 8);
                    if (IsPhaseNumInList2 == false)
                        IsConcurGroup2Active[activeStage] = false;
                    break;
                case 4:
                    IsPhaseNumInList2 = CheckForPhasesPresentInTimingStage(timingStage.IncludedPhases, 1, 2, 3);
                    if (IsPhaseNumInList2 == false)
                        IsRing1Active[activeStage] = false;

                    IsPhaseNumInList2 = CheckForPhasesPresentInTimingStage(timingStage.IncludedPhases, 3, 7, 8);
                    if (IsPhaseNumInList2 == false)
                        IsConcurGroup2Active[activeStage] = false;
                    break;
                case 5:
                    IsPhaseNumInList2 = CheckForPhasesPresentInTimingStage(timingStage.IncludedPhases, 6, 7, 8);
                    if (IsPhaseNumInList2 == false)
                        IsRing2Active[activeStage] = false;

                    IsPhaseNumInList2 = CheckForPhasesPresentInTimingStage(timingStage.IncludedPhases, 1, 2, 6);
                    if (IsPhaseNumInList2 == false)
                        IsConcurGroup1Active[activeStage] = false;
                    break;
                case 6:
                    IsPhaseNumInList2 = CheckForPhasesPresentInTimingStage(timingStage.IncludedPhases, 5, 7, 8);
                    if (IsPhaseNumInList2 == false)
                        IsRing2Active[activeStage] = false;

                    IsPhaseNumInList2 = CheckForPhasesPresentInTimingStage(timingStage.IncludedPhases, 1, 2, 5);
                    if (IsPhaseNumInList2 == false)
                        IsConcurGroup1Active[activeStage] = false;
                    break;
                case 7:
                    IsPhaseNumInList2 = CheckForPhasesPresentInTimingStage(timingStage.IncludedPhases, 5, 6, 8);
                    if (IsPhaseNumInList2 == false)
                        IsRing2Active[activeStage] = false;

                    IsPhaseNumInList2 = CheckForPhasesPresentInTimingStage(timingStage.IncludedPhases, 3, 4, 8);
                    if (IsPhaseNumInList2 == false)
                        IsConcurGroup2Active[activeStage] = false;
                    break;
                case 8:
                    IsPhaseNumInList2 = CheckForPhasesPresentInTimingStage(timingStage.IncludedPhases, 5, 6, 7);
                    if (IsPhaseNumInList2 == false)
                        IsRing2Active[activeStage] = false;

                    IsPhaseNumInList2 = CheckForPhasesPresentInTimingStage(timingStage.IncludedPhases, 3, 4, 7);
                    if (IsPhaseNumInList2 == false)
                        IsConcurGroup2Active[activeStage] = false;
                    break;
            }
        }

        public static bool AddPhaseToStageIfNoConflict(TimingStageData timingStage, int phaseNum, int activeStage)
        {
            bool DoNotEnablePhase = false;

            switch (phaseNum)
            {
                case 1:
                    if (IsRing1Active[activeStage] == true || IsConcurGroup2Active[activeStage] == true)
                        DoNotEnablePhase = true;
                    else
                    {
                        IsRing1Active[activeStage] = true;
                        IsConcurGroup1Active[activeStage] = true;
                    }
                    break;
                case 2:
                    if (IsRing1Active[activeStage] == true || IsConcurGroup2Active[activeStage] == true)
                        DoNotEnablePhase = true;
                    else
                    {
                        IsRing1Active[activeStage] = true;
                        IsConcurGroup1Active[activeStage] = true;
                    }
                    break;
                case 3:
                    if (IsRing1Active[activeStage] == true || IsConcurGroup1Active[activeStage] == true)
                        DoNotEnablePhase = true;
                    else
                    {
                        IsRing1Active[activeStage] = true;
                        IsConcurGroup2Active[activeStage] = true;
                    }
                    break;
                case 4:
                    if (IsRing1Active[activeStage] == true || IsConcurGroup1Active[activeStage] == true)
                        DoNotEnablePhase = true;
                    else
                    {
                        IsRing1Active[activeStage] = true;
                        IsConcurGroup2Active[activeStage] = true;
                    }
                    break;
                case 5:
                    if (IsRing2Active[activeStage] == true || IsConcurGroup2Active[activeStage] == true)
                        DoNotEnablePhase = true;
                    else
                    {
                        IsRing2Active[activeStage] = true;
                        IsConcurGroup1Active[activeStage] = true;
                    }
                    break;
                case 6:
                    if (IsRing2Active[activeStage] == true || IsConcurGroup2Active[activeStage] == true)
                        DoNotEnablePhase = true;
                    else
                    {
                        IsRing2Active[activeStage] = true;
                        IsConcurGroup1Active[activeStage] = true;
                    }
                    break;
                case 7:
                    if (IsRing2Active[activeStage] == true || IsConcurGroup1Active[activeStage] == true)
                        DoNotEnablePhase = true;
                    else
                    {
                        IsRing2Active[activeStage] = true;
                        IsConcurGroup2Active[activeStage] = true;
                    }
                    break;
                case 8:
                    if (IsRing2Active[activeStage] == true || IsConcurGroup1Active[activeStage] == true)
                        DoNotEnablePhase = true;
                    else
                    {
                        IsRing2Active[activeStage] = true;
                        IsConcurGroup2Active[activeStage] = true;
                    }
                    break;
            }

            if (DoNotEnablePhase == true)
            {
                //MessageBox.Show("A conflicting movement is already specified for this timing stage.", "Timing Stage Input Error");
            }
            else
            {
                //IsPhaseActive[activeStage, phaseNum] = true;
                timingStage.IncludedPhases.Add((byte)phaseNum);
            }
            return DoNotEnablePhase;
        }

        
        private static bool CheckForPhasesPresentInTimingStage(List<byte> includedPhases, byte PhaseId1, byte PhaseId2, byte PhaseId3)  //  List<byte> phaseIdsToCheck)
        {
            foreach (byte PhaseId in includedPhases)
            {
                if (PhaseId == PhaseId1 || PhaseId == PhaseId2 || PhaseId == PhaseId3)
                    return true;
            }
            return false;
        }


        public static void SetPhaseTimes(TimingPlanData timingPlan)
        {
            bool[] IsPhaseInPreviousStage = new bool[9];

            timingPlan.TimingRings[0].Phases.Clear();
            timingPlan.TimingRings[1].Phases.Clear();
            timingPlan.TimingRings[0].PhaseSequence.Clear();
            timingPlan.TimingRings[1].PhaseSequence.Clear();

            PhaseTimingData newPhase;
            newPhase = new PhaseTimingData();
            timingPlan.TimingRings[0].Phases.Add(newPhase);  //phase 0 is dummy phase
            newPhase = new PhaseTimingData();
            timingPlan.TimingRings[1].Phases.Add(newPhase);

            for (int StageNum = 1; StageNum < timingPlan.TimingStages.Count; StageNum++)
            {
                foreach (byte phaseNum in timingPlan.TimingStages[StageNum].IncludedPhases)
                {
                    if (phaseNum < 5)
                    {
                        bool PhaseAlreadyInTimingRing = timingPlan.TimingRings[0].Phases.Exists(Phase => Phase.Id.Equals(phaseNum));
                        //byte value = Array.Find(timingPlan.TimingRings[0].Phases, element => element.Equals(phaseNum));  //see if phase # already exists in ring

                        if (PhaseAlreadyInTimingRing == false)
                        {
                            timingPlan.TimingRings[0].PhaseSequence.Add(phaseNum);
                        }

                        if (IsPhaseInPreviousStage[phaseNum] == false)
                        {
                            newPhase = new PhaseTimingData(phaseNum, timingPlan.TimingStages[StageNum].GreenMin, timingPlan.TimingStages[StageNum].GreenMax, timingPlan.TimingStages[StageNum].YellowTime, timingPlan.TimingStages[StageNum].AllRedTime);

                            timingPlan.TimingRings[0].Phases.Add(newPhase);
                            IsPhaseInPreviousStage[phaseNum] = true;
                        }
                        else
                        {
                            int PhaseIndex = timingPlan.TimingRings[0].Phases.Count - 1;

                            //for overlapping phase, also include yellow+all-red time from previous timing stage in green time calc
                            float AddedTime = timingPlan.TimingStages[StageNum - 1].YellowTime + timingPlan.TimingStages[StageNum - 1].AllRedTime + timingPlan.TimingStages[StageNum].GreenMax;
                            timingPlan.TimingRings[0].Phases[PhaseIndex].GreenMax += AddedTime;

                            IsPhaseInPreviousStage[phaseNum] = false;
                        }
                    }
                    else
                    {
                        bool PhaseAlreadyInTimingRing = timingPlan.TimingRings[1].Phases.Exists(Phase => Phase.Id.Equals(phaseNum));
                        //byte value = Array.Find(timingPlan.TimingRings[1].Phases, element => element.Equals(phaseNum));  //see if phase # already exists in ring

                        if (PhaseAlreadyInTimingRing == false)
                        {
                            timingPlan.TimingRings[1].PhaseSequence.Add(phaseNum);
                        }

                        if (IsPhaseInPreviousStage[phaseNum] == false)
                        {
                            newPhase = new PhaseTimingData(phaseNum, timingPlan.TimingStages[StageNum].GreenMin, timingPlan.TimingStages[StageNum].GreenMax, timingPlan.TimingStages[StageNum].YellowTime, timingPlan.TimingStages[StageNum].AllRedTime);

                            timingPlan.TimingRings[1].Phases.Add(newPhase);                       
                            IsPhaseInPreviousStage[phaseNum] = true;
                        }
                        else
                        {
                            int PhaseIndex = timingPlan.TimingRings[1].Phases.Count - 1;

                            //for overlapping phase, also include yellow+all-red time from previous timing stage in green time calc
                            float AddedTime = timingPlan.TimingStages[StageNum - 1].YellowTime + timingPlan.TimingStages[StageNum - 1].AllRedTime + timingPlan.TimingStages[StageNum].GreenMax;
                            timingPlan.TimingRings[1].Phases[PhaseIndex].GreenMax += AddedTime;

                            IsPhaseInPreviousStage[phaseNum] = false;
                        }

                    }
                }
            }
        }





    }

}
