using System;
using System.IO;

namespace SwashSim_SignalControllerActuated
{
    public class OutputSignalTSD
    {

        public static void WriteSignalTSDfile(string filename, SignalControllerActuated controller, int numTimeSteps, Single[] simTime)
        {            
            StreamWriter sw = new StreamWriter(filename); 

            sw.Write("SimTime, Phase Num, Status");//, Interval, Time Remain");
            sw.WriteLine();

            for (int TimeIndex = 0; TimeIndex < numTimeSteps; TimeIndex++)
            {
                foreach (ControllerPhase phase in controller.Phases)
                {
                    sw.Write(simTime[TimeIndex]);
                    sw.Write(",");
                    sw.Write(phase.ID);
                    sw.Write(",");
                    sw.Write(phase.StatusRecord[TimeIndex]);
                    //sw.Write(",");
                    //sw.Write(timingPlan.Phases[PhaseNum].ActiveInterval[TimeIndex]);
                    //sw.Write(",");
                    //sw.Write(timingPlan.Phases[PhaseNum].IntervalTimeRemaining[TimeIndex]);
                    sw.WriteLine();
                }
            }
            sw.Close();
        }

        
    }
}
