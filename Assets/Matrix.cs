using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class Matrix<T> : IEnumerable<T>
{
    //IMPLEMENTAR: ESTRUCTURA INTERNA- DONDE GUARDO LOS DATOS?
    T[,] matrix;

    public Matrix(int width, int height)
    {
        //IMPLEMENTAR: constructor
        matrix = new T[width, height];
    }

	public Matrix(T[,] copyFrom)
    {
        //IMPLEMENTAR: crea una version de Matrix a partir de una matriz básica de C#
        matrix = new T[copyFrom.GetLength(0), copyFrom.GetLength(1)];
        for (int i = 0; i < matrix.GetLength(0); i++) 
            for (int j = 0; j < matrix.GetLength(1); j++)
                matrix[i,j] = copyFrom[i,j];
            
        
    }

	public Matrix<T> Clone() {
        Matrix<T> aux = new Matrix<T>(Width, Height);
        //IMPLEMENTAR
        return aux;
    }

	public void SetRangeTo(int x0, int y0, int x1, int y1, T item) {
        //IMPLEMENTAR: iguala todo el rango pasado por parámetro a item

        for (int x = x0; x < x1; x++)       
            for (int y = y0; y < y1; y++)            
                matrix[x,y] = item;
                    
    }

    //Todos los parametros son INCLUYENTES
    public List<T> GetRange(int x0, int y0, int x1, int y1) {
        List<T> l = new List<T>();

        for (int x = x0; x < x1; x++)
            for (int y = y0; y < y1; y++)
                l.Add(matrix[x,y]);
              
         return l;
	}

    //Para poder igualar valores en la matrix a algo
    public T this[int x, int y] {
		get
        {
            return matrix[x, y];
		}
		set 
        {
            matrix[x, y] = value;
        }
	}

    public int Width { get; private set; }

    public int Height { get; private set; }

    public int Capacity { get; private set; }

    public IEnumerator<T> GetEnumerator()
    {
        int x0=0;
        int y0=0;
        yield return matrix[x0,y0];

    }

	IEnumerator IEnumerable.GetEnumerator() {
		return GetEnumerator();
	}
}
