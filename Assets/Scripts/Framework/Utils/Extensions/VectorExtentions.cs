using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

namespace Extensions
{
    public static class VectorExtentions
    {
        /// <summary>
        /// gets closest point from given list of positions
        /// </summary>
        /// <param name="otherPositions">list of other positions</param>
        /// <returns>the closest point</returns>
        public static Vector3 GetClosest(this Vector3 vector3, IEnumerable<Vector3> otherPositions)
        {
            return otherPositions.OrderBy(trans => Vector3.Distance(trans, vector3)).First();
        }

        /// <summary>
        /// gets furthest point from given list of positions
        /// </summary>
        /// <param name="otherPositions">list of other positions</param>
        /// <returns>the furthest point</returns>
        public static Vector3 GetFurthest(this Vector3 vector3, IEnumerable<Vector3> otherPositions)
        {
            return otherPositions.OrderBy(trans => Vector3.Distance(trans, vector3)).Last();
        }


        /// <summary>
        /// gets closest point from given list of positions
        /// </summary>
        /// <param name="otherPositions">list of other positions</param>
        /// <returns>the closest point</returns>
        public static Vector2 GetClosest(this Vector2 vector2, IEnumerable<Vector2> otherPositions)
        {
            return otherPositions.OrderBy(trans => Vector2.Distance(trans, vector2)).First();
        }

        /// <summary>
        /// gets furthest point from given list of positions
        /// </summary>
        /// <param name="otherPositions">list of other positions</param>
        /// <returns>the furthest point</returns>
        public static Vector2 GetFurthest(this Vector2 vector3, IEnumerable<Vector3> otherPositions)
        {
            return otherPositions.OrderBy(trans => Vector2.Distance(trans, vector3)).Last();
        }

        /// <summary>
        /// Get the list of positions ordered by closest to furthest  
        /// </summary>
        /// <param name="otherPositions">list of other positions</param>
        /// <returns>list of ordered position by distance</returns>
        public static Vector3[] GetPositionsByDistance(this Vector3 vector3, IEnumerable<Vector3> otherPositions)
        {
            return otherPositions.OrderBy(trans => Vector3.Distance(trans, vector3)).ToArray();
        }

        /// <summary>
        /// Get the list of positions ordered by closest to furthest  
        /// </summary>
        /// <param name="otherPositions">list of other positions</param>
        /// <returns>list of ordered position by distance</returns>
        public static Vector2[] GetPositionsByDistance(this Vector2 vector2, IEnumerable<Vector2> otherPositions)
        {
            return otherPositions.OrderBy(trans => Vector2.Distance(trans, vector2)).ToArray();
        }

        /// <summary>
        /// checks if the vector is 0 
        /// </summary>
        /// <param name="dir">the vector</param>
        /// <returns>return true or false</returns>
        public static bool IsLengthZero(this Vector3 dir)
        {
            return dir.sqrMagnitude == 0;
        }

        /// <summary>
        /// checks if the vector is 0 
        /// </summary>
        /// <param name="dir">the vector</param>
        /// <returns>return true or false</returns>
        public static bool IsLengthZero(this Vector2 dir)
        {
            return dir.sqrMagnitude == 0;
        }

        /// <summary>
        /// Check if the vector is pointing in the same direction as otherVector
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="otherVector"></param>
        /// <param name="treshold">The treshold to allow a margin</param>
        /// <returns></returns>
        public static bool IsSameDirectionAs(this Vector3 dir, Vector3 otherVector, float treshold = 0.1f)
        {
            return Vector3.Dot(dir.normalized, otherVector.normalized) > 1 - treshold;
        }


        /// <summary>
        /// Check if the vector is pointing in the same direction as otherVector
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="otherVector"></param>
        /// <param name="treshold">The treshold to allow a margin</param>
        /// <returns></returns>
        public static bool IsSameDirectionAs(this Vector2 dir, Vector3 otherVector, float treshold = 0.1f)
        {
            return Vector3.Dot(dir.normalized, otherVector.normalized) > 1 - treshold;
        }

        /// <summary>
        /// Check if the vector is pointing in the opposite direction as otherVector
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="otherVector"></param>
        /// <param name="treshold">The treshold to allow a margin</param>
        /// <returns></returns>
        public static bool IsOppositeDirectionAs(this Vector3 dir, Vector3 otherVector, float treshold = 0.1f)
        {
            return Vector3.Dot(dir.normalized, otherVector.normalized) < -1 + treshold;
        }

        /// <summary>
        /// Check if the vector is pointing in the opposite direction as otherVector
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="otherVector"></param>
        /// <param name="treshold">The treshold to allow a margin</param>
        /// <returns></returns>
        public static bool IsOppositeDirectionAs(this Vector2 dir, Vector2 otherVector, float treshold = 0.1f)
        {
            return Vector3.Dot(dir.normalized, otherVector.normalized) < -1 + treshold;
        }

        /// <summary>
        /// Check if the vector is pointing in the same OR opposite direction as otherVector
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="otherVector"></param>
        /// <param name="treshold">The treshold to allow a margin</param>
        /// <returns></returns>
        public static bool IsParallelDirectionAs(this Vector3 dir, Vector3 otherVector, float treshold = 0.1f)
        {
            return Mathf.Abs(Vector3.Dot(dir.normalized, otherVector.normalized)) > 1 - treshold;
        }

        /// <summary>
        /// Check if the vector is pointing in the same OR opposite direction as otherVector
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="otherVector"></param>
        /// <param name="treshold">The treshold to allow a margin</param>
        /// <returns></returns>
        public static bool IsParallelDirectionAs(this Vector2 dir, Vector2 otherVector, float treshold = 0.1f)
        {
            return Mathf.Abs(Vector3.Dot(dir.normalized, otherVector.normalized)) > 1 - treshold;
        }

        public static bool IsDirectingTowards(this Vector2 dir)
        {
            return dir.sqrMagnitude == 0;
        }

        public static Vector2 OffSetToDirectionXY(this Vector2 vec, Vector2 prev, Vector2 next, float offset)
        {
            Vector2 pointBetween = Vector3.Lerp(prev, next, .5f);
            Vector2 dirToCurrent = pointBetween.Direction(vec);
            return dirToCurrent * -offset + vec;
        }

        public static Vector2 Direction(this Vector2 v, Vector2 targetVector)
        {
            Vector2 dir = v - targetVector;
            return dir;
        }

        public static Vector2 xy(this Vector3 v)
        {
            return new Vector2(v.x, v.y);
        }

        public static Vector3 SetX(this Vector3 v, float x)
        {
            return new Vector3(x, v.y, v.z);
        }

        public static Vector3 SetY(this Vector3 v, float y)
        {
            return new Vector3(v.x, y, v.z);
        }

        public static Vector3 SetZ(this Vector3 v, float z)
        {
            return new Vector3(v.x, v.y, z);
        }

        public static Vector2 SetX(this Vector2 v, float x)
        {
            return new Vector2(x, v.y);
        }

        public static Vector2 SetY(this Vector2 v, float y)
        {
            return new Vector2(v.x, y);
        }

        public static Vector3 SetZ(this Vector2 v, float z)
        {
            return new Vector3(v.x, v.y, z);
        }


        //adding
        public static Vector3 AddX(this Vector3 v, float x)
        {
            return new Vector3(v.x + x, v.y, v.z);
        }

        public static Vector3 AddY(this Vector3 v, float y)
        {
            return new Vector3(v.x, v.y + y, v.z);
        }

        public static Vector3 AddZ(this Vector3 v, float z)
        {
            return new Vector3(v.x, v.y, v.z + z);
        }

        public static Vector2 AddX(this Vector2 v, float x)
        {
            return new Vector2(v.x + x, v.y);
        }

        public static Vector2 AddY(this Vector2 v, float y)
        {
            return new Vector2(v.x, v.y + y);
        }

        public static Vector2 InvertedX(this Vector2 targetVector)
        {
            return new Vector2(-targetVector.x, targetVector.y);
        }

        public static Vector2 InvertedY(this Vector2 targetVector)
        {
            return new Vector2(targetVector.x, -targetVector.y);
        }

        public static float LengthSquare(this Vector2 vec1, Vector2 vec2)
        {
            float xDiff = vec1.x - vec2.x;
            float yDiff = vec2.y - vec1.y;
            return xDiff * xDiff + yDiff * yDiff;
        }

        public static Vector2 AddVector(this Vector2 v, Vector2 addVec)
        {
            return new Vector2(v.x + addVec.x, v.y + addVec.y);
        }

        public static Vector3 AddVector(this Vector3 v, Vector3 addVec)
        {
            return new Vector3(v.x + addVec.x, v.y + addVec.y, v.z + addVec.z);
        }
        
        public static int GetDirection(this Vector3 targetVector)
        {
            return Math.Sign(targetVector.x);
        }

        public static Vector3 Rotate(this Vector3 target, float degrees)
        {
            float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
            float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

            float tx = target.x;
            float ty = target.y;
            target.x = (cos * tx) - (sin * ty);
            target.y = (sin * tx) + (cos * ty);
            return target;
        }

        public static Vector3 RotateAround(this Vector3 target, Vector3 origin, float degrees)
        {
            var step = target - origin;
            float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
            float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

            float tx = step.x;
            float ty = step.y;
            step.x = (cos * tx) - (sin * ty);
            step.y = (sin * tx) + (cos * ty);
            return step;
        }
    }
}
