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

    public class SignalControllerOnRamp : SignalController
    {        
        RampMeterControlAlgorithm _controlAlgorithm;
        List<VehicleControlPointData> _associatedControlPoints;
        List<DetectorData> _associatedDetectors;
        List<PhaseData> _phases;

        public SignalControllerOnRamp(byte id, SignalControlMode controlMode, RampMeterControlAlgorithm controlAlgorithm, string label = "") : base(id, controlMode, label)
        {            
            _controlAlgorithm = controlAlgorithm;
            _associatedControlPoints = new List<VehicleControlPointData>();
            _associatedDetectors = new List<DetectorData>();
            _phases = new List<PhaseData>();
    }

        
        public RampMeterControlAlgorithm ControlAlgorithm { get => _controlAlgorithm; set => _controlAlgorithm = value; }        
        public List<VehicleControlPointData> AssociatedControlPoints { get => _associatedControlPoints; set => _associatedControlPoints = value; }
        public List<DetectorData> AssociatedDetectors { get => _associatedDetectors; set => _associatedDetectors = value; }
        public List<PhaseData> Phases { get => _phases; set => _phases = value; }
    }

    public class PhaseData
    {
        int ArraySize = 90002;
        byte _id;
        List<VehicleControlPointData> _associatedControlPoints;
        ControlDisplayIndication[] _display;

        public PhaseData()
        {
            _associatedControlPoints = new List<VehicleControlPointData>();
            _display = new ControlDisplayIndication[ArraySize];
        }

        public byte Id { get => _id; set => _id = value; }
        public List<VehicleControlPointData> AssociatedControlPoints { get => _associatedControlPoints; set => _associatedControlPoints = value; }
        public ControlDisplayIndication[] Display { get => _display; set => _display = value; }
    }
}