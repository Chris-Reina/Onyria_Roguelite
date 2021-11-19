namespace DoaT
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using System;

    //Filas = Width => Length(0)
    //Columnas = Height => Length(1)
    [Serializable]
    public class Matrix<T> : IEnumerable<T>
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Capacity { get; private set; }
        
        public bool IsEmpty => _internal.Length == 0;

        [SerializeField] private T[] _internal;

        public Matrix(int width, int height)
        {
            if (width <= 0 || height <= 0) throw new ArgumentOutOfRangeException();
            
            Width = width;
            Height = height;
            Capacity = Width * Height;
            _internal = new T[Capacity];
        }

        public Matrix(T[,] copyFrom)
        {
            Width = copyFrom.GetLength(0);
            Height = copyFrom.GetLength(1);
            Capacity = Width * Height;
            _internal = new T[Capacity];

            for (int x = 0; x < Height; x++)
                for (int y = 0; y < Width; y++)
                    _internal[y+(x*Width)] = copyFrom[x,y];
        }

        public Matrix(Matrix<T> copyFrom)
        {
            Width = copyFrom.Width;
            Height = copyFrom.Height;
            Capacity = copyFrom.Capacity;
            _internal = new List<T>(copyFrom._internal).ToArray();
        }
    
        public T this[int x, int y] 
        {
            get
            {
                if (x < 0 || x > Width || y < 0 || y > Height) throw new IndexOutOfRangeException();
                
                return _internal[x + y * Width];
            }
            set
            {
                if (x < 0 || x > Width || y < 0 || y > Height) throw new IndexOutOfRangeException();
                
                _internal[x + y * Width] = value;
            }
        }

        public Matrix<T> Clone() 
        {
            var aux = new Matrix<T>(Width, Height) {Width = Width, Height = Height, Capacity = Capacity};
        
            for (int i = 0; i < _internal.Length; i++)
            {
                aux._internal[i] = _internal[i];
            }
        
            return aux;
        }

        public void SetRangeTo(int x0, int y0, int x1, int y1, T item)
        {
            var auxX = Mathf.Abs(x0 - x1);
            var auxY = Mathf.Abs(y0 - y1);

            for (int x = 0; x < auxX; x++)
                for (int y = 0; y < auxY; y++)
                    _internal[(y0 + y) + (x0 + x) * Width] = item;
        }

        public List<T> GetRange(int x0, int y0, int x1, int y1)
        {
            var newList = new List<T>();

            for (int x = x0; x <= x1; x++)
                for (int y = y0; y <= y1; y++)
                    newList.Add(_internal[y + x * Width]);

            return newList;
        }


        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Capacity; i++)
                yield return _internal[i];
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        
        public override string ToString()
        {
            var ser = "";
	    
            for (var i = 0; i < Height; i++)
            {
                var auxSer = "";
                for (var j = 0; j < Width; j++)
                {
                    auxSer = string.Concat(auxSer, $"{_internal[j + i * Width]},");
                    //auxSer += $"{_internal[j + i * Width]},";
                }
                ser = string.Concat(ser, "\n");
                //ser += auxSer + "\n";
            }

            return ser;
        }
    }
}