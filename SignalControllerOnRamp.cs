using System.Collections.Generic;
using SwashSim_VehControlPoint;
using SwashSim_VehicleDetector;


namespace SwashSim_SignalControl
{
    public enum RampMeterControlAlgorithm
    {
        None,
        Pretimed,
        DemandCapacity,
        ALINEA,
        FuzzyLogic
    }

    public class PhaseData
    {
        int ArraySize = 90002;
        byte _id;
        //List<VehicleControlPointData> _associatedControlPoints;
        VehicleControlPointsList _vehicleControlPoints;
        DetectorsList _detectors;
        ControlDisplayIndication[] _display;

        public PhaseData()
        {
            //_associatedControlPoints = new List<VehicleControlPointData>();
            _vehicleControlPoints = new VehicleControlPointsList();
            _detectors = new DetectorsList();
            _display = new ControlDisplayIndication[ArraySize];
        }

        public byte Id { get => _id; set => _id = value; }
        //public List<VehicleControlPointData> AssociatedControlPoints { get => _associatedControlPoints; set => _associatedControlPoints = value; }
        public VehicleControlPointsList VehicleControlPoints { get => _vehicleControlPoints; set => _vehicleControlPoints = value; }
        public DetectorsList Detectors { get => _detectors; set => _detectors = value; }
        public ControlDisplayIndication[] Display { get => _display; set => _display = value; }

        //The following two methods are duplicates of the ones in PhaseTimingData class (TimingPhaseData.cs file)
        public string ConvertControlPointIdListToString(List<VehicleControlPointData> controlPoints)
        {
            //Create string of vehicle control point IDs using comma to separate
            System.Text.StringBuilder controlPointsList = new System.Text.StringBuilder();

            int TotalControlPoints = controlPoints.Count;
            int NumControlPoints = 0;

            foreach (VehicleControlPointData controlPoint in controlPoints)
            {
                //controlPointsList.Append(controlPoint.Id.ToString());
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
    }


    public class SignalControllerOnRamp : SignalController
    {
        RampMeterControlAlgorithm _controlAlgorithm;
        //List<VehicleControlPointData> _associatedControlPoints;
        VehicleControlPointsList _vehicleControlPoints;
        DetectorsList _detectors;
        List<PhaseData> _phases;
        bool _isCoordinated;

        public SignalControllerOnRamp(byte id, SignalControlMode controlMode, RampMeterControlAlgorithm controlAlgorithm, string label = "") : base(id, controlMode, label)
        {
            _controlAlgorithm = controlAlgorithm;
            //_associatedControlPoints = new List<VehicleControlPointData>();
            _vehicleControlPoints = new VehicleControlPointsList();
            _detectors = new DetectorsList();
            _phases = new List<PhaseData>();
        }

        public RampMeterControlAlgorithm ControlAlgorithm { get => _controlAlgorithm; set => _controlAlgorithm = value; }
        //public List<VehicleControlPointData> AssociatedControlPoints { get => _associatedControlPoints; set => _associatedControlPoints = value; }
        public List<PhaseData> Phases { get => _phases; set => _phases = value; }
        public VehicleControlPointsList VehicleControlPoints { get => _vehicleControlPoints; set => _vehicleControlPoints = value; }
        public DetectorsList Detectors { get => _detectors; set => _detectors = value; }
        public bool IsCoordinated { get => _isCoordinated; set => _isCoordinated = value; }
        
    }
}