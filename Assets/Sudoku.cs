using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class Sudoku : MonoBehaviour {
	public Cell prefabCell;
	public Canvas canvas;
	public Text feedback;
	public float stepDuration = 0.5f;
	[Range(1, 82)]public int difficulty = 40;

	Matrix<Cell> _board;
	Matrix<int> _createdMatrix;
    List<int> posibles = new List<int>();
	int _smallSide;
	int _bigSide;
    string memory = "";
    string canSolve = "";
    bool canPlayMusic = false;
    List<int> nums = new List<int>();



    float r = 1.0594f;
    float frequency = 440;
    float gain = 0.5f;
    float increment;
    float phase;
    float samplingF = 48000;


    void Start()
    {
        long mem = System.GC.GetTotalMemory(true);
        feedback.text = string.Format("MEM: {0:f2}MB", mem / (1024f * 1024f));
        memory = feedback.text;
        _smallSide = 3;
        _bigSide = _smallSide * 3;
        frequency = frequency * Mathf.Pow(r, 2);
        CreateEmptyBoard();
        ClearBoard();
    }

    void ClearBoard() {
		_createdMatrix = new Matrix<int>(_bigSide, _bigSide);
		foreach(var cell in _board) {
			cell.number = 0;
			cell.locked = cell.invalid = false;
		}
	}

	void CreateEmptyBoard() {
		float spacing = 68f;
		float startX = -spacing * 4f;
		float startY = spacing * 4f;

		_board = new Matrix<Cell>(_bigSide, _bigSide);
		for(int x = 0; x<_board.Width; x++) {
			for(int y = 0; y<_board.Height; y++) {
                var cell = _board[x, y] = Instantiate(prefabCell);
				cell.transform.SetParent(canvas.transform, false);
				cell.transform.localPosition = new Vector3(startX + x * spacing, startY - y * spacing, 0);
			}
		}
	}

   

    //IMPLEMENTAR Punto 2
    int watchdog = 0;
    bool RecuSolve(Matrix<int> matrixParent, int x, int y, int protectMaxDepth, List<Matrix<int>> solution)
    {
        watchdog++;

        if (watchdog >= 10000)
        {
            Debug.LogError("WatchDogBroke");
            return false;
        }

        if (_board[x, y].locked)
        {
            Debug.Log($"Casiillero {x},{y} Bloqueado,entro al siguiente, su valor es {matrixParent[x,y]}");
           
            #region  Sumo Index
            x++;
            if (x >= matrixParent.Width)
            {
                x = 0;
                y++;
                if (y >= matrixParent.Height)
                {
                   
                    return true;
                }

            }
            #endregion
            
           
            Debug.Log($"entra al siguiente Casiillero, el cual es {x},{y}, ya que el anterior estaba bloqueado ");
            return RecuSolve(matrixParent, x, y,protectMaxDepth,solution);
        }

        for (int i = 1; i <= 9; i++)
        {
            if (CanPlaceValue(matrixParent, i, x, y))
            {
                //copia, no la matrix original
                matrixParent[x, y] = i;
                int auxX = x;
                int auxY = y;


                #region sumo en index
                auxX++;
                if (auxX >= matrixParent.Width)
                {
                    auxX = 0;
                    auxY = y+1;
                  
                    if (auxY >= matrixParent.Height)
                    {
                        step++;
                        solution.Add(matrixParent.Clone());
                        return true;
                    }
                }
                #endregion

                if (RecuSolve(matrixParent, auxX, auxY, protectMaxDepth, solution))
                {
                    Debug.Log($"se puede resolver el indice{x},{y} con {i}, asi que resuelvo ese indice");
                    solution.Add(matrixParent.Clone());
                    matrixParent[auxX, auxY] = i;
                    step++;
                    return true;
                }
                else
                {
                    matrixParent[x, y] = 0; 
                }
                #region Obsolete
                //    #region  resto index

                //     x--;
                //   if (x < 0)
                //   {
                //       x = matrixParent.Width;
                //       y--;

                //       if (y < 0)
                //       {
                //            Debug.Log($"estoy abajo de todo, devuelvo falsoo");
                //            solution.Add(matrixParent.Clone());
                //            return false;
                //       }
                //        matrixParent[x, y] = 0;
                //        Debug.Log($"Entro a {x}+{y} para arreglar");
                //        return RecuSolve(matrixParent, x, y, protectMaxDepth, solution);
                //   }
                //}

                //#endregion
                #endregion
            }
            else
            {               
             Debug.Log($"no puedo posicionar {i} en {x},{y}");
                
            }

        }
        #region coment
        //chequear si el casillero esta bloqueado o no 
        //if(_board[x,y[.locked == true)
        //si es verdadero ir al sig casillero directamente

        //if(CanPlaceValue(matrixParent, valor q quiero poner, posX, posY)
        //hacer bucle para probar todos los valores q pueden ir (probar de 1 a 9 ya ta)
        //cuadno devuelva verdadero: matrixParent[x,y[ = igualar a ese numero que dio true.

        //ahora que haga lo mismo pero en el casillero de al lado.
        //RecuSolve(matrixParent,x++,y,protectMaxDepth, solution); 
        //si terminaste de recorrer todo x e y, devolver true.

        //si veo q no puedo poner ningun numero en un casillero, volver hacia atras
        //para corregir los anteriores y asi poder poner algo en el actual
        #endregion

        

        return false;
    }
    int step = 0;

  

   


    void OnAudioFilterRead(float[] array, int channels)
    {
        if(canPlayMusic)
        {
            increment = frequency * Mathf.PI / samplingF;
            for (int i = 0; i < array.Length; i++)
            {
                phase = phase + increment;
                array[i] = (float)(gain * Mathf.Sin((float)phase));
            }
        }
        
    }
    void changeFreq(int num)
    {
        frequency = 440 + num * 80;
    }

	//IMPLEMENTAR - punto 3
	IEnumerator ShowSequence(List<Matrix<int>> seq)
    {      
        for (int i = 0; i < seq.Count; i++)
        {
            feedback.text = "Pasos: " + i + "/" + (seq.Count-1) + " - " + memory + " - " + canSolve;
            TranslateAllValues(seq[i]);
            yield return new WaitForSeconds(stepDuration);
        }
        //notas
        //TranslateAllValues(seq[indice[)
        //for haciendo translateallvalues por cada indice de la lista de la solucion
        
    }

    void Update() 
    {
        if (Input.GetKeyDown(KeyCode.R) || Input.GetMouseButtonDown(1))
            SolvedSudoku();
        else if (Input.GetKeyDown(KeyCode.C) || Input.GetMouseButtonDown(0))
        {
            CreateSudoku();
            Debug.Log(_createdMatrix.Width+" " + _createdMatrix.Height);
          
            List<Matrix<int>> z = new List<Matrix<int>>();
            /*
            if (RecuSolve(_createdMatrix,0,0,0,z))
            {
                Debug.Log("se pudo resolver!");            
            }
            else
            {
                Debug.Log("No se pudo resolver");
              
            }
            */
            DebugMatrix(z[z.Count - 1]);
        }     
           
	}

	//modificar lo necesario para que funcione.
    void SolvedSudoku()
    {
        StopAllCoroutines();
        nums = new List<int>();
        var solution = new List<Matrix<int>>();
        watchdog = 100000;
        var result =false;//????
        long mem = System.GC.GetTotalMemory(true);       

        if (RecuSolve(_createdMatrix, 0, 0, 0, solution))
        {
            StartCoroutine(ShowSequence(solution));
            Debug.Log("se pudo resolver!");
        }
        
        memory = string.Format("MEM: {0:f2}MB", mem / (1024f * 1024f));
        canSolve = result ? " VALID" : " INVALID";
    }

    void CreateSudoku()
    {
        CreateNew();
        return;
        StopAllCoroutines();
        nums = new List<int>();
        canPlayMusic = false;
        ClearBoard();
        List<Matrix<int>> l = new List<Matrix<int>>();
        watchdog = 100000;
        GenerateValidLine(_createdMatrix, 0, 0);
        var result =false;
        _createdMatrix = l[0].Clone();
        LockRandomCells();
        ClearUnlocked(_createdMatrix);
        TranslateAllValues(_createdMatrix);
        long mem = System.GC.GetTotalMemory(true);
        memory = string.Format("MEM: {0:f2}MB", mem / (1024f * 1024f));
        canSolve = result ? " VALID" : " INVALID";
        feedback.text = "Pasos: " + l.Count + "/" + l.Count + " - " + memory + " - " + canSolve;
    }
	void GenerateValidLine(Matrix<int> mtx, int x, int y)
	{
		int[]aux = new int[9];
		for (int i = 0; i < 9; i++) 
		{
			aux [i] = i + 1;
		}
		int numAux = 0;
		for (int j = 0; j < aux.Length; j++) 
		{
			int r = 1 + Random.Range(j,aux.Length);
			numAux = aux [r-1];
			aux [r-1] = aux [j];
			aux [j] = numAux;
		}
		for (int k = 0; k < aux.Length; k++) 
		{
			mtx [k, 0] = aux [k];
		}
	}


	void ClearUnlocked(Matrix<int> mtx)
	{
		for (int i = 0; i < _board.Height; i++) {
			for (int j = 0; j < _board.Width; j++) {
				if (!_board [j, i].locked)
					mtx[j,i] = Cell.EMPTY;
			}
		}
	}

	void LockRandomCells()
	{
		List<Vector2> posibles = new List<Vector2> ();
		for (int i = 0; i < _board.Height; i++) {
			for (int j = 0; j < _board.Width; j++) {
				if (!_board [j, i].locked)
					posibles.Add (new Vector2(j,i));
			}
		}
		for (int k = 0; k < 82-difficulty; k++) {
			int r = Random.Range (0, posibles.Count);
			_board [(int)posibles [r].x, (int)posibles [r].y].locked = true;
			posibles.RemoveAt (r);
		}
	}

    void TranslateAllValues(Matrix<int> matrix)
    {
        for (int y = 0; y < _board.Height; y++)
        {
            for (int x = 0; x < _board.Width; x++)
            {
                _board[x, y].number = matrix[x, y];
            }
        }
    }

    void TranslateSpecific(int value, int x, int y)
    {
        _board[x, y].number = value;
    }

    void TranslateRange(int x0, int y0, int xf, int yf)
    {
        for (int x = x0; x < xf; x++)
        {
            for (int y = y0; y < yf; y++)
            {
                _board[x, y].number = _createdMatrix[x, y];
            }
        }
    }
    void CreateNew()
    {
        ClearBoard();
        _createdMatrix = new Matrix<int>(Tests.validBoards[Tests.validBoards.Length-1]);
        DebugMatrix(_createdMatrix);
        LockRandomCells();
        ClearUnlocked(_createdMatrix);
      
        DebugMatrix(_createdMatrix);
    }

    void DebugMatrix(Matrix<int> z)
    {
        string debug = "";
        for (int x = 0; x < z.Width; x++)
        {
            for (int y = 0; y < z.Height; y++)
            {
                debug += z[x, y];
            }
            debug += "\n";
        }
        Debug.Log(debug);
    }
    bool CanPlaceValue(Matrix<int> mtx, int value, int x, int y)
    {
        List<int> fila = new List<int>();
        List<int> columna = new List<int>();
        List<int> area = new List<int>();
        List<int> total = new List<int>();

        Vector2 cuadrante = Vector2.zero;

        for (int i = 0; i < mtx.Height; i++)
        {
            for (int j = 0; j < mtx.Width; j++)
            {
                if (i != y && j == x) columna.Add(mtx[j, i]);
                else if(i == y && j != x) fila.Add(mtx[j,i]);
            }
        }



        cuadrante.x = (int)(x / 3);

        if (x < 3)
            cuadrante.x = 0;     
        else if (x < 6)
            cuadrante.x = 3;
        else
            cuadrante.x = 6;

        if (y < 3)
            cuadrante.y = 0;
        else if (y < 6)
            cuadrante.y = 3;
        else
            cuadrante.y = 6;
         
        area = mtx.GetRange((int)cuadrante.x, (int)cuadrante.y, (int)cuadrante.x + 3, (int)cuadrante.y + 3);
        total.AddRange(fila);
        total.AddRange(columna);
        total.AddRange(area);
        total = FilterZeros(total);

        if (total.Contains(value))
            return false;
        else
            return true;
    }


    List<int> FilterZeros(List<int> list)
    {
        List<int> aux = new List<int>();
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] != 0) aux.Add(list[i]);
        }
        return aux;
    }
}
