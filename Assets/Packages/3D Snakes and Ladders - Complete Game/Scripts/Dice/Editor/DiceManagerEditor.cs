using UnityEngine;
using UnityEditor;
namespace MyDice.Editors
{
    [CustomEditor(typeof(DiceManager))]
    public class DiceManagerEditor : Editor
    {
        private DiceManager Target;
        private void OnValidate()
        {
            Target = (DiceManager)target;
        }
        #region Inspector
        public override void OnInspectorGUI()
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                base.OnInspectorGUI();
                if (check.changed)
                {

                }
                draw();
            }
        }
        private void draw()
        {
            if (Target == null) return;
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Information");
            EditorGUILayout.LabelField("State: " + Target.getDiceState().ToString());
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        #endregion
    }
}