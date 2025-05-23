// Dynamic Radial Masks <https://u3d.as/1w0H>
// Copyright (c) Amazing Assets <https://amazingassets.world>
 
using UnityEngine;
using UnityEditor;


namespace AmazingAssets.DynamicRadialMasks.Editor
{
    [CustomEditor(typeof(DRMController))]
    [CanEditMultipleObjects]
    public class DRMControllerEditor : UnityEditor.Editor
    {
        SerializedProperty materials;
        SerializedProperty updateMethod;
        SerializedProperty drawInEditor;

        void OnEnable()
        {
            materials = serializedObject.FindProperty("materials");
            updateMethod = serializedObject.FindProperty("updateMethod");
            drawInEditor = serializedObject.FindProperty("drawInEditor");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            if (serializedObject.FindProperty("scope").enumValueIndex == (int)DynamicRadialMasks.Enum.MaskScope.Local)
            {
                materials.isExpanded = EditorGUILayout.Foldout(materials.isExpanded, "Materials", true);
                if (materials.isExpanded)
                {
                    using (new EditorGUIHelper.EditorGUIIndentLevel(1))
                    {
                        materials.arraySize = EditorGUILayout.IntField("Size", materials.arraySize);

                        for (int i = 0; i < materials.arraySize; ++i)
                        {
                            SerializedProperty transformProp = materials.GetArrayElementAtIndex(i);
                            EditorGUILayout.PropertyField(transformProp, new GUIContent("Element " + i));
                        }
                    }
                }
            }


            EditorGUILayout.PropertyField(updateMethod);
            EditorGUILayout.PropertyField(drawInEditor);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
