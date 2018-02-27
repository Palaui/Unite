using System;
using UnityEngine;

namespace Unite
{
    public class MathExt
    {
        // Values
        #region Values

        public static float Remap(float value, float from1, float to1, float from2, float to2)
        {
            if (value < from1)
                return from2;
            else if (value > to1)
                return to2;

            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }

        #endregion

        // Direction and Rotation
        #region Direction and Rotation

        public static Vector3 AddVectorLength(Vector3 vector, float size)
        {
            float magnitude = Vector3.Magnitude(vector);
            float newMagnitude = magnitude + size;
            float scale = newMagnitude / magnitude;
            return vector * scale;
        }

        public static Vector3 TransformDirectionMath(Quaternion rotation, Vector3 vector)
        { return rotation * vector; }

        public static Vector3 InverseTransformDirectionMath(Quaternion rotation, Vector3 vector)
        { return Quaternion.Inverse(rotation) * vector; }

        public static Vector3 RotateVectorFromTo(Quaternion from, Quaternion to, Vector3 vector)
        {
            Quaternion Q = SubtractRotation(to, from);
            Vector3 A = InverseTransformDirectionMath(from, vector);
            Vector3 B = Q * A;
            Vector3 C = TransformDirectionMath(from, B);
            return C;
        }

        public static Quaternion SubtractRotation(Quaternion B, Quaternion A)
        { return Quaternion.Inverse(A) * B; }

        public static float SignedDotProduct(Vector3 vectorA, Vector3 vectorB, Vector3 normal)
        {
            Vector3 perpVector = Vector3.Cross(normal, vectorA);
            return Vector3.Dot(perpVector, vectorB);
        }

        public static float SignedVectorAngle(Vector3 referenceVector, Vector3 otherVector, Vector3 normal)
        {
            Vector3 perpVector = Vector3.Cross(normal, referenceVector);
            float angle = Vector3.Angle(referenceVector, otherVector);
            return Mathf.Sign(Vector3.Dot(perpVector, otherVector)) * angle;
        }

        public static float DotProductAngle(Vector3 vec1, Vector3 vec2)
        {
            double dot = Vector3.Dot(vec1, vec2);
            if (dot < -1.0f)
                dot = -1.0f;
            if (dot > 1.0f)
                dot = 1.0f;

            return (float)Math.Acos(dot);
        }

        public static void LookRotationExtended(ref GameObject gameObjectInOut, Vector3 alignWithVector,
            Vector3 alignWithNormal, Vector3 customForward, Vector3 customUp)
        {
            Quaternion rotationA = Quaternion.LookRotation(alignWithVector, alignWithNormal);
            Quaternion rotationB = Quaternion.LookRotation(customForward, customUp);
            gameObjectInOut.transform.rotation = rotationA * Quaternion.Inverse(rotationB);
        }

        public static void VectorsToTransform(ref GameObject gameObjectInOut, Vector3 positionVector,
            Vector3 directionVector, Vector3 normalVector)
        {
            gameObjectInOut.transform.position = positionVector;
            gameObjectInOut.transform.rotation = Quaternion.LookRotation(directionVector, normalVector);
        }

        #endregion

        // Line
        #region Line and Point

        public static bool LinePlaneIntersection(Vector3 linePoint, Vector3 lineVec, Vector3 planeNormal, Vector3 planePoint,
            out Vector3 intersection)
        {
            Vector3 vector;
            float length;
            float dotNumerator;
            float dotDenominator;

            dotNumerator = Vector3.Dot((planePoint - linePoint), planeNormal);
            dotDenominator = Vector3.Dot(lineVec, planeNormal);
            intersection = Vector3.zero;

            if (dotDenominator != 0.0f)
            {
                length = dotNumerator / dotDenominator;
                vector = lineVec.normalized * length;
                intersection = linePoint + vector;
                return true;
            }
            else
                return false;
        }

        public static bool LineLineIntersection(Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2,
            out Vector3 intersection)
        {
            Vector3 lineVec3 = linePoint2 - linePoint1;
            Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
            Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);
            float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

            if (Mathf.Abs(planarFactor) < 0.0001f && crossVec1and2.sqrMagnitude > 0.0001f)
            {
                float dot = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
                intersection = linePoint1 + (lineVec1 * dot);
                return true;
            }
            else
            {
                intersection = Vector3.zero;
                return false;
            }
        }

        public static bool ClosestPointsOnTwoLines(Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2,
            out Vector3 closestPointLine1, out Vector3 closestPointLine2)
        {
            float a = Vector3.Dot(lineVec1, lineVec1);
            float b = Vector3.Dot(lineVec1, lineVec2);
            float e = Vector3.Dot(lineVec2, lineVec2);
            float d = a * e - b * b;

            closestPointLine1 = Vector3.zero;
            closestPointLine2 = Vector3.zero;

            if (d != 0.0f)
            {
                Vector3 r = linePoint1 - linePoint2;
                float c = Vector3.Dot(lineVec1, r);
                float f = Vector3.Dot(lineVec2, r);

                float s = (b * f - c * e) / d;
                float t = (a * f - c * b) / d;

                closestPointLine1 = linePoint1 + lineVec1 * s;
                closestPointLine2 = linePoint2 + lineVec2 * t;

                return true;
            }
            else
                return false;
        }

        public static Vector3 ProjectPointOnLine(Vector3 linePoint, Vector3 lineVec, Vector3 point)
        {
            Vector3 linePointToPoint = point - linePoint;
            float t = Vector3.Dot(linePointToPoint, lineVec);
            return linePoint + lineVec * t;
        }

        public static Vector3 ProjectPointOnLineSegment(Vector3 linePoint1, Vector3 linePoint2, Vector3 point)
        {
            Vector3 vector = linePoint2 - linePoint1;
            Vector3 projectedPoint = ProjectPointOnLine(linePoint1, vector.normalized, point);
            int side = PointOnWhichSideOfLineSegment(linePoint1, linePoint2, projectedPoint);

            if (side == 0)
                return projectedPoint;
            if (side == 1)
                return linePoint1;
            if (side == 2)
                return linePoint2;
            return Vector3.zero;
        }

        public static int PointOnWhichSideOfLineSegment(Vector3 linePoint1, Vector3 linePoint2, Vector3 point)
        {
            Vector3 lineVec = linePoint2 - linePoint1;
            Vector3 pointVec = point - linePoint1;
            float dot = Vector3.Dot(pointVec, lineVec);

            if (dot > 0)
            {
                if (pointVec.magnitude <= lineVec.magnitude)
                    return 0;
                else
                    return 2;
            }
            else
                return 1;
        }

        public static bool IsLineInRectangle(Vector3 linePoint1, Vector3 linePoint2, Vector3 rectA, Vector3 rectB, Vector3 rectC, Vector3 rectD)
        {
            bool pointAInside = IsPointInRectangle(linePoint1, rectA, rectC, rectB, rectD);
            bool pointBInside = false;

            if (!pointAInside)
                pointBInside = IsPointInRectangle(linePoint2, rectA, rectC, rectB, rectD);

            if (!pointAInside && !pointBInside)
            {
                bool lineACrossing = AreLineSegmentsCrossing(linePoint1, linePoint2, rectA, rectB);
                bool lineBCrossing = AreLineSegmentsCrossing(linePoint1, linePoint2, rectB, rectC);
                bool lineCCrossing = AreLineSegmentsCrossing(linePoint1, linePoint2, rectC, rectD);
                bool lineDCrossing = AreLineSegmentsCrossing(linePoint1, linePoint2, rectD, rectA);

                if (lineACrossing || lineBCrossing || lineCCrossing || lineDCrossing)
                    return true;
                else
                    return false;
            }
            else
                return true;
        }

        public static bool IsPointInRectangle(Vector3 point, Vector3 rectA, Vector3 rectC, Vector3 rectB, Vector3 rectD)
        {
            Vector3 vector = rectC - rectA;
            float size = -(vector.magnitude / 2f);
            vector = AddVectorLength(vector, size);
            Vector3 middle = rectA + vector;

            Vector3 xVector = rectB - rectA;
            float width = xVector.magnitude / 2f;

            Vector3 yVector = rectD - rectA;
            float height = yVector.magnitude / 2f;

            Vector3 linePoint = ProjectPointOnLine(middle, xVector.normalized, point);
            vector = linePoint - point;
            float yDistance = vector.magnitude;

            linePoint = ProjectPointOnLine(middle, yVector.normalized, point);
            vector = linePoint - point;
            float xDistance = vector.magnitude;

            if ((xDistance <= width) && (yDistance <= height))
                return true;
            else
                return false;
        }

        public static bool AreLineSegmentsCrossing(Vector3 pointA1, Vector3 pointA2, Vector3 pointB1, Vector3 pointB2)
        {
            Vector3 closestPointA;
            Vector3 closestPointB;
            int sideA;
            int sideB;

            Vector3 lineVecA = pointA2 - pointA1;
            Vector3 lineVecB = pointB2 - pointB1;

            bool valid = ClosestPointsOnTwoLines(pointA1, lineVecA.normalized, pointB1, lineVecB.normalized,
                out closestPointA, out closestPointB);

            if (valid)
            {
                sideA = PointOnWhichSideOfLineSegment(pointA1, pointA2, closestPointA);
                sideB = PointOnWhichSideOfLineSegment(pointB1, pointB2, closestPointB);

                if ((sideA == 0) && (sideB == 0))
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        #endregion

        // Plane
        #region Plane

        public static bool PlanePlaneIntersection(Vector3 plane1Normal, Vector3 plane1Position, Vector3 plane2Normal,
            Vector3 plane2Position, out Vector3 linePoint, out Vector3 lineVec)
        {
            linePoint = Vector3.zero;
            lineVec = Vector3.Cross(plane1Normal, plane2Normal);

            Vector3 ldir = Vector3.Cross(plane2Normal, lineVec);
            float denominator = Vector3.Dot(plane1Normal, ldir);

            if (Mathf.Abs(denominator) > 0.006f)
            {
                Vector3 plane1ToPlane2 = plane1Position - plane2Position;
                float t = Vector3.Dot(plane1Normal, plane1ToPlane2) / denominator;
                linePoint = plane2Position + t * ldir;
                return true;
            }
            else
                return false;
        }

        public static Vector3 ProjectPointOnPlane(Vector3 planeNormal, Vector3 planePoint, Vector3 point)
        {
            float distance = -SignedDistancePlanePoint(planeNormal, planePoint, point);
            Vector3 translationVector = planeNormal.normalized * distance;
            return point + translationVector;
        }

        public static Vector3 ProjectVectorOnPlane(Vector3 planeNormal, Vector3 vector)
        { return vector - (Vector3.Dot(vector, planeNormal) * planeNormal); }

        public static float SignedDistancePlanePoint(Vector3 planeNormal, Vector3 planePoint, Vector3 point)
        { return Vector3.Dot(planeNormal, (point - planePoint)); }

        public static float AngleVectorPlane(Vector3 vector, Vector3 normal)
        {
            float dot = Vector3.Dot(vector, normal);
            float angle = (float)Math.Acos(dot);
            return 1.570796326794897f - angle;
        }

        #endregion

        // Matrix
        #region Matrix4x4

        public static Quaternion QuaternionFromMatrix(Matrix4x4 m)
        { return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1)); }

        public static Vector3 PositionFromMatrix(Matrix4x4 m)
        {
            Vector4 vector4Position = m.GetColumn(3);
            return new Vector3(vector4Position.x, vector4Position.y, vector4Position.z);
        }

        #endregion

        // Mouse
        #region Mouse

        public static float MouseDistanceToLine(Vector3 linePoint1, Vector3 linePoint2)
        {
            Camera currentCamera = Camera.main;
            Vector3 mousePosition = Input.mousePosition;
            Vector3 screenPos1 = currentCamera.WorldToScreenPoint(linePoint1);
            Vector3 screenPos2 = currentCamera.WorldToScreenPoint(linePoint2);
            Vector3 projectedPoint = ProjectPointOnLineSegment(screenPos1, screenPos2, mousePosition);

            projectedPoint = new Vector3(projectedPoint.x, projectedPoint.y, 0f);
            return (projectedPoint - mousePosition).magnitude;
        }

        public static float MouseDistanceToCircle(Vector3 point, float radius)
        {
            Camera currentCamera = Camera.main;
            Vector3 screenPos = currentCamera.WorldToScreenPoint(point);
            screenPos = new Vector3(screenPos.x, screenPos.y, 0f);

            Vector3 vector = screenPos - Input.mousePosition;
            float fullDistance = vector.magnitude;
            return fullDistance - radius;
        }

        #endregion

    }
}
