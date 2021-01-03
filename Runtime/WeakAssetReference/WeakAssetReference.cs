// Copyright 2020 Felix Kahle. All rights reserved.

using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FelixKahle.UnityProjectsCore
{
    /// <summary>
    /// Interface for a BundledAssetProvider.
    /// </summary>
    public interface IBundledAssetProvider
    {
        /// <summary>
        /// Adds bundled assets.
        /// </summary>
        /// <param name="buildType">The build type.</param>
        /// <param name="assets">Assets to add.</param>
        void AddBundledAssets(BuildType buildType, List<WeakAssetReference> assets);
    }

    /// <summary>
    /// Use this attribute to limit the types allowed on a weak asset reference field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class AssetTypeAttribute : Attribute
    {
        /// <summary>
        /// The type of the asset.
        /// </summary>
        public Type AssetType;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="assetType">The type of the asset.</param>
        public AssetTypeAttribute(Type assetType)
        {
            AssetType = assetType;
        }
    }

    /// <summary>
    /// Weak asset reference that does not result in assets getting pulled in. Use has
    /// responsibility to find another way to actually get asset loaded
    /// </summary>
    [System.Serializable]
    public struct WeakAssetReference
    {
        /// <summary>
        /// Default WeakAssetReference instance.
        /// </summary>
        public static WeakAssetReference Default => new WeakAssetReference { };

        /// <summary>
        /// First value.
        /// The GUID of an asset gets converted to four integers.
        /// This is the first integer.
        /// </summary>
        public int value0;

        /// <summary>
        /// Second value.
        /// The GUID of an asset gets converted to four integers.
        /// This is the second integer.
        /// </summary>
        public int value1;

        /// <summary>
        /// Third value.
        /// The GUID of an asset gets converted to four integers.
        /// This is the third integer.
        /// </summary>
        public int value2;

        /// <summary>
        /// Fourth value.
        /// The GUID of an asset gets converted to four integers.
        /// This is the fourth integer.
        /// </summary>
        public int value3;


        /// <summary>
        /// Constructs a WeakAssetReference from a string which represents a GUID.
        /// </summary>
        /// <param name="guid">A string representing a GUID.</param>
        public WeakAssetReference(string guid)
        {
            Guid actualGuid = new Guid(guid);
            byte[] guidBytes = actualGuid.ToByteArray();
            value0 = BitConverter.ToInt32(guidBytes, 0);
            value1 = BitConverter.ToInt32(guidBytes, 4);
            value2 = BitConverter.ToInt32(guidBytes, 8);
            value3 = BitConverter.ToInt32(guidBytes, 12);
        }

        /// <summary>
        /// Constructs a WeakAssetReference from four integers that represent a GUID.
        /// </summary>
        /// <param name="value0">First integer.</param>
        /// <param name="value1">Second integer.</param>
        /// <param name="value2">Third integer.</param>
        /// <param name="value3">Fourth integer.</param>
        public WeakAssetReference(int value0, int value1, int value2, int value3)
        {
            this.value0 = value0;
            this.value1 = value1;
            this.value2 = value2;
            this.value3 = value3;
        }

        /// <summary>
        /// Constructs a WeakAssetReference from a GUID.
        /// </summary>
        /// <param name="guid">The GUID.</param>
        public WeakAssetReference(Guid guid)
        {
            byte[] guidBytes = guid.ToByteArray();
            value0 = BitConverter.ToInt32(guidBytes, 0);
            value1 = BitConverter.ToInt32(guidBytes, 4);
            value2 = BitConverter.ToInt32(guidBytes, 8);
            value3 = BitConverter.ToInt32(guidBytes, 12);
        }

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="x">Left hand side WeakAssetReference.</param>
        /// <param name="y">Right hand side WeakAssetReference.</param>
        /// <returns>True if the left-hand side WeakAssetReference and right-hand side WeakAssetReference are equal, false otherwise.</returns>
        public static bool operator ==(WeakAssetReference x, WeakAssetReference y)
        {
            return x.value0 == y.value0 && x.value1 == y.value1 && x.value2 == y.value2 && x.value3 == y.value3;
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="x">Left hand side WeakAssetReference.</param>
        /// <param name="y">Right hand side WeakAssetReference.</param>
        /// <returns>True if the left-hand side WeakAssetReference and right-hand side WeakAssetReference are not equal, false otherwise.</returns>
        public static bool operator !=(WeakAssetReference x, WeakAssetReference y)
        {
            return !(x == y);
        }

        /// <summary>
        /// Checks whether the WeakAssetReference is set.
        /// </summary>
        /// <returns>True if the WeakAssetReference is set, false otherwise.</returns>
        public bool IsSet()
        {
            return value0 != 0 || value1 != 0 || value2 != 0 || value3 != 0;
        }

        /// <summary>
        /// Returns the WeakAssetReference as a GUID.
        /// </summary>
        /// <returns>The WeakAssetReference as a GUID.</returns>
        public Guid GetGuid()
        {
            byte[] gb = new byte[16];

            byte[] buf;
            buf = BitConverter.GetBytes(value0);
            Array.Copy(buf, 0, gb, 0, 4);
            buf = BitConverter.GetBytes(value1);
            Array.Copy(buf, 0, gb, 4, 4);
            buf = BitConverter.GetBytes(value2);
            Array.Copy(buf, 0, gb, 8, 4);
            buf = BitConverter.GetBytes(value3);
            Array.Copy(buf, 0, gb, 12, 4);

            return new Guid(gb);
        }

        /// <summary>
        /// Returns the WeakAssetReference as a string representing the GUID.
        /// </summary>
        /// <returns>The WeakAssetReference as a string representing the GUID.</returns>
        public string ToGuidStr()
        {
            return GetGuid().ToString("N");
        }

#if UNITY_EDITOR
        /// <summary>
        /// Loads the asset the WeakAssetReference is referencing.
        /// </summary>
        /// <typeparam name="T">Data type of the asset.</typeparam>
        /// <returns>The loaded asset.</returns>
        public T LoadAsset<T>() where T : UnityEngine.Object
        {
            string path = AssetDatabase.GUIDToAssetPath(ToGuidStr());
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }
#endif

        /// <summary>
        /// Checks whether the WeakAssetReference is equal to an object.
        /// </summary>
        /// <param name="obj">The object whose equality should be checked.</param>
        /// <returns>True if the WeakAssetReference is equal to the object, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is WeakAssetReference))
            {
                return false;
            }

            WeakAssetReference reference = (WeakAssetReference)obj;
            return value0 == reference.value0 &&
                   value1 == reference.value1 &&
                   value2 == reference.value2 &&
                   value3 == reference.value3;
        }

        /// <summary>
        /// Returns the hash code of the WeakAssetReference.
        /// </summary>
        /// <returns>The hash code of the WeakAssetReference.</returns>
        public override int GetHashCode()
        {
            int hashCode = -345130910;
            hashCode = hashCode * -1521134295 + value0.GetHashCode();
            hashCode = hashCode * -1521134295 + value1.GetHashCode();
            hashCode = hashCode * -1521134295 + value2.GetHashCode();
            hashCode = hashCode * -1521134295 + value3.GetHashCode();
            return hashCode;
        }
    }

    /// <summary>
    /// This base is here to allow CustomPropertyDrawer to pick it up.
    /// </summary>
    [System.Serializable]
    public class WeakBase
    {
        /// <summary>
        /// String representing the GUID.
        /// </summary>
        public string guid = "";
    }

    /// <summary>
    /// Derive from this to create a typed weak asset reference.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    [System.Serializable]
    public class Weak<T> : WeakBase
    {
    }
}