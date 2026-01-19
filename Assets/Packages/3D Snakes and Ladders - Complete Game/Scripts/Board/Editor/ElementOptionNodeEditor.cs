using UnityEditor;
using UnityEngine;

namespace MyDice.Board
{
    public class ElementOptionNodeEditor : EditorWindow
    {
        public static int selectedIndex;
        public static ElementNodeCreator target;
        public static ElementNodesManager elementNodesManager;
        protected static int _redirectIndex;
        public static void Open(ref ElementNodeCreator enEditor, int selectedNodeIndex)
        {
            ElementOptionNodeEditor window = GetWindow<ElementOptionNodeEditor>("Element option " + selectedNodeIndex.ToString());
            target = enEditor;
            selectedIndex = selectedNodeIndex;
            elementNodesManager = new ElementNodesManager(ref target.nodes);
            _redirectIndex = elementNodesManager.getNode(selectedIndex).redirectIndex;
        }

        private void OnGUI()
        {
            if (target == null) this.Close();
            if (elementNodesManager.getNode(selectedIndex) == null)
            {
                target.initNode(selectedIndex, target.points[selectedIndex]);
            }
            ElementNode node = elementNodesManager.getNode(selectedIndex);
            if (node == null)
            {
                node = target.nodes[selectedIndex].AddComponent<ElementNode>();
            }
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Index: " + selectedIndex.ToString() + " / " + target.getPointsCount().ToString());
            GUILayout.EndHorizontal();
            /*
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Set as start point"))
            {
                target.ChangeIndexAsStart(selectedIndex);
                this.Close();
            }
            GUILayout.EndHorizontal();
            */
            GUILayout.EndVertical();
            ///
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Node type: ");
                var obj = (ElementNodeType)EditorGUILayout.Popup((int)node.getElementNodeType(), System.Enum.GetNames(typeof(ElementNodeType)));
                if (obj != node.getElementNodeType())
                {
                    node.setElementNodeType(obj);
                    _redirectIndex = node.redirectIndex;
                    EditorUtility.SetDirty(target);
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            ///
            {
                if (node.getElementNodeType() == ElementNodeType.RedirectPoint)
                {
                    GUILayout.BeginVertical();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Redirect index: ");
                    _redirectIndex = EditorGUILayout.IntField(_redirectIndex);
                    if (GUILayout.Button("Set"))
                    {
                        if (_redirectIndex != node.redirectIndex && _redirectIndex != selectedIndex)
                        {
                            node.setRedirectIndex(_redirectIndex);
                            _redirectIndex = node.redirectIndex;
                            EditorUtility.SetDirty(target);
                        }
                    }
                    if (GUILayout.Button("Refresh"))
                    {
                        _redirectIndex = node.redirectIndex;
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.HelpBox("'-1' means nothing. redirect index has highest priority.", MessageType.Info);
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                }
            }
            ///
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            {
                var obj = EditorGUILayout.ObjectField("Prefab:", node.prefab, typeof(GameObject), true);
                if (obj == null || obj != node.prefab)
                {
                    ElementNode element;
                    if ((element = elementNodesManager.getNode(selectedIndex)) != null)
                    {
                        element.UpdatePrefab((GameObject)obj);
                        if (target.autoRotation)
                        {
                            //element.setRotation(target.getPointRotation(selectedIndex));
                        }
                    }
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            ///
            GUILayout.Space(10);
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Assign prefab for all node(s)"))
            {
                target.AssignPrefabForAllNodes(selectedIndex);
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            ///
            GUILayout.Space(10);
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Remove node"))
            {
                target.RemovePoint(selectedIndex);
                this.Close();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            ///
            GUILayout.Space(10);
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(" << "))
            {
                int index = (selectedIndex - 1);
                if (index < 0) index = target.getPointsCount() - 1;
                this.Close();
                ElementOptionNodeEditor.Open(ref target, index);
            }
            if (GUILayout.Button(" >> "))
            {
                this.Close();
                ElementOptionNodeEditor.Open(ref target, (selectedIndex + 1) % target.getPointsCount());
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.Space(2);
        }

    }
}