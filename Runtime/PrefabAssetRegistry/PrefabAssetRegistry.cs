// Copyright 2021 Felix Kahle. All rights reserved.

using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace FelixKahle.UnityProjectsCore
{
    public class PrefabAssetRegistry
    {
        /// <summary>
        /// Tag of the PrefabAssetRegistry.
        /// </summary>
        public struct Tag : IComponentData
        {
        }

        /// <summary>
        /// Entry in the PrefabAssetRegistry.
        /// </summary>
        public struct Entry : IBufferElementData
        {
            /// <summary>
            /// The WeakAssetReference to the prefab/asset.
            /// </summary>
            public WeakAssetReference Reference;

            /// <summary>
            /// The prefab entity of the prefab/asset.
            /// </summary>
            public Entity EntityPrefab;
        }

        /// <summary>
        /// Returns the registry entity of the PrefabAssetRegistry.
        /// </summary>
        /// <param name="entityManager">The EntityManager to use.</param>
        /// <returns>The registry entity of the PrefabAssetRegistry.</returns>
        private static Entity GetRegistryEntity(EntityManager entityManager)
        {
            EntityQuery query = entityManager.CreateEntityQuery(typeof(PrefabAssetRegistry.Entry));
            NativeArray<Entity> entityArray = query.ToEntityArray(Allocator.TempJob);
            if (entityArray.Length == 0)
            {
                Debug.LogError("Failed to find PrefabAssetRegistry. Have you included the PrefabAssetRegistry subscene ?");
            }
            if (entityArray.Length > 1)
            {
                Debug.LogWarning("Found " + entityArray.Length + " PrefabAssetRegistries. First one will be used");
            }

            Entity entity = entityArray.Length > 0 ? entityArray[0] : Entity.Null;
            entityArray.Dispose();
            query.Dispose();
            return entity;
        }

        /// <summary>
        /// Returns all entries of the registry.
        /// </summary>
        /// <param name="entityManager">The EntityManager to use.</param>
        /// <param name="allocator">The Allocator to use.</param>
        /// <returns>NativeArray filled with all entries of the registry.</returns>
        public static NativeArray<Entry> GetAllResources(EntityManager entityManager, Allocator allocator)
        {
            Entity entity = GetRegistryEntity(entityManager);
            DynamicBuffer<PrefabAssetRegistry.Entry> entries = entityManager.GetBuffer<PrefabAssetRegistry.Entry>(entity);
            return entries.ToNativeArray(allocator);
        }

        /// <summary>
        /// Finds a prefab entity based on a WeakAssetReference.
        /// </summary>
        /// <param name="entityManager">The EntityManager to use.</param>
        /// <param name="reference">The WeakAssetReference to search the entity prefab for.</param>
        /// <returns>The prefab entity (of the WeakAssetReference).</returns>
        public static Entity FindEntityPrefab(EntityManager entityManager, WeakAssetReference reference)
        {
            Entity entity = GetRegistryEntity(entityManager);
            if (entity == Entity.Null)
            {
                return Entity.Null;
            }


            DynamicBuffer<PrefabAssetRegistry.Entry> entries = entityManager.GetBuffer<PrefabAssetRegistry.Entry>(entity);
            for (int i = 0; i < entries.Length; i++)
            {
                if (entries[i].Reference == reference)
                {
                    return entries[i].EntityPrefab;
                }
            }

            return Entity.Null;
        }

        /// <summary>
        /// Returns true if the PrefabAssetRegistry has been loaded and false otherwise.
        /// </summary>
        /// <param name="entityManager">The EntityManager to use.</param>
        /// <returns>True if the PrefabAssetRegistry has been loaded and false otherwise.</returns>
        public static bool IsLoaded(EntityManager entityManager)
        {
            EntityQuery query = entityManager.CreateEntityQuery(typeof(PrefabAssetRegistry.Tag));
            bool ready = query.CalculateEntityCount() > 0;
            query.Dispose();
            return ready;
        }
    }
}