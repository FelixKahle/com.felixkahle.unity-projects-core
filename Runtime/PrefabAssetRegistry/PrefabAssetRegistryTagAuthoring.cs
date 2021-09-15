// Copyright 2021 Felix Kahle. All rights reserved.

#if UNITY_EDITOR

using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace FelixKahle.UnityProjectsCore
{
    /// <summary>
    /// Authoring for the PrefabAssetRegistry.Tag.
    /// </summary>
    public class PrefabAssetRegistryTagAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData<PrefabAssetRegistry.Tag>(entity, new PrefabAssetRegistry.Tag());
        }
    }
}

#endif