// Copyright 2021 Felix Kahle. All rights reserved.

using UnityEngine;
using UnityEditor;

namespace FelixKahle.UnityProjectsCore.Editor
{
    /// <summary>
    /// Custom PropertyDrawer for the WeakAssetReference.
    /// </summary>
    [CustomPropertyDrawer(typeof(WeakAssetReference))]
    public class WeakAssetReferenceDrawer : UnityEditor.PropertyDrawer
    {
        public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
        {
            // Figure out what asset types we allow. Default to all.
            System.Type assetType = typeof(Object);
            if (fieldInfo != null)
            {
                AssetTypeAttribute assetTypeProperty = System.Attribute.GetCustomAttribute(fieldInfo, typeof(AssetTypeAttribute)) as AssetTypeAttribute;
                assetType = assetTypeProperty != null ? assetTypeProperty.AssetType : typeof(Object);
            }

            SerializedProperty value0 = prop.FindPropertyRelative("value0");
            SerializedProperty value1 = prop.FindPropertyRelative("value1");
            SerializedProperty value2 = prop.FindPropertyRelative("value2");
            SerializedProperty value3 = prop.FindPropertyRelative("value3");

            WeakAssetReference reference = new WeakAssetReference(value0.intValue, value1.intValue, value2.intValue, value3.intValue);

            string guidStr = "";
            Object obj = null;
            if (reference.IsSet())
            {
                guidStr = reference.ToGuidStr();
                string path = AssetDatabase.GUIDToAssetPath(guidStr);

                if (assetType == null)
                {
                    assetType = AssetDatabase.GetMainAssetTypeAtPath(path);
                }

                obj = AssetDatabase.LoadAssetAtPath(path, assetType);
            }

            pos = EditorGUI.PrefixLabel(pos, GUIUtility.GetControlID(FocusType.Passive), new GUIContent(label.text + "(" + guidStr + ")"));
            Object newObj = EditorGUI.ObjectField(pos, obj, assetType, false);

            if (newObj != obj)
            {
                WeakAssetReference newRef = new WeakAssetReference();
                if (newObj != null)
                {
                    var path = AssetDatabase.GetAssetPath(newObj);
                    newRef = new WeakAssetReference(AssetDatabase.AssetPathToGUID(path));
                }

                value0.intValue = newRef.value0;
                value1.intValue = newRef.value1;
                value2.intValue = newRef.value2;
                value3.intValue = newRef.value3;
            }
        }
    }

    /// <summary>
    /// CCustom PropertyDrawer for the WeakBase.
    /// </summary>
    [CustomPropertyDrawer(typeof(WeakBase), true)]
    public class WeakBaseDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
        {
            // Figure out what asset types we allow. Default to all.
            System.Type assetType = typeof(GameObject);
            System.Type baseType = fieldInfo.FieldType.BaseType;
            if (baseType != null && baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(Weak<>))
            {
                assetType = baseType.GetGenericArguments()[0];
            }

            SerializedProperty guid = prop.FindPropertyRelative("guid");

            string path = AssetDatabase.GUIDToAssetPath(guid.stringValue);

            Object obj = AssetDatabase.LoadAssetAtPath(path, assetType);

            pos = EditorGUI.PrefixLabel(pos, GUIUtility.GetControlID(FocusType.Passive), label);
            Object newObj = EditorGUI.ObjectField(pos, obj, assetType, false);
            if (newObj != obj)
            {
                if (newObj != null)
                {
                    path = AssetDatabase.GetAssetPath(newObj);
                    guid.stringValue = AssetDatabase.AssetPathToGUID(path);
                }
                else
                {
                    guid.stringValue = "";
                }
                guid.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
