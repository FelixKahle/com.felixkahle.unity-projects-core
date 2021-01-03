// Copyright 2020 Felix Kahle. All rights reserved.

using System.Runtime.CompilerServices;
using Unity.Entities;
using UnityEngine;

namespace FelixKahle.UnityProjectsCore
{
    /// <summary>
    /// Wrapper around a singleton component (ECS).
    /// </summary>
    /// <typeparam name="T">The component type.</typeparam>
    public class SingletonComponent<T> where T : struct, IComponentData
    {
        /// <summary>
        /// The EntityManager to use.
        /// </summary>
        private EntityManager entityManager;

        /// <summary>
        /// The EntityQuery.
        /// </summary>
        private EntityQuery entityQuery;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="manager">The EntityManager to use.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SingletonComponent(EntityManager manager)
        {
            entityManager = manager;
            entityQuery = entityManager.CreateEntityQuery(typeof(T));

            int entityCount = entityQuery.CalculateEntityCount();
            if (entityCount <= 0)
            {
                CreateActualSingletonComponent();
                return;
            }
            else if (entityCount > 1)
            {
                Debug.LogError("There are to many entities for a singleton component from type: " + typeof(T));
            }
        }

        /// <summary>
        /// Sets the actual singleton.
        /// </summary>
        /// <param name="singleton">The new singleton.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetActualSingleton(T singleton)
        {
            entityQuery.SetSingleton<T>(singleton);
        }

        /// <summary>
        /// Returns the actual singleton.
        /// </summary>
        /// <returns>The actual singleton.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetActualSingletonComponent()
        {
            return entityQuery.GetSingleton<T>();
        }

        /// <summary>
        /// Deletes the actual singleton.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DeleteActualSingleton()
        {
            Entity singletonEntity = entityQuery.GetSingletonEntity();
            entityManager.DestroyEntity(singletonEntity);
        }

        /// <summary>
        /// Creates the actual singleton component entity.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CreateActualSingletonComponent()
        {
            entityManager.CreateEntity(typeof(T));
        }
    }
}
