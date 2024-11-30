using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class BoundsGenerator : MonoBehaviour
{
    [SerializeField] GameObject sprinkler;
    [SerializeField] GameObject cottonCandy;
    float cottonCandyY = 0.296f;
    [SerializeField] bool on = true;
    [SerializeField] bool visualize = false;
    [SerializeField] GameObject[] fences1;
    [SerializeField] GameObject[] fences2;
    [SerializeField] GameObject[] hedges;

    [SerializeField] GameObject[] houses;
    [SerializeField] GameObject floor;
    [Serializable] public struct GameObjectData {
        public GameObject obj;
        public float mean;
        public RotationMode rotation;
        public float stdDev;
        public float houseBias;
    };

    [SerializeField] GameObjectData[] obstacles;

    public struct FreeSpaces {
        public int x;
        public int y;
    };

    public enum RotationMode {
        Random,
        NearestWall,
        House,
        Up,
        Down,
        Left,
        Right
    };

    GameObject[][] allWalls;
    GameObject[] chosenWalls;
    GameObject wall3m;
    GameObject wall2m;
    GameObject wall1m;

    Dictionary<int, GameObject> WallLengths = new Dictionary<int, GameObject>();
    
    int maxWidth = 40;
    int minWidth = 16;
    int maxLength = 50;
    int minLength = 30;

    int wPadding = 10;
    int lPadding = 10;

    float house_pos = 17.28516f;
    int length;

    GameObject root;
    List<FreeSpaces> freeSpacesList = new List<FreeSpaces>();

    GridState[,] grid;

    
    public enum GridState
    {
        EmptyAvailable,
        EmptyOuter,
        WallPadding,
        Wall,
        GameObject,
        ObjectPadding
    }

    List<int> widths = new List<int>();

    // Start is called before the first frame update
    void Start()
    {   if (on){
            hideExampleObjects();
            root = new GameObject("Root");
            root.transform.position = Vector3.zero;
            GenerateMap();
            GenerateObstacles();
            if (visualize)
            {
                VisualizeGrid();
            }
        }
    }

    void hideExampleObjects(){
        GameObject[] exampleObjects = GameObject.FindGameObjectsWithTag("Example");
        foreach (GameObject go in exampleObjects)
        {
            go.SetActive(false);
        }
    }
    void GenerateObstacles(){
        for (int i = 0; i < obstacles.Length; i++)
        {
            GameObjectData go = obstacles[i];
            // Generate two independent random numbers uniformly distributed in (0, 1)
            float u1 = Random.Range(0f,1f); // Random value between 0 and 1
            float u2 = Random.Range(0f,1f); // Another random value between 0 and 1

            // Apply the Box-Muller transform
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);

            // Scale to the desired mean and standard deviation
            double randNormal = go.mean + go.stdDev * randStdNormal;

            // Convert to an integer
            int num_instances = Math.Clamp((int)Math.Round(randNormal), 0, 50);
            for (int j = 0; j < num_instances; j++)
            { 
                FreeSpaces space = freeSpacesList[Random.Range(0, freeSpacesList.Count)];
                grid[space.x, space.y] = GridState.GameObject;
                freeSpacesList.Remove(space);

                UpdateSurroundingSquares(space.x, space.y, new GridState[]{GridState.EmptyAvailable, GridState.EmptyOuter}, GridState.ObjectPadding);

                Vector3 pos = new Vector3(space.x - maxWidth - wPadding, 0, house_pos - space.y + 3/2 + 1 + lPadding);

                Quaternion rotation = CalculateRotation(go.rotation, space);
                Instantiate(obstacles[i].obj, pos, rotation, root.transform);}            
        }

    }

    Quaternion CalculateRotation(RotationMode rotation, FreeSpaces position){

        if (rotation == RotationMode.Random)
        {
            return Quaternion.Euler(0, Random.Range(0, 360), 0);
        }
        else if (rotation == RotationMode.NearestWall)
        {
            return Quaternion.Euler(0, 0, 0);
        }
        else if (rotation == RotationMode.House)
        {
            return Quaternion.Euler(0, 0, 0);
        }
        else if (rotation == RotationMode.Up)
        {
            return Quaternion.Euler(0, 0, 0);
        }
        else if (rotation == RotationMode.Down)
        {
            return Quaternion.Euler(0, 180, 0);
        }
        else if (rotation == RotationMode.Left)
        {
            return Quaternion.Euler(0, 270, 0);
        }
        else if (rotation == RotationMode.Right)
        {
            return Quaternion.Euler(0, 90, 0);
        }
        else
        {
            return Quaternion.Euler(0, 0, 0);
        }
    }

    void VisualizeGrid(){
        
        float grid_height = -0.4f;
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                Vector3 pos = new Vector3(i - maxWidth - wPadding, grid_height, house_pos - j + 3/2 + 1 + lPadding);
                if (grid[i, j] == GridState.Wall)
                {
                    SpawnPrimitiveCube(pos, new Vector3(1, 1, 1), Color.red);
                } else if (grid[i, j] == GridState.EmptyOuter)
                {
                    SpawnPrimitiveCube(pos, new Vector3(1, 1, 1), Color.blue);
                } else if (grid[i, j] == GridState.WallPadding)
                {
                    SpawnPrimitiveCube(pos, new Vector3(1, 1, 1), Color.yellow);
                }
                else if (grid[i, j] == GridState.EmptyAvailable)
                {
                    SpawnPrimitiveCube(pos, new Vector3(1, 1, 1), Color.green);
                }
                else if (grid[i, j] == GridState.GameObject)
                {
                    SpawnPrimitiveCube(pos, new Vector3(1, 1, 1), Color.magenta);
                }
                else if (grid[i, j] == GridState.ObjectPadding)
                {
                    SpawnPrimitiveCube(pos, new Vector3(1, 1, 1), Color.cyan);
                    }

            }
        }

    }

    void SpawnPrimitiveCube(Vector3 pos, Vector3 scale, Color color, bool translucent = false){
        GameObject visCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        visCube.GetComponent<Renderer>().material.color = color;
        visCube.transform.localScale = scale;
        visCube.transform.position = pos;
        visCube.transform.localScale = new Vector3(1, 1, 1);


    }   
    void GenerateMap(){
        int chosen_len = Random.Range(minLength, maxLength);
        int chosen_width = Random.Range(minWidth, maxWidth);
        grid = new GridState[maxWidth*2 + wPadding*2, maxLength + 1 + lPadding*2];
        // fill grid with empty available
        InitializeAllWalls();
        ResetWalls();
        MakePerimiter(chosen_len, chosen_width);
        FillPadding();
        SpawnHouse();
        MakeWalls();
        UpdateGrid();
        SpawnSprinklers();
        SpawnCottonCandy();
        MakeFloor();
    }

    void SpawnSprinklers(int numSprinklers = 5){
        for (int i = 0; i < numSprinklers; i++)
        {
            FreeSpaces space = freeSpacesList[Random.Range(0, freeSpacesList.Count)];
            grid[space.x, space.y] = GridState.GameObject;
            freeSpacesList.Remove(space);

            //UpdateSurroundingSquares(space.x, space.y, new GridState[]{GridState.EmptyAvailable, GridState.EmptyOuter}, GridState.ObjectPadding);

            Vector3 pos = new Vector3(space.x - maxWidth - wPadding, 0, house_pos - space.y + 3/2 + 1 + lPadding);

            Quaternion rotation = Quaternion.Euler(-90, Random.Range(0, 360), 0);
            Instantiate(sprinkler, pos, rotation, root.transform);
        }
    }
    void SpawnCottonCandy(int numCandy = 3){
        for (int i = 0; i < numCandy; i++)
        {
            FreeSpaces space = freeSpacesList[Random.Range(0, freeSpacesList.Count)];
            grid[space.x, space.y] = GridState.GameObject;
            freeSpacesList.Remove(space);

            //UpdateSurroundingSquares(space.x, space.y, new GridState[]{GridState.EmptyAvailable, GridState.EmptyOuter}, GridState.ObjectPadding);

            Vector3 pos = new Vector3(space.x - maxWidth - wPadding, cottonCandyY, house_pos - space.y + 3/2 + 1 + lPadding);
            Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
            Instantiate(cottonCandy, pos, rotation, root.transform);
        }
    }

    // Update is called once per frame

    void UpdateGrid(){
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                if (grid[i, j] == GridState.Wall)
                {
                    UpdateSurroundingSquares(i, j, new GridState[]{GridState.EmptyAvailable, GridState.EmptyOuter}, GridState.WallPadding);
                }
            }
        }

        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                if (grid[i, j] == GridState.EmptyAvailable)
                {
                    freeSpacesList.Add(new FreeSpaces{x = i, y = j});
                }
            }
        }


    }

    void UpdateSurroundingSquares(int x, int y, GridState[] oldStates, GridState newState){
        for (int k = -1; k < 2; k++)
            {
                for (int l = -1; l < 2; l++)
                {
                    if (x + k >= 0 && x + k < grid.GetLength(0) && y + l >= 0 && y + l < grid.GetLength(1) && oldStates.Contains(grid[x + k, y + l]))
                    {
                        grid[x + k, y + l] = newState;
                    }
                }
            }
    }
    void Update()
    {
        
    }
    void InitializeAllWalls(){
        allWalls = new GameObject[3][];
        allWalls[0] = fences1;
        allWalls[1] = fences2;
        allWalls[2] = hedges;

    }

    void ResetWalls(){
        chosenWalls = allWalls[Random.Range(0, allWalls.Length)];
        wall3m = chosenWalls[0];
        wall2m = chosenWalls[1];
        wall1m = chosenWalls[2];

        WallLengths[3] = wall3m;
        WallLengths[2] = wall2m;
        WallLengths[1] = wall1m;
    }

    void SpawnHouse(){  
        int houseIndex = Random.Range(0, houses.Length);
        
        Instantiate(houses[houseIndex], new Vector3(0, 0, house_pos), Quaternion.identity, root.transform);
    }
    

    void MakePerimiter(int chosen_len, int chosen_width){
        length = chosen_len;
        int currentWidth = chosen_width;
        for (int i = 0; i < length; i++)
        {
            widths.Add(currentWidth);
            if (Random.Range(0, 3) == 0)
            {
            currentWidth += Random.Range(-3, 3);
            currentWidth = Mathf.Clamp(currentWidth, minWidth - 1, maxWidth - 1);
            }

        }
    }

    void FillPadding(){
        for (int i = 0; i < lPadding; i++)
        {
            for (int j = 0; j < maxWidth*2 + wPadding*2; j++)
            {
                grid[j, i] = GridState.EmptyOuter;
            }
        }

        for (int i = 0; i < (maxLength + lPadding*2) - (length + lPadding); i++)
        {
            for (int j = 0; j < maxWidth*2 + wPadding*2; j++)            
            {
                Debug.Log("length: " + length);
                grid[j, (maxLength + lPadding*2) - i] = GridState.EmptyOuter;
            }
        }

        // last "Screw you" clause - fill all edges with EmptyOuter
        for (int i = 0; i < maxLength + lPadding*2; i++)
        {
            grid[0, i] = GridState.EmptyOuter;
        }
    }

    void MakeWalls(){
        int offsetVal;
        int prev_width = 0;
        for (int i = 0; i < length; i += 3)
        {
            int fence_length = Mathf.Clamp(3, 1, length-i);
            if(fence_length != 3){Debug.Log("offseg: " + (3-fence_length));}
            // make cube at position i, 0, 0
            Vector3 pos1 = new Vector3(-widths[i], 0, house_pos - i - 3/2 + (3-fence_length));
            Vector3 pos2 = new Vector3(widths[i], 0, house_pos - i - 3/2 + (3-fence_length));
            // rotate identity 90 degrees
            Quaternion rot = Quaternion.Euler(0, 90, 0);

            GameObject fenceToUse = WallLengths[fence_length];
            Instantiate(fenceToUse, pos1, rot, root.transform);
            Instantiate(fenceToUse, pos2, rot, root.transform);
            
            for (int j = 0; j < fence_length; j++)
            {
                grid[widths[i] + maxWidth + wPadding, i+j + lPadding] = GridState.Wall; // TODO: crashes sometimes???
                grid[-widths[i] + maxWidth + wPadding, i+j + lPadding] = GridState.Wall;

                for (int k = 1; widths[i] + k < maxWidth + wPadding; k++)
                    {
                        grid[widths[i] + k + maxWidth + wPadding, i+j + lPadding] = GridState.EmptyOuter;
                        grid[k - 1, i+j + lPadding] = GridState.EmptyOuter;
                    }
            }


            if (prev_width != widths[i]){

                int diff = widths[i] - prev_width;
                int absDiff = Math.Abs(diff);
                if (absDiff > minWidth/2){  
                    prev_width = widths[i];
                    ConnectWalls(prev_width, house_pos - i - 3/2 + 3, i + lPadding);
                    continue;
                }
                
                int progress = 0;
                while (progress < absDiff){
                    int segmentLen = Mathf.Clamp(3, 1, absDiff - progress);
                    GameObject wallToUse = WallLengths[segmentLen];
                    int progressVal = diff > 0? progress: -progress;

                    Vector3 negPos = new Vector3(-prev_width - progressVal, 0, house_pos - i - 3/2 + 3);
                    Vector3 posPos = new Vector3(prev_width + progressVal, 0, house_pos - i - 3/2 + 3);

                    Quaternion negRot = diff>0? Quaternion.identity: Quaternion.Euler(0, 180, 0);
                    Quaternion posRot = diff<0? Quaternion.identity: Quaternion.Euler(0, 180, 0);

                    Instantiate(wallToUse, negPos, negRot, root.transform);
                    Instantiate(wallToUse, posPos, posRot, root.transform);

                    for (int j = 0; j < segmentLen; j++)
                    {
                        offsetVal = diff > 0 ? j + progress : -j -progress;
                        grid[prev_width + offsetVal + maxWidth + wPadding, i + lPadding] = GridState.Wall;
                        grid[-(prev_width + offsetVal) + maxWidth + wPadding, i + lPadding] = GridState.Wall;}
                    
                    progress += segmentLen;
                }

            }

            prev_width = widths[i];   
        }

        for (int k = 0; k < maxWidth*2 + wPadding*2; k++)
        {
            if (grid[k, length + lPadding] == GridState.EmptyAvailable)
            {
                grid[k, length + lPadding] = GridState.EmptyOuter;
            }
        }

        ConnectWalls(prev_width, house_pos - length - 3/2 + 3, length+lPadding);
    }

    void ConnectWalls(int width, float zCoord, int gridIndex){

        int currentWidth = width;

        while (currentWidth > 0)
        {
            Vector3 negPos = new Vector3(-currentWidth, 0, zCoord);
            Vector3 posPos = new Vector3(currentWidth, 0, zCoord);

            Quaternion posRot = Quaternion.identity;
            Quaternion negRot = Quaternion.Euler(0, 180, 0);

            int wallLen = Mathf.Clamp(3, 1, currentWidth);
            GameObject wallToUse = WallLengths[wallLen];

            Instantiate(wallToUse, negPos, negRot, root.transform);
            Instantiate(wallToUse, posPos, posRot, root.transform);

            currentWidth -= wallLen;

            for (int j = 0; j < wallLen; j++)
            {
                Debug.Log("gridIndex: " + gridIndex);
                grid[currentWidth + maxWidth + wPadding + j, gridIndex] = GridState.Wall;
                grid[-currentWidth + maxWidth + wPadding - j, gridIndex] = GridState.Wall;
            }
        }

    }

    void MakeFloor(){
        // floor is 20x20
        // want to cover all possible max width and length
        
        for(int i = 0; i < maxLength*2; i+=20){
            for (int j = 0; j < maxWidth*2; j+=20){
                float xpos = j + 20;
                float ypos = (house_pos * 2) - i;
                Vector3 pos = new Vector3(xpos, 0, ypos);
                Vector3 npos = new Vector3(-xpos+20, 0, ypos);
                Instantiate(floor, pos, Quaternion.identity, root.transform);
                Instantiate(floor, npos, Quaternion.identity, root.transform);
            }
        }
    }
}