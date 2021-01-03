// Copyright 2020 Felix Kahle. All rights reserved.

#if UNITY_EDITOR

using UnityEngine;

namespace FelixKahle.UnityProjectsCore.Editor
{
    /// <summary>
    /// Assets or prefabs that should be included in the PrefabAssetRegistry can be marked with this component.
    /// If "Only gather PrefabAssetRegistry Assets" is ticked, only prefabs with this component will be picked up.
    /// </summary>
    public class PrefabAssetRegistryAsset : MonoBehaviour, IPrefabAsset
    {
    }
}

#endif
