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


        public byte Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public List<byte> PhaseId
        {
            get { return _phaseId; }
            set { _phaseId = value; }
        }
        public List<PhaseMetricsData> PhaseMetrics
        {
            get { return _phaseMetrics; }
            set { _phaseMetrics = value; }
        }

    }


    public class PhaseMetricsData
    {

        byte _phaseId;
        int _startGreen;
        int _endGreen;
        int _beginVehicle;
        int _endVehicle;


        public byte PhaseId
        {
            get { return _phaseId; }
            set { _phaseId = value; }
        }
        public int StartGreen
        {
            get { return _startGreen; }
            set { _startGreen = value; }
        }

        public int EndGreen
        {
            get { return _endGreen; }
            set { _endGreen = value; }
        }

        public int BeginVehicle
        {
            get { return _beginVehicle; }
            set { _beginVehicle = value; }
        }

        public int EndVehicle
        {
            get { return _endVehicle; }
            set { _endVehicle = value; }
        }


        public PhaseMetricsData()
        {

        }
    }


}
