using UnityEditor;
using UnityEditor.UI;

namespace UIExtensions.Editor
{
    [CustomEditor(typeof(SkewedText))]
    public class SkewedTextInspector : TextEditor
    {
        private SkewedText _skewedText;
        private SerializedProperty _skewVector;

        protected override void OnEnable()
        {
            base.OnEnable();
            _skewedText = (SkewedText) target;
            _skewVector = serializedObject.FindProperty("SkewVector");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();
            EditorGUILayout.PropertyField(_skewVector);

            if (_skewVector.vector2Value != _skewedText.SkewVector)
            {
                Undo.RecordObject(_skewedText, "Changed Skew");
                _skewedText.SkewVector = _skewVector.vector2Value;
                _skewedText.OnRebuildRequested();
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}