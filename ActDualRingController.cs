using System;
using System.Collections.Generic;
using System.Linq;
using SwashSim_VehControlPoint;


namespace SwashSim_SignalControl
{
    public class ActDualRingController : SignalControllerActuated
    {
        private List<Ring> _rings;
        private List<Barrier> _barriers;

        public ActDualRingController(byte ID) : base(ID)
        {
            _rings = new List<Ring>();
            _barriers = new List<Barrier>();
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            foreach (Ring ring in _rings)
            {
                if (ring.ActivePhaseID == 0) continue;
                uint activePhaseID = ring.ActivePhaseID;
                double activeDuration = this.Phases.GetByID(activePhaseID).ActiveDuration(this._elapsedSimTime);
                if (this.Phases.GetByID(activePhaseID).TimingPlanParameters.MaxRecall)
                {
                    this.Phases.GetByID(activePhaseID).SetDesiredPhaseEnd(this._elapsedSimTime - activeDuration + this.Phases.GetByID(activePhaseID).TimingPlanParameters.GreenMax);
                }
                else
                {
                    if (this.Phases.GetByID(activePhaseID).Detectors.Call)
                    {
                        double maxGreenEnd = (double)this.Phases.GetByID(activePhaseID).TimingPlanParameters.GreenMax + this._elapsedSimTime - activeDuration;
                        double DesiredGreenEnd = this._elapsedSimTime + (double)Phases.GetByID(activePhaseID).TimingPlanParameters.GapTime;
                        this.Phases.GetByID(activePhaseID).SetDesiredPhaseEnd(maxGreenEnd < DesiredGreenEnd ? maxGreenEnd : DesiredGreenEnd);
                    }
                    if (activeDuration < this.Phases.GetByID(activePhaseID).TimingPlanParameters.GreenMin) continue;
                }

                if (activeDuration < this.Phases.GetByID(activePhaseID).DesiredDuration) continue;
                uint NextPhaseID = ring.GetNextPhase(activePhaseID);

                if (ring.SoftRecallPhaseID == 0)
                {
                    ring.TargetPhaseID = activePhaseID;
                }
                else
                {
                    if (this.Phases.GetByID(activePhaseID).Detectors.Call)
                    {
                        ring.TargetPhaseID = activePhaseID;
                    }
                    else
                    {
                        ring.TargetPhaseID = ring.SoftRecallPhaseID;
                    }
                }
                while (NextPhaseID != activePhaseID)
                {
                    if (this.Phases.GetByID(NextPhaseID).TimingPlanParameters.MinRecall || this.Phases.GetByID(NextPhaseID).TimingPlanParameters.MaxRecall || this.Phases.GetByID(NextPhaseID).Detectors.Call)
                    {
                        ring.TargetPhaseID = NextPhaseID;
                        break;
                    }
                    NextPhaseID = ring.GetNextPhase(NextPhaseID);
                }
                if (ring.ExistBarrier(activePhaseID, ring.TargetPhaseID))
                {
                    List<uint> BarrierIDs = ring.GetRelativeBarrierID(activePhaseID, ring.TargetPhaseID);
                    foreach (Barrier barrier in _barriers)
                    {
                        if (BarrierIDs.Contains(barrier.ID))
                        {
                            barrier.GetBarrierInRing(ring.ID).Activate();
                        }
                    }
                }
                else
                {
                    if (ring.TargetPhaseID != activePhaseID)
                    {
                        SwitchPhase(activePhaseID, ring.TargetPhaseID);
                    }
                }
            }
            foreach (Ring ring in _rings)
            {
                if (ring.ActivePhaseID == 0) continue;
                List<uint> BarrierIDs = ring.GetRelativeBarrierID(ring.ActivePhaseID, ring.TargetPhaseID);
                if (BarrierIDs.Count == 0) continue;
                bool BarriersUnlocked = true;
                int firstBlockedBarrierIndex = 0;
                foreach (uint barrierID in BarrierIDs)
                {
                    foreach (Barrier barrier in _barriers)
                    {
                        if (barrier.ID == barrierID && !barrier.Unlocked)
                        {
                            BarriersUnlocked = false;
                            firstBlockedBarrierIndex = BarrierIDs.IndexOf(barrierID);
                            goto Next;
                        }
                    }
                }
                Next:
                if (BarriersUnlocked)
                {
                    if (ring.TargetPhaseID != ring.ActivePhaseID)
                    {
                        SwitchPhase(ring.ActivePhaseID, ring.TargetPhaseID);
                    }
                }
                else if (firstBlockedBarrierIndex > 0)
                {
                    foreach (BarrierInRing barrier in ring.BarrierInRings)
                    {
                        if (barrier.BarrierID == BarrierIDs[firstBlockedBarrierIndex - 1])
                        {
                            SwitchPhase(ring.ActivePhaseID, barrier.NextPhaseID);
                            break;
                        }
                    }
                }
                ring.TargetPhaseID = 0;
            }
            foreach (Barrier barrier in _barriers)
            {
                barrier.Reset();
            }
        }

        public override void LoadTimingPlan()
        {
            base.LoadTimingPlan();
            for (int i = 0; i < ActiveTimingPlan.Phases.Length; i++)
            {
                PhaseTimingData phaseParameters = ActiveTimingPlan.Phases[i];
                if (!phaseParameters.PhaseOmit)
                {
                    ControllerPhase tempCPhase = new ControllerPhase((uint)phaseParameters.Id, phaseParameters);  //, Detectors);
                    this.Phases.Add(tempCPhase);
                }
            }
            for (int i = 0; i < ActiveTimingPlan.Rings.Count; i++)
            {
                Ring newRing = new Ring((uint)i + 1, ref _phases);
                for (int j = 0; j < ActiveTimingPlan.Rings[i].Count; j++)
                {
                    if (ActiveTimingPlan.Rings[i][j] != 0)
                    {
                        newRing.Phases.Add(ActiveTimingPlan.Rings[i][j]);
                    }
                }
                this._rings.Add(newRing);
            }
            //Create barrier objects
            List<BarrierInRing> barriers = new List<BarrierInRing>();
            for (int i = 0; i < ActiveTimingPlan.Rings.Count(); i++)
            {
                uint barrierID = 0;
                List<BarrierInRing> barriersInRing = new List<BarrierInRing>();
                for (int j = 0; j < ActiveTimingPlan.Rings[i].Count(); j++)
                {
                    if (ActiveTimingPlan.Rings[i][j] == 0)
                    {
                        barrierID++;
                        BarrierInRing newBarrierInRing;
                        if (j == 0)
                        {
                            newBarrierInRing = new BarrierInRing(Convert.ToUInt16(i + 1), barrierID, Convert.ToUInt16(ActiveTimingPlan.Rings[i][ActiveTimingPlan.Rings[i].Count - 1]), Convert.ToUInt16(ActiveTimingPlan.Rings[i][j + 1]));
                        }
                        else if (j == ActiveTimingPlan.Rings[i].Count - 1)
                        {
                            newBarrierInRing = new BarrierInRing(Convert.ToUInt16(i + 1), barrierID, Convert.ToUInt16(ActiveTimingPlan.Rings[i][j - 1]), Convert.ToUInt16(ActiveTimingPlan.Rings[i][0]));
                        }
                        else
                        {
                            newBarrierInRing = new BarrierInRing(Convert.ToUInt16(i + 1), barrierID, Convert.ToUInt16(ActiveTimingPlan.Rings[i][j - 1]), Convert.ToUInt16(ActiveTimingPlan.Rings[i][j + 1]));
                        }
                        barriers.Add(newBarrierInRing);
                    }
                }
            }
            List<uint> barrierIDs = new List<uint>();
            foreach (BarrierInRing barrierInRing in barriers)
            {
                if (!barrierIDs.Contains(barrierInRing.BarrierID))
                {
                    barrierIDs.Add(barrierInRing.BarrierID);
                }
            }
            foreach (uint barrierID in barrierIDs)
            {
                Barrier newBarrier = new Barrier(barrierID);
                foreach (BarrierInRing barrierInRing in barriers)
                {
                    if (barrierInRing.BarrierID == barrierID)
                    {
                        newBarrier.Add(barrierInRing);
                    }
                }
                this._barriers.Add(newBarrier);
            }
            foreach (Ring thisring in _rings)
            {
                foreach (BarrierInRing barrierInRing in barriers)
                {
                    if (barrierInRing.RingID == thisring.ID)
                    {
                        thisring.BarrierInRings.Add(barrierInRing);
                    }
                }
                foreach (uint phaseID in thisring.Phases)
                {
                    if (Phases.GetByID(phaseID).TimingPlanParameters.SoftRecall)
                    {
                        thisring.SoftRecallPhaseID = phaseID;
                    }
                }
            }

            foreach (ControllerPhase phaseFrom in this.Phases)
            {
                foreach (ControllerPhase phaseTo in this.Phases)
                {
                    if (phaseFrom.ID != phaseTo.ID)
                    {
                        this.InterGreens.Add(new InterGreen(phaseFrom.ID, phaseTo.ID, phaseFrom.TimingPlanParameters.YellowTime + phaseFrom.TimingPlanParameters.AllRedTime));
                    }
                }
            }
        }

        public void Initialize()
        {
            foreach (VehicleControlPointData controlpoint in this.VehicleControlPoints)
            {
                controlpoint.DisplayIndication = ControlDisplayIndication.Red;
            }
            foreach (ControllerPhase phase in this.Phases)
            {
                phase.Status = SignalStatus.Red;
            }
            foreach (Ring thisring in _rings)
            {
                Phases.GetByID(thisring.Phases[0]).Status = SignalStatus.Green;
            }
        }
    }

    public class Barrier : List<BarrierInRing>
    {
        private uint _id;
        public uint ID { get => _id; }

        public Barrier(uint ID)
            : base()
        {
            _id = ID;
        }

        public BarrierInRing GetBarrierInRing(uint RingID)
        {
            foreach (BarrierInRing temp in this)
            {
                if (temp.RingID == RingID)
                {
                    return temp;
                }
            }
            return null;
        }

        public bool Unlocked
        {
            get
            {
                foreach (BarrierInRing temp in this)
                {
                    if (!temp.Activated) return false;
                }
                return true;
            }
        }

        public void Reset()
        {
            foreach (BarrierInRing barrier in this)
            {
                barrier.Activated = false;
            }
        }
    }

    public class Ring
    {
        private uint _id;
        private List<uint> _phaseIDs;
        private List<BarrierInRing> _barrierInRings;
        private uint _softRecallPhaseID;
        private uint _targetPhaseID;
        private ControllerPhases _phasesInController;

        public uint ID { get => _id; }
        public List<uint> Phases { get => _phaseIDs; }
        public List<BarrierInRing> BarrierInRings { get => _barrierInRings; }
        public uint SoftRecallPhaseID { get => _softRecallPhaseID; set => _softRecallPhaseID = value; }
        public uint TargetPhaseID { get => _targetPhaseID; set => _targetPhaseID = value; }


        public Ring(uint ID, ref ControllerPhases Phases)
        {
            _id = ID;
            _phasesInController = Phases;
            _phaseIDs = new List<uint>();
            _barrierInRings = new List<BarrierInRing>();
        }

        public uint ActivePhaseID
        {
            get
            {
                foreach (uint phase in _phaseIDs)
                {
                    if (_phasesInController.GetByID(phase).PhaseActive)
                    {
                        return phase;
                    }
                }
                //0 means that the ring is proceeding phase switching, which is a symbol for whether the logic is to be run.
                return 0;
            }
        }


        public uint GetNextCallingPhase()
        {
            int index = _phaseIDs.IndexOf(ActivePhaseID);
            int searchingIndex = (index + 1) < _phaseIDs.Count ? (index + 1) : 0;
            while (index != searchingIndex)
            {
                if (_phasesInController.GetByID(_phaseIDs[searchingIndex]).Detectors.Call)
                {
                    return _phaseIDs[searchingIndex];
                }
                else
                {
                    searchingIndex = (searchingIndex + 1) < _phaseIDs.Count ? (searchingIndex + 1) : 0;
                }
            }
            //0 means there is no calling phase.
            return 0;
        }

        public uint GetNextPhase(uint ThisPhaseID)
        {
            int index = _phaseIDs.IndexOf(ThisPhaseID);
            if (index == this._phaseIDs.Count - 1)
            {
                return this._phaseIDs[0];
            }
            else
            {
                return this._phaseIDs[index + 1];
            }
        }

        public bool ExistBarrier(uint ActivePhaseID, uint CallingPhaseID)
        {
            if (CallingPhaseID == 0) return false;
            if (ActivePhaseID == CallingPhaseID) return true;
            int index = _phaseIDs.IndexOf(ActivePhaseID);
            while (index != _phaseIDs.IndexOf(CallingPhaseID))
            {
                foreach (BarrierInRing barrierInRing in _barrierInRings)
                {
                    if (barrierInRing.RingID == this._id)
                    {
                        if (_phaseIDs[index] == barrierInRing.PreviousPhaseID)
                        {
                            return true;
                        }
                    }
                }
                index = (index + 1) < _phaseIDs.Count ? (index + 1) : 0;
            }
            return false;
        }

        public List<uint> GetRelativeBarrierID(uint ActivePhaseID, uint CallingPhaseID)
        {
            List<uint> BarrierIDs = new List<uint>();
            if (CallingPhaseID == 0) return BarrierIDs;
            int index = _phaseIDs.IndexOf(ActivePhaseID);
            if (ActivePhaseID == CallingPhaseID)
            {
                do
                {
                    foreach (BarrierInRing barrierInRing in _barrierInRings)
                    {
                        if (barrierInRing.RingID == this._id)
                        {
                            if (_phaseIDs[index] == barrierInRing.PreviousPhaseID)
                            {
                                BarrierIDs.Add(barrierInRing.BarrierID);
                            }
                        }
                    }
                    index = (index + 1) < _phaseIDs.Count ? (index + 1) : 0;
                }
                while (index != _phaseIDs.IndexOf(CallingPhaseID));
            }
            else
            {
                while (index != _phaseIDs.IndexOf(CallingPhaseID))
                {
                    foreach (BarrierInRing barrierInRing in _barrierInRings)
                    {
                        if (barrierInRing.RingID == this._id)
                        {
                            if (_phaseIDs[index] == barrierInRing.PreviousPhaseID)
                            {
                                BarrierIDs.Add(barrierInRing.BarrierID);
                            }
                        }
                    }
                    index = (index + 1) < _phaseIDs.Count ? (index + 1) : 0;
                }
            }
            return BarrierIDs;
        }
    }

    public class BarrierInRing
    {
        private uint _ringID;
        private uint _barrierID;
        private uint _previousPhaseID;
        private uint _nextPhaseID;
        private bool _active;

        public uint RingID { get => _ringID; }
        public uint BarrierID { get => _barrierID; }
        public uint PreviousPhaseID { get => _previousPhaseID; set => _previousPhaseID = value; }
        public uint NextPhaseID { get => _nextPhaseID; set => _nextPhaseID = value; }
        public bool Activated { get => _active; set => _active = value; }

        public BarrierInRing(uint RingID, uint BarrierID, uint PreviousPhaseID, uint NextPhaseID)
        {
            _ringID = RingID;
            _barrierID = BarrierID;
            _previousPhaseID = PreviousPhaseID;
            _nextPhaseID = NextPhaseID;
            _active = false;
        }

        public void Activate()
        {
            this._active = true;
        }
    }
}
