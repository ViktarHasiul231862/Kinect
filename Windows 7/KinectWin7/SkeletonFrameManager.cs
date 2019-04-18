using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonFrameManager
{
    [Serializable]
    public class CustomJoint
    {
        private double X;
        private double Y;
        private TrackingState state;
        public enum TrackingState
        {
            NotTracked = 0,
            Inferred = 1,
            Tracked = 2
        }
        public CustomJoint(double X, double Y, TrackingState state)
        {
            this.X = X;
            this.Y = Y;
            this.state = state;
        }

        public double getX()
        {
            return this.X;
        }

        public double getY()
        {
            return this.Y;
        }

        public void setX(double y)
        {
            this.X = y;
        }

        public void setY(double y)
        {
            this.Y = y;
        }
    };
    [Serializable]
    public class SkeletonFrame
    {
        private Dictionary<int, Dictionary<int, CustomJoint>> frame = new Dictionary<int, Dictionary<int, CustomJoint>>();

        public void addJointToPerson(int person, int jointType, double X, double Y, int state)
        {
            frame[person].Add(jointType, new CustomJoint(X, Y, (CustomJoint.TrackingState)state));
        }

        public void addPerson(int person, Dictionary<int, CustomJoint> joints)
        {
            frame.Add(person, joints);
        }

        public int getNumberOfBodies()
        {
            return frame.Count;
        }

        public CustomJoint getJoint(int person, int joint)
        {
            return frame[person][joint];
        }

    };

    public enum JointType
    {
        SpineBase = 0,
        SpineMid = 1,
        Neck = 2,
        Head = 3,
        ShoulderLeft = 4,
        ElbowLeft = 5,
        WristLeft = 6,
        HandLeft = 7,
        ShoulderRight = 8,
        ElbowRight = 9,
        WristRight = 10,
        HandRight = 11,
        HipLeft = 12,
        KneeLeft = 13,
        AnkleLeft = 14,
        FootLeft = 15,
        HipRight = 16,
        KneeRight = 17,
        AnkleRight = 18,
        FootRight = 19,
        SpineShoulder = 20,
        HandTipLeft = 21,
        ThumbLeft = 22,
        HandTipRight = 23,
        ThumbRight = 24
    }
}
