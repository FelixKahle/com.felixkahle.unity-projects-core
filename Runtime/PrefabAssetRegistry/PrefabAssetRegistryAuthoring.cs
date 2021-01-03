// Copyright 2020 Felix Kahle. All rights reserved.

#if UNITY_EDITOR

using System.Collections.Generic;
using Unity.Entities;
using UnityEditor;
using UnityEngine;

namespace FelixKahle.UnityProjectsCore
{
    public interface IPrefabAsset
    {
    }

    /// <summary>
    /// Authoring for the PrefabAssetRegistry.
    /// </summary>
    [RequireComponent(typeof(PrefabAssetRegistryTagAuthoring))]
    public class PrefabAssetRegistryAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
    {
        /// <summary>
        /// List of Assets in the PrefabAssetRegistry.
        /// </summary>
        public List<GameObject> Assets = new List<GameObject>();

        /// <summary>
        /// True if only prefabs should be gathered that have a PrefabAssetRegistryAsset component attached.
        /// </summary>
        public bool GatherOnlyPrefabAssetRegistryAssets = false;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            DynamicBuffer<PrefabAssetRegistry.Entry> buffer = dstManager.AddBuffer<PrefabAssetRegistry.Entry>(entity);

            for (int i = 0; i < Assets.Count; i++)
            {
                string assetPath = AssetDatabase.GetAssetPath(Assets[i]);
                string assetGUID = AssetDatabase.AssetPathToGUID(assetPath);
                WeakAssetReference reference = new WeakAssetReference(assetGUID);

                Entity prefabEntity = conversionSystem.GetPrimaryEntity(Assets[i]);

                buffer.Add(new PrefabAssetRegistry.Entry
                {
                    Reference = reference,
                    EntityPrefab = prefabEntity,
                });
            }

            dstManager.AddComponentData(entity, new PrefabAssetRegistry.Tag());
        }

        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            for (int i = 0; i < Assets.Count; i++)
            {
                referencedPrefabs.Add(Assets[i]);
            }
        }
    }
}

#endif