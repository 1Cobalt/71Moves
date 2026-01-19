using UnityEngine;

namespace SceneManagement.MyCamera
{

    [System.Serializable]
    public class SnapPoint
    {
        #region variable
        public float pivot;
        public float angle;
        public float orbitDampening = 1;
        #endregion
        #region functions
        #region PivotVector
        public Vector3 getPivotVectorDirection()
        {
            Vector3 v = new Vector3(1, 0, 0);
            return (getPivotVectorRotation() * v).normalized;
        }
        public Vector3 getPivotVector(Vector3 point, float magnitude)
        {
            return getPivotVectorDirection() * magnitude + point;
        }
        public Quaternion getPivotVectorRotation()
        {
            return (Quaternion.AngleAxis(pivot, Vector3.up));
        }
        public Quaternion getPivotVectorRotation(Vector3 point)
        {
            Vector3 v = getPivotVector(point, 1);
            return Quaternion.LookRotation(-v);
        }
        #endregion
        public void getX_Directions(out Vector3 x1, out Vector3 x2)
        {
            angle = Mathf.Clamp(angle, 0, 180);
            angle %= 360f;
            Vector3 v = getPivotVectorDirection();
            x1 = Quaternion.AngleAxis(angle, Vector3.up) * v;
            x2 = Quaternion.AngleAxis(-angle, Vector3.up) * v;
        }
        public void getX_Vectors(Vector3 point, float magnitude, out Vector3 x1, out Vector3 x2)
        {
            getX_Directions(out x1, out x2);
            x1 = x1 * magnitude + point;
            x2 = x2 * magnitude + point;
        }
        public bool isInRegion(Vector3 direction, Vector3 center)
        {
            return EqualToPivot(direction, center, angle);
        }
        public bool EqualToPivot(Vector3 direction, Vector3 center, float theta)
        {
            Vector3 v = new Vector3(direction.x, 0, direction.z);

            Vector3 pivot = getPivotVector(center, v.magnitude);

            float deltaTheta = Vector3.Angle(pivot - center, v - center);
            return deltaTheta <= theta;
        }
        public float getDeltaX(Vector3 direction, Vector3 center, float theta)
        {
            Vector3 v = new Vector3(direction.x, 0, direction.z);
            Vector3 pivot = getPivotVector(center, v.magnitude);

            v = v - center;
            pivot = pivot - center;
            int dir = 1;
            // float deltaTheta = Vector3.Angle(v, pivot);
            Vector3 cross = Vector3.Cross(v, pivot);
            if (cross.y < 0)
            {
                // deltaTheta *= -1;
                dir = -1;
            };

            return dir * theta;
        }
        #endregion
    }
}