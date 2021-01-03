// Copyright 2020 Felix Kahle. All rights reserved.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace FelixKahle.UnityProjectsCore.Editor
{
    /// <summary>
    /// Editor for the PrefabAssetRegistryAuthoring component.
    /// </summary>
    [CustomEditor(typeof(PrefabAssetRegistryAuthoring))]
    public class PrefabAssetRegistryAuthoringEditor : UnityEditor.Editor
    {
        /// <summary>
        /// List in which all gathered assets will be displayed.
        /// </summary>
        private ReorderableList list;

        /// <summary>
        /// The GatherOnlyPrefabAssetRegistryAssets property.
        /// </summary>
        private SerializedProperty gatherOnlyPrefabAssetRegistryAssetsProperty;

        public void OnEnable()
        {
            gatherOnlyPrefabAssetRegistryAssetsProperty = serializedObject.FindProperty("GatherOnlyPrefabAssetRegistryAssets");

            list = new ReorderableList(serializedObject, serializedObject.FindProperty("Assets"), true, true, true, true);
            list.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Gathered Assets");
            };
            list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = list.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
            };
        }

        public override void OnInspectorGUI()
        {
            PrefabAssetRegistryAuthoring prefabAssetRef = target as PrefabAssetRegistryAuthoring;
            serializedObject.Update();

            bool tmp = EditorGUILayout.Toggle("Marked only", gatherOnlyPrefabAssetRegistryAssetsProperty.boolValue);
            if (tmp != gatherOnlyPrefabAssetRegistryAssetsProperty.boolValue)
            {
                gatherOnlyPrefabAssetRegistryAssetsProperty.boolValue = tmp;
            }

            GUILayout.Space(2);

            if (GUILayout.Button("Gather Prefabs"))
            {
                var refCollection = new WeakAssetReferenceCollection();
                GatherAssetReferences(ref refCollection, BuildType.Client, gatherOnlyPrefabAssetRegistryAssetsProperty.boolValue);
                GatherAssetReferences(ref refCollection, BuildType.Server, gatherOnlyPrefabAssetRegistryAssetsProperty.boolValue);

                prefabAssetRef.Assets.Clear();

                foreach (var reference in refCollection.References)
                {
                    if (!reference.IsSet())
                    {
                        continue;
                    }

                    var path = AssetDatabase.GUIDToAssetPath(reference.ToGuidStr());
                    var asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                    if (asset == null)
                    {
                        Debug.LogWarning("Loading asset:" + reference.ToGuidStr() + " failed. Not a gameobject ?");
                        continue;
                    }

                    prefabAssetRef.Assets.Add(asset);
                }

                EditorUtility.SetDirty(target);
            }

            GUILayout.Space(2);

            list.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Gather AssetReferences.
        /// </summary>
        /// <param name="refCollection">WeakAssetReferenceCollection.</param>
        /// <param name="buildType">The build type.</param>
        /// <param name="onlyGatherPrefabAssetRegistryAssets">True if only prefabs should be gathered that have a PrefabAssetRegistryAsset component attached.</param>
        private void GatherAssetReferences(ref WeakAssetReferenceCollection refCollection, BuildType buildType, bool onlyGatherPrefabAssetRegistryAssets)
        {
            string[] prefabGuids = AssetDatabase.FindAssets("t:" + typeof(GameObject).Name);
            foreach (var guid in prefabGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                if (onlyGatherPrefabAssetRegistryAssets == true)
                {
                    if (go.GetComponent<PrefabAssetRegistryAsset>() != null)
                    {
                        refCollection.AddReference(new WeakAssetReference(guid));
                    }
                }
                else
                {
                    refCollection.AddReference(new WeakAssetReference(guid));
                }

            }
            refCollection.ResolveDerivedDependencies(buildType);
        }
    }
}
