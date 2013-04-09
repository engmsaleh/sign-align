﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using Microsoft.Kinect;



namespace SignAlign
{


    /// <summary>
    /// Holds a series of joint positions through time from the sensor.
    /// Will give an instance of a given gesture
    /// </summary>
    class GestureRecording
    {
        private long totalTime; //We store the total time of the recording
        private Stopwatch sw = new Stopwatch(); //We use the stopwatch object to do this

        //This is a constanst array which contains those JointTypes which we track (the upper body)
        public static readonly JointType[] trackedJoints = 
        {
            JointType.HandRight, 
            JointType.HandLeft, 
            JointType.WristRight, 
            JointType.WristLeft,
            JointType.ElbowLeft,
            JointType.ElbowRight,
            JointType.ShoulderLeft,
            JointType.ShoulderRight,
            JointType.ShoulderCenter,
            JointType.Head
        };

        private Dictionary<JointType, List<double[]>> jointReadings 
            = new Dictionary<JointType,List<double[]>>();

        //We record the positions of the upper body joints given by kinect
        /*public List<Tuple<float, float, float>> hand_right        {get; private set;}
        public List<Tuple<float, float, float>> wrist_right       {get; private set;}
        public List<Tuple<float, float, float>> elbow_right       {get; private set;}
        public List<Tuple<float, float, float>> shoulder_right    {get; private set;}
        public List<Tuple<float, float, float>> hand_left         {get; private set;}
        public List<Tuple<float, float, float>> wrist_left        {get; private set;}
        public List<Tuple<float, float, float>> elbow_left        {get; private set;}
        public List<Tuple<float, float, float>> shoulder_left     {get; private set;}
        public List<Tuple<float, float, float>> shoulder_centre   {get; private set;}
        public List<Tuple<float, float, float>> head              {get; private set;}*/


        public GestureRecording()
        {
            jointReadings = new Dictionary<JointType, List<double[]>>(trackedJoints.Count());
            foreach (JointType j in trackedJoints)
            {
                jointReadings.Add(j, new List<double[]>());
            }
            sw.Stop(); //Start timing
                                  
            
        }
        //given a joint and a dimension (0=x, 1=y, 2=z) returns as a string the position sequence of that joint in that dimension
        public string asString(JointType j, int dimension)
        {

            StringBuilder builder = new StringBuilder();

            List<double[]> jStream;
            jointReadings.TryGetValue(j, out jStream);
            
            bool firstColumn = true;
            foreach (double[] pos in jStream)
            {
                if (firstColumn)
                {
                    builder.Append(pos[dimension].ToString());
                    firstColumn = false;
                }
                else
                {
                    builder.Append(",");
                    builder.Append(pos[dimension].ToString());
                }
            }
          
            return builder.ToString();
        }

        public bool lengthIsLessThan(int length)
        {
            List<double[]> reads;
            jointReadings.TryGetValue(trackedJoints[0], out reads);
            return reads.Count < length;
        }

        //Takes a skeleton and adds it's position to the current recording
        public void addReading(Skeleton skeleton)
        {
            foreach (JointType j in trackedJoints)
            {
                List<double[]> jReadings;
                jointReadings.TryGetValue(j, out jReadings);
                jReadings.Add(asDoubleArray(skeleton.Joints[j].Position)); 
                //Note that this works as jReadings is passed by reference. We really do update the value in the hash table.
            }
        }

        /// <summary>
        /// Saves this gesture recording as a test case. 
        /// </summary>
        /// <param name="fileLoc">Where to save the test case</param>
        /// <param name="name">The file name for this test</param>
        /// <param name="correctSign">The correct sign to associate with this test</param>
        public void saveAsTest(string fileLoc, string name, string correctSign)
        {

        }

        private double[] asDoubleArray(SkeletonPoint sp)
        {
            double[] sparr = {sp.X, sp.Y, sp.Z};
            return sparr;
        }

        private Tuple<float, float, float> makeTuple(SkeletonPoint sp)
        {
            return new Tuple<float, float, float>(sp.X, sp.Y, sp.Z); 
        }

        public void finish()
        {
            sw.Stop();
            totalTime = sw.ElapsedMilliseconds;
        }

    }
}
