using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Maze {

    Cell[,] mazeCells;

    /// <summary>
    /// Gets the number of columns in this maze
    /// </summary>
    public int Columns { get; private set; }
    /// <summary>
    /// Gets the number of rows in this maze
    /// </summary>
    public int Rows { get; private set; }
    /// <summary>
    /// Gets the scale in which this maze is drawn
    /// </summary>
    public float Scale { get; private set; }

    public Maze(int rows, int columns, float scale)
    {
        Rows = rows;
        Columns = columns;
        mazeCells = new Cell[Rows, Columns];

        Scale = scale;

        drawing = new MazeDrawEventHandler(Maze_Drawing);
    }

    public void Draw(GameObject runner, GameObject wall)
    {
        drawing(this, new MazeDrawEventArgs(runner, wall, Scale));
    }
    /// <summary>
    /// Gets all the cells in this maze as a simple linear collection.
    /// It simply "flattens" the maze from a multi-dimensional array to a one-dimensional list
    /// </summary>
    /// <returns></returns>
    IEnumerable<Cell> GetAllCells()
    {
        List<Cell> allCells = new List<Cell>(Rows * Columns);
        foreach (var cell in mazeCells)
        {
            allCells.Add(cell);
        }
        //for (int i = 0; i < Rows; i++)
        //{
        //    for (int j = 0; j < Columns; j++)
        //    {
        //        allCells.Add(mazeCells[i, j]);
        //    }
        //}
        return allCells;
    }
    /// <summary>
    /// Gets the cell at the specified row and column
    /// </summary>
    /// <param name="row"></param>
    /// <param name="column"></param>
    /// <returns></returns>
    public Cell GetCell(int row, int column)
    {
        return GetAllCells().FirstOrDefault(c => c.Row == row && c.Column == column);
    }
    /// <summary>
    /// Randomly picks one cell from a collection of cells
    /// </summary>
    /// <param name="cells">An optional collection to use. if null, all cells in the maze are considered.</param>
    /// <returns></returns>
    Cell GetRandomCell(IEnumerable<Cell> cells = null)
    {
        if (cells == null)
            cells = GetAllCells();

        //Random random = new Random();
        //CryptoRandom random = new CryptoRandom();
        int randomIndex = UnityEngine.Random.Range(0, cells.Count());//random.Next(cells.Count());
        return cells.ToList()[randomIndex];
    }
    /// <summary>
    /// Fills all the squares in the maze with actual cells
    /// </summary>
    void Init()
    {
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                mazeCells[i, j] =
                    new Cell(this)
                    {
                        Column = j,
                        Row = i,
                    };
            }
        }

        //make the last added cell the end cell
        mazeCells[Rows - 1, Columns - 1].ExitCell = true;
    }
    /// <summary>
    /// This is the method that actually defines the paths of the maze.
    /// </summary>
    public void Initialize()
    {
        Init();

        GetAllCells().ToList().ForEach(c => c.Visited = false);

        Stack<Cell> stack = new Stack<Cell>(Rows * Columns);
        Cell currentCell = GetRandomCell();

        stack.Push(currentCell);
        while (stack.Any())
        {
            currentCell.Visited = true;
            IEnumerable<Cell> unvisitedNeighbours = currentCell.GetNeighbours().Where(n => !n.Visited);
            if (unvisitedNeighbours.Any())
            {
                Cell temp = GetRandomCell(unvisitedNeighbours);
                currentCell.Connect(temp);
                currentCell = temp;
                stack.Push(temp);
            }
            else
            {
                currentCell = stack.Pop();
            }
        }
    }

    void Maze_Drawing(object sender, MazeDrawEventArgs e) { }
    event MazeDrawEventHandler drawing;
    public event MazeDrawEventHandler Drawing
    {
        add { drawing += value; }
        remove { drawing -= value; }
    }
}

public delegate void MazeDrawEventHandler(object sender, MazeDrawEventArgs e);

public class MazeDrawEventArgs : EventArgs
{
    public MazeDrawEventArgs(GameObject runner, GameObject wall, float scale)
    {
        Scale = scale;
        Runner = runner;
        Wall = wall;
    }

    public float Scale { get; private set; }
    public GameObject Runner { get; private set; }
    public GameObject Wall { get; private set; }
}

///// <summary>
///// This class is AWESOME.
///// This truly random num gen is what makes my maze look like a real maze.
///// http://eimagine.com/how-to-generate-better-random-numbers-in-c-net-2/
///// </summary>
//public class CryptoRandom : RandomNumberGenerator
//{
//    public static RandomNumberGenerator r;

//    public CryptoRandom()
//    {
//        r = RandomNumberGenerator.Create();
//    }

//    public override void GetBytes(byte[] data)
//    {
//        r.GetBytes(data);
//    }

//    public double NextDouble()
//    {
//        byte[] b = new byte[4];
//        r.GetBytes(b);
//        return (double)BitConverter.ToUInt32(b, 0) / UInt32.MaxValue;
//    }
//    public int Next(int minValue, int maxValue)
//    {
//        return (int)Math.Round(NextDouble() * (maxValue - minValue - 1)) + minValue;
//    }
//    public int Next()
//    {
//        return Next(0, Int32.MaxValue);
//    }
//    public int Next(int maxValue)
//    {
//        return Next(0, maxValue);
//    }
//}
