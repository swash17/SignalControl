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
        Pedestrian
    }
    
    public class SignalController //Parent class to determine which controller type to use
    {
        byte _Id;
        string _label;
        SignalControlMode _controlMode;

        public SignalController(byte id, SignalControlMode controlMode, string label = "") //Logic method that tells the program which controller class to use (pretimed or actuated)
        {
            _Id = id;
            _label = label;
            _controlMode = controlMode;

            if (_controlMode == SignalControlMode.Actuated)
            {
                
            }
            else
            {
                _controlMode = SignalControlMode.Pretimed;
            }

        }

    }
}
