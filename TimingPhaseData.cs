using SwashSim_VehControlPoint;
using SwashSim_VehicleDetector;
using System;
using System.Collections.Generic;


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


    public class PhaseTimingData
    {
        int ArraySize = 54002;
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

        ControlDisplayIndication[] _display;  // = new PhaseDisplay[54002];
        //private List<byte> _associatedControlPointIds;
        string _associatedControlPointIdsString;
        //private List<byte> _associatedDetectorIds;
        string _associatedDetectorIdsString;
        //private System.Drawing.Color _displayColor;

        List<VehicleControlPointData> _associatedControlPoints;
        List<DetectorData> _associatedDetectors;
        

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


        public byte Id
        {
            get { return _id; }
            set { _id = value; }
        }
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value; }
        }
        public Single GreenMin
        {
            get { return _greenMin; }
            set { _greenMin = value; }
        }
        public Single GreenMax
        {
            get { return _greenMax; }
            set { _greenMax = value; }
        }
        public Single YellowTime
        {
            get { return _yellowTime; }
            set { _yellowTime = value; }
        }
        public Single RedTime
        {
            get { return _redTime; }
            set { _redTime = value; }
        }
        public Single AllRedTime
        {
            get { return _allRedTime; }
            set { _allRedTime = value; }
        }
        public Single GreenTimeElapsed
        {
            get { return _greenTimeElapsed; }
            set { _greenTimeElapsed = value; }
        }
        public Single YellowTimeElapsed
        {
            get { return _yellowTimeElapsed; }
            set { _yellowTimeElapsed = value; }
        }
        public Single AllRedTimeElapsed
        {
            get { return _allRedTimeElapsed; }
            set { _allRedTimeElapsed = value; }
        }
        public Single RedTimeElapsed
        {
            get { return _redTimeElapsed; }
            set { _redTimeElapsed = value; }
        }
        public Single[] IntervalTimeRemaining
        {
            get { return _intervalTimeRemaining; }
            set { _intervalTimeRemaining = value; }
        }
        public PhaseInterval[] ActiveInterval
        {
            get { return _activeInterval; }
            set { _activeInterval = value; }
        }
        public Single UnitExtension
        {
            get { return _unitExtension; }
            set { _unitExtension = value; }
        }
        public int[] IntervalTimer
        {
            get { return _intervalTimer; }
            set { _intervalTimer = value; }
        }
        public ControlDisplayIndication[] Display
        {
            get { return _display; }
            set { _display = value; }
        }

        public List<VehicleControlPointData> AssociatedControlPoints
        {
            get { return _associatedControlPoints; }
            set { _associatedControlPoints = value; }
        }

        public List<DetectorData> AssociatedDetectors
        {
            get { return _associatedDetectors; }
            set { _associatedDetectors = value; }
        }

        public string AssociatedControlPointIdsString
        {
            get { return _associatedControlPointIdsString; }
            set { _associatedControlPointIdsString = value; }
        }

        public string AssociatedDetectorIdsString
        {
            get { return _associatedDetectorIdsString; }
            set { _associatedDetectorIdsString = value; }
        }
    }

}
