using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Environment : MonoBehaviour
{
    public int voxage;

    public int maxvoxage;

    int totalPopulation;

    // VARIABLES
    public Text popText;

    public Text allPopText;

    private int popCount;

    // Texture to be used as start of CA input
    public Texture2D seedImage;

    // Number of frames to run which is also the height of the CA
    public int timeEnd = 2;
    int currentFrame = 0;

    //variables for size of the 3d grid
    int width;
    int length;
    int height;

    // Array for storing voxels
    GameObject[,,] voxelGrid;

    // Reference to the voxel we are using
    public GameObject voxelPrefab;

    public GameObject aboveVox;
    // Spacing between voxels
    float spacing = 1.0f;

    //Layer Densities
    int totalAliveCells = 0;
    float layerdensity = 0;
    float[] layerDensities; // array
    private float maxlayerdensity = 0;
    private float minlayerdensity = 100000000;


    //Max Age
    int maxAge = 0;

    //Max Densities
    int maxDensity3dMO = 0;
    int maxDensity3dVN = 0;
    int maxDensity3dYT = 0;
    int maxDensity3dNR = 0;
    int maxDensity3dMT = 0;
    int maxDensity2d = 0;

    // Setup Different Game of Life Rules
    GOLRule deathrule = new GOLRule();
    GOLRule rule1 = new GOLRule();
    GOLRule rule2 = new GOLRule();
    GOLRule rule3 = new GOLRule();
    GOLRule densityrules = new GOLRule();

    //boolean switches
    //toggles pausing the game
    bool pause = false;

    private int vizmode = 0;

    // FUNCTIONS

    // Use this for initialization
    void Awake()
    {
        // Read the image width and height
        width = seedImage.width;
        length = seedImage.height;
        height = timeEnd;

        
	    rule1.setupRule(1, 1, 3, 6);
        rule2.setupRule(1, 2, 4, 5);
        deathrule.setupRule(0, 0, 0, 0);//////////////////
        
        //Layer Densities
        layerDensities = new float[timeEnd];

        // Create a new CA grid
        CreateGrid();
        SetupNeighbors3d();
        popCount = 0;

        //popText.text = "Population: " + totalAliveCells.ToString();


    }

    // Update is called once per frame
    void Update()
    {

        // Calculate the CA state, save the new state, display the CA and increment time frame
        if (currentFrame < timeEnd - 1)
        {
            if (pause == false)
            {
                // Calculate the future state of the voxels
                CalculateCA();
                // Update the voxels that are printing
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < length; j++)
                    {
                        GameObject currentVoxel = voxelGrid[i, j, 0];
                        currentVoxel.GetComponent<Voxel>().UpdateVoxel();
                    }

                }
                // Save the CA state
                SaveCA();
                AllVoxCounter();
                SetCountText();
                SetCountText2();
                //Update 3d Densities
                updateDensities3d();
                // Increment the current frame count
                currentFrame++;
            }


        }
        else
        {
            // PrintAge();
        }

        // Display the voxels
        // Display the printed voxels
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < length; j++)
            {
                for (int k = 1; k < height; k++)
                {
                    if (vizmode == 0)
                    {
                        voxelGrid[i, j, k].GetComponent<Voxel>().VoxelDisplay();
                    }
                    if (vizmode == 1)
                    {
                        voxelGrid[i, j, k].GetComponent<Voxel>().VoxelDisplayAge(maxAge);
                    }
                    if (vizmode == 2)
                    {
                        voxelGrid[i, j, k].GetComponent<Voxel>().VoxelDisplayDensitye2d(maxDensity2d);
                    }
                    if (vizmode == 3)
                    {
                        voxelGrid[i, j, k].GetComponent<Voxel>()
                            .VoxelDisplayLayerDensity(layerDensities[k], minlayerdensity, maxlayerdensity);
                    }
                    if (vizmode == 4)
                    {
                        //voxelGrid[i, j, k].GetComponent<Voxel>().VoxelDisplay();
                        //voxelGrid[i, j, k].GetComponent<Voxel>().VoxelDisplayAge(maxAge);
                        //voxelGrid[i, j, k].GetComponent<Voxel>().VoxelDisplayDensity3dMO(maxDensity3dMO);
                        //voxelGrid[i, j, k].GetComponent<Voxel>().VoxelDisplayDensity3dVN(maxDensity3dVN);
                        //voxelGrid[i, j, k].GetComponent<Voxel>().VoxelDisplayDensityedYT(maxDensity3dYT);
                        //voxelGrid[i, j, k].GetComponent<Voxel>().VoxelDisplayDensityedNR(maxDensity3dNR);
                        //voxelGrid[i, j, k].GetComponent<Voxel>().VoxelDisplayDensityedMT(maxDensity3dMT);
                        //voxelGrid[i, j, k].GetComponent<Voxel>().VoxelDisplayDensityedMT(maxDensity2d );

                    }


                }
            }
          
        }






        //   int voxel101010Density = voxelGrid[10, 10, 10].GetComponent<Voxel>().getDensity3dVN();
        //print("The voxel 10,10,10 density is:" + voxel101010Density.ToString());

        KeyPressMethod();
        if (Input.GetKeyDown(KeyCode.E))
        {
            ExportPrepare();
        }

    }

    public void KeyPressMethod()
    {

        // Spin the CA if spacebar is pressed (be careful, GPU instancing will be lost!)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (gameObject.GetComponent<ModelDisplay>() == null)
            {
                gameObject.AddComponent<ModelDisplay>();
            }
            else
            {
                Destroy(gameObject.GetComponent<ModelDisplay>());
            }
        }


        //toggle pause with "p" key
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (pause == false)
            {
                pause = true;
            }
            else
            {
                pause = false;
            }
        }

        //toggle pause with "p" key
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (vizmode <= 4)
            {
                vizmode++;
            }
            if (vizmode > 4)
            {
                vizmode = 0;
            }
        }
    }


    // Create grid function
    void CreateGrid()
    {
        // Allocate space in memory for the array
        voxelGrid = new GameObject[width, length, height];
        // Populate the array with voxels from a base image
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < length; j++)
            {
                for (int k = 0; k < height; k++)
                {
                    // Create values for the transform of the new voxel
                    Vector3 currentVoxelPos = new Vector3(i * spacing, k * spacing, j * spacing);
                    Quaternion currentVoxelRot = Quaternion.identity;
                    //create the game object of the voxel
                    GameObject currentVoxelObj = Instantiate(voxelPrefab, currentVoxelPos, currentVoxelRot);
                    //run the setupVoxel() function inside the 'Voxel' component of the voxelPrefab
                    //this sets up the instance of Voxel class inside the Voxel game object
                    currentVoxelObj.GetComponent<Voxel>().SetupVoxel(i, j, k, 1);

                    // Set the state of the voxels
                    if (k == 0)
                    {
                        // Create a new state based on the input image
                        int currentVoxelState = (int)seedImage.GetPixel(i, j).grayscale;
                        currentVoxelObj.GetComponent<Voxel>().SetState(currentVoxelState);
                    }
                    else
                    {
                        // Set the state to death
                        currentVoxelObj.GetComponent<MeshRenderer>().enabled = false;
                        currentVoxelObj.GetComponent<Voxel>().SetState(0);
                    }
                    // Save the current voxel in the voxelGrid array
                    voxelGrid[i, j, k] = currentVoxelObj;
                    // Attach the new voxel to the grid game object
                    currentVoxelObj.transform.parent = gameObject.transform;
                }
            }
        }
    }

    // Calculate CA function
    public void CalculateCA()
    {
        // Go over all the voxels stored in the voxels array
        for (int i = 1; i < width - 1; i++)
        {
            for (int j = 1; j < length - 1; j++)
            {
                GameObject currentVoxelObj = voxelGrid[i, j, 0];
                int currentVoxelState = currentVoxelObj.GetComponent<Voxel>().GetState();
                int aliveNeighbours = 0;

                // Calculate how many alive neighbours are around the current voxel
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        GameObject currentNeigbour = voxelGrid[i + x, j + y, 0];
                        int currentNeigbourState = currentNeigbour.GetComponent<Voxel>().GetState();
                        aliveNeighbours += currentNeigbourState;
                    }
                }



                aliveNeighbours -= currentVoxelState;
                ///////////////////

                Voxel frontVoxel = voxelGrid[i, j + 1, 0].GetComponent<Voxel>();
                currentVoxelObj.GetComponent<Voxel>().setVoxelFront(frontVoxel);
                int frontVoxelState = frontVoxel.GetState();

                Voxel backVoxel = voxelGrid[i, j - 1, 0].GetComponent<Voxel>();
                currentVoxelObj.GetComponent<Voxel>().setVoxelBack(backVoxel);
                int backVoxelState = backVoxel.GetState();

                Voxel leftVoxel = voxelGrid[i - 1, j, 0].GetComponent<Voxel>();
                currentVoxelObj.GetComponent<Voxel>().setVoxelLeft(leftVoxel);
                int leftVoxelState = leftVoxel.GetState();

                Voxel rightVoxel = voxelGrid[i + 1, j, 0].GetComponent<Voxel>();
                currentVoxelObj.GetComponent<Voxel>().setVoxelRight(rightVoxel);
                int rightVoxelState = rightVoxel.GetState();


                Voxel upRightVoxel = voxelGrid[i + 1, j + 1, 0].GetComponent<Voxel>();
                currentVoxelObj.GetComponent<Voxel>().setFarUpRightVoxel(upRightVoxel);
                int voxelFarUpRightState = upRightVoxel.GetState();

                Voxel upLeftVoxel = voxelGrid[i - 1, j + 1, 0].GetComponent<Voxel>();
                currentVoxelObj.GetComponent<Voxel>().setFarUpLeftVoxel(upLeftVoxel);
                int voxelFarUpLeftState = upLeftVoxel.GetState();

                Voxel dwnLeftVoxel = voxelGrid[i - 1, j - 1, 0].GetComponent<Voxel>();
                currentVoxelObj.GetComponent<Voxel>().setFarDwnLeftVoxel(dwnLeftVoxel);
                int voxelFarDwnLeftState = dwnLeftVoxel.GetState();

                Voxel dwnRightVoxel = voxelGrid[i + 1, j - 1, 0].GetComponent<Voxel>();
                currentVoxelObj.GetComponent<Voxel>().setFarDwnRightVoxel(dwnRightVoxel);
                int voxelFarDwnRightState = dwnRightVoxel.GetState();
                ////////////
                //CHANGE RULE BASED ON CONDITIONS HERE:
                GOLRule currentRule = rule1;
                //CHANGE RULE BASED ON CONDITIONS HERE:
                if (currentFrame < 20)
                {
                    currentRule = rule2;
                }
                if (currentFrame > 20)
                {
                    currentRule = rule1;
                }



                //get the instructions
                int inst0 = currentRule.getInstruction(0);
                int inst1 = currentRule.getInstruction(1);
                int inst2 = currentRule.getInstruction(2);
                int inst3 = currentRule.getInstruction(3);
                ////Set_1 of tables:
                //if (aliveNeighbours == 8)
                //{
                //    currentVoxelObj.GetComponent<Voxel>().SetFutureState(0);
                //}
                //if (aliveNeighbours > inst3 && aliveNeighbours < 8)
                //{
                //    if (voxelFarUpLeftState == 1 && frontVoxelState == 1)
                //    {
                //        currentVoxelObj.GetComponent<Voxel>().SetFutureState(1);
                //    }
                //    if (voxelFarUpRightState == 1 && frontVoxelState == 1)
                //    {
                //        currentVoxelObj.GetComponent<Voxel>().SetFutureState(1);
                //    }
                //    if (voxelFarUpRightState == 1 && frontVoxelState == 1 && voxelFarUpLeftState == 1)
                //    {
                //        currentVoxelObj.GetComponent<Voxel>().SetFutureState(0);
                //    }
                //    if (voxelFarDwnLeftState == 1 && backVoxelState == 1)
                //    {
                //        currentVoxelObj.GetComponent<Voxel>().SetFutureState(1);
                //    }
                //    if (voxelFarDwnRightState == 1 && backVoxelState == 1)
                //    {
                //        currentVoxelObj.GetComponent<Voxel>().SetFutureState(1);//for Table1_4 change to 0
                //    }                   


                //}

                //if (currentVoxelState == 1)
                //{

                //    if (currentFrame < 20)
                //    {
                //        ///It is here within the low mid density range weher we can make the most effect
                //        if (aliveNeighbours >= inst2 && aliveNeighbours < inst3)
                //        {
                //            currentVoxelObj.GetComponent<Voxel>().SetFutureState(1);//for Table1_2 change to 1
                //            dwnRightVoxel.GetComponent<Voxel>().SetFutureState(0);//for Table1_2 change to 1
                //            dwnLeftVoxel.GetComponent<Voxel>().SetFutureState(0);
                //            upLeftVoxel.GetComponent<Voxel>().SetFutureState(0);
                //            upLeftVoxel.GetComponent<Voxel>().SetFutureState(0);

                //        }
                //    }
                //    if (currentFrame > 20)
                //    {
                //        ///It is here within the low mid density range weher we can make the most effect
                //        if (aliveNeighbours >= inst0 && aliveNeighbours < inst3)
                //        {
                //            currentVoxelObj.GetComponent<Voxel>().SetFutureState(1);//for Table1_3 change to 1
                //            dwnRightVoxel.GetComponent<Voxel>().SetFutureState(0);
                //            dwnLeftVoxel.GetComponent<Voxel>().SetFutureState(0);
                //            upLeftVoxel.GetComponent<Voxel>().SetFutureState(0);
                //            upLeftVoxel.GetComponent<Voxel>().SetFutureState(0);//for Table1_3 change to 1


                //        }
                //    }

                //}
                //if (currentVoxelState == 0)
                //{

                //    if (currentFrame < 20)
                //    {
                //        ///It is here within the low mid density range weher we can make the most effect
                //        if (aliveNeighbours >= inst2 && aliveNeighbours < inst3)
                //        {
                //            currentVoxelObj.GetComponent<Voxel>().SetFutureState(1);


                //            dwnRightVoxel.GetComponent<Voxel>().SetFutureState(0);
                //            dwnLeftVoxel.GetComponent<Voxel>().SetFutureState(0);
                //            upLeftVoxel.GetComponent<Voxel>().SetFutureState(0);
                //            upLeftVoxel.GetComponent<Voxel>().SetFutureState(0);

                //        }
                //    }
                //    if (currentFrame > 20)
                //    {
                //        ///It is here within the low mid density range weher we can make the most effect
                //        if (aliveNeighbours >= inst0 && aliveNeighbours < inst3)
                //        {
                //            currentVoxelObj.GetComponent<Voxel>().SetFutureState(1);

                //        }
                //    }

                //}
                //if (aliveNeighbours < inst0)
                //{
                //    currentVoxelObj.GetComponent<Voxel>().SetFutureState(0);

                //}
                /////Set_2 of tables:
                //if (aliveNeighbours == 8)
                //{
                //    currentVoxelObj.GetComponent<Voxel>().SetFutureState(0);
                //}
                //if (aliveNeighbours > inst3 && aliveNeighbours < 8)
                //{
                //    if (voxelFarUpLeftState == 1 && frontVoxelState == 1)
                //    {
                //        currentVoxelObj.GetComponent<Voxel>().SetFutureState(1);
                //    }
                //    if (voxelFarUpRightState == 1 && frontVoxelState == 1)
                //    {
                //        currentVoxelObj.GetComponent<Voxel>().SetFutureState(1);
                //    }
                //    if (voxelFarUpRightState == 1 && frontVoxelState == 1 && voxelFarUpLeftState == 1)
                //    {
                //        currentVoxelObj.GetComponent<Voxel>().SetFutureState(0);
                //    }
                //    if (voxelFarDwnLeftState == 1 && backVoxelState == 1)
                //    {
                //        currentVoxelObj.GetComponent<Voxel>().SetFutureState(1);
                //    }
                //    if (voxelFarDwnRightState == 1 && backVoxelState == 1)
                //    {
                //        currentVoxelObj.GetComponent<Voxel>().SetFutureState(1);
                //    }

                //}
                //if (currentFrame < 20)
                //{
                //    ///It is here within the low mid density range weher we can make the most effect
                //    if (aliveNeighbours >= inst2 && aliveNeighbours < inst3)
                //    {
                //        currentVoxelObj.GetComponent<Voxel>().SetFutureState(1);
                //        dwnRightVoxel.GetComponent<Voxel>().SetFutureState(0);//for Table2_4 change to 1
                //        dwnLeftVoxel.GetComponent<Voxel>().SetFutureState(0);//for Table2_4 change to 1
                //        upLeftVoxel.GetComponent<Voxel>().SetFutureState(0);
                //        upLeftVoxel.GetComponent<Voxel>().SetFutureState(0);

                //    }
                //}
                //if (aliveNeighbours >= inst0 && aliveNeighbours < inst3 && currentFrame > 20)
                //{
                //    if (i % 2 == 0)
                //    {
                //        if (currentVoxelState == 1)
                //        {
                //            currentVoxelObj.GetComponent<Voxel>().SetFutureState(1);
                //            dwnRightVoxel.GetComponent<Voxel>().SetFutureState(1);
                //            dwnLeftVoxel.GetComponent<Voxel>().SetFutureState(0);//for Table2_3 change to 1
                //            upLeftVoxel.GetComponent<Voxel>().SetFutureState(0);//for Table2_3 change to 1
                //            upLeftVoxel.GetComponent<Voxel>().SetFutureState(1);
                //        }
                //        if (currentVoxelState == 0)
                //        {
                //            currentVoxelObj.GetComponent<Voxel>().SetFutureState(1);
                //            dwnRightVoxel.GetComponent<Voxel>().SetFutureState(0);//for Table2_2 change to right voxel
                //            dwnLeftVoxel.GetComponent<Voxel>().SetFutureState(0);//for Table2_2 change to left voxel
                //            upLeftVoxel.GetComponent<Voxel>().SetFutureState(0);//for Table2_2 change to front voxel
                //            upLeftVoxel.GetComponent<Voxel>().SetFutureState(0);//for Table2_2 change to back voxel
                //        }

                //    }
                //    else
                //    {
                //        currentVoxelObj.GetComponent<Voxel>().SetFutureState(0);
                //    }
                //}

                //if (aliveNeighbours < inst0)
                //{
                //    currentVoxelObj.GetComponent<Voxel>().SetFutureState(0);

                //}
                /////Set_3 of tables:3_1//3_2//3_3//3_4
                //if (aliveNeighbours == 8)
                //{
                //    currentVoxelObj.GetComponent<Voxel>().SetFutureState(0);
                //}

                //if (aliveNeighbours > inst3 && aliveNeighbours < 8)
                //{
                //    if (voxelFarUpLeftState == 1 && frontVoxelState == 1)
                //    {
                //        currentVoxelObj.GetComponent<Voxel>().SetFutureState(1);
                //    }
                //    if (voxelFarUpRightState == 1 && frontVoxelState == 1)
                //    {
                //        currentVoxelObj.GetComponent<Voxel>().SetFutureState(1);
                //    }
                //    if (voxelFarUpRightState == 1 && frontVoxelState == 1 && voxelFarUpLeftState == 1)
                //    {
                //        currentVoxelObj.GetComponent<Voxel>().SetFutureState(0);
                //    }
                //    if (voxelFarDwnLeftState == 1 && backVoxelState == 1)
                //    {
                //        currentVoxelObj.GetComponent<Voxel>().SetFutureState(1);
                //    }
                //    if (voxelFarDwnRightState == 1 && backVoxelState == 1)
                //    {
                //        currentVoxelObj.GetComponent<Voxel>().SetFutureState(1);//for table3_4 we keep the cnages of tables
                //        //3 and 2 and we change this condtion to(0) then we add another one that sets the future state of the
                //        //upper right voxel in the corner to(1)
                //    }
                //}

                //if (currentFrame < 30)
                //{
                //    ///It is here within the low mid density range weher we can make the most effect
                //    if (aliveNeighbours >= inst2 && aliveNeighbours < inst3)
                //    {
                //        currentVoxelObj.GetComponent<Voxel>().SetFutureState(1);

                //        dwnRightVoxel.GetComponent<Voxel>().SetFutureState(0);
                //        dwnLeftVoxel.GetComponent<Voxel>().SetFutureState(0);
                //        upLeftVoxel.GetComponent<Voxel>().SetFutureState(1);///for table3_2 change to 1
                //        upRightVoxel.GetComponent<Voxel>().SetFutureState(1);////for table3_3 change to 1

                //    }
                //}
                //if (currentFrame > 30)
                //{
                //    if (aliveNeighbours >= inst0 && aliveNeighbours < inst3)
                //    {
                //        if (currentVoxelState == 1)
                //        {
                //            currentVoxelObj.GetComponent<Voxel>().SetFutureState(1);

                //            dwnRightVoxel.GetComponent<Voxel>().SetFutureState(0);
                //            dwnLeftVoxel.GetComponent<Voxel>().SetFutureState(0);
                //            upLeftVoxel.GetComponent<Voxel>().SetFutureState(0);
                //            upRightVoxel.GetComponent<Voxel>().SetFutureState(0);
                //        }
                //        if (currentVoxelState == 0)
                //        {
                //            currentVoxelObj.GetComponent<Voxel>().SetFutureState(0);

                //            dwnRightVoxel.GetComponent<Voxel>().SetFutureState(1);//
                //            dwnLeftVoxel.GetComponent<Voxel>().SetFutureState(1);///
                //            upLeftVoxel.GetComponent<Voxel>().SetFutureState(1);///
                //            upRightVoxel.GetComponent<Voxel>().SetFutureState(1);///
                //        }

                //    }

                //}
                //if (aliveNeighbours < inst0)
                //{
                //    currentVoxelObj.GetComponent<Voxel>().SetFutureState(0);

                //}
                /////Set_3 of tables:3_5//3_6
                //if (aliveNeighbours == 8)
                //{
                //    currentVoxelObj.GetComponent<Voxel>().SetFutureState(0);
                //}

                //if (aliveNeighbours > inst3 && aliveNeighbours < 8)
                //{
                //    if (voxelFarUpLeftState == 1 && frontVoxelState == 1)
                //    {
                //        currentVoxelObj.GetComponent<Voxel>().SetFutureState(1);
                //    }
                //    if (voxelFarUpRightState == 1 && frontVoxelState == 1)
                //    {
                //        currentVoxelObj.GetComponent<Voxel>().SetFutureState(1);
                //    }
                //    if (voxelFarUpRightState == 1 && frontVoxelState == 1 && voxelFarUpLeftState == 1)
                //    {
                //        currentVoxelObj.GetComponent<Voxel>().SetFutureState(0);
                //    }
                //    if (voxelFarDwnLeftState == 1 && backVoxelState == 1)
                //    {
                //        currentVoxelObj.GetComponent<Voxel>().SetFutureState(1);
                //    }
                //    if (voxelFarDwnRightState == 1 && backVoxelState == 1)
                //    {
                //        currentVoxelObj.GetComponent<Voxel>().SetFutureState(0);//for table 3-5 change to 0
                //        //upRightVoxel.GetComponent<Voxel>().SetFutureState(1);
                //    }
                //}

                //if (currentFrame < 30)
                //{
                //    ///It is here within the low mid density range weher we can make the most effect
                //    if (aliveNeighbours >= inst2 && aliveNeighbours < inst3)
                //    {
                //        currentVoxelObj.GetComponent<Voxel>().SetFutureState(1);

                //        dwnRightVoxel.GetComponent<Voxel>().SetFutureState(0);
                //        dwnLeftVoxel.GetComponent<Voxel>().SetFutureState(0);
                //        upLeftVoxel.GetComponent<Voxel>().SetFutureState(1);///
                //        upRightVoxel.GetComponent<Voxel>().SetFutureState(1);////
                //        //for table 3-5 we add : leftVoxel.GetComponent<Voxel>().SetFutureState(1);
                //        ////we add also this one table 3-5: backVoxel.GetComponent<Voxel>().SetFutureState(1);

                //    }
                //}
                //if (currentFrame > 30)
                //{
                //    if (aliveNeighbours >= inst0 && aliveNeighbours < inst3)
                //    {
                //        if (currentVoxelState == 1)
                //        {
                //            currentVoxelObj.GetComponent<Voxel>().SetFutureState(1);

                //            dwnRightVoxel.GetComponent<Voxel>().SetFutureState(0);
                //            dwnLeftVoxel.GetComponent<Voxel>().SetFutureState(0);
                //            upLeftVoxel.GetComponent<Voxel>().SetFutureState(0);
                //            upRightVoxel.GetComponent<Voxel>().SetFutureState(0);
                //        }
                //        if (currentVoxelState == 0)
                //        {
                //            currentVoxelObj.GetComponent<Voxel>().SetFutureState(0);

                //            dwnRightVoxel.GetComponent<Voxel>().SetFutureState(1);//
                //            dwnLeftVoxel.GetComponent<Voxel>().SetFutureState(1);///
                //            upLeftVoxel.GetComponent<Voxel>().SetFutureState(1);///
                //            upRightVoxel.GetComponent<Voxel>().SetFutureState(1);///
                //            ///for table 3_6 we add: backVoxel.GetComponent<Voxel>().SetFutureState(1);
                //        }

                //    }

                //}
                //if (aliveNeighbours < inst0)
                //{
                //    currentVoxelObj.GetComponent<Voxel>().SetFutureState(0);

                //}
                ////Set_4 of tables:
                //if (aliveNeighbours > inst3 && aliveNeighbours < 8)
                //{
                //    currentVoxelObj.GetComponent<Voxel>().SetFutureState(0);

                //}

                //if (currentFrame < 30)//for table 3_4 we change to from 30 to 10
                //{

                //    if (i % 2 == 0)
                //    {
                //        if (aliveNeighbours > 8)
                //        {
                //            currentVoxelObj.GetComponent<Voxel>().SetFutureState(0);
                //        }
                //        if (aliveNeighbours > inst3 && aliveNeighbours < 8)
                //        {
                //            currentVoxelObj.GetComponent<Voxel>().SetFutureState(1);
                //            upRightVoxel.GetComponent<Voxel>().SetFutureState(0);
                //            dwnRightVoxel.GetComponent<Voxel>().SetFutureState(0);

                //        }
                //        if (aliveNeighbours >= inst0 && aliveNeighbours < inst3)
                //        {
                //            currentVoxelObj.GetComponent<Voxel>().SetFutureState(1);
                //            currentVoxelObj.GetComponent<Voxel>().SetAge(3);
                //            upLeftVoxel.GetComponent<Voxel>().SetFutureState(0);

                //        }
                //        if (aliveNeighbours < inst0)
                //        {
                //            currentVoxelObj.GetComponent<Voxel>().SetFutureState(0);
                //            upLeftVoxel.GetComponent<Voxel>().SetFutureState(0);
                //            dwnRightVoxel.GetComponent<Voxel>().SetFutureState(0);
                //        }

                //    }
                //}
                //if (currentFrame > 30)//for table 3_4 we change to from 30 to 10
                //{
                //    if (aliveNeighbours >= inst0 && aliveNeighbours < inst3)
                //    {
                //        if (voxelFarDwnLeftState == 1 || voxelFarDwnRightState == 1 || voxelFarUpRightState == 1 && voxelFarUpLeftState == 1)
                //        {
                //            currentVoxelObj.GetComponent<Voxel>().SetFutureState(1);//for table 5-2 we add this :
                //                                                                    // frontVoxel.GetComponent<Voxel>().SetFutureState(0);//for table 3_5 we only change to 0
                //        }
                //        if (leftVoxelState == 1 || frontVoxelState == 1)
                //        {
                //            currentVoxelObj.GetComponent<Voxel>().SetFutureState(1);//for table 3_5 we change to 0
                //        }
                //        if (leftVoxelState == 1 || backVoxelState == 1)
                //        {
                //            currentVoxelObj.GetComponent<Voxel>().SetFutureState(1);//for table 3_5 we change to 0
                //        }
                //        if (rightVoxelState == 1 || frontVoxelState == 1)
                //        {
                //            currentVoxelObj.GetComponent<Voxel>().SetFutureState(1);//for table 5_3 we add this:
                //            //dwnLeftVoxel.GetComponent<Voxel>().SetFutureState(0);//for table 3_5 we change to 0
                //        }
                //        if (rightVoxelState == 1 || backVoxelState == 1)
                //        {
                //            currentVoxelObj.GetComponent<Voxel>().SetFutureState(1);//for table 3_5 we change to 0
                //        }
                //        else
                //        {
                //            currentVoxelObj.GetComponent<Voxel>().SetFutureState(0);//for table 3_5 we add this:
                //                                                                    // upLeftVoxel.GetComponent<Voxel>().SetFutureState(1);
                //        }
                //    }
                //}
                //if (aliveNeighbours < inst0)
                //{
                //    currentVoxelObj.GetComponent<Voxel>().SetFutureState(0);
                //}
            }        
        }
    }
    // Save the CA states - this is run after the future state of all cells is calculated to update/save
    //current state on the current level
    void SaveCA()
    {

        //counter stores the number of live cells on this level and is incremented below 
        //in the for loop for each cell with a state of 1
        totalAliveCells = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < length; j++)
            {
                GameObject currentVoxelObj = voxelGrid[i, j, 0];
                int currentVoxelState = currentVoxelObj.GetComponent<Voxel>().GetState();
                // Save the voxel state
                GameObject savedVoxel = voxelGrid[i, j, currentFrame];
                savedVoxel.GetComponent<Voxel>().SetState(currentVoxelState);
                // Save the voxel age if voxel is alive
                if (currentVoxelState == 1)
                {
                    int currentVoxelAge = currentVoxelObj.GetComponent<Voxel>().GetAge();
                    savedVoxel.GetComponent<Voxel>().SetAge(currentVoxelAge);
                    totalAliveCells++;
                    //SetCountText();
                    //track oldest voxels
                    if (currentVoxelAge > maxAge)
                    {
                        maxAge = currentVoxelAge;
                    }
                }
            }
        }

        float totalcells = length * width;
        layerdensity = totalAliveCells / totalcells;

        if (layerdensity > maxlayerdensity)
        {
            maxlayerdensity = layerdensity;
        }
        //this stores the density of live cells for each entire layer of cells(each level)
        layerDensities[currentFrame] = layerdensity;

    }


    // Separate data based on density (getDensity3dMO)
    void SeparateVoxelsByDensity()
    {
        // Get all the stored desnities from the voxels
        List<int> availableDensities = new List<int>();
        for (int i = 1; i < width - 1; i++)
        {
            for (int j = 1; j < length - 1; j++)
            {
                for (int k = 1; k < height - 1; k++)
                {
                    Voxel currentVoxel = voxelGrid[i, j, k].GetComponent<Voxel>();
                    int currentVoxelDensity = currentVoxel.getDensity3dMO();
                    if (availableDensities.Contains(currentVoxelDensity) == false)
                    {
                        availableDensities.Add(currentVoxelDensity);
                    }
                }
            }
        }
    }




    void SetCountText()
    {
        // popText.text = "Current Population: " + totalAliveCells.ToString();

        allPopText.text = "Total Population: " + totalPopulation.ToString();
    }

    void SetCountText2()
    {
        // popText.text = "Current Population: " + totalAliveCells.ToString();
        popText.text = "Current Layer Population: " + totalAliveCells.ToString();

    }

    void AllVoxCounter()
    {
        totalPopulation = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < length; j++)
            {
                for (int k = 0; k < height; k++)
                {
                    GameObject populant = voxelGrid[i, j, k];
                    int populantState = populant.GetComponent<Voxel>().GetState();
                    if (populantState == 1)
                    {
                        totalPopulation++;
                    }
                }
            }
        }
    }


    /// <summary>
    /// SETUP MOORES & VON NEUMANN 3D NEIGHBORS
    /// </summary>
    void SetupNeighbors3d()
    {
        for (int i = 1; i < width - 1; i++)
        {
            for (int j = 1; j < length - 1; j++)
            {
                for (int k = 1; k < height - 1; k++)
                {
                    //the current voxel we are looking at...
                    GameObject currentVoxelObj = voxelGrid[i, j, k];

                    ////SETUP Von Neumann Neighborhood Cells////
                    Voxel[] tempNeighborsVN = new Voxel[6];
                    Voxel[] tempNeighborsYT = new Voxel[8];
                    Voxel[] tempNeighborsNR = new Voxel[3];
                    Voxel[] tempNeighborsMT = new Voxel[2];
                    Voxel[] tempNeighbor2d = new Voxel[8];
                    //left
                    //
                    //START OF MT
                    //Lilac
                    Voxel VoxelWuw = voxelGrid[i + 1, j + 1, k].GetComponent<Voxel>();
                    currentVoxelObj.GetComponent<Voxel>().setVoxelLeft(VoxelWuw);
                    tempNeighborsMT[0] = VoxelWuw;
                    //
                    //
                    //Lily2
                    Voxel VoxelLily2 = voxelGrid[i - 1, j - 1, k].GetComponent<Voxel>();
                    currentVoxelObj.GetComponent<Voxel>().setVoxelLeft(VoxelLily2);
                    tempNeighborsMT[1] = VoxelLily2;
                    //                   
                    //END OF MT
                    //
                    //
                    //START OF NR
                    //Lilac
                    Voxel VoxelLilac = voxelGrid[i - 1, j, k].GetComponent<Voxel>();
                    currentVoxelObj.GetComponent<Voxel>().setVoxelLeft(VoxelLilac);
                    tempNeighborsNR[0] = VoxelLilac;
                    //
                    //
                    //Lily
                    Voxel VoxelLily = voxelGrid[i - 1, j - 1, k].GetComponent<Voxel>();
                    currentVoxelObj.GetComponent<Voxel>().setVoxelLeft(VoxelLily);
                    tempNeighborsNR[1] = VoxelLily;
                    //
                    //
                    //Tulip
                    Voxel VoxelTulip = voxelGrid[i, j - 1, k].GetComponent<Voxel>();
                    currentVoxelObj.GetComponent<Voxel>().setVoxelLeft(VoxelTulip);
                    tempNeighborsNR[2] = VoxelTulip;
                    //
                    //END OF NR
                    //
                    //left
                    Voxel VoxelLeft = voxelGrid[i - 1, j, k].GetComponent<Voxel>();
                    currentVoxelObj.GetComponent<Voxel>().setVoxelLeft(VoxelLeft);
                    tempNeighborsVN[0] = VoxelLeft;

                    //
                    //MY top left corner
                    Voxel Voxel_1 = voxelGrid[i - 1, j, k + 1].GetComponent<Voxel>();
                    currentVoxelObj.GetComponent<Voxel>().setVoxelAbove(Voxel_1);
                    tempNeighborsYT[0] = Voxel_1;
                    //
                    //
                    // 
                    // 
                    //right
                    Voxel VoxelRight = voxelGrid[i + 1, j, k].GetComponent<Voxel>();
                    currentVoxelObj.GetComponent<Voxel>().setVoxelRight(VoxelRight);
                    tempNeighborsVN[2] = VoxelRight;

                    ///
                    //MY top right corner
                    Voxel Voxel_2 = voxelGrid[i + 1, j, k + 1].GetComponent<Voxel>();
                    currentVoxelObj.GetComponent<Voxel>().setVoxelAbove(Voxel_2);
                    tempNeighborsYT[1] = Voxel_2;
                    //

                    //back
                    Voxel VoxelBack = voxelGrid[i, j - 1, k].GetComponent<Voxel>();
                    currentVoxelObj.GetComponent<Voxel>().setVoxelBack(VoxelBack);
                    tempNeighborsVN[3] = VoxelBack;

                    ///
                    //My bottom left corner

                    Voxel Voxel_3 = voxelGrid[i - 1, j, k - 1].GetComponent<Voxel>();
                    currentVoxelObj.GetComponent<Voxel>().setVoxelAbove(Voxel_3);
                    tempNeighborsYT[2] = Voxel_3;
                    //

                    //front
                    Voxel VoxelFront = voxelGrid[i, j + 1, k].GetComponent<Voxel>();
                    currentVoxelObj.GetComponent<Voxel>().setVoxelFront(VoxelFront);
                    tempNeighborsVN[1] = VoxelFront;

                    ///
                    //My bottom left corner

                    Voxel Voxel_4 = voxelGrid[i + 1, j, k - 1].GetComponent<Voxel>();
                    currentVoxelObj.GetComponent<Voxel>().setVoxelAbove(Voxel_4);
                    tempNeighborsYT[3] = Voxel_4;
                    //

                    //below
                    Voxel VoxelBelow = voxelGrid[i, j, k - 1].GetComponent<Voxel>();
                    currentVoxelObj.GetComponent<Voxel>().setVoxelBelow(VoxelBelow);
                    tempNeighborsVN[4] = VoxelBelow;

                    //
                    //My Apple Voxel (top back corner)

                    Voxel Voxel_5 = voxelGrid[i, j + 1, k + 1].GetComponent<Voxel>();
                    currentVoxelObj.GetComponent<Voxel>().setVoxelAbove(Voxel_5);
                    tempNeighborsYT[4] = Voxel_5;

                    //above
                    Voxel VoxelAbove = voxelGrid[i, j, k + 1].GetComponent<Voxel>();
                    currentVoxelObj.GetComponent<Voxel>().setVoxelAbove(VoxelAbove);
                    tempNeighborsVN[5] = VoxelAbove;
                    //
                    //My Carrot Voxel (top front corner)

                    Voxel Voxel_6 = voxelGrid[i, j - 1, k + 1].GetComponent<Voxel>();
                    currentVoxelObj.GetComponent<Voxel>().setVoxelAbove(Voxel_6);
                    tempNeighborsYT[5] = Voxel_6;

                    //
                    //My Onion Voxel (top front corner)

                    Voxel Voxel_7 = voxelGrid[i, j - 1, k - 1].GetComponent<Voxel>();
                    currentVoxelObj.GetComponent<Voxel>().setVoxelAbove(Voxel_7);
                    tempNeighborsYT[6] = Voxel_7;

                    //
                    //My Potato Voxel (top front corner)

                    Voxel Voxel_8 = voxelGrid[i, j + 1, k - 1].GetComponent<Voxel>();
                    currentVoxelObj.GetComponent<Voxel>().setVoxelAbove(Voxel_7);
                    tempNeighborsYT[7] = Voxel_8;

                    //2d Neighborhood Voxels


                    Voxel frontVoxel = voxelGrid[i, j + 1, k].GetComponent<Voxel>();
                    currentVoxelObj.GetComponent<Voxel>().setVoxelFront(frontVoxel);
                    tempNeighbor2d[0] = frontVoxel;

                    Voxel backVoxel = voxelGrid[i, j - 1, k].GetComponent<Voxel>();
                    currentVoxelObj.GetComponent<Voxel>().setVoxelBack(backVoxel);
                    tempNeighbor2d[1] = backVoxel;

                    Voxel leftVoxel = voxelGrid[i - 1, j, k].GetComponent<Voxel>();
                    currentVoxelObj.GetComponent<Voxel>().setVoxelLeft(leftVoxel);
                    tempNeighbor2d[2] = leftVoxel;

                    Voxel rightVoxel = voxelGrid[i + 1, j, k].GetComponent<Voxel>();
                    currentVoxelObj.GetComponent<Voxel>().setVoxelRight(rightVoxel);
                    tempNeighbor2d[3] = rightVoxel;

                    Voxel upRightVoxel = voxelGrid[i + 1, j + 1, k].GetComponent<Voxel>();
                    currentVoxelObj.GetComponent<Voxel>().setFarUpRightVoxel(upRightVoxel);
                    tempNeighbor2d[4] = upRightVoxel;

                    Voxel upLeftVoxel = voxelGrid[i - 1, j + 1, k].GetComponent<Voxel>();
                    currentVoxelObj.GetComponent<Voxel>().setFarUpLeftVoxel(upLeftVoxel);
                    tempNeighbor2d[5] = upLeftVoxel;

                    Voxel dwnLeftVoxel = voxelGrid[i - 1, j - 1, k].GetComponent<Voxel>();
                    currentVoxelObj.GetComponent<Voxel>().setFarDwnLeftVoxel(dwnLeftVoxel);
                    tempNeighbor2d[6] = dwnLeftVoxel;

                    Voxel dwnRightVoxel = voxelGrid[i + 1, j - 1, k].GetComponent<Voxel>();
                    currentVoxelObj.GetComponent<Voxel>().setFarDwnRightVoxel(dwnRightVoxel);
                    tempNeighbor2d[7] = dwnRightVoxel;


                    //Set the Von Neumann Neighbors [] in this Voxel
                    currentVoxelObj.GetComponent<Voxel>().setNeighbors3dVN(tempNeighborsVN);

                    ////SETUP Moore's Neighborhood////
                    Voxel[] tempNeighborsMO = new Voxel[26];
                    //
                    //
                    //Set the Yekta Neighbors [] in the voxel
                    currentVoxelObj.GetComponent<Voxel>().setNeighbors3dYT(tempNeighborsYT);
                    //
                    //Set the Matt Neighbors [] in the voxel
                    currentVoxelObj.GetComponent<Voxel>().setNeighbors3dMT(tempNeighborsMT);
                    //
                    //Set the Noura Neighbors [] in the voxel
                    currentVoxelObj.GetComponent<Voxel>().setNeighbors3dNR(tempNeighborsNR);
                    //
                    //Set the 2d Neighbors [] in the voxel
                    currentVoxelObj.GetComponent<Voxel>().setNeighbors2d(tempNeighbor2d);

                    int tempcount = 0;
                    for (int m = -1; m < 2; m++)
                    {
                        for (int n = -1; n < 2; n++)
                        {
                            for (int p = -1; p < 2; p++)
                            {
                                if ((i + m >= 0) && (i + m < width) && (j + n >= 0) && (j + n < length) && (k + p >= 0) && (k + p < height))
                                {
                                    GameObject neighborVoxelObj = voxelGrid[i + m, j + n, k + p];
                                    if (neighborVoxelObj != currentVoxelObj)
                                    {
                                        Voxel neighborvoxel = voxelGrid[i + m, j + n, k + p].GetComponent<Voxel>();
                                        tempNeighborsMO[tempcount] = neighborvoxel;
                                        tempcount++;
                                    }
                                }
                            }
                        }
                    }
                    currentVoxelObj.GetComponent<Voxel>().setNeighbors3dMO(tempNeighborsMO);
                }
            }
        }
    }
    /// <summary>
    /// Update 3d Densities for Each Voxel
    /// </summary>
    void updateDensities3d()
    {
        for (int i = 1; i < width - 1; i++)
        {
            for (int j = 1; j < length - 1; j++)
            {
                for (int k = 1; k < currentFrame; k++)
                {
                    GameObject currentVoxelObj = voxelGrid[i, j, k];

                    //UPDATE THE VON NEUMANN NEIGHBORHOOD DENSITIES FOR EACH VOXEL//
                    Voxel[] tempNeighborsVN = currentVoxelObj.GetComponent<Voxel>().getNeighbors3dVN();
                    int alivecount = 0;
                    foreach (Voxel vox in tempNeighborsVN)
                    {
                        if (vox.GetState() == 1)
                        {
                            alivecount++;
                        }
                    }
                    currentVoxelObj.GetComponent<Voxel>().setDensity3dVN(alivecount);
                    if (alivecount > maxDensity3dVN)
                    {
                        maxDensity3dVN = alivecount;
                    }

                    //UPDATE THE MOORES NEIGHBORHOOD DENSITIES FOR EACH VOXEL//
                    Voxel[] tempNeighborsMO = currentVoxelObj.GetComponent<Voxel>().getNeighbors3dMO();
                    alivecount = 0;
                    foreach (Voxel vox in tempNeighborsMO)
                    {
                        if (vox.GetState() == 1)
                        {
                            alivecount++;
                        }
                    }
                    currentVoxelObj.GetComponent<Voxel>().setDensity3dMO(alivecount);
                    if (alivecount > maxDensity3dMO)
                    {
                        maxDensity3dMO = alivecount;
                    }
                    //UPDATE THE Yekta NEIGHBORHOOD DENSITIES FOR EACH VOXEL//
                    Voxel[] tempNeighborsYT = currentVoxelObj.GetComponent<Voxel>().getNeighbors3dYT();
                    alivecount = 0;

                    foreach (Voxel vox in tempNeighborsYT)
                    {
                        if (vox.GetState() == 1)
                        {
                            alivecount++;
                        }
                    }
                    currentVoxelObj.GetComponent<Voxel>().setDensity3dYT(alivecount);
                    if (alivecount > maxDensity3dYT)
                    {
                        maxDensity3dYT = alivecount;
                    }
                    //UPDATE THE Noura NEIGHBORHOOD DENSITIES FOR EACH VOXEL//
                    Voxel[] tempNeighborsNR = currentVoxelObj.GetComponent<Voxel>().getNeighbors3dNR();
                    alivecount = 0;

                    foreach (Voxel vox in tempNeighborsNR)
                    {
                        if (vox.GetState() == 1)
                        {
                            alivecount++;
                        }
                    }
                    currentVoxelObj.GetComponent<Voxel>().setDensity3dNR(alivecount);
                    if (alivecount > maxDensity3dNR)
                    {
                        maxDensity3dNR = alivecount;
                    }
                    //UPDATE THE Matt NEIGHBORHOOD DENSITIES FOR EACH VOXEL//
                    Voxel[] tempNeighborsMT = currentVoxelObj.GetComponent<Voxel>().getNeighbors3dMT();
                    alivecount = 0;

                    foreach (Voxel vox in tempNeighborsMT)
                    {
                        if (vox.GetState() == 1)
                        {
                            alivecount++;
                        }
                    }
                    currentVoxelObj.GetComponent<Voxel>().setDensity3dMT(alivecount);
                    if (alivecount > maxDensity3dMT)
                    {
                        maxDensity3dMT = alivecount;
                    }

                    //UPDATE THE 2d Neighborhood densities for each voxel

                    Voxel[] tempNeighbors2d = currentVoxelObj.GetComponent<Voxel>().getNeighbors2d();
                    alivecount = 0;
                    foreach (Voxel vox in tempNeighbors2d)
                    {
                        if (vox.GetState() == 1)
                        {
                            alivecount++;
                        }
                    }
                    currentVoxelObj.GetComponent<Voxel>().setDensity2d(alivecount);
                    if (alivecount > maxDensity2d)
                    {
                        maxDensity2d = alivecount;
                    }

                }
            }
        }
    }



    /// <summary>
    /// TESTING VON NEUMANN NEIGHBORS
    /// We can look at the specific voxels above,below,left,right,front,back and color....
    /// We can get all von neumann neighbors and color
    /// </summary>
    /// 
    void VonNeumannLookup()
    {
        //color specific voxel in the grid - [1,1,1]
        GameObject voxel_1 = voxelGrid[1, 1, 1];
        voxel_1.GetComponent<Voxel>().SetState(1);
        voxel_1.GetComponent<Voxel>().VoxelDisplay(1, 0, 0);

        //color specific voxel in the grid - [10,10,10]
        GameObject voxel_2 = voxelGrid[10, 10, 10];
        voxel_2.GetComponent<Voxel>().SetState(1);
        voxel_2.GetComponent<Voxel>().VoxelDisplay(1, 0, 0);

        //get neighbor right and color green
        Voxel voxel_1right = voxel_1.GetComponent<Voxel>().getVoxelRight();
        voxel_1right.SetState(1);
        voxel_1right.VoxelDisplay(0, 1, 0);

        //get neighbor above and color green
        Voxel voxel_1above = voxel_1.GetComponent<Voxel>().getVoxelAbove();
        voxel_1above.SetState(1);
        voxel_1above.VoxelDisplay(1, 0, 1);

        //get neighbor above and color magenta
        Voxel voxel_2above = voxel_2.GetComponent<Voxel>().getVoxelAbove();
        voxel_2above.SetState(1);
        voxel_2above.VoxelDisplay(1, 0, 1);

        //get all VN neighbors of a cell and color yellow
        //color specific voxel in the grid - [12,12,12]
        GameObject voxel_3 = voxelGrid[12, 12, 12];
        Voxel[] tempVNNeighbors = voxel_3.GetComponent<Voxel>().getNeighbors3dVN();
        foreach (Voxel vox in tempVNNeighbors)
        {
            vox.SetState(1);
            vox.VoxelDisplay(1, 1, 0);
        }

    }

    /// <summary>
    /// TESTING MOORES NEIGHBORS
    /// We can look at the specific voxels above,below,left,right,front,back and color....
    /// We can get all von neumann neighbors and color
    /// </summary>
    /// 
    void MooreLookup()
    {
        //get all MO neighbors of a cell and color CYAN
        //color specific voxel in the grid - [14,14,14]
        GameObject voxel_1 = voxelGrid[14, 14, 14];
        Voxel[] tempMONeighbors = voxel_1.GetComponent<Voxel>().getNeighbors3dMO();
        foreach (Voxel vox in tempMONeighbors)
        {
            vox.SetState(1);
            vox.VoxelDisplay(0, 1, 1);
        }

    }
    void PrintAge()
    {
        for (int i = 1; i < width - 1; i++)
        {
            for (int j = 1; j < length - 1; j++)
            {
                for (int k = 1; k < height - 1; k++)
                {
                    Voxel currentVoxel = voxelGrid[i, j, k].GetComponent<Voxel>();
                    print("Voxel[" + i + "][" + j + "][" + k + "] age is: " + currentVoxel.GetAge().ToString());
                }
            }
        }
    }


    void ExportPrepare()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < length; j++)
            {
                for (int k = 0; k < height; k++)
                {
                    Voxel currentVoxel = voxelGrid[i, j, k].GetComponent<Voxel>();
                    if (currentVoxel.GetState() == 0)
                    {
                        Destroy(currentVoxel.gameObject);
                    }
                }
            }
        }
    }
}
