using System;
using System.Collections.Generic;




namespace SwashSim_SignalControl
{

    public class CycleData
    {
        byte _id;
        List<byte> _phaseId;
        List<PhaseMetricsData> _phaseMetrics;


        public CycleData()
        {
            _phaseMetrics = new List<PhaseMetricsData>();
        }

        public byte Id { get => _id; set => _id = value; }
        public List<byte> PhaseId { get => _phaseId; set => _phaseId = value; }
        public List<PhaseMetricsData> PhaseMetrics { get => _phaseMetrics; set => _phaseMetrics = value; }
    }


    public class PhaseMetricsData
    {

        byte _phaseId;
        int _startGreen;
        int _endGreen;
        int _beginVehicle;
        int _endVehicle;

        public PhaseMetricsData()
        {

        }

        public byte PhaseId { get => _phaseId; set => _phaseId = value; }
        public int StartGreen { get => _startGreen; set => _startGreen = value; }
        public int EndGreen { get => _endGreen; set => _endGreen = value; }
        public int BeginVehicle { get => _beginVehicle; set => _beginVehicle = value; }
        public int EndVehicle { get => _endVehicle; set => _endVehicle = value; }
    }


}
