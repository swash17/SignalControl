using System.Collections.Generic;
using SwashSim_VehControlPoint;


namespace SwashSim_SignalControl
{
    public enum RampMeterControlAlgorithm
    {
        None,
        Pretimed,
        ALINEA,
        FuzzyLogic
    }

    public class RampSignalController : SignalController
    {
        //byte _id;
        //string _label;
        RampMeterControlAlgorithm _controlAlgorithm;
        //List<uint> _associatedLinkIds;
        List<VehicleControlPointData> _associatedControlPoints;
        List<PhaseData> _phases;

        public RampSignalController(byte id, SignalControlMode controlMode, RampMeterControlAlgorithm controlAlgorithm, string label = "") : base(id, controlMode, label)
        {
            //_id = id;
            //_label = label;
            _controlAlgorithm = controlAlgorithm;
            //_associatedLinkIds = new List<uint>();
            _associatedControlPoints = new List<VehicleControlPointData>();
            _phases = new List<PhaseData>();
        }

        //public byte Id { get => _id; set => _id = value; }
        //public string Label { get => _label; set => _label = value; }
        public RampMeterControlAlgorithm ControlAlgorithm { get => _controlAlgorithm; set => _controlAlgorithm = value; }
        //public List<uint> AssociatedLinkIds { get => _associatedLinkIds; set => _associatedLinkIds = value; }
        public List<VehicleControlPointData> AssociatedControlPoints { get => _associatedControlPoints; set => _associatedControlPoints = value; }
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
