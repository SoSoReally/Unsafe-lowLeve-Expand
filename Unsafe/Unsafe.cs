using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

public static partial class UnsafeCode
{

    public unsafe static ref T As<T>(this Span<byte> span) where T : unmanaged
    {
        return ref Unsafe.AsRef<T>(span.GetIntPtr().ToPointer());
    }

    public unsafe static ref readonly T AsReadOnly<T>(this ReadOnlySpan<byte> span) where T : unmanaged
    {
        return ref Unsafe.As<byte, T>(ref Unsafe.AsRef(in span.GetPinnableReference()));
    }


    #region Get IntPtr
    public unsafe static IntPtr GetUnSafeIntPtr<T>(this NativeSlice<T> values) where T : unmanaged
    {

        return new IntPtr(values.GetUnsafePtr());
    }

    public unsafe static IntPtr GetUnSafeIntPtr<T>(this NativeArray<T> values) where T : unmanaged
    {
        return new IntPtr(values.GetUnsafePtr());
    }

    public unsafe static IntPtr GetIntPtr<T>(this Span<T> values) where T : unmanaged
    {
        return new IntPtr(UnsafeUtility.AddressOf<T>(ref values.GetPinnableReference()));
    }
    #endregion

    #region As Span and ReadOnlySpan
    public unsafe static Span<T> AsSpan<T>(this NativeSlice<T> values) where T : unmanaged
    {
        return new Span<T>(values.GetUnsafePtr(), values.Length);
    }
    public unsafe static ReadOnlySpan<T> AsReadOnlySpan<T>(ref this NativeSlice<T> values) where T : unmanaged
    {
        return new ReadOnlySpan<T>(values.GetUnsafePtr(), values.Length);
    }
    public unsafe static Span<T> AsSpan<T>(this IntPtr intPtr, int length)
    {
        return new Span<T>(intPtr.ToPointer(), length);
    }
    public unsafe static ReadOnlySpan<T> AsReadOnlySpan<T>(this IntPtr ptr, int length)
    {
        return new ReadOnlySpan<T>(ptr.ToPointer(), length);
    }
    public unsafe static Span<T> AsSpan<T>(this NativeList<T> nativeList) where T : unmanaged
    {
        return new Span<T>(nativeList.GetUnsafeList()->Ptr, nativeList.GetUnsafeList()->Length);
    }
    #endregion

    #region NativeContainer As From Span and ReadOnlySpan

    /// <summary>
    /// 需要保证原子操作安全
    /// span 总是 none，copy 使用 <see cref="ToNativeArray{T}(Span{T}, Allocator)"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ns"></param>
    /// <param name="allocator"></param>
    /// <returns></returns>
    public unsafe static NativeArray<T> AsNativeArray<T>(this Span<T> ns) where T : unmanaged
    {
        return CollectionHelper.ConvertExistingDataToNativeArray<T>(UnsafeUtility.AddressOf<T>(ref ns.GetPinnableReference()), ns.Length, Allocator.None, true);
        //        var result = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>(UnsafeUtility.AddressOf(ref ns[0]), ns.Length, Allocator.None);
        //#if ENABLE_UNITY_COLLECTIONS_CHECKS
        //        var safety = AtomicSafetyHandle.GetTempMemoryHandle();
        //        NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref result, safety);
        //#endif
        //        return result;
    }
    public unsafe static NativeArray<T>.ReadOnly AsNativeArrayReadOnly<T>(this ReadOnlySpan<T> ns) where T : unmanaged
    {
        ref var point = ref System.Runtime.InteropServices.MemoryMarshal.GetReference(ns);
        var result = CollectionHelper.ConvertExistingDataToNativeArray<T>(UnsafeUtility.AddressOf<T>(ref point), ns.Length, Allocator.None, true);
        //#if ENABLE_UNITY_COLLECTIONS_CHECKS
        //        var safetyHandle = NativeArrayUnsafeUtility.GetAtomicSafetyHandle(result);
        //        AtomicSafetyHandle.SetAllowReadOrWriteAccess(safetyHandle, true);
        //#endif
        return result.AsReadOnly();
    }
    public unsafe static NativeArray<T> ToNativeArray<T>(this ReadOnlySpan<T> ns, Allocator allocator) where T : unmanaged
    {
        NativeArray<T> result = new NativeArray<T>(ns.Length, allocator, NativeArrayOptions.UninitializedMemory);
        UnsafeUtility.MemCpyReplicate(UnsafeUtility.AddressOf<T>(ref System.Runtime.InteropServices.MemoryMarshal.GetReference(ns)), result.GetUnsafePtr(), UnsafeUtility.SizeOf<T>(), ns.Length);
        return result;
    }
    public unsafe static NativeArray<T> ToNativeArray<T>(this Span<T> ns, Allocator allocator) where T : unmanaged
    {
        NativeArray<T> result = new NativeArray<T>(ns.Length, allocator, NativeArrayOptions.UninitializedMemory);
        UnsafeUtility.MemCpyReplicate(UnsafeUtility.AddressOf<T>(ref System.Runtime.InteropServices.MemoryMarshal.GetReference(ns)), result.GetUnsafePtr(), UnsafeUtility.SizeOf<T>(), ns.Length);
        return result;
    }

    public unsafe static NativeArray<T> AsNativeArray<T>(this IntPtr intPtr,int length,Allocator allocator) where T : unmanaged
    {
        return CollectionHelper.ConvertExistingDataToNativeArray<T>(intPtr.ToPointer(), length, allocator);
    }

    public unsafe static UnsafeList<T> AsUnsafeList<T>(this Span<T> ns) where T : unmanaged
    {
        return new Unity.Collections.LowLevel.Unsafe.UnsafeList<T>((T*)Unsafe.AsPointer(ref ns[0]), ns.Length);
    }

    public unsafe static Span<T> AsSpan<T>(this UnsafeList<T> ts) where T : unmanaged
        {
            return new Span<T>(ts.Ptr, ts.Length);
        }
    public unsafe static ReadOnlySpan<T> AsReadOnlySpan<T>(this UnsafeList<T> ts) where T : unmanaged
    {
        return new ReadOnlySpan<T>(ts.Ptr, ts.Length);
    }

    public unsafe static ref T UnsafeElementAt<T>(this UnsafeList<T> ts,int index) where T : unmanaged
    {
        return ref ts.Ptr[index];
    }
    #endregion

    #region NativeContainer to NativeContainer

    public unsafe static NativeArray<T> ToNativeArray<T>(this NativeSlice<T> ns ,Allocator allocator)where T : unmanaged
    {
        NativeArray<T> result = new NativeArray<T>(ns.Length,allocator);
        ns.CopyTo(result);
        return result;
    }

    #endregion

    public unsafe static void UnSafeAddSpan<T>(this NativeList<T> values,Span<T> values1) where T : unmanaged
    {
        values.GetUnsafeList()->AddRange(UnsafeUtility.AddressOf<T>(ref values1.GetPinnableReference()), values1.Length);
    }



}
