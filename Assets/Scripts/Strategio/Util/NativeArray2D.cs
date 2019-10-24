using System;
using Unity.Collections;

namespace Strategio.Util
{
    public struct NativeArray2D<T> : IDisposable
        where T : struct
    {
        public int XMax { get; }
        public int YMax { get; }

        public bool IsCreated => _arr.IsCreated;

        private NativeArray<T> _arr;

        public NativeArray2D(int xMax,
                             int yMax,
                             Allocator allocator,
                             NativeArrayOptions nativeArrayOptions = NativeArrayOptions.ClearMemory)
        {
            XMax = xMax;
            YMax = yMax;
            _arr = new NativeArray<T>(xMax * yMax, allocator, nativeArrayOptions);
        }

        public NativeArray2D(NativeArray2D<T> toCopy, Allocator allocator)
        {
            XMax = toCopy.XMax;
            YMax = toCopy.YMax;
            _arr = new NativeArray<T>(toCopy._arr, allocator);
        }

        public void CopyFrom(NativeArray2D<T> toCopy)
        {
#if UNITY_EDITOR
            if (toCopy.XMax != XMax || toCopy.YMax != YMax)
                throw new InvalidOperationException("sizes don't match");
#endif
            _arr.CopyFrom(toCopy._arr);
        }

        public void Dispose()
        {
            _arr.Dispose();
        }

        public void At(int i, out int x, out int y)
        {
            x = i % XMax;
            y = i / XMax;
        }

        public T this[int i]
        {
            //(z * xMax * yMax) + (y * xMax) + x;
            get => _arr[i];
            set => _arr[i] = value;
        }

        public T this[int x, int y]
        {
            //(z * xMax * yMax) + (y * xMax) + x;
            get => _arr[y * XMax + x];
            set => _arr[y * XMax + x] = value;
        }
    }
}