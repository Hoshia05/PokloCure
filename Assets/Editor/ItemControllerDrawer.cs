#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(IItemController), true)]
public class ItemControllerDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Display a script field in the Inspector
        EditorGUI.ObjectField(position, property, GUIContent.none);

        EditorGUI.EndProperty();
    }
}
#endif