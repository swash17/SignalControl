using System.Collections.Generic;

namespace SwashSim_SignalControl
{
    public class CoordinationMasterClock
    {
        byte _id;
        float _maxTimeSeconds;
        List<byte> _associatedSignalControllerIds;

        public CoordinationMasterClock(byte id)
        {
            Id = id;
        }

        public byte Id { get => _id; set => _id = value; }
        public float MaxTimeSeconds { get => _maxTimeSeconds; set => _maxTimeSeconds = value; }
        public List<byte> AssociatedSignalControllerIds { get => _associatedSignalControllerIds; set => _associatedSignalControllerIds = value; }
    }
    
    public class CoordinationData
    {
        byte _masterSignalId;
        float _cycleLength;
        float _offsetPctOfCycle;
        byte _offsetReferencePhaseId;
        float _yieldPointPctOfCycle;
        float _forceOffPctOfCycle;
        int _referenceTimingStageIndex;
        float _referencePhaseCycleOffset;
        float _referencePhaseOffsetRelativeToMasterSeconds;

        public byte MasterSignalId { get => _masterSignalId; set => _masterSignalId = value; }
        public float OffsetPctOfCycle { get => _offsetPctOfCycle; set => _offsetPctOfCycle = value; }
        public byte OffsetReferencePhaseId { get => _offsetReferencePhaseId; set => _offsetReferencePhaseId = value; }
        public float CycleLength { get => _cycleLength; set => _cycleLength = value; }
        public float YieldPointPctOfCycle { get => _yieldPointPctOfCycle; set => _yieldPointPctOfCycle = value; }
        public float ForceOffPctOfCycle { get => _forceOffPctOfCycle; set => _forceOffPctOfCycle = value; }
        public float ReferencePhaseCycleOffset { get => _referencePhaseCycleOffset; set => _referencePhaseCycleOffset = value; }
        public float ReferencePhaseOffsetRelativeToMasterSeconds { get => _referencePhaseOffsetRelativeToMasterSeconds; set => _referencePhaseOffsetRelativeToMasterSeconds = value; }
        public int ReferenceTimingStageIndex { get => _referenceTimingStageIndex; set => _referenceTimingStageIndex = value; }
    }
}
