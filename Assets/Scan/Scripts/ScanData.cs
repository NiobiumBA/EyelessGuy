using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scan
{
    public class ScanData : IList<ScanData.SphereInfo>, IDisposable
    {
        private struct Enumerator : IEnumerator<SphereInfo>, IEnumerator, IDisposable
        {
            private int m_currentId;
            private ScanData m_data;

            public readonly SphereInfo Current => m_data[m_currentId];

            readonly object IEnumerator.Current => m_data[m_currentId];

            public Enumerator(ScanData data)
            {
                m_data = data;
                m_currentId = -1;
            }

            public bool MoveNext()
            {
                m_currentId++;
                return m_currentId < m_data.Count;
            }

            public void Reset()
            {
                m_currentId = -1;
            }

            public void Dispose()
            {

            }
        }

        public struct SphereInfo : IEquatable<SphereInfo>
        {
            public Vector3 position;
            public float r, g, b;

            public SphereInfo(Vector3 position, Color color)
            {
                this.position = position;
                r = color.r;
                g = color.g;
                b = color.b;
            }

            public override readonly string ToString()
            {
                return "Position: " + position + ", Color: " + new Color(r, g, b);
            }

            public override readonly bool Equals(object other)
            {
                if (other is SphereInfo otherSphere)
                    return this == otherSphere;

                return false;
            }

            public readonly bool Equals(SphereInfo other)
            {
                return this == other;
            }

            public override readonly int GetHashCode()
            {
                return position.GetHashCode() ^ r.GetHashCode() ^ g.GetHashCode() ^ b.GetHashCode();
            }

            public static bool operator ==(SphereInfo a, SphereInfo b)
            {
                return a.position == b.position && a.r == b.r && a.g == b.g && a.b == b.b;
            }

            public static bool operator !=(SphereInfo a, SphereInfo b)
            {
                return !(a == b);
            }
        }

        private int m_maxSpheres;
        private int m_countSpheres;
        private ComputeBuffer m_buffer;
        private SphereInfo[] m_array;

        public int MaxSpheres { get => m_maxSpheres; }
        public int Count { get => m_countSpheres; }
        public ComputeBuffer ComputeBuffer { get => m_buffer; }
        public IReadOnlyCollection<SphereInfo> AsReadOnly { get => m_array; }
        public int Stride
        {
            get => System.Runtime.InteropServices.Marshal.SizeOf<SphereInfo>();
        }

        public bool IsReadOnly => false;

        public SphereInfo this[int index]
        {
            get
            {
                if (index >= m_countSpheres)
                    throw new IndexOutOfRangeException("ScanData");

                return m_array[index];
            }
            set
            {
                if (index >= m_countSpheres)
                    throw new IndexOutOfRangeException("ScanData");

                m_array[index] = value;
            }
        }

        public ScanData(int maxSpheres)
        {
            m_countSpheres = 0;
            m_maxSpheres = maxSpheres;
            m_buffer = new ComputeBuffer(maxSpheres, Stride);
            m_array = new SphereInfo[maxSpheres];

            //for (int i = 0; i < maxSpheres; i++)
            //    m_array[i].x = float.PositiveInfinity;
        }

        // Send data to GPU
        public void Apply()
        {
            m_buffer.SetData(m_array);
        }

        public void SetData(SphereInfo[] sphereInfos)
        {
            if (sphereInfos.Length > m_maxSpheres)
                throw new ArgumentOutOfRangeException("Input array is too big.");

            for (int i = 0; i < sphereInfos.Length; i++)
            {
                m_array[i] = sphereInfos[i];
            }

            //for (int i = sphereInfos.Length; i < m_maxSpheres; i++)
            //{
            //    m_array[i].x = float.PositiveInfinity;
            //}
        }

        public void Dispose()
        {
            m_buffer.Dispose();
        }

        public void Add(SphereInfo info)
        {
            m_array[m_countSpheres] = info;
            m_countSpheres++;
        }

        public void Clear()
        {
            m_countSpheres = 0;

            //for (int i = 0; i < m_maxSpheres; ++i)
            //    m_array[i].x = float.PositiveInfinity;
        }

        public int IndexOf(SphereInfo item)
        {
            int i;
            for (i = 0; i < m_countSpheres; i++)
            {
                if (item == m_array[i])
                    break;
            }

            return i;
        }

        public void Insert(int index, SphereInfo item)
        {
            if (index >= m_countSpheres)
                throw new IndexOutOfRangeException();

            SphereInfo last = item;
            SphereInfo temp;

            m_countSpheres++;

            for (int i = index; i < m_countSpheres; i++)
            {
                temp = m_array[i];
                m_array[i] = last;
                last = temp;
            }
        }

        public void RemoveAt(int index)
        {
            if (index >= m_countSpheres)
                throw new IndexOutOfRangeException();

            for (int i = index; i < m_countSpheres; i++)
            {
                m_array[i] = m_array[i + 1];
            }

            m_countSpheres--;
        }

        public bool Contains(SphereInfo item)
        {
            for (int i = 0; i < m_countSpheres; i++)
            {
                if (item == m_array[i])
                    return true;
            }
            return false;
        }

        public void CopyTo(SphereInfo[] array, int arrayIndex)
        {
            array = new SphereInfo[arrayIndex + m_countSpheres];

            for (int i = 0; i < m_countSpheres; i++)
            {
                array[i + arrayIndex + 1] = m_array[i];
            }
        }

        public bool Remove(SphereInfo item)
        {
            int foundIndex = IndexOf(item);

            if (foundIndex == m_countSpheres)
                return false;

            RemoveAt(foundIndex);
            return true;
        }

        public IEnumerator<SphereInfo> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }
    }
}