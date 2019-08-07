using System.Collections.Generic;
using SwashSim_VehControlPoint;
using SwashSim_VehicleDetector;
using SwashSim_Network;

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
    }

    public class SignalControllerOnRamp : SignalController
    {
        RampMeterControlAlgorithm _controlAlgorithm;
        //List<VehicleControlPointData> _associatedControlPoints;
        VehicleControlPointsList _vehicleControlPoints;
        DetectorsList _detectors;
        List<PhaseData> _phases;
        CoordinationData _coordination;

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
        public CoordinationData Coordination { get => _coordination; set => _coordination = value; }
    }
}