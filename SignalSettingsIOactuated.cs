using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;


namespace SwashSim_SignalControl
{
    public class SignalSettingsIOactuated
    {
        public static void SerializeTimingPlan(string filename, List<ActTimingPlan> timingPlans)
        {
            // Writing the file requires a TextWriter.
            TextWriter myStreamWriter = new StreamWriter(filename);

            // Create the XmlSerializer instance.
            XmlSerializer mySerializer = new XmlSerializer(typeof(List<ActTimingPlan>));

            mySerializer.Serialize(myStreamWriter, timingPlans);
            myStreamWriter.Close();
        }

        public static List<ActTimingPlan> DeserializeTimingPlan(string filename)  //List<ActTimingPlan> timingPlans)
        {            
            FileStream myFileStream = new FileStream(filename, FileMode.Open);
            // Create the XmlSerializer instance.
            XmlSerializer mySerializer = new XmlSerializer(typeof(List<ActTimingPlan>));
            List<ActTimingPlan> timingPlans = (List<ActTimingPlan>)mySerializer.Deserialize(myFileStream);
            myFileStream.Close();

            return timingPlans;
        }

        public static VehicleControlPointsList OpenControlPointsFile(string filename) //(VehicleControlPointsList controlPoints)
        {
            //controlPoints = new VehicleControlPointsList();  // = new List<VehicleControlPointData>();           

            //string filename = @"X:\OneDrive\SwashSim\Projects\Signalized Intersections\Signal Timing_Actuated\Sig2-VehicleControlPoints.xml";
            //string filename = @"C:\Temp\SwashSim\Test Intersection\ActControlPoints.xml";
            System.IO.FileStream myFileStream = new System.IO.FileStream(filename, System.IO.FileMode.Open);
            System.Xml.Serialization.XmlSerializer mySerializer = new System.Xml.Serialization.XmlSerializer(typeof(VehicleControlPointsList)); // (typeof(List<VehicleControlPointData>));
            VehicleControlPointsList controlPoints = (VehicleControlPointsList)mySerializer.Deserialize(myFileStream);  //(List<VehicleControlPointData>)mySerializer.Deserialize(myFileStream);

            return controlPoints;
        }

        public static DetectorsList OpenDetectorsFile(string filename) //(DetectorsList detectors)
        {
            //detectors = new DetectorsList();  // = new List<DetectorData>();

            //string filename = @"X:\OneDrive\SwashSim\Projects\Signalized Intersections\Signal Timing_Actuated\Sig2-Detectors.xml";
            //string filename = @"C:\Temp\SwashSim\Test Intersection\ActDetectors.xml";
            System.IO.FileStream myFileStream = new System.IO.FileStream(filename, System.IO.FileMode.Open);
            System.Xml.Serialization.XmlSerializer mySerializer = new System.Xml.Serialization.XmlSerializer(typeof(DetectorsList)); //(typeof(List<DetectorData>));
            DetectorsList detectors = (DetectorsList)mySerializer.Deserialize(myFileStream); //(List<DetectorData>)mySerializer.Deserialize(myFileStream);

            return detectors;
        }

        public static void SaveControlPointsFile(VehicleControlPointsList controlPoints)  //(string filename)
        {
            string filename = @"C:\Temp\SwashSim\Test Intersection\ActControlPoints.xml";

            System.IO.TextWriter myStreamWriter = new System.IO.StreamWriter(filename);
            System.Xml.Serialization.XmlSerializer mySerializer = new System.Xml.Serialization.XmlSerializer(typeof(VehicleControlPointsList));
            mySerializer.Serialize(myStreamWriter, controlPoints);
            myStreamWriter.Close();
        }

        public static void SaveDetectorsFile(DetectorsList detectors)  //(string filename)
        {
            string filename = @"C:\Temp\SwashSim\Test Intersection\ActDetectors.xml";
            System.IO.TextWriter myStreamWriter = new System.IO.StreamWriter(filename);
            System.Xml.Serialization.XmlSerializer mySerializer = new System.Xml.Serialization.XmlSerializer(typeof(DetectorsList));
            mySerializer.Serialize(myStreamWriter, detectors);
            myStreamWriter.Close();
        }


    }
}
