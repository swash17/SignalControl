using System;
using System.Collections.Generic;


namespace SwashSim_SignalControl
{

    

    public class TimingStageData
    {
        byte _id;
        bool _isEnabled = false;
        List<byte> _includedPhases;

        private Single _greenMin;
        private Single _greenMax;
        private Single _yellowTime;
        private Single _redTime;
        private Single _allRedTime;


        public TimingStageData(byte id)
        {
            _id = id;
            _isEnabled = true;
            _includedPhases = new List<byte>();

            _greenMin = 0;
            _greenMax = 10;
            _yellowTime = 3;
            _allRedTime = 2;
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

        public List<byte> IncludedPhases
        {
            get { return _includedPhases; }
            set { _includedPhases = value; }
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


    }
}
