using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Maze : MonoBehaviour
{


    public int width;
    public int height;

    //Each cells are objects contained in a array. We have access to the cells from X and Y coordinates
    public MazeCell cellPrefab;
    public MazeCell[,] cellTab;

    public int currentx;
    public int currenty;

    //The wall also works with coordinates but we have access
    //to a specific wall through the naming system
    public Wall wallPrefab;
    private List<Wall> wallList = new List<Wall>();

    //Will be used for BackTracking
    int randomDirection;
    MazeCell previousCell;
    MazeCell currentCell;
    float delaytime = 0.01f;


    /* This method will generate the map, It's the main function of this class
     * Basically, It first creates all the cells and walls, then destroys the walls according to 
     * the chosen algorithm.
     */

    public void GenerateMap(int width, int height, string algorithm)
    {
        this.width = width;
        this.height = height;
        cellTab = new MazeCell[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GenerateCell(i, j);
            }
        }

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GenerateWallX(i, j);
                GenerateWallY(i, j);
                //Generate the last walls at the top
                GenerateWallX(width, j);
            }
            //Generate the last walls on the right
            GenerateWallY(i, height);
        }



        if (algorithm == "Recursive Backtracking")
        {
            StartCoroutine(RecursiveBackTracking());
        }

        else if(algorithm == "Kruskal's Algorithm")
        {
            StartCoroutine(Kruskal());
        }

    }


    //This method creates a cells of type MazeCell.
    /*
     * @param int i = x coordinate
     * @param int j = y coordinate
     */
    public void GenerateCell(int i, int j)
    {
        MazeCell newCell = Instantiate(cellPrefab) as MazeCell;
        cellTab[i, j] = newCell;
        newCell.name = "Cell[" + i + ";" + j + "]";
        newCell.x = i; // Set position attribute
        newCell.y = j; 
        newCell.transform.parent = transform;
        newCell.transform.localPosition = new Vector3(i - width * 0.5f + 0.5f, 0 , j - height * 0.5f + 0.5f);
    }


    //This method creates a wall facing West/East
    /*
     * @param int i = x coordinate
     * @param int j = y coordinate
     */
    public void GenerateWallX(int i, int j)
    {
        Wall newWallX = Instantiate(wallPrefab) as Wall;
        float x = i - 0.5f;
        float y = j;


        newWallX.x = x;
        newWallX.y = y;
        newWallX.name = "Wall[" + x + ";" + y + "]";
        newWallX.transform.parent = transform;
        newWallX.transform.rotation = new Quaternion(90, 90, 0, 0);
        newWallX.transform.localPosition = new Vector3(i - width * 0.5f, 0.5f, j - height * 0.5f + 0.5f);


        
        //For Kruskal's Algorithm
        if(x > 0 && x < width - 1)
        {
            wallList.Add(newWallX);
            newWallX.GetComponent<Renderer>().material.color = Color.green;
        }
        
        if (i > 0 && i < width)
        {
            newWallX.cell1 = cellTab[i - 1, j];
            newWallX.cell2 = cellTab[i, j];
        }
        
        
        
    }


    //This method creates a wall facing North/South
    /*
     * @param int i = x coordinate
     * @param int j = y coordinate
     */

    public void GenerateWallY(int i, int j)
    {
        Wall newWallY = Instantiate(wallPrefab) as Wall;
        float x = i;
        float y = j - 0.5f;
        newWallY.x = x;
        newWallY.y = y;
        newWallY.name = "Wall[" + x  + ";" + y + "]";
        newWallY.transform.parent = transform;
        newWallY.transform.localPosition = new Vector3(i - width * 0.5f + 0.5f, 0.5f, j - height * 0.5f);

        
        //For Kruskal's Algorithm
        if (y > 0 && y < height - 1)
        {
            wallList.Add(newWallY);
            newWallY.GetComponent<Renderer>().material.color = Color.green;
        }

        
        if (j > 0 && j < height)
        {
            newWallY.cell1 = cellTab[i, j - 1];
            newWallY.cell2 = cellTab[i, j];
        }
        

       
    }




    //This function destroy a wall with the coordinate and the direction as parameters
    /*
     * @param int paramx = x coordinate;
     * @param int paramy = y coordinate;
     * @param int direction = Direction given through 1 = North; 2 = South; 3 = West; 4 = East;
     */
    public void DestroyWall(int paramx, int paramy, int direction)
    {
        float x = paramx;
        float y = paramy;

        switch(direction)
        {
            case 1: //NORTH
                y = y + 0.5f;
                break;
            case 2: //SOUTH
                y = y - 0.5f;
                break;
            case 3: //WEST
                x = x - 0.5f;
                break;
            case 4: //EAST
                x = x + 0.5f;
                break;
        }
        string nametofind = "Wall[" + x + ";" + y + "]";
        Destroy(GameObject.Find(nametofind));
            
    }


    /* This method use Recursive Backtracking in order 
     * to create a maze.
     */
    public IEnumerator RecursiveBackTracking()
    {
        int directionchoice;
        WaitForSeconds delay = new WaitForSeconds(delaytime);
        /*
         * Here I select a starting point for the algorithm
         */
        currentCell = cellTab[Random.Range(0, width), Random.Range(0, height)];
        currentCell.visitcell();
        currentx = currentCell.x;
        currenty = currentCell.y;

        do
        {
            yield return delay;
            directionchoice = Checkdirections();

            if (directionchoice == 0)
            {
                backup();
            }
            else
                destroyAndChange(directionchoice);
        } while (currentCell.previousDirection != 0);
 


      

    }


    /* This method destroys a wall then change currentCell to continue the backtracking.
     * It updates the currentx & currenty attribute and the currentCell.previousDirection to save where we come from
     * It uses the current position + direction(from parameters)
     */
    public void destroyAndChange(int direction)
    {
        DestroyWall(currentx, currenty, direction);
        switch (direction)
        {

            case 1: //North
                cellTab[currentx, currenty + 1].visitcell();
                currenty = currenty + 1;
                currentCell = cellTab[currentx, currenty];
                currentCell.previousDirection = 2;
                break;

            case 2: //South
                cellTab[currentx, currenty - 1].visitcell();
                currenty = currenty - 1;
                currentCell = cellTab[currentx, currenty];
                currentCell.previousDirection = 1;
                break;

            case 3: //West
                cellTab[currentx - 1, currenty].visitcell();
                currentx = currentx - 1;
                currentCell = cellTab[currentx, currenty];
                currentCell.previousDirection = 4;
                break;

            case 4: //East
                cellTab[currentx + 1, currenty].visitcell();
                currentx = currentx + 1;
                currentCell = cellTab[currentx, currenty];
                currentCell.previousDirection = 3;
                break;
    
        }
    }
    /* This method check which direction are OK to go to. (Check out of bound and the .visited attribute);
     */
    public int Checkdirections()
    {
        List<int> directions = new List<int>();
        int randomInList;

        if (currenty + 1 < height)
        {
            if (cellTab[currentx, currenty + 1].visited == false) //North
            {
                directions.Add(1);
            }
        }
        if(currenty - 1 >= 0)
        {
            if (cellTab[currentx, currenty - 1].visited == false)  //South
            {
                directions.Add(2);
            }
        }
        if(currentx - 1 >= 0)
        {
            if (cellTab[currentx - 1, currenty].visited == false) //West
            {
                directions.Add(3);
            }
        }
        if(currentx + 1 < width)
        {
            if (cellTab[currentx + 1, currenty].visited == false) //East
            {
                directions.Add(4);
            }
        }


        if(directions.Count == 0)
        {
            return 0;
        }
        else
        {
            int randomindex = Random.Range(0, directions.Count);
            randomInList = directions[randomindex];
            return randomInList;
        }

    }

    //This method is used in backtracking, to go back one tile if we can't go further ahead.
    public void backup()
    {
        int backupdirection = currentCell.previousDirection;

        switch(backupdirection)
        {
            case 1: //North
                currenty = currenty + 1;
                currentCell = cellTab[currentx, currenty];
                break;
            case 2: //South
                currenty = currenty - 1;
                currentCell = cellTab[currentx, currenty];
                break;
            case 3: //West
                currentx = currentx - 1;
                currentCell = cellTab[currentx, currenty];
                break;
            case 4: //East
                currentx = currentx + 1;
                currentCell = cellTab[currentx, currenty];
                break;

        }
    }




    public IEnumerator Kruskal()
    {
        WaitForSeconds delay = new WaitForSeconds(delaytime);
        int random = Random.Range(0, wallList.Count);
        Wall currentwall = wallList[random];
        float currentwallx;
        float currentwally;
        string nametofind;
        int outif = 0;
        int inif = 0;

        currentwallx = currentwall.x;
        currentwally = currentwall.y;


        /* I check the 2 cells around this wall(currentwall), if the parent of the 2 cells are different, I break the wall in between them
         * Otherwise, I restart the loop until the List is empty
         */


        while (wallList.Count > 1)
        {

            if (currentwall.cell1.getFirstParent(currentwall.cell1).name != currentwall.cell2.getFirstParent(currentwall.cell2).name)
            {
                yield return delay;
                currentwall.cell1.rewriteAllParent(currentwall.cell1, currentwall.cell2.getFirstParent(currentwall.cell2));
                nametofind = "Wall[" + currentwall.x + ";" + currentwall.y + "]";
                Destroy(GameObject.Find(nametofind));

            }
            wallList.RemoveAt(random);
            random = Random.Range(0, wallList.Count);
            currentwall = wallList[random];
            Debug.Log(wallList.Count);
        }


        Debug.Log(wallList.Count);
    }





}




