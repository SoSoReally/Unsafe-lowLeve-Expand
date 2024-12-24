using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnsafeBuffer;

namespace UnsafeBuffer
{
    public readonly unsafe struct UnsafeReadOnlySpan<T>
    where T : unmanaged
    {
        internal readonly T* ptr;

        public UnsafeReadOnlySpan(in T value)
        {
            fixed (T* valuePtr = &value)
            {
                ptr = valuePtr;
            }
        }

        public UnsafeReadOnlySpan(T* value)
        {
            ptr = (T*)value;
        }

        public readonly ref readonly T this[int index]
        {
            get => ref ptr[index];
        }

        public static implicit operator UnsafeReadOnlySpan<T>(Span<T> value)
        {
            return new UnsafeReadOnlySpan<T>(in value[0]);
        }

        public static implicit operator UnsafeReadOnlySpan<T>(NativeArray<T> value)
        {
            return new UnsafeReadOnlySpan<T>((T*)value.GetUnsafePtr());
        }

        public static implicit operator UnsafeReadOnlySpan<T>(UnsafeSpan<T> value)
        {
            return new UnsafeReadOnlySpan<T>(value.ptr);
        }

        public static implicit operator UnsafeReadOnlySpan<T>(NativeArray<T>.ReadOnly value)
        {
            return new UnsafeReadOnlySpan<T>((T*)value.GetUnsafeReadOnlyPtr());
        }

        public static implicit operator UnsafeReadOnlySpan<T>(ReadOnlySpan<T> value)
        {
            return new UnsafeReadOnlySpan<T>(in value[0]);
        }
    }

    public readonly unsafe struct UnsafeRO<T>
        where T : unmanaged
    {
        internal readonly T* ptr;

        public UnsafeRO(in T value)
        {
            fixed (T* valuePtr = &value)
            {
                ptr = valuePtr;
            }
        }

        public UnsafeRO(T* value)
        {
            ptr = value;
        }

        public UnsafeRO(void* value)
        {
            ptr = (T*)value;
        }

        public readonly ref readonly T Value
        {
            get { return ref *ptr; }
        }

        public static implicit operator UnsafeRO<T>(UnsafeRW<T> value)
        {
            return new UnsafeRO<T>(value.ptr);
        }
    }

    public unsafe struct UnsafeRW<T>
        where T : unmanaged
    {
        internal T* ptr;

        public UnsafeRW(ref T value)
        {
            fixed (T* valuePtr = &value)
            {
                ptr = valuePtr;
            }
        }

        public UnsafeRW(T* value)
        {
            ptr = value;
        }

        public UnsafeRW(void* value)
        {
            ptr = (T*)value;
        }

        public ref T Value
        {
            get
            {
                unsafe
                {
                    return ref *ptr;
                }
            }
        }
    }

    public unsafe struct UnsafeSpan<T>
        where T : unmanaged
    {
        internal T* ptr;

        public UnsafeSpan(ref T value)
        {
            fixed (T* valuePtr = &value)
            {
                ptr = valuePtr;
            }
        }

        public UnsafeSpan(T* value)
        {
            ptr = (T*)value;
        }

        public ref T this[int index]
        {
            get => ref ptr[index];
        }

        public static implicit operator UnsafeSpan<T>(Span<T> value)
        {
            return new UnsafeSpan<T>(ref value[0]);
        }

        public static implicit operator UnsafeSpan<T>(NativeArray<T> value)
        {
            return new UnsafeSpan<T>((T*)value.GetUnsafePtr());
        }

        public UnsafeRO<T> AsUnsafeRO(int index)
        {
            return new UnsafeRO<T>(in ptr[index]);
        }

        public UnsafeRW<T> AsUnsafeRW(int index)
        {
            return new UnsafeRW<T>(ref ptr[index]);
        }
    }
}



public static partial class UnsafeCode
{
    public static unsafe NativeArray<T> ArrayOnlyOneElement<T>(in T value)
        where T : unmanaged
    {
        var na = new NativeArray<T>(1, Allocator.Temp);
        na[0] = value;
        return na;
    }

    public static unsafe ref T As<T>(this Span<byte> span)
        where T : unmanaged
    {
        return ref Unsafe.AsRef<T>(span.GetIntPtr().ToPointer());
    }

    public static unsafe IntPtr AsIntPtr(this Span<byte> bytes)
    {
        return new IntPtr(Unsafe.AsPointer(ref bytes[0]));
    }

    public static unsafe IntPtr AsIntPtrByReadOnly<T>(in T value)
        where T : unmanaged
    {
        return new IntPtr(Unsafe.AsPointer(ref Unsafe.AsRef(in value)));
    }

    public static unsafe ref readonly T AsReadOnly<T>(this ReadOnlySpan<byte> span)
        where T : unmanaged
    {
        return ref Unsafe.As<byte, T>(ref Unsafe.AsRef(in span[0]));
    }

    public static unsafe ref readonly T AsReadOnly<T>(this Span<byte> span)
        where T : unmanaged
    {
        return ref Unsafe.As<byte, T>(ref Unsafe.AsRef(in span[0]));
    }

    public static unsafe ReadOnlySpan<byte> AsReadOnlySpanByte<T>(this ref T value)
        where T : unmanaged
    {
        return new ReadOnlySpan<byte>(Unsafe.AsPointer(ref value), Unsafe.SizeOf<T>());
    }

    public static unsafe Span<byte> AsSpanByte<T>(this ref T value)
        where T : unmanaged
    {
        return new Span<byte>(Unsafe.AsPointer(ref value), Unsafe.SizeOf<T>());
    }

    public static unsafe UnsafeRO<T> AsUnsafeRO<T>(this ref T value)
        where T : unmanaged
    {
        return new UnsafeRO<T>(in value);
    }

    public static unsafe UnsafeRW<T> AsUnsafeRW<T>(this ref T value)
        where T : unmanaged
    {
        return new UnsafeRW<T>(ref value);
    }

    #region Get IntPtr

    public static unsafe IntPtr AsIntPtr<T>(this ref T values)
        where T : unmanaged
    {
        return new IntPtr(Unsafe.AsPointer(ref values));
    }

    public static unsafe IntPtr GetBufferUnSafeIntPtr<T>(this ref NativeSlice<T> values)
        where T : unmanaged
    {
        return new IntPtr(values.GetUnsafePtr());
    }

    public static unsafe IntPtr GetBufferUnSafeIntPtr<T>(this ref NativeArray<T> values)
        where T : unmanaged
    {
        return new IntPtr(values.GetUnsafePtr());
    }

    public static unsafe IntPtr GetIntPtr<T>(this ref Span<T> values)
        where T : unmanaged
    {
        return new IntPtr(Unsafe.AsPointer(ref values[0]));
    }
    #endregion Get IntPtr

    #region As Span and ReadOnlySpan

    public static unsafe ref T As<T>(this IntPtr intPtr)
        where T : unmanaged
    {
        return ref Unsafe.AsRef<T>(intPtr.ToPointer());
    }

    public static unsafe ReadOnlySpan<T> AsReadOnlySpan<T>(this in NativeSlice<T> values)
        where T : unmanaged
    {
        return new ReadOnlySpan<T>(values.GetUnsafePtr(), values.Length);
    }

    public static unsafe ReadOnlySpan<T> AsReadOnlySpan<T>(this IntPtr ptr, int length)
    {
        return new ReadOnlySpan<T>(ptr.ToPointer(), length);
    }

    public static unsafe ReadOnlySpan<T> AsReadOnlySpan<T>(this in NativeList<T> nativeList)
        where T : unmanaged
    {
        return new ReadOnlySpan<T>(
            nativeList.GetUnsafeList()->Ptr,
            nativeList.GetUnsafeList()->Length
        );
    }

    public static unsafe Span<T> AsSpan<T>(this ref NativeSlice<T> values)
        where T : unmanaged
    {
        return new Span<T>(values.GetUnsafePtr(), values.Length);
    }

    public static unsafe Span<T> AsSpan<T>(this IntPtr intPtr, int length)
    {
        return new Span<T>(intPtr.ToPointer(), length);
    }

    public static unsafe Span<T> AsSpan<T>(this ref NativeList<T> nativeList)
        where T : unmanaged
    {
        return new Span<T>(nativeList.GetUnsafeList()->Ptr, nativeList.GetUnsafeList()->Length);
    }
    #endregion As Span and ReadOnlySpan

    #region NativeContainer As From Span and ReadOnlySpan

    public static unsafe NativeArray<T> AsNativeArray<T>(this Span<T> ns)
        where T : unmanaged
    {
        fixed (void* b = &ns[0])
        {
            return CollectionHelper.ConvertExistingDataToNativeArray<T>(
                b,
                ns.Length,
                Allocator.None,
                true
            );
        }

        //        var result = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>(UnsafeUtility.AddressOf(ref ns[0]), ns.Length, Allocator.None);
        //#if ENABLE_UNITY_COLLECTIONS_CHECKS
        //        var safety = AtomicSafetyHandle.GetTempMemoryHandle();
        //        NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref result, safety);
        //#endif
        //        return result;
    }

    public static unsafe NativeArray<T> AsNativeArray<T>(
        this IntPtr intPtr,
        int length,
        Allocator allocator
    )
        where T : unmanaged
    {
        return CollectionHelper.ConvertExistingDataToNativeArray<T>(
            intPtr.ToPointer(),
            length,
            allocator
        );
    }

    public static unsafe NativeArray<T>.ReadOnly AsNativeArrayReadOnly<T>(this ReadOnlySpan<T> ns)
        where T : unmanaged
    {
        ref var point = ref System.Runtime.InteropServices.MemoryMarshal.GetReference(ns);
        var result = CollectionHelper.ConvertExistingDataToNativeArray<T>(
            UnsafeUtility.AddressOf<T>(ref point),
            ns.Length,
            Allocator.None,
            true
        );
        //#if ENABLE_UNITY_COLLECTIONS_CHECKS
        //        var safetyHandle = NativeArrayUnsafeUtility.GetAtomicSafetyHandle(result);
        //        AtomicSafetyHandle.SetAllowReadOrWriteAccess(safetyHandle, true);
        //#endif
        return result.AsReadOnly();
    }

    public static unsafe ReadOnlySpan<T> AsReadOnlySpan<T>(this in UnsafeList<T> ts)
        where T : unmanaged
    {
        return new ReadOnlySpan<T>(ts.Ptr, ts.Length);
    }

    public static unsafe Span<T> AsSpan<T>(this ref UnsafeList<T> ts)
        where T : unmanaged
    {
        return new Span<T>(ts.Ptr, ts.Length);
    }

    public static unsafe UnsafeList<T> AsUnsafeList<T>(this Span<T> ns)
        where T : unmanaged
    {
        return new Unity.Collections.LowLevel.Unsafe.UnsafeList<T>(
            (T*)Unsafe.AsPointer(ref ns.GetPinnableReference()),
            ns.Length
        );
    }

    public static unsafe T Pop<T>(this ref UnsafeList<T> values)
        where T : unmanaged
    {
        T result = values[values.Length - 1];
        values.RemoveAt(values.Length - 1);
        return result;
    }

    public static unsafe void Push<T>(this ref UnsafeList<T> list, T value)
        where T : unmanaged
    {
        list.Add(value);
        var span = list.AsSpan();
        for (int i = 0; i < list.Length - 2; i++)
        {
            span[i + 1] = span[i];
        }
        span[0] = value;
    }

    public static unsafe NativeArray<T> ToNativeArray<T>(
        this ReadOnlySpan<T> ns,
        Allocator allocator
    )
        where T : unmanaged
    {
        NativeArray<T> result = new NativeArray<T>(
            ns.Length,
            allocator,
            NativeArrayOptions.UninitializedMemory
        );
        UnsafeUtility.MemCpyReplicate(
            UnsafeUtility.AddressOf<T>(
                ref System.Runtime.InteropServices.MemoryMarshal.GetReference(ns)
            ),
            result.GetUnsafePtr(),
            UnsafeUtility.SizeOf<T>(),
            ns.Length
        );
        return result;
    }

    public static unsafe NativeArray<T> ToNativeArray<T>(this Span<T> ns, Allocator allocator)
        where T : unmanaged
    {
        NativeArray<T> result = new NativeArray<T>(
            ns.Length,
            allocator,
            NativeArrayOptions.UninitializedMemory
        );
        UnsafeUtility.MemCpyReplicate(
            UnsafeUtility.AddressOf<T>(
                ref System.Runtime.InteropServices.MemoryMarshal.GetReference(ns)
            ),
            result.GetUnsafePtr(),
            UnsafeUtility.SizeOf<T>(),
            ns.Length
        );
        return result;
    }

    public static unsafe ref T UnsafeElementAt<T>(this ref UnsafeList<T> ts, int index)
        where T : unmanaged
    {
        return ref ts.Ptr[index];
    }

    #endregion NativeContainer As From Span and ReadOnlySpan

    #region NativeContainer to NativeContainer

    public static unsafe void AddRange<T>(this ref UnsafeList<T> us, in NativeArray<T> values)
        where T : unmanaged
    {
        us.AddRangeNoResize(values.GetUnsafePtr(), values.Length);
    }

    public static unsafe NativeArray<T> ToNativeArray<T>(
        this ref NativeSlice<T> ns,
        Allocator allocator
    )
        where T : unmanaged
    {
        NativeArray<T> result = new NativeArray<T>(ns.Length, allocator);
        ns.CopyTo(result);
        return result;
    }
    #endregion NativeContainer to NativeContainer

    public static unsafe void UnSafeAddSpan<T>(this NativeList<T> values, Span<T> values1)
        where T : unmanaged
    {
        values
            .GetUnsafeList()
            ->AddRange(
                UnsafeUtility.AddressOf<T>(ref values1.GetPinnableReference()),
                values1.Length
            );
    }
}
