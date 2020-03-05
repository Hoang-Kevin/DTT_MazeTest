using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCell : MonoBehaviour
{
    public int x;
    public int y;
    public bool visited;
    public int previousDirection;

    //For Kruskal's Algorithm
    [SerializeField]public MazeCell parent;

    MazeCell()
    {
        visited = false;
        parent = this;
    }

    public void visitcell()
    {
        if (this.visited == false)
        {
            this.visited = true;
            this.GetComponentInChildren<Renderer>().material.color = Color.red;
        }
    }



    /* For Kruskal's Algorithm, we will use disjoint-set data structure
     * We input a cell, and it gives the first parent of the cell.
     * At the start, each cell is his own parent
     * @param cell = Object of type MazeCell
     */
    public MazeCell getFirstParent(MazeCell cell)
    {
        if (cell.parent == this)
        {
            return cell;
        }

        else
        {
            //Debug.Log(this.name);
            return cell.getFirstParent(cell.parent);
        }

    }

    //This method will rewrite all the parent of a group recursively.
    public void rewriteAllParent(MazeCell cell, MazeCell celllastparent)
    {

        if(cell.parent == this)
        {
            this.parent = celllastparent;
        }
        else
        {
            cell.rewriteAllParent(cell.parent, celllastparent);
            this.parent = celllastparent;
        }
    }
}
