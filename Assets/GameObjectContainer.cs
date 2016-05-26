using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PathFinding;


/*
 Holds all the game objects.
 Allows the loading of new games.
 */
public class GameObjectContainer : MonoBehaviour {

    public int gridSize = 15;

    public List<GameObject> platforms;
    public List<GameObject> movableObjects;
    public List<GameObject> unmovableObjects;
    public GameObject shadowBall;


    // Use this for initialization
    void Start () {

        platforms = new List<GameObject>();
        
        movableObjects = new List<GameObject>();

        unmovableObjects = new List<GameObject>();

        createGrid();

        addBallToCoordinate(5, 5);
        addKeyPlatformToCoordinate(5, 5);

        addBallToCoordinate(5, 2);
        addKeyPlatformToCoordinate(5, 2);

        addBallToCoordinate(5, 9);
        addKeyPlatformToCoordinate(9, 5);

        addPlayerToCoordinate(1, 8);

        addCubeToCoordinate(8, 9);

        addShadowBallToCoordinate(1, 1);

    }

    private void createGrid()
    {
        for (int i = 1; i <= gridSize; i++){
            for(int j = 1; j <= gridSize; j++)
            {
                addGridPlatformAtCoordinate(i, j);
            }
        }
    }

    private void addKeyPlatformToCoordinate(int row, int column)
    {
        // Do not attempt to add it IF it's off the grid.
        if (row <= 0 || row > gridSize || column > gridSize || column <= 0) return;

        GameObject obj = getPlatform(row, column);

        platforms.Remove(obj);
        Destroy(obj);

        int xCoordinate = Coordinate.cellToCoordinate(row);
        int yCoordinate = Coordinate.cellToCoordinate(column);
        string platformLabel = "keyplatform(" + row + ")(" + column + ")";
        addGridPlatform(xCoordinate, yCoordinate, true, platformLabel);
    }

    private void addCubeToCoordinate(int row, int column)
    {
        // Do not attempt to add it IF it's off the grid.
        if (row <= 0 || row > gridSize || column > gridSize || column <= 0) return;

        int xCoordinate = Coordinate.cellToCoordinate(row);
        int yCoordinate = Coordinate.cellToCoordinate(column);
        addCube(xCoordinate, yCoordinate);
    }

    private void addShadowBallToCoordinate(int row, int column)
    {
        // Do not attempt to add it IF it's off the grid.
        if (row <= 0 || row > gridSize || column > gridSize || column <= 0) return;

        int xCoordinate = Coordinate.cellToCoordinate(row);
        int yCoordinate = Coordinate.cellToCoordinate(column);
        addBall(xCoordinate, yCoordinate, true);
    }

    private void addBallToCoordinate(int row, int column)
    {
        // Do not attempt to add it IF it's off the grid.
        if (row <= 0 || row > gridSize || column > gridSize || column <= 0) return;

        int xCoordinate = Coordinate.cellToCoordinate(row);
        int yCoordinate = Coordinate.cellToCoordinate(column);
        addBall(xCoordinate, yCoordinate, false);
    }



    private void addCube(int x, int z)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.AddComponent<Rigidbody>();
        cube.GetComponent<BoxCollider>();
        cube.transform.position = new Vector3(x, 1, z);
        cube.name = "block";
        float scale = 2f;
        cube.transform.localScale = new Vector3(scale, scale, scale);

        unmovableObjects.Add(cube);
    }

    private void addBall(int x, int z, bool isShadowBall)
    {
        GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        
        ball.name = "ball";
        ball.GetComponent<Renderer>().material = MaterialContainer.Instance.ShadowBallMaterial;
        float scale = 2f;
        ball.transform.localScale = new Vector3(scale, scale, scale);
        ball.transform.position = new Vector3(x, 2, z);
        ball.transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

        if (isShadowBall)
        {
            ball.name = "shadowBall";
            ball.GetComponent<Renderer>().material = MaterialContainer.Instance.ShadowBallMaterial;
            ball.GetComponent<Renderer>().enabled = false;
            shadowBall = ball;
        }
        else
        {
            ball.name = "ball";
            ball.GetComponent<Renderer>().material = MaterialContainer.Instance.BallMaterial;
            movableObjects.Add(ball);
            ball.AddComponent<BallController>();
        }
        
    }

    private void addGridPlatformAtCoordinate(int row, int column)
    {
        // Do not attempt to add it IF it's off the grid.
        if (row <= 0 || row > gridSize || column > gridSize || column <= 0) return;

        int xCoordinate = Coordinate.cellToCoordinate(row);
        int yCoordinate = Coordinate.cellToCoordinate(column);
        string platformLabel = "platform(" + row + ")(" + column + ")";
        addGridPlatform(xCoordinate, yCoordinate, false, platformLabel);
    }


    private void addGridPlatform(int x, int z, bool keyPlatform, string platformLabel)
    {
        GameObject platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
        platform.name = platformLabel;

        if (keyPlatform)
        {
            platform.GetComponent<Renderer>().material = MaterialContainer.Instance.KeyFloorMaterial;
        }
        else
        {
            platform.GetComponent<Renderer>().material = MaterialContainer.Instance.FloorMaterial;
        }
        
        platform.GetComponent<BoxCollider>();
        
        float scale = 2f;

        platform.transform.position = new Vector3(x, 0, z);
        platform.transform.localScale = new Vector3(scale, scale, scale);        

        platform.AddComponent<PlatformController>();
        platforms.Add(platform);
        
    }

    public GameObject getPlatform(int row, int column)
    {
        return GameObject.Find("platform(" + row + ")(" + column + ")");
    }

    private void addPlayerToCoordinate(int row, int column)
    {
        // Do not attempt to add it IF it's off the grid.
        if (row <= 0 || row > gridSize || column > gridSize || column <= 0) return;

        int xCoordinate = Coordinate.cellToCoordinate(row);
        int yCoordinate = Coordinate.cellToCoordinate(column);
        addPlayer(xCoordinate, yCoordinate);
    }

    private void addPlayer(int x, int z)
    {
        GameObject player = GameObject.Find("player");
        //player.AddComponent<BoxCollider>();
        //player.AddComponent<Rigidbody>();
        player.transform.position = new Vector3(x, 2, z);
        player.transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        player.GetComponent<Renderer>().material = MaterialContainer.Instance.PlayerMaterial;
        float scale = 2f;
        player.name = "player";
        player.transform.localScale = new Vector3(scale, scale/2, scale);
    }

    public GameObject getPlatform(Coordinate coordinate)
    {
        return platforms.Find(x => x.name.Contains("platform(" + (coordinate.Column + 1) + ")(" + (coordinate.Row + 1) + ")"));
    }
    
}
