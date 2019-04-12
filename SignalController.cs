using System;
using System.Collections.Generic;
using SwashSim_VehControlPoint;
using SwashSim_VehicleDetector;

namespace SwashSim_SignalControl
{
    public enum SignalControlMode //Signal control mode available options
    {
        None,
        Pretimed,
        Actuated,
        RampMetering,
        Pedestrian
    }
    
    public class SignalController //Parent class to determine which controller type to use
    {
        byte _Id;
        string _label;
        SignalControlMode _controlMode;
        bool _isMaster;
        float _localClockCurrentTimeSeconds;
        List<uint> _associatedLinkIds;
        string _associatedLinkIdsString;
        List<CycleData> _cycleInfo;

        public SignalController(byte id, SignalControlMode controlMode, string label = "") //Logic method that tells the program which controller class to use (pretimed or actuated)
        {
            _Id = id;
            _label = label;
            _controlMode = controlMode;
            _associatedLinkIds = new List<uint>();
            _cycleInfo = new List<CycleData>();

            //if (_controlMode == SignalControlMode.Actuated)
            //{                
            //}
            //else
            //{
            //    _controlMode = SignalControlMode.Pretimed;
            //}

        }

        public byte Id { get => _Id; set => _Id = value; }
        public string Label { get => _label; set => _label = value; }
        public SignalControlMode ControlMode { get => _controlMode; set => _controlMode = value; }
        public float LocalClockCurrentTimeSeconds { get => _localClockCurrentTimeSeconds; set => _localClockCurrentTimeSeconds = value; }
        public bool IsMaster { get => _isMaster; set => _isMaster = value; }
        public List<uint> AssociatedLinkIds { get => _associatedLinkIds; set => _associatedLinkIds = value; }
        public string AssociatedLinkIdsString { get => _associatedLinkIdsString; set => _associatedLinkIdsString = value; }
        public List<CycleData> CycleInfo { get => _cycleInfo; set => _cycleInfo = value; }

        public string ConvertLinkIdListToString(List<UInt32> linkIds)
        {
            //Create string of vehicle control point IDs using comma to separate
            System.Text.StringBuilder linksLIst = new System.Text.StringBuilder();

            int TotalLinks = linkIds.Count;
            int NumLinks = 0;

            foreach (UInt32 linkID in linkIds)
            {
                linksLIst.Append(linkID);
                NumLinks++;

                if (NumLinks < TotalLinks)
                    linksLIst.Append(",");
            }

            string LinkIDsString = linksLIst.ToString();
            return LinkIDsString;
        }


    }
}
