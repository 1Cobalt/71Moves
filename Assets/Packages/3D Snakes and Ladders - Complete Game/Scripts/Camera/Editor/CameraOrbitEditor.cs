using UnityEngine;
using UnityEditor;
using SceneManagement.MyCamera;

namespace SceneManagement.Editors
{
    [CustomEditor(typeof(CameraOrbit))]
    public class CameraOrbitEditor : Editor
    {
        #region variable
        protected CameraOrbit Target;
        protected bool needsRepaint;
        protected float screenSpaceSize = 3;
        #endregion
        private void OnValidate()
        {
            Target = (CameraOrbit)target;
        }
        #region SceneGUI
        private void OnSceneGUI()
        {
            Event e = Event.current;

            if (e.type == EventType.Repaint || needsRepaint)
            {
                draw();
            }
            else if (Event.current.type == EventType.Layout)
            {
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
            }
            else
            {
                if (needsRepaint)
                {
                    HandleUtility.Repaint();
                }
            }
        }

        #region functions
        private Vector3 getMousePosition()
        {
            Ray mouseRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            float drawPlaneHeight = 0;
            float distanceToDrawPlane = (drawPlaneHeight - mouseRay.origin.y) / mouseRay.direction.y;
            return mouseRay.GetPoint(distanceToDrawPlane);
        }
        #region draw
        private void draw()
        {
            Target = (CameraOrbit)target;
            if (Target == null || Target.target == null) return;
            Vector3 center = Target.target.transform.position;
            if (Target.showCameraArea)
            {
                Handles.color = Color.green;
                Handles.RadiusHandle(Quaternion.identity, center, Target.maxDistance);
                Handles.Label(center + Vector3.up * Target.maxDistance, "Maximum camera");

                Handles.RadiusHandle(Quaternion.identity, center, Target.minDistance);
                Handles.BeginGUI();
                Handles.Label(center + Vector3.up * Target.minDistance, "Minimum camera");
                Handles.EndGUI();

                Handles.color = Color.red;
                Handles.RadiusHandle(Quaternion.identity, center, .1f);
                Handles.BeginGUI();
                Handles.Label(center, "Camera pivot");
                Handles.EndGUI();
            }
            if (Target.showSnapPoints)
            {
                if (Target.snapPoints != null)
                {
                    SnapPoint snapPoint;
                    for (int i = 0; i < Target.snapPoints.Length; i++)
                    {
                        if ((snapPoint = Target.snapPoints[i]) == null) continue;
                        draw_AdsorbentPoint(snapPoint, center, i, screenSpaceSize);
                    }
                }
            }
            needsRepaint = false;
        }
        private void draw_AdsorbentPoint(SnapPoint snapPoint, Vector3 center, int index, float spaceSize)
        {
            Handles.color = Color.red;
            Vector3 pivot = snapPoint.getPivotVectorDirection();

            Vector3 pivot1 = snapPoint.getPivotVector(center, Target.minDistance);

            Handles.DrawDottedLine(center, pivot1, spaceSize);

            Vector3 pivot2 = snapPoint.getPivotVector(center, Target.maxDistance);
            Handles.DrawLine(pivot1, pivot2);
            Handles.Label(pivot2, "Snap Point[" + index.ToString() + "].Pivot");

            Handles.color = Color.yellow;
            Vector3 x1, x2;
            snapPoint.getX_Vectors(center, Target.minDistance, out x1, out x2);
            Vector3 x3, x4;
            snapPoint.getX_Vectors(center, Target.maxDistance, out x3, out x4);
            Handles.DrawDottedLine(center, x1, spaceSize);
            Handles.DrawDottedLine(center, x2, spaceSize);

            Handles.DrawLine(x1, x3);
            Handles.DrawLine(x2, x4);

            Handles.Label(x3, "Snap Point[" + index.ToString() + "].x1");
            Handles.Label(x4, "Snap  Point[" + index.ToString() + "].x2");
            /* Vector3[] verts = new Vector3[] { x1, x2, x3, x4 };
             Handles.DrawSolidRectangleWithOutline(verts, new Color(0.5f, 0.5f, 0.5f, 0.1f), new Color(0, 0, 0, 1));*/
        }
        #endregion
        #endregion
        #endregion
        #region Inspector
        public override void OnInspectorGUI()
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                base.OnInspectorGUI();
                if (check.changed)
                {

                }
            }
        }
        #endregion
    }
}