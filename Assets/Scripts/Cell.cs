using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Cell {

	List<Cell> neighbours;

	/// <summary>
    /// Gets a value to determine if the bottom wall is still standing
    /// </summary>
    public bool BottomWall { get; private set; }
    /// <summary>
    /// Gets a list of (neighbouring) cells that are connected to this cell.
    /// Two connected cells have no wall separating them.
    /// </summary>
    public List<Cell> ConnectedCells { get; private set; }
    /// <summary>
    /// Gets the zero-based column index of this cell in the maze
    /// </summary>
    public int Column { get; internal set; }
    /// <summary>
    /// If true, this is where the maze exits/opens
    /// </summary>
    public bool ExitCell { get; internal set; }
    /// <summary>
    /// Gets a value to determine if the left wall is still standing
    /// </summary>
    public bool LeftWall { get; private set; }
    /// <summary>
    /// Gets the Maze that contains this cell
    /// </summary>
    public Maze Maze { get; private set; }
    /// <summary>
    /// Gets a value to determine if the right wall is still standing
    /// </summary>
    public bool RightWall { get; private set; }
    /// <summary>
    /// Gets the zero-based row index of this cell in the maze
    /// </summary>
    public int Row { get; internal set; }
    /// <summary>
    /// Gets a value to determine if the top wall is still standing
    /// </summary>
    public bool TopWall { get; private set; }
    /// <summary>
    /// Determines if this cell has been considered during an operation
    /// </summary>
    public bool Visited { get; internal set; }

    public Cell(Maze container)
    {
        BottomWall = LeftWall = RightWall = TopWall = true;
        Maze = container;
        Maze.Drawing -= Cell_Drawing;
        Maze.Drawing += Cell_Drawing;
    }

    /// <summary>
    /// Connects two cells together, removing the walls between them
    /// </summary>
    /// <param name="cell"></param>
    public void Connect(Cell cell)
    {
        if (ConnectedCells == null)
            ConnectedCells = new List<Cell>(4);
        if (cell.ConnectedCells == null)
            cell.ConnectedCells = new List<Cell>(4);

        //u can connect only neighbours
        if (!GetNeighbours().Contains(cell))
            return;

        this.ConnectedCells.Add(cell);
        cell.ConnectedCells.Add(this);

        //if cell is top neighbour
        if (this.Row - 1 == cell.Row)
        {
            this.TopWall = cell.BottomWall = false;
        }
        //if cell is bottom neighbour
        else if (this.Row + 1 == cell.Row)
        {
            this.BottomWall = cell.TopWall = false;
        }
        //if cell is left neighbour
        else if (this.Column - 1 == cell.Column)
        {
            this.LeftWall = cell.RightWall = false;
        }
        //if cell is right neighbour
        else if (this.Column + 1 == cell.Column)
        {
            this.RightWall = cell.LeftWall = false;
        }
    }
    /// <summary>
    /// Gets all neighbouring cells
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Cell> GetNeighbours()
    {
        if (neighbours != null)
            return neighbours;

        neighbours = new List<Cell>(4);

        Cell topCell = Maze.GetCell(Row - 1, Column);
        if (topCell != null)
            neighbours.Add(topCell);

        Cell bottomCell = Maze.GetCell(Row + 1, Column);
        if (bottomCell != null)
            neighbours.Add(bottomCell);

        Cell leftCell = Maze.GetCell(Row, Column - 1);
        if (leftCell != null)
            neighbours.Add(leftCell);

        Cell rightCell = Maze.GetCell(Row, Column + 1);
        if (rightCell != null)
            neighbours.Add(rightCell);

        return neighbours;
    }
    public override string ToString()
    {
        return string.Format("row {0}, column {1}", Row, Column);
    }

    void Cell_Drawing(object sender, MazeDrawEventArgs e)
    {
        int totalWidth = (int)(10 * e.Scale);
        int totalHeight = (int)(10 * e.Scale);
        float horiWallScale = totalWidth / Maze.Columns;
        float vertWallScale = totalHeight / Maze.Rows;
        float x = ((5 * e.Scale) - (Column * horiWallScale)) - (horiWallScale / 2);
        float z = ((Row * vertWallScale) - (5 * e.Scale));

        //create the runner in the first cell
        if (Row == 0 && Column == 0)
        {
            GameObject runner = (GameObject)GameObject.Instantiate(e.Runner);
            runner.transform.position = new Vector3(x - (horiWallScale / 2), 0, z + (vertWallScale / 2));
            runner.transform.localScale = new Vector3(0.5f * e.Scale, 0.5f * e.Scale, 0.5f * e.Scale);
        }

        //draw top wall
        if (TopWall)
        {
            GameObject wall = (GameObject)GameObject.Instantiate(e.Wall);
            wall.transform.position = new Vector3(x, 0, z);
            wall.transform.localScale = new Vector3(horiWallScale, e.Scale, 1);
        }
        //draw bottom wall
        if (BottomWall && !ExitCell)
        {
            GameObject wall = (GameObject)GameObject.Instantiate(e.Wall);
            wall.transform.position = new Vector3(x, 0, z + vertWallScale);
            wall.transform.localScale = new Vector3(horiWallScale, e.Scale, 1);
        }
        //draw left wall
        if (LeftWall)
        {
            GameObject wall = (GameObject)GameObject.Instantiate(e.Wall);
            wall.transform.position = new Vector3(x + (horiWallScale / 2), 0, z + (vertWallScale / 2));
            wall.transform.Rotate(0, 270, 0);
            wall.transform.localScale = new Vector3(vertWallScale, e.Scale, 1);
        }
        //draw right wall
        if (RightWall && !ExitCell)
        {
            GameObject wall = (GameObject)GameObject.Instantiate(e.Wall);
            wall.transform.position = new Vector3((x - horiWallScale) + (horiWallScale / 2), 0, z + (vertWallScale / 2));
            wall.transform.Rotate(0, 270, 0);
            wall.transform.localScale = new Vector3(vertWallScale, e.Scale, 1);
        }
    }
}
