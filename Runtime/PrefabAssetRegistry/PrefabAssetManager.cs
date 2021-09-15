// Copyright 2021 Felix Kahle. All rights reserved.

using Unity.Entities;
using UnityEngine;

namespace FelixKahle.UnityProjectsCore
{
    /// <summary>
    /// Manager for the creation of prefab/asset entities.
    /// </summary>
    public class PrefabAssetManager
    {
        /// <summary>
        /// Creates an entity based on a WeakAssetReference.
        /// </summary>
        /// <param name="entityManager">The EntityManager to use.</param>
        /// <param name="reference">The WeakAssetReference.</param>
        /// <returns>The instantiated Entity.</returns>
        public static Entity CreateEntity(EntityManager entityManager, WeakAssetReference reference)
        {
            Entity entityPrefab = PrefabAssetRegistry.FindEntityPrefab(entityManager, reference);
            if (entityPrefab == Entity.Null)
            {
                Debug.LogError("Failed to create prefab for asset:" + reference.ToGuidStr());
                return Entity.Null;
            }

            Entity entity = entityManager.Instantiate(entityPrefab);
            return entity;
        }

        /// <summary>
        /// Creates an entity based on a WeakAssetReference.
        /// </summary>
        /// <param name="entityManager">The EntityManager to use.</param>
        /// <param name="cmdBuffer">The EntityCommandBuffer to use.</param>
        /// <param name="reference">The WeakAssetReference.</param>
        /// <returns></returns>
        public static Entity CreateEntity(EntityManager entityManager, EntityCommandBuffer cmdBuffer, WeakAssetReference reference)
        {
            Entity entityPrefab = PrefabAssetRegistry.FindEntityPrefab(entityManager, reference);
            if (entityPrefab == Entity.Null)
            {
                Debug.LogError("Failed to create prefab for asset:" + reference.ToGuidStr());
                return Entity.Null;
            }
            Entity instance = cmdBuffer.Instantiate(entityPrefab);
            return instance;
        }

        /// <summary>
        /// Destroys an entity.
        /// </summary>
        /// <param name="commandBuffer">The EntityCommandBuffer to use.</param>
        /// <param name="entity">The Entity to destroy.</param>
        public static void DestroyEntity(EntityCommandBuffer commandBuffer, Entity entity)
        {
            commandBuffer.DestroyEntity(entity);
        }


        /// <summary>
        /// Destroys an entity.
        /// </summary>
        /// <param name="entityManager">The EntityManager to use.</param>
        /// <param name="entity">The Entity to destroy.</param>
        public static void DestroyEntity(EntityManager entityManager, Entity entity)
        {
            if (entityManager.HasComponent<Transform>(entity))
            {
                var transform = entityManager.GetComponentObject<Transform>(entity);
                Object.Destroy(transform.gameObject);

                // GameObjectEntity will take care of destorying entities
                if (transform.GetComponent<GameObjectEntity>() != null)
                {
                    return;
                }
            }

            if (entityManager.HasComponent<RectTransform>(entity))
            {
                var transform = entityManager.GetComponentObject<RectTransform>(entity);
                Object.Destroy(transform.gameObject);

                // GameObjectEntity will take care of destorying entities
                if (transform.GetComponent<GameObjectEntity>() != null)
                {
                    return;
                }
            }

            entityManager.DestroyEntity(entity);
        }

    }
}
