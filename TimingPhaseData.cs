using SwashSim_VehControlPoint;
using SwashSim_VehicleDetector;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;


namespace SwashSim_SignalControl
{

    public enum PhaseInterval
    {
        MinGreen,
        Extension,
        MaxGreen,
        Yellow,
        AllRed,
        Red
    }

    [Serializable]
    public class PhaseTimingData
    {
        int ArraySize = 90002;
        [XmlAttribute("ID")]
        byte _id;
        bool _isEnabled;
        Single _greenMin;
        Single _greenMax;
        Single _yellowTime;
        Single _allRedTime;
        Single _redTime;
        Single _greenTimeElapsed;
        Single _yellowTimeElapsed;
        Single _allRedTimeElapsed;
        Single _redTimeElapsed;
        Single[] _intervalTimeRemaining;
        PhaseInterval[] _activeInterval;
        Single _unitExtension;
        int[] _intervalTimer;
        string _associatedControlPointIdsString;
        string _associatedDetectorIdsString;
        List<VehicleControlPointData> _associatedControlPoints;
        List<DetectorData> _associatedDetectors;
        ControlDisplayIndication[] _display;  // = new PhaseDisplay[54002];
        //private System.Drawing.Color _displayColor;

        //integrated from actuated control class
        float _gapTime;
        float _splitTime;
        bool _phaseOmit;
        bool _minRecall;
        bool _maxRecall;
        bool _softRecall;
        bool _gapReduction;
        float _timeBeforeReduction;
        float _timeToReduce;
        float _minimumGap;
        bool _leadLeft;
        float _pedGreen;
        float _pedClearance;
        List<byte> _associatedControlPointIds;
        List<byte> _associatedDetectorIds;


        public PhaseTimingData()
        {
            _intervalTimeRemaining = new Single[ArraySize];
            _activeInterval = new PhaseInterval[ArraySize];
            _display = new ControlDisplayIndication[ArraySize];
            //_associatedControlPointIds = new List<byte>();
            _associatedControlPoints = new List<VehicleControlPointData>();
            //_associatedDetectorIds = new List<byte>();
            _associatedDetectors = new List<DetectorData>();
        }

        public PhaseTimingData(byte phaseNum, Single minGreen, Single maxGreen, Single yellow, Single allRed)
        {
            _id = phaseNum;
            _isEnabled = true;
            _greenMin = minGreen;
            _greenMax = maxGreen;
            _yellowTime = yellow;
            _allRedTime = allRed;

            _intervalTimeRemaining = new Single[ArraySize];
            _activeInterval = new PhaseInterval[ArraySize];
            _display = new ControlDisplayIndication[ArraySize];
            //_associatedControlPointIds = new List<byte>();
            _associatedControlPoints = new List<VehicleControlPointData>();
            //_associatedDetectorIds = new List<byte>();
            _associatedDetectors = new List<DetectorData>();
        }

        public string ConvertControlPointIdListToString(List<VehicleControlPointData> controlPoints)
        {
            //Create string of vehicle control point IDs using comma to separate
            System.Text.StringBuilder controlPointsList = new System.Text.StringBuilder();

            int TotalControlPoints = controlPoints.Count;
            int NumControlPoints = 0;

            foreach (VehicleControlPointData controlPoint in controlPoints)
            {
                controlPointsList.Append(controlPoint.LinkId.ToString() + "-" + controlPoint.Id.ToString());
                NumControlPoints++;

                if (NumControlPoints < TotalControlPoints)
                    controlPointsList.Append(",");
            }

            string ControlPointIDs = controlPointsList.ToString();
            return ControlPointIDs;
        }

        public string ConvertDetectorIdListToString(List<DetectorData> detectors)
        {

            //Create string of Detector IDs using comma to separate
            System.Text.StringBuilder DetectorList = new System.Text.StringBuilder();

            int TotalDetectors = detectors.Count;
            int NumDetectors = 0;

            foreach (DetectorData detector in detectors)
            {
                DetectorList.Append(detector.LinkId.ToString() + "-" + detector.Id.ToString());
                NumDetectors++;

                if (NumDetectors < TotalDetectors)
                    DetectorList.Append(",");
            }

            string DetectorIDs = DetectorList.ToString();
            return DetectorIDs;
        }



        public byte Id { get => _id; set => _id = value; }
        public bool IsEnabled { get => _isEnabled; set => _isEnabled = value; }
        public float GreenMin { get => _greenMin; set => _greenMin = value; }
        public float GreenMax { get => _greenMax; set => _greenMax = value; }
        public float YellowTime { get => _yellowTime; set => _yellowTime = value; }
        public float AllRedTime { get => _allRedTime; set => _allRedTime = value; }
        public float RedTime { get => _redTime; set => _redTime = value; }
        public float GreenTimeElapsed { get => _greenTimeElapsed; set => _greenTimeElapsed = value; }
        public float YellowTimeElapsed { get => _yellowTimeElapsed; set => _yellowTimeElapsed = value; }
        public float AllRedTimeElapsed { get => _allRedTimeElapsed; set => _allRedTimeElapsed = value; }
        public float RedTimeElapsed { get => _redTimeElapsed; set => _redTimeElapsed = value; }
        public float[] IntervalTimeRemaining { get => _intervalTimeRemaining; set => _intervalTimeRemaining = value; }
        public PhaseInterval[] ActiveInterval { get => _activeInterval; set => _activeInterval = value; }
        public float UnitExtension { get => _unitExtension; set => _unitExtension = value; }
        public int[] IntervalTimer { get => _intervalTimer; set => _intervalTimer = value; }
        public string AssociatedControlPointIdsString { get => _associatedControlPointIdsString; set => _associatedControlPointIdsString = value; }
        public string AssociatedDetectorIdsString { get => _associatedDetectorIdsString; set => _associatedDetectorIdsString = value; }
        public List<VehicleControlPointData> AssociatedControlPoints { get => _associatedControlPoints; set => _associatedControlPoints = value; }
        public List<DetectorData> AssociatedDetectors { get => _associatedDetectors; set => _associatedDetectors = value; }
        public ControlDisplayIndication[] Display { get => _display; set => _display = value; }


        //integrated from actuated control class
        public float GapTime { get => _gapTime; set => _gapTime = value; }
        public float SplitTime { get => _splitTime; set => _splitTime = value; }
        public bool PhaseOmit { get => _phaseOmit; set => _phaseOmit = value; }
        public bool MinRecall { get => _minRecall; set => _minRecall = value; }
        public bool MaxRecall { get => _maxRecall; set => _maxRecall = value; }
        public bool SoftRecall { get => _softRecall; set => _softRecall = value; }
        public bool GapReduction { get => _gapReduction; set => _gapReduction = value; }
        public float TimeBeforeReduction { get => _timeBeforeReduction; set => _timeBeforeReduction = value; }
        public float TimeToReduce { get => _timeToReduce; set => _timeToReduce = value; }
        public float MinimumGap { get => _minimumGap; set => _minimumGap = value; }
        public bool LeadLeft { get => _leadLeft; set => _leadLeft = value; }
        public float PedGreen { get => _pedGreen; set => _pedGreen = value; }
        public float PedClearance { get => _pedClearance; set => _pedClearance = value; }
        public List<byte> AssociatedControlPointIds { get => _associatedControlPointIds; set => _associatedControlPointIds = value; }
        public List<byte> AssociatedDetectorIds { get => _associatedDetectorIds; set => _associatedDetectorIds = value; }

    }

}
