using System;
using System.Runtime.CompilerServices;
using BEPUphysics.CollisionTests.CollisionAlgorithms;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
namespace Unity.Collections
{
    [Serializable]
    public unsafe struct FixedReadOnlyHashMap<TValue> : INativeDisposable where TValue : unmanaged
    {
        private UnsafeHashMap<int, int> Map;
        private UnsafeList<TValue> Value;

        public readonly ref readonly TValue this[int key]
        {
            get
            {
                return ref Unsafe.AsRef<TValue>(Value.Ptr[Map[key]]);
            }
        }

        public FixedReadOnlyHashMap(Span<int> key, Span<TValue> values)
        {
            //System.Runtime.CompilerServices.UnsafeValueTypeAttribute
            Map = new UnsafeHashMap<int, int>(key.Length, Allocator.Persistent);
            Value = new UnsafeList<TValue>(key.Length, Allocator.Persistent);
            for (int i = 0; i < key.Length; i++)
            {
                Map.TryAdd(key[i], i);
                Value.AddNoResize(values[i]);
            }
        }

        public readonly ReadOnlySpan<TValue> AsReadOnlySpan()
        {
            return Value.AsReadOnlySpan();
        }

        public JobHandle Dispose(JobHandle inputDeps)
        {
            var m = Map.Dispose(inputDeps);
            return Value.Dispose(m);
        }

        public void Dispose()
        {
            Map.Dispose();
            Value.Dispose();
        }
    }


}