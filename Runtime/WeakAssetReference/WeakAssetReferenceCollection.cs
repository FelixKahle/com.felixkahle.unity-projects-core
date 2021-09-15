// Copyright 2021 Felix Kahle. All rights reserved.

#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FelixKahle.UnityProjectsCore
{
    /// <summary>
    /// WeakAssetReferenceCollection.
    /// </summary>
    public class WeakAssetReferenceCollection
    {
        /// <summary>
        /// List with all WeakAssetReferences.
        /// </summary>
        private List<WeakAssetReference> references = new List<WeakAssetReference>();

        /// <summary>
        /// Getter for all WeakAssetReferences.
        /// </summary>
        public List<WeakAssetReference> References
        {
            get { return references; }
        }

        /// <summary>
        /// Adds a WeakAssetReference.
        /// </summary>
        /// <param name="reference">The WeakAssetReference to add.</param>
        public void AddReference(WeakAssetReference reference)
        {
            if (!reference.IsSet())
            {
                return;
            }

            if (references.Contains(reference))
            {
                return;
            }

            string path = AssetDatabase.GUIDToAssetPath(reference.ToGuidStr());
            if (String.IsNullOrEmpty(path))
            {
                Debug.LogWarning("Asset does not exist:" + reference.ToGuidStr());
                return;
            }

            Debug.Log("Adding asset:" + reference.ToGuidStr() + " " + path);

            references.Add(reference);
        }

        /// <summary>
        /// Resolves derived dependencies.
        /// </summary>
        /// <param name="buildType">The build type.</param>
        public void ResolveDerivedDependencies(BuildType buildType)
        {
            Debug.Log("Resolving derived dependencies");

            int i = 0;
            while (i < references.Count)
            {
                EditorUtility.DisplayProgressBar("Resolve derived dependencies", "Weakassetreference " + i + "/" + references.Count, (float)i / references.Count);

                string path = AssetDatabase.GUIDToAssetPath(references[i].ToGuidStr());
                GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                if (go != null)
                {
                    AddDerivedAssets(go, buildType);
                }

                i++;
            }

            EditorUtility.ClearProgressBar();
        }

        /// <summary>
        /// Adds derived Assets.
        /// </summary>
        /// <param name="go">The GameObject.</param>
        /// <param name="buildType">The build type.</param>
        void AddDerivedAssets(GameObject go, BuildType buildType)
        {
            foreach (var component in go.GetComponentsInChildren<Component>())
            {
                IBundledAssetProvider provider = component as IBundledAssetProvider;
                if (provider != null)
                {
                    List<WeakAssetReference> refs = new List<WeakAssetReference>();
                    provider.AddBundledAssets(buildType, refs);
                    foreach (var reference in refs)
                    {
                        AddReference(reference);
                    }
                }
            }
        }
    }
}

#endif