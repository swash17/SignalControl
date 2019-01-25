using System;
using System.Collections.Generic;




namespace SwashSim_SignalControl
{


    public class TimingRingData
    {

        byte _id;        
        List<PhaseTimingData> _phases;
        private List<byte> _phaseSequence;
        byte _activePhaseIndex;
        byte _currentGreenPhaseId;
        byte _nextGreenPhaseId;


        public TimingRingData()
        {
            _phases = new List<PhaseTimingData>();
            _phaseSequence = new List<byte>();
            _activePhaseIndex = 1;
        }

        public TimingRingData(byte ringNum, byte[] includedPhases, Single[,] phaseIntervalTimes)
        {

            _id = ringNum;
            _phases = new List<PhaseTimingData>();
            _phaseSequence = new List<byte>();
            _activePhaseIndex = 1;
                        
            //PhaseData newPhase = new PhaseData();  //add dummy phase to zero index
            //_phases.Add(newPhase);

            SetPhaseIntervalTimes(includedPhases, phaseIntervalTimes);
            
        }

        public void SetPhaseIntervalTimes(byte[] includedPhases, Single[,] phaseIntervalTimes)
        {
            int PhaseIndex = -1;
            _phases.Clear();

            foreach (byte phaseNum in includedPhases)
            {
                //if (phaseNum == 0)
                //    break;

                //PhaseIndex = phaseNum - (4 * (ringNum - 1));
                PhaseIndex++;
                PhaseTimingData newPhase = new PhaseTimingData(phaseNum, phaseIntervalTimes[PhaseIndex, 0], phaseIntervalTimes[PhaseIndex, 1], phaseIntervalTimes[PhaseIndex, 2], phaseIntervalTimes[PhaseIndex, 3]);
                _phases.Add(newPhase);

                if (phaseIntervalTimes[PhaseIndex, 0] > 0)
                    _phaseSequence.Add(phaseNum);
            }       
        }

        public byte Id
        {
            get { return _id; }
            set { _id = value; }
        }

        //public List<byte> IncludedPhases
        //{
        //    get { return _includedPhases; }
        //    set { _includedPhases = value; }
        //}

        public List<PhaseTimingData> Phases
        {
            get { return _phases; }
            set { _phases = value; }
        }

        public List<byte> PhaseSequence
        {
            get { return _phaseSequence; }
            set { _phaseSequence = value; }
        }
        public byte ActivePhaseIndex
        {
            get { return _activePhaseIndex; }
            set { _activePhaseIndex = value; }
        }
        public byte CurrentGreenPhaseId
        {
            get { return _currentGreenPhaseId; }
            set { _currentGreenPhaseId = value; }
        }

        public byte NextGreenPhaseId
        {
            get { return _nextGreenPhaseId; }
            set { _nextGreenPhaseId = value; }
        }


    }

}
