using System;
using System.Drawing;
using NUnit.Framework;

namespace Manipulation
{
    public static class AnglesToCoordinatesTask
    {
        public static PointF[] GetJointPositions(double shoulder, double elbow, double wrist)
        {
            var elbowPos = new PointF(0, 0) + GetPoint(Manipulator.UpperArm, shoulder);
            var wristAngle = elbow + shoulder - Math.PI;
            var wristPos = elbowPos + GetPoint(Manipulator.Forearm, wristAngle);
            var palmEndAngle = wristAngle + wrist - Math.PI;
            var palmEndPos = wristPos + GetPoint(Manipulator.Palm, palmEndAngle);
            return new PointF[]
            {
                elbowPos,
                wristPos,
                palmEndPos
            };
        }
        private static SizeF GetPoint(float distance, double angle)
        {
            return new SizeF(distance * (float)Math.Cos(angle),
                distance * (float)Math.Sin(angle));
        }
    }

    [TestFixture]
    public class AnglesToCoordinatesTask_Tests
    {
        public static double Distance(PointF a, PointF b)
        {
            double dx = b.X - a.X;
            double dy = b.Y - a.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
        [TestCase(Math.PI / 2, Math.PI / 2, Math.PI, Manipulator.Forearm + Manipulator.Palm, Manipulator.UpperArm)]
        [TestCase(Math.PI / 2, Math.PI / 2, Math.PI / 2, Manipulator.Forearm, Manipulator.UpperArm - Manipulator.Palm)]
        [TestCase(Math.PI / 2, 3 * Math.PI / 2, 3 * Math.PI / 2,
            -Manipulator.Forearm, Manipulator.UpperArm - Manipulator.Palm)]
        [TestCase(Math.PI / 2, Math.PI, 3 * Math.PI, 0, Manipulator.Forearm + Manipulator.UpperArm + Manipulator.Palm)]
        public void TestGetJointPositions(double shoulder, double elbow, double wrist, double palmEndX, double palmEndY)
        {
            var joints = AnglesToCoordinatesTask.GetJointPositions(shoulder, elbow, wrist);
            Assert.AreEqual(palmEndX, joints[2].X, 1e-5, "palm endX");
            Assert.AreEqual(palmEndY, joints[2].Y, 1e-5, "palm endY");
            Assert.AreEqual(Manipulator.UpperArm, Distance(joints[0], new PointF(0, 0)),
                            1e-5, "upper arm length");
            Assert.AreEqual(Manipulator.Forearm, Distance(joints[1], joints[0]),
                1e-5, "forearm length");
            Assert.AreEqual(Manipulator.Palm, Distance(joints[2], joints[1]),
                1e-5, "palm length");
        }
    }
}