using System.Collections.Generic;



namespace SwashSim_SignalControl
{


    public class TimingPlanData
    {
        uint _id;
        string _label;
        List<TimingStageData> _timingStages;        
        List<TimingRingData> _timingRings;
        float _cycleLength;
        uint _startTimeSeconds;
        uint _endTimeSeconds;
        bool _isRunningCoordination;
        CoordinationData _coordination;


        public TimingPlanData(byte id, string label)  // byte[] ringOneIncludedPhases, byte[] ringTwoIncludedPhases, Single[,] ringOnePhaseIntervalTimes, Single[,] ringTwoPhaseIntervalTimes)
        {

            _id = id;
            _label = label;
            _timingStages = new List<TimingStageData>();
            _timingRings = new List<TimingRingData>();
            _startTimeSeconds = 0;
            _coordination = new CoordinationData();

        }

        public uint Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Label
        {
            get { return _label; }
            set { _label = value; }
        }
        public List<TimingStageData> TimingStages
        {
            get { return _timingStages; }
            set { _timingStages = value; }
        }
        public List<TimingRingData> TimingRings
        {
            get { return _timingRings; }
            set { _timingRings = value; }
        }

        public uint StartTimeSeconds
        {
            get { return _startTimeSeconds; }
            set { _startTimeSeconds = value; }
        }

        public uint EndTimeSeconds
        {
            get { return _endTimeSeconds; }
            set { _endTimeSeconds = value; }
        }

        public bool IsRunningCoordination { get => _isRunningCoordination; set => _isRunningCoordination = value; }
        public CoordinationData Coordination { get => _coordination; set => _coordination = value; }
        public float CycleLength { get => _cycleLength; set => _cycleLength = value; }
    }

    

}
