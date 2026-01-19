using UnityEditor;
using UnityEngine;
using MyDice.Board;

namespace MyDice.Editors
{
    public enum ElementNodeEditorMode
    {
        Point, PlayerHome
    }
    [CustomEditor(typeof(BoardGameManager))]
    public class ElementNodeEditor : Editor
    {
        #region variable
        protected ElementNodeCreator Target;
        protected bool needsRepaint;

        protected ElementNodeEditorMode editorMode;
        private SelectionInfo pointSelectionInfo;
        private SelectionInfo playerHomeSelectionInfo;
        private MousePositionHandler mousePositions;
        private ElementNodesManager elementNodesManager;

        #endregion
        private void OnEnable()
        {
            Target = (ElementNodeCreator)target;
            pointSelectionInfo = new SelectionInfo();
            playerHomeSelectionInfo = new SelectionInfo();
            mousePositions = new MousePositionHandler(2);
            editorMode = ElementNodeEditorMode.Point;
            elementNodesManager = new ElementNodesManager(ref Target.nodes);
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
                handleInput(e);
                if (needsRepaint)
                {
                    HandleUtility.Repaint();
                }
            }
        }

        #region functions
        private void updateMouseOverSelection(Vector3 mousePosition)
        {
            int mouseOverPointIndex = -1;
            for (int i = 0; i < Target.getPointsCount(); i++)
            {
                if (Vector3.Distance(mousePosition, Target.getPoint(i)) < Target.handlePointRadius)
                {
                    mouseOverPointIndex = i;
                    break;
                }
            }
            if (mouseOverPointIndex != pointSelectionInfo.pointIndex)
            {
                pointSelectionInfo.pointIndex = mouseOverPointIndex;
                pointSelectionInfo.mouseIsOverPoint = (mouseOverPointIndex != -1);
                needsRepaint = true;
            }
            if (pointSelectionInfo.mouseIsOverPoint)
            {
                pointSelectionInfo.mouseIsOverLine = false;
                pointSelectionInfo.lineIndexConnectionValue = pointSelectionInfo.lineIndex = -1;
            }
            else if (Event.current.modifiers == EventModifiers.Control || Event.current.modifiers == EventModifiers.Alt)
            {
                int mouseOverLineIndex = -1;
                int mouseOverLineIndexConnection = -1;
                float distanceFromMouseToLine;
                float closestDistance = Target.handlePointRadius;
                ElementNode node;
                for (int i = 0; i < Target.getPointsCount(); i++)
                {
                    if ((node = elementNodesManager.getNode(i)) == null) continue;

                    for (int k = 0; k < node.getConnectionsSize(); k++)
                    {
                        int j = node.connections[k];
                        distanceFromMouseToLine = HandleUtility.DistancePointToLineSegment(
                            mousePosition.ToXZ(),
                            Target.getPoint(i).ToXZ(),
                            Target.getPoint(j).ToXZ());
                        if (distanceFromMouseToLine < closestDistance)
                        {
                            mouseOverLineIndex = i;
                            mouseOverLineIndexConnection = j;
                            closestDistance = distanceFromMouseToLine;
                        }
                    }
                }
                if (mouseOverLineIndex != pointSelectionInfo.lineIndex)
                {
                    pointSelectionInfo.lineIndex = mouseOverLineIndex;
                    pointSelectionInfo.lineIndexConnectionValue = mouseOverLineIndexConnection;
                    pointSelectionInfo.mouseIsOverLine = mouseOverLineIndex != -1;
                    needsRepaint = true;
                }
            }
        }
        private void updateMouseOverSelection_playerHome(Vector3 mousePosition)
        {
            int mouseOverPointIndex = -1;
            float closestDistance = Target.handlePlayerHomesRadius;
            for (int i = 0; i < Target.playerHomes.Count; i++)
            {
                float dist = Vector3.Distance(mousePosition, Target.playerHomes[i].center);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    mouseOverPointIndex = i;
                }
            }
            if (mouseOverPointIndex != playerHomeSelectionInfo.pointIndex)
            {
                playerHomeSelectionInfo.pointIndex = mouseOverPointIndex;
                playerHomeSelectionInfo.mouseIsOverPoint = (mouseOverPointIndex != -1);
                needsRepaint = true;
            }
            if (playerHomeSelectionInfo.mouseIsOverPoint)
            {
                playerHomeSelectionInfo.mouseIsOverLine = false;
                playerHomeSelectionInfo.lineIndexConnectionValue = playerHomeSelectionInfo.lineIndex = -1;
            }
        }
        #region Event Handling
        private void handleInput(Event e)
        {
            mousePositions.AddPosition(mousePositions.getMousePosition());
            if (e.isMouse)
            {
                //control
                if (e.modifiers == EventModifiers.Control)
                {
                    if (e.type == EventType.MouseDown && e.button == 0)
                    {
                        cntr_leftMouseDown(mousePositions.getFreshPosition());
                    }
                    else if (e.type == EventType.MouseUp && e.button == 0)
                    {
                        cntr_leftMouseUp(mousePositions.getFreshPosition());
                    }
                    else if (e.type == EventType.MouseDrag && e.button == 0)
                    {
                        cntr_leftMouseDrag(mousePositions.getFreshPosition());
                    }
                    else if (e.type == EventType.MouseDown && e.button == 1)
                    {
                        cntr_rightMouseDown(mousePositions.getFreshPosition());
                    }
                    else
                    {
                        cntr_mouseOver();
                    }
                }
                //alt
                else if (e.modifiers == EventModifiers.Alt)
                {
                    if (e.type == EventType.MouseDown && e.button == 0)
                    {
                        alt_leftMouseDown(mousePositions.getFreshPosition());
                    }
                    else if (e.type == EventType.MouseDown && e.button == 1)
                    {
                        alt_rightMouseDown(mousePositions.getFreshPosition());
                    }
                    else
                    {
                        cntr_mouseOver();
                    }
                }
                // shift
                else if (e.modifiers == EventModifiers.Shift)
                {
                    if (e.type == EventType.MouseDown && e.button == 0)
                    {
                        shift_leftMouseDown(mousePositions.getFreshPosition());
                    }

                }
                else
                {
                    if (pointSelectionInfo.followerLineFromPointIndex != -1)
                    {
                        pointSelectionInfo.followerLineFromPointIndex = -1;
                        needsRepaint = true;
                    }
                }
            }

            if (!pointSelectionInfo.pointIsSelected)
            {
                updateMouseOverSelection(mousePositions.getFreshPosition());
            }
            switch (editorMode)
            {
                case ElementNodeEditorMode.PlayerHome:
                    if (!playerHomeSelectionInfo.pointIsSelected)
                    {
                        updateMouseOverSelection_playerHome(mousePositions.getFreshPosition());
                    }
                    break;
            }

        }
        #region control
        #region cntr_leftMouseDown
        private void cntr_leftMouseDown(Vector3 mousePosition)
        {
            switch (editorMode)
            {
                case ElementNodeEditorMode.Point:
                    cntr_leftMouseDown_Point(mousePosition);
                    break;
                case ElementNodeEditorMode.PlayerHome:
                    cntr_leftMouseDown_PlayerHome(mousePosition);
                    break;
            }
            pointSelectionInfo.followerLineFromPointIndex = -1;
            needsRepaint = true;
        }
        private void cntr_leftMouseDown_Point(Vector3 mousePosition)
        {
            //insert node
            #region insert node
            if (!pointSelectionInfo.mouseIsOverPoint)
            {
                int newPointIndex = Target.getPointsCount();
                if (newPointIndex > 0)
                {
                    ElementNode node = elementNodesManager.getNode(newPointIndex - 1);
                    Undo.RecordObjects(new Object[] { node, Target }, "Add point");
                }
                else
                {
                    Undo.RecordObject(Target, "Add point");
                }

                Target.insertPoint(Target.getPointsCount(), mousePosition);
                if (pointSelectionInfo.mouseIsOverLine)
                {
                    elementNodesManager.nodesChangeConnections(pointSelectionInfo.lineIndex, newPointIndex, pointSelectionInfo.lineIndexConnectionValue, ref Target.nodes);
                }
                else
                {
                    int followerIndex = pointSelectionInfo.followerLineFromPointIndex == -1 ? newPointIndex - 1 : pointSelectionInfo.followerLineFromPointIndex;
                    Target.nodeConnection(followerIndex, newPointIndex);
                }
                pointSelectionInfo.followerLineFromPointIndex = -1;
                pointSelectionInfo.pointIndex = newPointIndex;
            }
            pointSelectionInfo.pointIsSelected = true;
            pointSelectionInfo.positionAtStartOfDrag = mousePosition;
            #endregion
        }
        private void cntr_leftMouseDown_PlayerHome(Vector3 mousePosition)
        {
            if (!playerHomeSelectionInfo.mouseIsOverPoint)
            {
                int pointIndex = pointSelectionInfo.followerLineFromPointIndex == -1 ? 0 : pointSelectionInfo.followerLineFromPointIndex;
                Target.addPlayerHome(mousePosition, pointIndex);
                playerHomeSelectionInfo.pointIndex = Target.playerHomes.Count - 1;
            }
            playerHomeSelectionInfo.pointIsSelected = true;
            playerHomeSelectionInfo.positionAtStartOfDrag = mousePosition;
        }
        #endregion
        #region cntr_leftMouseUp
        private void cntr_leftMouseUp(Vector3 mousePosition)
        {
            switch (editorMode)
            {
                case ElementNodeEditorMode.Point:
                    cntr_leftMouseUp_Point(mousePosition);
                    break;
                case ElementNodeEditorMode.PlayerHome:
                    cntr_leftMouseUp_PlayerHome(mousePosition);
                    break;
            }
        }
        private void cntr_leftMouseUp_Point(Vector3 mousePosition)
        {
            if (pointSelectionInfo.pointIsSelected)
            {
                //Target.createNode(selectionInfo.pointIndex, selectionInfo.positionAtStartOfDrag);
                {
                    ElementNode node = elementNodesManager.getNode(pointSelectionInfo.pointIndex);
                    if (node != null && node.prefab != null)
                    {
                        Undo.RecordObject(node.prefab.transform, "Move transform");
                    }
                }
                Undo.RecordObject(Target, "Move point");
                Target.setPoint(pointSelectionInfo.pointIndex, mousePosition);
                pointSelectionInfo.pointIsSelected = false;
                pointSelectionInfo.pointIndex = -1;
                needsRepaint = true;
            }
        }
        private void cntr_leftMouseUp_PlayerHome(Vector3 mousePosition)
        {
            if (playerHomeSelectionInfo.pointIsSelected)
            {
                //Target.setPlayerHome(playerHomeSelectionInfo.pointIndex, Target.getPlayerHome(playerHomeSelectionInfo.pointIndex).center);
                Undo.RegisterCompleteObjectUndo(Target, "Move transform");
                Target.setPlayerHome(playerHomeSelectionInfo.pointIndex, mousePosition);
                playerHomeSelectionInfo.pointIsSelected = false;
                playerHomeSelectionInfo.pointIndex = -1;
                needsRepaint = true;
            }
        }
        #endregion
        #region cntr_rightMouseDown
        private void cntr_rightMouseDown(Vector3 mousePosition)
        {
            switch (editorMode)
            {
                case ElementNodeEditorMode.Point:
                    cntr_rightMouseDown_Point(mousePosition);
                    break;
                case ElementNodeEditorMode.PlayerHome:
                    cntr_rightMouseDown_PlayerHome(mousePosition);
                    break;
            }
        }
        private void cntr_rightMouseDown_Point(Vector3 mousePosition)
        {
            if (pointSelectionInfo.mouseIsOverPoint)
            {
                ElementOptionNodeEditor.Open(ref Target, pointSelectionInfo.pointIndex);
            }
        }
        private void cntr_rightMouseDown_PlayerHome(Vector3 mousePosition)
        {
            if (playerHomeSelectionInfo.mouseIsOverPoint)
            {
                PlayerHomeEditorWindow.Open(playerHomeSelectionInfo.pointIndex, ref Target);
            }
        }
        #endregion
        private void cntr_mouseOver()
        {
            if (pointSelectionInfo.mouseIsOverPoint && pointSelectionInfo.followerLineFromPointIndex == -1)
            {
                pointSelectionInfo.followerLineFromPointIndex = pointSelectionInfo.pointIndex;
            }
            needsRepaint = true;
        }
        #region cntr_leftMouseDrag
        private void cntr_leftMouseDrag(Vector3 mousePosition)
        {
            switch (editorMode)
            {
                case ElementNodeEditorMode.Point:
                    cntr_leftMouseDrag_Point(mousePosition);
                    break;
                case ElementNodeEditorMode.PlayerHome:
                    cntr_leftMouseDrag_PlayerHome(mousePosition);
                    break;
            }
        }
        private void cntr_leftMouseDrag_Point(Vector3 mousePosition)
        {
            if (pointSelectionInfo.pointIsSelected)
            {
                Target.setPoint(pointSelectionInfo.pointIndex, mousePosition);
                needsRepaint = true;
            }
        }
        private void cntr_leftMouseDrag_PlayerHome(Vector3 mousePosition)
        {
            if (playerHomeSelectionInfo.pointIsSelected)
            {
                Target.setPlayerHome(playerHomeSelectionInfo.pointIndex, mousePosition);
                needsRepaint = true;
            }
        }
        #endregion
        #endregion
        #region Alt
        #region alt_leftMouseDown
        private void alt_leftMouseDown(Vector3 mousePosition)
        {
            switch (editorMode)
            {
                case ElementNodeEditorMode.Point:
                    alt_leftMouseDown_Point(mousePosition);
                    break;
            }
        }
        private void alt_leftMouseDown_Point(Vector3 mousePosition)
        {
            //add connection
            #region add connection
            if (pointSelectionInfo.followerLineFromPointIndex != -1
        && pointSelectionInfo.pointIndex != -1
        && pointSelectionInfo.mouseIsOverPoint
        && pointSelectionInfo.pointIndex != pointSelectionInfo.followerLineFromPointIndex)
            {
                ElementNode node = elementNodesManager.getNode(pointSelectionInfo.followerLineFromPointIndex);
                if (node != null)
                {
                    Undo.RecordObject(node, "Add connection");
                }
                Target.nodeConnection(pointSelectionInfo.followerLineFromPointIndex, pointSelectionInfo.pointIndex);
                pointSelectionInfo.followerLineFromPointIndex = -1;
                needsRepaint = true;
            }
            #endregion
        }
        #endregion
        #region alt_rightMouseDown
        private void alt_rightMouseDown(Vector3 mousePosition)
        {
            switch (editorMode)
            {
                case ElementNodeEditorMode.Point:
                    alt_rightMouseDown_Point(mousePosition);
                    break;
            }
        }
        private void alt_rightMouseDown_Point(Vector3 mousePosition)
        {
            if (pointSelectionInfo.mouseIsOverLine)
            {
                elementNodesManager.nodesRemoveConnection(pointSelectionInfo.lineIndex, pointSelectionInfo.lineIndexConnectionValue);
            }
        }
        #endregion
        #endregion
        #region shift
        private void shift_leftMouseDown(Vector3 mousePosition)
        {
            Undo.RecordObject(Target, "Change center");
            Target.SetNewCenter(mousePosition);
            needsRepaint = true;
        }
        #endregion
        #endregion
        #region draw
        private void draw()
        {
            for (int i = 0; i < Target.getPointsCount(); i++)
            {
                ElementNode node;
                if ((node = elementNodesManager.getNode(i)) == null) continue;
                foreach (int j in node.connections)
                {
                    //draw line
                    {
                        if (pointSelectionInfo.lineIndex == i && pointSelectionInfo.lineIndexConnectionValue == j)
                        {
                            Handles.color = Color.red;
                            Handles.DrawLine(Target.getPoint(i), Target.getPoint(j));
                        }
                        else
                        {
                            Handles.color = Color.black;
                            Handles.DrawDottedLine(Target.getPoint(i), Target.getPoint(j), 4);
                        }
                        //draw arrow
                        {

                            Vector3 targetLookRotation = Target.getPoint(j) - Target.getPoint(i);
                            if (targetLookRotation != Vector3.zero)
                            {
                                Quaternion q = Target.getPointRotation(i, j);
                                Vector3 v = Target.getPoint(j) - Target.getPoint(i);

                                Vector3 outgoing = Target.handlePointRadius * v.normalized + Target.getPoint(i);
                                Handles.color = Color.green;
                                Handles.ArrowHandleCap(0,
                                    outgoing,
                                    q,
                                    Target.handleArrowSize,
                                    EventType.Repaint);

                                Handles.color = Color.red;

                                Vector3 incomming = (v.magnitude - (Target.handleArrowSize + Target.handlePointRadius)) * v.normalized + Target.getPoint(i);
                                Handles.ArrowHandleCap(0,
                                    incomming,
                                    q,
                                    Target.handleArrowSize,
                                    EventType.Repaint);
                            }
                        }
                    }
                }
                //draw redirect line
                if (Target.showRedirectLines)
                {
                    Handles.color = Color.yellow;
                    if ((node = elementNodesManager.getNode(i)) != null && node.redirectIndex > -1)
                    {
                        Vector3 redirectTarget = Target.getPoint(node.redirectIndex);
                        Handles.DrawDottedLine(Target.getPoint(i), redirectTarget, 2);

                        Vector3 v = Extensions.ToCenter(new Vector3[] { Target.getPoint(i), redirectTarget });
                        if (v != Vector3.zero)
                        {
                            Handles.Label(v, "r<" + i + "," + node.redirectIndex + ">");
                        }
                        Handles.ArrowHandleCap(0, Target.getPoint(i), Target.getPointRotation(i, node.redirectIndex), Target.handleArrowSize, EventType.Repaint);
                    }
                }
                //draw point
                {
                    if (pointSelectionInfo.pointIndex == i)
                        Handles.color = (pointSelectionInfo.pointIsSelected) ? Color.black : Color.red;
                    else
                    {
                        switch (elementNodesManager.getNode(i).getElementNodeType())
                        {
                            case ElementNodeType.RedirectPoint:
                                Handles.color = Color.yellow;
                                break;
                            case ElementNodeType.ResetPoint:
                                Handles.color = Color.blue;
                                break;
                            default:
                                Handles.color = Color.white;
                                break;
                        }
                    }
                    Handles.DrawSolidDisc(Target.getPoint(i), Vector3.up, Target.handlePointRadius);
                }
                //text index
                {
                    Handles.Label(Target.getPoint(i), i.ToString());
                }
                if (Event.current.modifiers == EventModifiers.Shift)
                {
                    Handles.color = Color.green;
                    Handles.DrawSolidDisc(Target.points.ToArray().ToCenter(), Vector3.up, Target.handlePointRadius);
                }
            }
            //follower line
            if (pointSelectionInfo.followerLineFromPointIndex != -1)
            {
                switch (editorMode)
                {
                    case ElementNodeEditorMode.Point:
                        Handles.color = Color.green;
                        break;
                    case ElementNodeEditorMode.PlayerHome:
                        Handles.color = Color.black;
                        break;
                }
                Handles.DrawLine(Target.getPoint(pointSelectionInfo.followerLineFromPointIndex), mousePositions.getFreshPosition());
            }
            draw_playerHomes();
            Target.garbageCollector();
            needsRepaint = false;
        }
        private void draw_playerHomes()
        {
            for (int i = 0; i < Target.playerHomes.Count; i++)
            {
                var node = Target.playerHomes[i];
                if (playerHomeSelectionInfo.pointIndex == i)
                {
                    Handles.color = Color.red;
                }
                else
                    Handles.color = Color.black;
                //draw base
                Handles.CircleHandleCap(0, node.center, Quaternion.LookRotation(Vector3.up), Target.handlePlayerHomesRadius, EventType.Repaint);
                Handles.Label(node.center, node.getName());
                //draw start
                Handles.DrawDottedLine(Target.getPoint(node.startIndex), node.center, 4);
                Handles.Label(new Vector3[] { Target.getPoint(node.startIndex), node.center }.ToCenter(), node.getName() + " -> S");
                //draw target
                if (node.targetIndex > -1)
                {
                    Handles.Label(new Vector3[] { Target.getPoint(node.targetIndex), node.center }.ToCenter(), node.getName() + " -> T");
                    Handles.color = Color.cyan;
                    Handles.DrawDottedLine(Target.getPoint(node.targetIndex), node.center, 4);
                }
                //draw playerPositions
                Handles.color = Color.white;
                foreach (var p in node.players)
                {
                    Handles.DrawSolidDisc(p.transform.position, Vector3.up, Target.handlePointRadius);
                }
            }
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
            Target.checkPointsForRemovedNodes();
            {
                GUILayout.BeginVertical("box");
                GUILayout.BeginHorizontal();

                if (GUILayout.Button("Scale -"))
                {
                    Target.ChangeNodesScale(.95f);
                }
                if (GUILayout.Button("Scale +"))
                {
                    Target.ChangeNodesScale(1.05f);
                }
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }
            {
                GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Editor mode: ");
                editorMode = (ElementNodeEditorMode)EditorGUILayout.Popup((int)editorMode, System.Enum.GetNames(typeof(ElementNodeEditorMode)));
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }
            if (editorMode == ElementNodeEditorMode.Point)
            {
                OnInspectorGUI_Points();
            }
            else if (editorMode == ElementNodeEditorMode.PlayerHome)
            {
                OnInspectorGUI_Homes();
            }
        }
        private void OnInspectorGUI_Points()
        {
            //box
            {
                GUILayout.BeginVertical("box");

                //count
                {
                    GUILayout.BeginVertical();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Point(s) count: " + Target.getPointsCount().ToString());
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                }
                //shapes
                {
                    GUILayout.BeginVertical();
                    {
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("Square"))
                        {
                            SquareShape.Open(ref Target);
                        }
                        if (GUILayout.Button("Rectangle"))
                        {
                            RectangleShape.Open(ref Target);
                        }
                        GUILayout.EndHorizontal();
                    }
                    {
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("Circle"))
                        {
                            CircleShape.Open(ref Target);
                        }
                        if (GUILayout.Button("Ellipse"))
                        {
                            EllipseShape.Open(ref Target);
                        }
                        GUILayout.EndHorizontal();
                    }
                    {
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("Diamond"))
                        {
                            DiamondShape.Open(ref Target);
                        }
                        if (GUILayout.Button("Triangle"))
                        {
                            TriangleShape.Open(ref Target);
                        }
                        GUILayout.EndHorizontal();
                    }
                    {
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("N-Gon"))
                        {
                            PolygonShape.Open(ref Target);
                        }
                        if (GUILayout.Button("Line"))
                        {
                            LineShape.Open(ref Target);
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndVertical();
            }
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Reset point(s)"))
                {
                    Target.PointsClear();
                }
                GUILayout.EndHorizontal();
            }

        }
        private void OnInspectorGUI_Homes()
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Player home(s) count: " + Target.playerHomes.Count);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Reset Player home(s)"))
            {
                Target.playerHomeReset();
                needsRepaint = true;
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }
        #endregion
    }
}