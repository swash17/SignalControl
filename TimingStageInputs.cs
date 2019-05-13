using System;
using System.Collections.Generic;


namespace SwashSim_SignalControl
{

    public class TimingStageInputs
    {

        public static void SetTimingStages(TimingPlanData timingPlan)
        {

            List<TimingStageData> TimingStages = new List<TimingStageData>();
            TimingStageData newTimingStage;
            byte StageNum = 1;
            //int PhaseSequenceIndex;

            //List<byte[]> PhaseSequence = new List<byte[]>(2);
            List<byte> PhaseSequence1 = new List<byte>();
            List<byte> PhaseSequence2 = new List<byte>();
            

            if (timingPlan.TimingRings[0] != null)
            {
                //foreach (PhaseData Phase in Ring.Phases)
                //foreach (PhaseData Phase in Ring.PhaseSequence)
                foreach (PhaseTimingData Phase in timingPlan.TimingRings[0].Phases)
                {
                    //Ring.PhaseSequence.Add(Phase.Id);        
                    PhaseSequence1.Add(Phase.Id);
                }
            }
            

            if (timingPlan.TimingRings[1] != null)
            {
                //foreach (PhaseData Phase in Ring.Phases)
                //foreach (PhaseData Phase in Ring.PhaseSequence)
                foreach (PhaseTimingData Phase in timingPlan.TimingRings[1].Phases)
                {
                    //Ring.PhaseSequence.Add(Phase.Id);        
                    PhaseSequence2.Add(Phase.Id);
                }
            }

            int MaxPhasesInRing = Math.Max(PhaseSequence1.Count, PhaseSequence2.Count);
            int[] PhaseTimes = new int[4];
            int PhaseCounter = 0;

            for (int index = 0; index < MaxPhasesInRing; index++)
            {
                if (StageNum > 1)
                {                    
                    foreach(byte PhaseNum in TimingStages[StageNum-2].IncludedPhases)
                    {                        
                        //PhaseTimes[PhaseCounter] = timingPlan.TimingRings[1].Phases[PhaseNum].GreenMax;
                        PhaseCounter++;
                    }
                    
                }
                newTimingStage = new TimingStageData(StageNum);
                newTimingStage.IncludedPhases.Add(PhaseSequence1[index]);
                newTimingStage.IncludedPhases.Add(PhaseSequence2[index]);
                TimingStages.Add(newTimingStage);
                StageNum++;

            }
        }



    }
}
