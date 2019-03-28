using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwashSim_SignalControl
{
    public enum SignalControlMode
    {
        None,
        Pretimed,
        Actuated,
        Pedestrian
    }
    
    public class SignalController
    {
        byte _Id;
        string _label;
        SignalControlMode _controlMode;

        public SignalController(byte id, SignalControlMode controlMode, string label = "")
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
