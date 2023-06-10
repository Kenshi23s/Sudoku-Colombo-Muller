using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class Matrix<T> : IEnumerable<T>
{
    //IMPLEMENTAR: ESTRUCTURA INTERNA- DONDE GUARDO LOS DATOS?
    T[,] _matrix;

    public Matrix(int width, int height)
    {
        //IMPLEMENTAR: constructor
        _matrix = new T[width, height];

        Width  = width;
        Height = height;
        Capacity= width*height;
    }

	public Matrix(T[,] copyFrom)
    {
        //IMPLEMENTAR: crea una version de Matrix a partir de una matriz básica de C#
        Width = copyFrom.GetLength(0);
        Height = copyFrom.GetLength(1);
        Capacity = Width * Height;

        T[,] newMatrix = new T[Width, Height];
        for (int i = 0; i < Width; i++) 
            for (int j = 0; j < Height; j++)
                newMatrix[i,j] = copyFrom[i,j];

        _matrix = newMatrix;
    }

	public Matrix<T> Clone()
    {
        //IMPLEMENTAR
        var aux = new Matrix<T>(_matrix);
        for (int i = 0; i < Width; i++)
            for (int j = 0; j < Height; j++)
                aux[i, j] = _matrix[i, j];

        return aux;

    }

	public void SetRangeTo(int x0, int y0, int x1, int y1, T item) {
        //IMPLEMENTAR: iguala todo el rango pasado por parámetro a item

        for (int x = x0; x < x1; x++)       
            for (int y = y0; y < y1; y++)            
                _matrix[x,y] = item;
                    
    }

    //Todos los parametros son INCLUYENTES
    public List<T> GetRange(int x0, int y0, int x1, int y1) {
        List<T> l = new List<T>();

        for (int x = x0; x < x1; x++)
            for (int y = y0; y < y1; y++)
                l.Add(_matrix[x,y]);
              
         return l;
	}

    //Para poder igualar valores en la matrix a algo
    public T this[int x, int y] {
		get
        {
          
            return _matrix[x, y];
		}
		set 
        {
            _matrix[x, y] = value;
        }
	}

    public int Width { get; private set; }

    public int Height { get; private set; }

    public int Capacity { get; private set; }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < Width; i++)        
        for (int k = 0; k < Height; k++)
                yield return _matrix[i, k];
    }

	IEnumerator IEnumerable.GetEnumerator() {
		return GetEnumerator();
	}
}
