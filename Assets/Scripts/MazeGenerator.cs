using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QuantumTek.QuantumUI;
using TMPro;

public class MazeGenerator : MonoBehaviour
{

    /* This class will be used to generate the map
     * It handles the inputs and the camera aswell
     * 
     * It will also be used as a GameSystem
     */

    [SerializeField] Camera mainCamera;
    [SerializeField] Camera RotateCamera;

    [SerializeField] GameObject Remy;
    [SerializeField] GameObject ExitCube;
    [SerializeField] TMP_InputField widthInput;
    [SerializeField] TMP_InputField heightInput;
    [SerializeField] QUI_OptionList algorithm;
    [SerializeField] Button CreateMazebutton;
    [SerializeField] Button Playbutton;

    [SerializeField] GameObject Ball;
    public GameObject RemyCharacter;


    public Maze mazePrefab;
    private Maze mazeInstance;

    public int width;
    public int height;
    public string algorithmchoice;

    bool firstmaze = false;
    public int max;


    private float rotationspeed = 0.001f;


    // Start is called before the first frame update
    void Start()
    {
        mainCamera.gameObject.SetActive(true);
        RotateCamera.gameObject.SetActive(false);
        CreateMazebutton.onClick.AddListener(MazeButtonOnClick);
        Playbutton.onClick.AddListener(LaunchGameOnClick);
    }

    void Update()
    {

    }

    private void MazeButtonOnClick()
    {
        if (firstmaze == false)
        {
            CreateMaze();
            firstmaze = true;
        }
        else
            Restart();
    }

    public void LaunchGameOnClick()
    {
        // if we create a maze or destroy the last maze and make another
        if (firstmaze == false)
        {
            CreateMaze();
            firstmaze = true;
        }

        //Disable the buttons and UI menu
        widthInput.transform.gameObject.SetActive(false);
        heightInput.transform.gameObject.SetActive(false);
        algorithm.transform.gameObject.SetActive(false);
        CreateMazebutton.transform.gameObject.SetActive(false);
        Playbutton.transform.gameObject.SetActive(false);

        //Change camera to overview camera (game)
        mainCamera.gameObject.SetActive(false);
        RotateCamera.gameObject.SetActive(true);
        RotateCamera.fieldOfView = max * 2;

        //Character instantiation
        RemyCharacter = Instantiate(Remy) as GameObject;

        //GameSystem here
        ExitCube.transform.position = new Vector3(mazeInstance.cellTab[0,0].transform.position.x, 0.5f, mazeInstance.cellTab[0, 0].transform.position.z);
        ExitCube.SetActive(true);
        


    }

    //Main function launching the maze creation
    //The method creating the map is GenerateMap from Maze Script.
    private void CreateMaze()
    {
 
        //Read from the UI menu
        width = int.Parse(widthInput.text);
        height = int.Parse(heightInput.text);
        algorithmchoice = algorithm.option;

        //Max is determined in order to get the camera set correctly
        if (width >= height)
            max = width;
        else
            max = height;
        //Create a maze according to the Maze class script
        Debug.Log(algorithmchoice);
        mazeInstance = Instantiate(mazePrefab) as Maze;
        mazeInstance.GenerateMap(width, height, algorithmchoice);

        //Change camera position according to the maze size
        mainCamera.transform.position = mazeInstance.transform.position;
        mainCamera.transform.position = new Vector3(transform.position.x, max, transform.position.z);

    }


    //Restart maze generation
    public void Restart()
    {
        StopAllCoroutines();
        Destroy(mazeInstance.gameObject);
        CreateMaze();
    }


    public void RelaunchMenu()
    {
        //Disable the buttons and UI menu
        widthInput.transform.gameObject.SetActive(true);
        heightInput.transform.gameObject.SetActive(true);
        algorithm.transform.gameObject.SetActive(true);
        CreateMazebutton.transform.gameObject.SetActive(true);
        Playbutton.transform.gameObject.SetActive(true);

        Destroy(RemyCharacter);
        mainCamera.gameObject.SetActive(true);
        RotateCamera.gameObject.SetActive(false);

        ExitCube.SetActive(false);

    }
}
