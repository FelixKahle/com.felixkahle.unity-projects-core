// Copyright 2020 Felix Kahle. All rights reserved.

using System;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

namespace FelixKahle.UnityProjectsCore
{
    /// <summary>
    /// Collection of converted classic Unity (Mathf, Vector3 etc.) and some homegrown math functions using Unity.Mathematics.
    /// </summary>
    public static class Mathematics
    {
        public const float EpsilonNormalSqrt = 1e-15F;
        public const float Episilon = 1.17549435E-38f;

        public static uint Hash(uint i)
        {
            return i * 0x83B58237u + 0xA9D919BFu;
        }

        public static uint Hash(int i)
        {
            return (uint)i * 0x83B58237u + 0xA9D919BFu;
        }


        private static float MathfStyleZeroIsOneSign(float f)
        {
            return f >= 0F ? 1F : -1F;
        }

        public static float SignedAngle(float a, float b)
        {
            var difference = b - a;
            var sign = math.sign(difference);
            var offset = sign * 180.0f;

            return ((difference + offset) % 360.0f) - offset;
        }

        public static float SignedAngle(float3 from, float3 to, float3 axis)
        {
            float unsignedAngle = Angle(from, to);
            float sign = MathfStyleZeroIsOneSign(math.dot(axis, math.cross(from, to)));
            var result = unsignedAngle * sign;
            return result;
        }

        /// <summary>
        /// Moves a value current towards target.
        /// </summary>
        /// <param name="current">The current value</param>
        /// <param name="target">The value to move towards</param>
        /// <param name="maxDelta">The maximum change that should be applied to the value</param>
        /// <returns>Moved float.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MoveTowards(float current, float target, float maxDelta)
        {
            if (math.abs(target - current) <= maxDelta)
            {
                return target;
            }
            return current + math.sign(target - current) * maxDelta;
        }

        /// <summary>
        /// Moves a float3 current towards target.
        /// </summary>
        /// <param name="current">The current float3</param>
        /// <param name="target">The float3 to move towards</param>
        /// <param name="maxDelta">The maximum change that should be applied to the float3</param>
        /// <returns>Moved float3.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 MoveTowards(float3 current, float3 target, float maxDelta)
        {
            float toVectorX = target.x - current.x;
            float toVectorY = target.y - current.y;
            float toVectorZ = target.z - current.z;

            float sqdist = toVectorX * toVectorX + toVectorY * toVectorY + toVectorZ * toVectorZ;

            if (sqdist == 0 || (maxDelta >= 0 && sqdist <= maxDelta * maxDelta))
            {
                return target;
            }
            float dist = (float)math.sqrt(sqdist);

            return new float3(current.x + toVectorX / dist * maxDelta, current.y + toVectorY / dist * maxDelta, current.z + toVectorZ / dist * maxDelta);
        }

        public static float Angle(float3 from, float3 to)
        {
            float result;

            // sqrt(a) * sqrt(b) = sqrt(a * b) -- valid for real numbers
            float denominator = math.sqrt(math.lengthsq(from) * math.lengthsq(to));

            if (denominator < EpsilonNormalSqrt)
            {
                result = 0F;
            }
            else
            {
                float dot = math.clamp(math.dot(from, to) / denominator, -1F, 1F);
                result = math.degrees(math.acos(dot));
            }
            return result;
        }

        public static float LerpAngle(float a, float b, float t)
        {
            float delta = Repeat((b - a), 360);
            if (delta > 180)
            {
                delta -= 360;
            }
            var result = a + delta * math.clamp(t, 0f, 1f);
            return result;
        }

        public static float Repeat(float t, float length)
        {
            return math.clamp(t - math.floor(t / length) * length, 0.0f, length);
        }

        public static float DeltaAngle(float current, float target)
        {
            float delta = Repeat((target - current), 360.0F);
            if (delta > 180.0F)
            {
                delta -= 360.0F;
            }
            var result = delta;
            return result;
        }

        public static float SmoothDampAngle(float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
        {
            target = current + DeltaAngle(current, target);
            var result = SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
            return result;
        }

        public static float SmoothDamp(float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
        {
            smoothTime = math.max(0.0001F, smoothTime);
            float omega = 2F / smoothTime;

            float x = omega * deltaTime;
            float exp = 1F / (1F + x + 0.48F * x * x + 0.235F * x * x * x);
            float change = current - target;
            float originalTo = target;

            // Clamp maximum speed
            float maxChange = maxSpeed * smoothTime;
            change = math.clamp(change, -maxChange, maxChange);
            target = current - change;

            float temp = (currentVelocity + omega * change) * deltaTime;
            currentVelocity = (currentVelocity - omega * temp) * exp;
            float result = target + (change + temp) * exp;

            // Prevent overshooting
            if (originalTo - current > 0.0F == result > originalTo)
            {
                result = originalTo;
                currentVelocity = (result - originalTo) / deltaTime;
            }
            return result;
        }

        public static float3 Project(float3 vector, float3 onNormal)
        {
            float3 result;

            float sqrMag = math.dot(onNormal, onNormal);
            if (sqrMag < Episilon)
            {
                result = float3.zero;
            }
            else
            {
                result = onNormal * math.dot(vector, onNormal) / sqrMag;
            }
            return result;
        }

        public static float3 ProjectOnPlane(float3 vector, float3 planeNormal)
        {
            var result = vector - Project(vector, planeNormal);
            return result;
        }

        public static float2 ClampMagnitude(float2 vector, float maxLength)
        {
            if (math.lengthsq(vector) > maxLength * maxLength)
            {
                return math.normalizesafe(vector) * maxLength;
            }
            return vector;
        }

        public static float2 SmoothDamp(float2 current, float2 target, ref float2 currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
        {
            // Based on Game Programming Gems 4 Chapter 1.10
            smoothTime = math.max(0.0001F, smoothTime);
            float omega = 2F / smoothTime;

            float x = omega * deltaTime;
            float exp = 1F / (1F + x + 0.48F * x * x + 0.235F * x * x * x);
            float2 change = current - target;
            float2 originalTo = target;

            // Clamp maximum speed
            float maxChange = maxSpeed * smoothTime;
            change = ClampMagnitude(change, maxChange);
            target = current - change;

            float2 temp = (currentVelocity + omega * change) * deltaTime;
            currentVelocity = (currentVelocity - omega * temp) * exp;
            float2 output = target + (change + temp) * exp;

            // Prevent overshooting
            if (math.dot(originalTo - current, output - originalTo) > 0)
            {
                output = originalTo;
                currentVelocity = (output - originalTo) / deltaTime;
            }
            return output;
        }
    }
}
