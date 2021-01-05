// Copyright 2020 Felix Kahle. All rights reserved.

using System;
using System.Runtime.CompilerServices;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace FelixKahle.UnityProjectsCore
{
    public static class EntityUtility
    {
        /// <summary>
        /// Returns the first entity which is found by searching for the given Component.
        /// </summary>
        /// <typeparam name="T">The Component to search for.</typeparam>
        /// <param name="entityManager">The EntityManager to use.</param>
        /// <returns>The first entity which is found by searching for the given Component.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Entity GetSingleEntity<T>(EntityManager entityManager) where T : struct, IComponentData
        {
            NativeArray<Entity> entityArray = GetEntitiesByComponent<T>(entityManager);
            Entity entity = entityArray.Length > 0 ? entityArray[0] : Entity.Null;
            entityArray.Dispose();
            return entity;
        }

        /// <summary>
        /// Returns all entities found by searching for the given Component.
        /// </summary>
        /// <typeparam name="T">The Component to search for.</typeparam>
        /// <param name="entityManager">The EntityManager to use.</param>
        /// <returns>All entities found by searching for the given Component.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeArray<Entity> GetEntitiesByComponent<T>(EntityManager entityManager) where T : struct, IComponentData
        {
            EntityQuery entityQuery = entityManager.CreateEntityQuery(typeof(T));
            NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.TempJob);
            entityQuery.Dispose();
            return entityArray;
        }
    }
}