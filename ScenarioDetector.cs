using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;

namespace YouMote
{

/// <summary>
/// Takes in a skeleton and processes it.  Creates an internal state model of 
/// skeleton history (i.e.  states that represent the skeleton went sitting->standing->leaving).
/// The getScenarioState() function return the current state of the scenario, either completed or uncompleted or other variants if needed.
/// </summary>
    interface ScenarioDetector
    {
        /// <summary>
        /// gets the current state of the scenario (usually an enum meaning whether the scenario has happened or hasn't happened.
        /// We chose enum here because future detectors might be more nuanced than yes or no states.  It does this by looking at the history of 
        /// sub states for the skeleton (standing, sitting) and sees if these combine to a person leaving.
        /// </summary>
        /// <returns></returns>
        Boolean isScenarioDetected();

/// <summary>
/// Takes in a skeleton and uses posture detection and other qualities to determine a sub-state (standing,sitting,phoneing).  
/// It then adds this state to its history of states.
/// </summary>
/// <param name="skeleton"></param>
        void processSkeleton(Skeleton skeleton);
    }

    public class Point3D
    {
        public double X;
        public double Y;
        public double Z;

        public Point3D(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public Point3D(Joint j)
        {
            this.X = j.Position.X;
            this.Y = j.Position.Y;
            this.Z = j.Position.Z;
        }
        public Point3D add(Point3D p)
        {
            return new Point3D(this.X + p.X, this.Y + p.Y, this.Z + p.Z);
        }

        public Point3D subtract(Point3D p)
        {
            return new Point3D(this.X - p.X, this.Y - p.Y, this.Z - p.Z);
        }


        public double magnitude()
        {
            return Math.Sqrt(Math.Pow(this.X, 2) + Math.Pow(this.Y, 2) + Math.Pow(this.Z, 2));
        }


        public double calculateAngle(Point3D p)
        {
            double scalarProduct = this.dotProduct(p);
            double magnitudeA = this.magnitude();
            double magnitudeB = p.magnitude();

            double theta = Math.Acos(scalarProduct / (magnitudeA * magnitudeB)) * (180.0 / Math.PI);
            return theta;
        }

        public double dotProduct(Point3D p)
        {
            return (this.X*p.X + this.Y*p.Y + this.Z*p.Z);
        }
    }

}
