using System.Collections.Generic;
using SwashSim_VehControlPoint;
using SwashSim_VehicleDetector;



namespace SwashSim_SignalControl
{
    public class SignalControllerActuated : SignalController
    {
        //byte _id;
        protected ControllerPhases _phases;
        InterGreens _interGreens;
        List<ActTimingPlan> _timingPlans;
        ActTimingPlan _activeTimingPlan;
        VehicleControlPointsList _vehicleControlPoints;
        DetectorsList _detectors;
        //List<uint> _associatedLinkIds;
        //SignalControlMode _controlMode;

        protected double _elapsedSimTime;
        List<SignalStatusConvertor> _convertors;

        //public byte ID
        //{
        //    get { return _id; }
        //    set {_id = value;}
        //}

        public ControllerPhases Phases
        {
            get { return _phases; }
        }

        public List<ActTimingPlan> TimingPlans { get => _timingPlans; set => _timingPlans = value; }

        public ActTimingPlan ActiveTimingPlan
        {
            get { return _activeTimingPlan; }
            set { _activeTimingPlan = value; }
        }

        public DetectorsList Detectors
        {
            get { return _detectors; }
            set { _detectors = value; }  //added by SSW
        }

        public VehicleControlPointsList VehicleControlPoints
        {
            get { return _vehicleControlPoints; }
            set { _vehicleControlPoints = value; }  //added by SSW
        }

        public InterGreens InterGreens
        {
            get { return _interGreens; }
        }

        //public List<uint> AssociatedLinkIds { get => _associatedLinkIds; set => _associatedLinkIds = value; }

        public SignalControllerActuated(byte ID, SignalControlMode controlMode) : base(ID, controlMode)
        {
            //_id = ID;
            _elapsedSimTime = 0;
            //_controlMode = controlMode;
            _phases = new ControllerPhases();
            _interGreens = new InterGreens(ref _phases);
            _timingPlans = new List<ActTimingPlan>();
            _vehicleControlPoints = new VehicleControlPointsList();
            _detectors = new DetectorsList();
            _convertors = new List<SignalStatusConvertor>();
            //_associatedLinkIds = new List<uint>();
        }

        public virtual void LoadTimingPlan()
        { }

        public void AddDetector(DetectorData Detector)
        {
            this._detectors.Add(Detector);
        }

        public void AddTimingPlan(ActTimingPlan TimingPlan)
        {
            this._timingPlans.Add(TimingPlan);
        }

        public void AddControlPoint(VehicleControlPointData ControlPoint)
        {
            this._vehicleControlPoints.Add(ControlPoint);
        }

        public void SwitchPhase(uint FromPhaseID, uint ToPhaseID)
        {
            if (FromPhaseID == 0 || ToPhaseID == 0) return;
            _convertors.Add(new SignalStatusConvertor(FromPhaseID, ToPhaseID, _elapsedSimTime, (double)Phases.GetByID(FromPhaseID).TimingPlanParameters.YellowTime, InterGreens.GetInterGreenValue(FromPhaseID, ToPhaseID).InterGreenValue));
        }

        public void UpdateSimTime(double ElapsedSimTime)
        {
            this._elapsedSimTime = ElapsedSimTime;
        }

        public void UpdateSignalStatus()
        {
            int i = 0;
            while (i < _convertors.Count)
            {
                SignalStatusConvertor convertor = _convertors[i];
                if (!convertor.InProcess(Phases, _elapsedSimTime))
                {
                    _convertors.Remove(convertor);
                }
                i++;
            }
        }

        public virtual void UpdateLogic()
        {
            //Do not delete; this function is overridden in dual-ring controller class
        }

        /*
        public virtual void UpdateControlPoints()
        {
            foreach (ControllerPhase phase in this.Phases)
            {
                VehicleControlPointsList controlPoints = new VehicleControlPointsList();
                foreach (uint VCID in phase.TimingPlanParameters.AssociatedControlPointIds)
                {
                    controlPoints.Add(_vehicleControlPoints.GetByID(VCID));
                }
                foreach (VehicleControlPointData controlPoint in controlPoints)
                {
                    if (phase.Status == SignalStatus.Green)
                        controlPoint.DisplayIndication = ControlDisplayIndication.Green;
                    if (phase.Status == SignalStatus.Yellow)
                        controlPoint.DisplayIndication = ControlDisplayIndication.Yellow;
                    if (phase.Status == SignalStatus.Red)
                        controlPoint.DisplayIndication = ControlDisplayIndication.Red;
                }
            }
        }
        */

        // Replacement method for above, by SSW ---------------------------------------------------------
        public void UpdateControlPoints(SwashSim_Network.NetworkData Network, int timeIndex)
        {
            int LinkIndex;
            
            foreach (ControllerPhase phase in this.Phases)
            {
                foreach (VehicleControlPointData PhaseControlPoint in phase.TimingPlanParameters.AssociatedControlPoints)
                {
                    foreach (uint LinkId in this.AssociatedLinkIds)
                    {
                        try
                        {
                            LinkIndex = Network.Links.FindIndex(Link => Link.Id.Equals(LinkId));

                            foreach (SwashSim_Network.LaneData Lane in Network.Links[LinkIndex].Lanes)
                            {
                                foreach (VehicleControlPointData LanecontrolPoint in Lane.ControlPoints)
                                {
                                    if (LanecontrolPoint.LinkId == PhaseControlPoint.LinkId && LanecontrolPoint.LaneId == Lane.Id && LanecontrolPoint.ControlPhaseId == PhaseControlPoint.ControlPhaseId)
                                    {
                                        if (phase.Status == SignalStatus.Green)
                                        {
                                            LanecontrolPoint.DisplayIndication = ControlDisplayIndication.Green;
                                            phase.TimingPlanParameters.Display[timeIndex] = ControlDisplayIndication.Green;
                                        }
                                        if (phase.Status == SignalStatus.Yellow)
                                        {
                                            LanecontrolPoint.DisplayIndication = ControlDisplayIndication.Yellow;
                                            phase.TimingPlanParameters.Display[timeIndex] = ControlDisplayIndication.Yellow;
                                        }
                                        if (phase.Status == SignalStatus.Red)
                                        {
                                            LanecontrolPoint.DisplayIndication = ControlDisplayIndication.Red;
                                            phase.TimingPlanParameters.Display[timeIndex] = ControlDisplayIndication.Red;
                                        }
                                    }
                                }
                            }
                        }
                        catch (System.Exception ex)
                        {
                            throw new System.Exception("UpdateControlPointDisplay Method; Signal Controller Id: " + this.Id + " Associated Link Id: " + LinkId.ToString() + " could not be found in network;", ex);
                        }
                    }
                }
            }
        }
        //-----------------------------------------------------------------------


        public void RecordPhaseStatus()
        {
            foreach (ControllerPhase phase in this._phases)
            {
                phase.StatusRecord.Add(phase.Status.ToString("g"));
            }
        }

        public void RecordVCPStatus()
        {
            foreach (VehicleControlPointData vcp in this._vehicleControlPoints)
            {
                //add a Lint<string> record into VCP class as is done in ControllerPhase class.
                //then add a row as below:
                //vcp.StatusRecord.Add(vcp.DisplayIndication.ToString("g"));
            }
        }

        public void Run(SignalControllerActuated signalActuated, double ElapsedSimTime)
        {
            UpdateSimTime(ElapsedSimTime);
            UpdateLogic();
            UpdateSignalStatus();
            //UpdateControlPoints();  //SSW--moved call to SimEngineMain so that Network links could be passed in
        }

        public void Run(double ElapsedSimTime, bool IsRecordPhaseStatus, bool IsRecordVCPStatus)
        {
            //this.Run(ElapsedSimTime);
            if (IsRecordPhaseStatus)
            {
                RecordPhaseStatus();
            }
            if (IsRecordVCPStatus)
            {
                RecordVCPStatus();
            }
        }
    }

    public class ControllerPhase
    {
        uint _id;
        SignalStatus _status;
        DetectorsList _detectors;
        double _activeElapsedSimTime;
        double _desiredEndSimTime;
        PhaseTimingData _timingPlanParameters;
        List<string> _statusRecord;

        public ControllerPhase(uint ID, PhaseTimingData TimingPlanParameters) //, DetectorsList Detectors)
        {
            _id = ID;
            _status = SignalStatus.Red;
            _activeElapsedSimTime = 0;
            _desiredEndSimTime = 0;
            _timingPlanParameters = TimingPlanParameters;
            _detectors = new DetectorsList();
            _statusRecord = new List<string>();

            //_detectors = (DetectorsList)TimingPlanParameters.AssociatedDetectors;  Cast does not work
            foreach (DetectorData detector in TimingPlanParameters.AssociatedDetectors)
            {
                _detectors.Add(detector);
            }

            //foreach (DetectorData detector in Detectors)
            //{
            //    foreach (byte detectorID in TimingPlanParameters.AssociatedDetectorIds)
            //    {
            //        if (detectorID == detector.Id)
            //            _detectors.Add(detector);
            //    }
            //}

        }

        public uint ID
        {
            get { return _id; }
        }

        public SignalStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }

        public DetectorsList Detectors
        {
            get { return _detectors; }
        }

        public List<string> StatusRecord
        {
            get { return _statusRecord; }
        }

        public PhaseTimingData TimingPlanParameters
        {
            get { return this._timingPlanParameters; }
        }

        public double DesiredDuration
        {
            get { return _desiredEndSimTime - _activeElapsedSimTime; }
        }

        public double ActiveDuration(double ElapsedSimTime)
        {
            if (PhaseActive)
            {
                return ElapsedSimTime - _activeElapsedSimTime;
            }
            else
            {
                return 0;
            }
        }

        public void SetDesiredPhaseEnd(double DesiredPhaseEndSimTime)
        {
            this._desiredEndSimTime = DesiredPhaseEndSimTime;
        }

        public void SetPhaseStartTime(double PhaseStartTime)
        {
            _activeElapsedSimTime = PhaseStartTime;
        }

        public bool PhaseActive
        {
            get
            {
                if (_status == SignalStatus.Green || _status == SignalStatus.GreenFlash)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }

    public class ControllerPhases : List<ControllerPhase>
    {
        public ControllerPhase GetByID(uint ID)
        {
            foreach (ControllerPhase temp in this)
            {
                if (temp.ID == ID)
                {
                    return temp;
                }
            }
            return null;
        }
    }

    public class SignalStatusConvertor
    {
        private uint _endingPhaseID;
        private uint _startingPhaseID;
        private double _greenEnd;
        private double _yellowEnd;
        private double _redEnd;

        public SignalStatusConvertor(uint EndingPhase, uint StartingPhase, double GreenEndTime, double YellowDuration, double InterGreenDuration)
        {
            _endingPhaseID = EndingPhase;
            _startingPhaseID = StartingPhase;
            _greenEnd = GreenEndTime;
            _yellowEnd = _greenEnd + YellowDuration;
            _redEnd = GreenEndTime + InterGreenDuration;
        }

        public bool InProcess(ControllerPhases Phases, double ElapsedSimTime)
        {
            if (ElapsedSimTime >= _redEnd)
            {
                Phases.GetByID(_startingPhaseID).Status = SignalStatus.Green;
                Phases.GetByID(_startingPhaseID).SetPhaseStartTime(ElapsedSimTime);
                return false;
            }

            if (ElapsedSimTime >= _greenEnd)
            {
                Phases.GetByID(_endingPhaseID).Status = SignalStatus.Yellow;
            }
            if (ElapsedSimTime >= _yellowEnd)
            {
                Phases.GetByID(_endingPhaseID).Status = SignalStatus.Red;
            }

            return true;
        }
    }

    public class InterGreen
    {
        private uint _fromGroupID;
        private uint _toGroupID;
        private double _interGreen;

        public uint FromGroup
        {
            get { return _fromGroupID; }
        }

        public uint ToGroup
        {
            get { return _toGroupID; }
        }

        public double InterGreenValue
        {
            get { return _interGreen; }
            set { _interGreen = value; }
        }

        public InterGreen(uint FromGroup, uint ToGroup, double GreenSplitValue)
        {
            this._fromGroupID = FromGroup;
            this._toGroupID = ToGroup;
            this._interGreen = GreenSplitValue;
        }
    }

    public class InterGreens : List<InterGreen>
    {
        private ControllerPhases _phases;

        public InterGreens(ref ControllerPhases Phases)
            : base()
        {
            this._phases = Phases;
        }

        public double[,] InterGreenMatrix
        {
            get
            {
                if (_phases.Count == 0)
                {
                    return null;
                }
                double[,] matrix = new double[_phases.Count, _phases.Count];
                for (int i = 0; i < _phases.Count; i++)
                {
                    for (int j = 0; j < _phases.Count; j++)
                    {
                        matrix[i, j] = -1;
                    }
                }
                foreach (InterGreen temp in this)
                {
                    int i = _phases.IndexOf(_phases.GetByID(temp.FromGroup));
                    int j = _phases.IndexOf(_phases.GetByID(temp.ToGroup));
                    matrix[i, j] = temp.InterGreenValue;
                }
                return matrix;
            }
        }

        public InterGreen GetInterGreenValue(uint FromGroup, uint ToGroup)
        {
            foreach (InterGreen temp in this)
            {
                if (temp.FromGroup == FromGroup && temp.ToGroup == ToGroup)
                {
                    return temp;
                }
            }
            return null;
        }

        public void SetInterGreen(uint FromGroup, uint ToGroup, double GreenSplitValue)
        {
            InterGreen tempGreenSplit = GetInterGreenValue(FromGroup, ToGroup);
            if (tempGreenSplit == null)
            {
                if (GreenSplitValue == -1)
                {
                    return;
                }
                else
                {
                    this.Add(new InterGreen(FromGroup, ToGroup, GreenSplitValue));
                }
            }
            else
            {
                if (GreenSplitValue == -1)
                {
                    this.Remove(tempGreenSplit);
                }
                else
                {
                    tempGreenSplit.InterGreenValue = GreenSplitValue;
                }
            }
        }
    }

    public class DetectorsList : List<DetectorData>
    {
        public DetectorData GetByID(uint ID)
        {
            foreach (DetectorData tempDetector in this)
            {
                if (tempDetector.Id == (byte)ID)
                {
                    return tempDetector;
                }
            }
            return null;
        }

        public bool Call
        {
            get
            {
                bool detectorCall = false;
                foreach (DetectorData detector in this)
                {
                    if (detector.IsOccupied)
                        detectorCall = true;
                }
                return detectorCall;
            }
        }
    }

    public class VehicleControlPointsList : List<VehicleControlPointData>
    {
        public VehicleControlPointData GetByID(uint ID)
        {
            foreach (VehicleControlPointData temp in this)
            {
                if (temp.Id == ID)
                {
                    return temp;
                }
            }
            return null;
        }
    }

    public enum SignalStatus
    {
        Red = 1,
        Yellow = 2,
        Green = 3,
        GreenFlash = 4,
        YellowFlash = 5,
        RedYellow = 6
    }
}
