using UnityEngine;
using System.Collections;
using PathFinding;
using System.Collections.Generic;
using System;

public class PlayerController : MonoBehaviour {

    public static PlayerController Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    GameObject player;
    static bool movingPlayer;

    private float speed = 15.0F;
    private float startTime;
    private float journeyLength;
    private PlayerMovementPath path;

    GameObject ballToMove;
    Vector3 movingBallFromCoordinate;
    Vector3 movingBallToCoordinate;

    private Vector3 movingFromCoordinate;
    private Vector3 movingToCoordinate;

    GameObjectContainer gameObjects;
    private int gridSize;

    // Use this for initialization
    void Start () {
        player = GameObject.Find("Player");
        movingPlayer = false;

        gameObjects = GameObjectContainer.Instance;
    }
	
	void Update () {

        // Win state test
        if (gameIsComplete())
        {
            // Handle end of game
            //return;
        }

        // User left clicked
        if (Input.GetMouseButtonDown(0) && !movingPlayer)
        {
            userClickEvent();
        }

        // Handle user movement
        if (movingPlayer)
        {
            playerMovementLogic();
        }
        
    }

    private bool gameIsComplete()
    {
        if (movingPlayer)
        {
            return false;
        }

        //movable objects
        List<Coordinate> movableObjectCoordinates = new List<Coordinate>();
        gameObjects.movableObjects.ForEach(delegate(GameObject item) {
            movableObjectCoordinates.Add(new Coordinate(item.transform.position));
        });

        // If there is a key platform that doesn't have a ball on it
        if (GameObjectContainer.Instance.keyPlatformCoordinates.Exists(x => !movableObjectCoordinates.Contains(x)))
        {
            // Game is not over!
            return false;
        }

        // Game has ended.
        return true;
    }

    private void userClickEvent()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            GameObject selectedObject = hit.transform.gameObject;
            GameObject ball = null;

            // If the user selected a platform
            if (hit.transform.gameObject.name.Contains("platform"))
            {
                bool userIsMovingTheBall = false;
                List<GameObject> ballMovementCoordinates = new List<GameObject>();

                if (GridHighlighter.Instance.BallMovementMode)
                {
                    // Has the user selected a platform on the movement path?
                    ballMovementCoordinates = getBallMovementModeCoordinates(GridHighlighter.Instance.BallMovementModeTargetBall.transform.position);
                    userIsMovingTheBall = ballMovementCoordinates.Contains(selectedObject);
                }

                if (userIsMovingTheBall)
                {
                    // Will need to travel in a straight line to the target location.
                    PlayerMovementPath moveTheBallMovementPath = new PlayerMovementPath();

                    Coordinate ballCoordinate = new Coordinate(GridHighlighter.Instance.BallMovementModeTargetBall.transform.position);
                    Coordinate playerCoordinate = new Coordinate(player.transform.position);
                    char direction = playerCoordinate.getDirection(ballCoordinate);

                    Coordinate clickedCoordinate = new Coordinate(hit.transform.gameObject.transform.position);
                    Coordinate nextCoordinate = new Coordinate(playerCoordinate, direction);

                    // While the next coordinate doesn't equal the click coordinate, add to movement path
                    while (!nextCoordinate.Equals(clickedCoordinate))
                    {
                        moveTheBallMovementPath.CoordinatePath.Add(nextCoordinate);

                        nextCoordinate = new Coordinate(nextCoordinate, direction);
                    }
                    moveTheBallMovementPath.CoordinatePath.Add(clickedCoordinate);

                    queueMovement(hit.transform.gameObject.transform.position, moveTheBallMovementPath);
                }
                else
                {
                    GridHighlighter.Instance.BallMovementMode = false;

                    Coordinate platformCoordinate = new Coordinate(hit.transform.gameObject.transform.position);
                    ball = gameObjects.movableObjects.Find(x => new Coordinate(x.transform.position).Equals(platformCoordinate));

                    // If user has clicked the platform underneith the ball
                    if (ball == null)
                        queueMovement(selectedObject.transform.position);
                }

            }

            // User has clicked on a ball to move
            if (hit.transform.gameObject.name.Contains("ball"))
            {
                ball = hit.transform.gameObject;
            }

            //Maybe make generic - not just a ball

            // if the user selected a ball to move
            if (ball != null)
            {
                Coordinate ballCoordinate = new Coordinate(ball.transform.position);
                // Select the platform underneath the ball
                GameObject targetPlatform = gameObjects.getPlatform(ballCoordinate);

                if (targetPlatform != null)
                {
                    CollisionMap map = getCollisionMap(targetPlatform.transform.position);
                    // We want to find the path to the ball
                    map.setBlock(ballCoordinate, false);

                    PlayerMovementPath newPath = PathFindingLogic.ProcessSolution(map);

                    if (newPath.CoordinatePath.Count > 1)
                    {
                        GridHighlighter.Instance.BallMovementMode = false;
                        GridHighlighter.Instance.BallMovementModeTargetBall = null;

                        // Move the player to the ball
                        newPath.CoordinatePath.RemoveAt(newPath.CoordinatePath.Count - 1);
                        queueMovement(targetPlatform.transform.position, newPath);
                    }
                    else
                    {
                        // User has clicked the ball, while standing next to it
                        GridHighlighter.Instance.BallMovementMode = true;
                        GridHighlighter.Instance.BallMovementModeTargetBall = ball;
                    }

                }

            }

        }
    }

    private void playerMovementLogic()
    {
        if (GridHighlighter.Instance.BallMovementMode)
        {
            // If the player is moving and is in ball movement mode
            // Disable ball movement mode
            GridHighlighter.Instance.BallMovementMode = false;
            GridHighlighter.Instance.BallMovementModeTargetBall = null;
        }

        float distCovered = (Time.time - startTime) * speed;
        float fracJourney = distCovered / journeyLength;

        // if above 1, then the journey has complete, attempt to get the next position
        if (fracJourney > 1)
        {
            player.transform.position = movingToCoordinate;
            fracJourney = 1;
        }

        if (fracJourney == 1)
        {
            //Move to destination (first)
            if (ballToMove != null)
            {
                ballToMove.transform.position = movingBallToCoordinate;
            }

            player.transform.position = movingToCoordinate;

            moveToNewPosition();
        }
        else
        {

            // Move Ball, if there is one.
            if (ballToMove != null)
            {
                ballToMove.transform.position = Vector3.Lerp(movingBallFromCoordinate, movingBallToCoordinate, fracJourney);
            }

            player.transform.position = Vector3.Lerp(movingFromCoordinate, movingToCoordinate, fracJourney);
        }
    }


    private void queueMovement(Vector3 platformPosition, PlayerMovementPath newPath = null)
    {

        if (newPath == null)
        {
            CollisionMap map = getCollisionMap(platformPosition);
            this.path = PathFindingLogic.ProcessSolution(map);
        }
        else
        {
            this.path = newPath;
        }

        moveToNewPosition();
    }

    private CollisionMap getCollisionMap(Vector3 clickCoordinatePosition)
    {
        Coordinate playerCoordinate = new Coordinate(player.transform.position);
        Coordinate clickCoordinate = new Coordinate(clickCoordinatePosition);

        // Create a list of coordinates of objects the player cannot pass.
        List<Coordinate> blockedCoordinates = new List<Coordinate>();
        foreach (GameObject unmovableObject in gameObjects.unmovableObjects)
        {
            blockedCoordinates.Add(new Coordinate(unmovableObject.transform.position));
        }
        foreach (GameObject movableObject in gameObjects.movableObjects)
        {
            blockedCoordinates.Add(new Coordinate(movableObject.transform.position));
        }

        CollisionMap map = new CollisionMap(GameObjectContainer.Instance.gridSize, GameObjectContainer.Instance.gridSize, playerCoordinate, clickCoordinate);

        // Add coordinates to the collision map
        foreach (Coordinate coord in blockedCoordinates)
        {
            map.setBlock(coord, true);
        }

        return map;
    }

    public List<GameObject> getBallMovementModeCoordinates(Vector3 ballVector)
    {
        if (!GridHighlighter.Instance.BallMovementMode) return new List<GameObject>();

        CollisionMap map = getCollisionMap(ballVector);
        Coordinate playerCoordinate = new Coordinate(player.transform.position);
        Coordinate ballCoordinate = new Coordinate(ballVector);
        char direction = playerCoordinate.getDirection(ballCoordinate);

        List<Coordinate> availableCoordinates = new List<Coordinate>();

        Coordinate nextCoordinate = new Coordinate(ballCoordinate, direction);
        while (true)
        {
            // if the next coordinate is within boundaries AND is not blocked by something...
            if (map.getElement(nextCoordinate) != null && !map.getElement(nextCoordinate).Blocked)
            {
                availableCoordinates.Add(ballCoordinate);
                ballCoordinate = new Coordinate(ballCoordinate, direction);
                nextCoordinate = new Coordinate(nextCoordinate, direction);
            }
            else
            {
                // Player can not move anymore. exit loop
                break;
            }
        }

        List<GameObject> listOfGameObjectPlatforms = new List<GameObject>();

        availableCoordinates.ForEach(delegate(Coordinate coord)
        {
            GameObject platform = gameObjects.getPlatform(coord);
             if (platform != null)
             {
                 listOfGameObjectPlatforms.Add(platform);
             }
        });

        return listOfGameObjectPlatforms;
    }

    public List<GameObject> getProposedPath(Vector3 clickCoordinatePosition)
    {
        CollisionMap map = getCollisionMap(clickCoordinatePosition);

        Coordinate playerCoordinate = new Coordinate(player.transform.position);
        GameObject possibleBallAtClickCoordinate = gameObjects.movableObjects.Find(x => new Coordinate(x.transform.position).Equals(new Coordinate(clickCoordinatePosition)));

        if (possibleBallAtClickCoordinate != null)
        {
            map.setBlock(new Coordinate(possibleBallAtClickCoordinate.transform.position), false);
        }

        PlayerMovementPath thePath = PathFindingLogic.ProcessSolution(map);

        List<GameObject> listOfGameObjects = new List<GameObject>();

        if (thePath!= null)
        {
            listOfGameObjects.Add(gameObjects.getPlatform(playerCoordinate));
            
            while (true)
            {
                
                Coordinate nextCoordinate = thePath.getNext();
                
                if (nextCoordinate == null)
                {
                    break;
                }

                GameObject platform = gameObjects.getPlatform(nextCoordinate);

                if (platform != null)
                    listOfGameObjects.Add(platform);

            }
            
        }

        return listOfGameObjects;
    }

    private void moveToNewPosition()
    {
        movingFromCoordinate = new Coordinate(player.transform.position).toVector3D(player.transform.position.y);
        Coordinate nextCoordinate = null;
        if (this.path != null)
        {
            nextCoordinate = this.path.getNext();
            if (nextCoordinate!=null)
                movingToCoordinate = nextCoordinate.toVector3D(player.transform.position.y);
        }

        if (nextCoordinate == null)
        {
            startTime = 0;
            journeyLength = 0;
            this.path = null;
            movingPlayer = false;
            ballToMove = null;

            GridHighlighter.Instance.BallMovementMode = false;
            GridHighlighter.Instance.BallMovementModeTargetBall = null;
        }
        else
        {

            bool playerCollidesWithBall = false;
            bool playerCannotMoveDueToBallBlock = false;

            Coordinate playerCoordinate = new Coordinate(player.transform.position);

            GameObject ball = gameObjects.movableObjects.Find(x => new Coordinate(x.transform.position).Equals(nextCoordinate));
            playerCollidesWithBall = ball != null;

            if (playerCollidesWithBall)
            {
                // If we continue in this direction, will be ball collide with anything else?
                char dir = playerCoordinate.getDirection(nextCoordinate);
                Coordinate newCoord = new Coordinate(nextCoordinate, dir);

                CollisionMap map = getCollisionMap(movingToCoordinate);
                
                // if the next coordinate is blocked
                if (map.getElement(newCoord)==null || map.getElement(newCoord).Blocked)
                {
                    playerCannotMoveDueToBallBlock = true;
                }
                else
                {
                    ballToMove = ball;
                    movingBallFromCoordinate = new Coordinate(ball.transform.position).toVector3D(ball.transform.position.y);
                    movingBallToCoordinate = newCoord.toVector3D(ball.transform.position.y);
                }


            }

            if (!playerCollidesWithBall)
            {
                playerCannotMoveDueToBallBlock = false;
            }

            if (!playerCannotMoveDueToBallBlock)
            {
                startTime = Time.time;
                journeyLength = Vector3.Distance(player.transform.position, movingToCoordinate);
                movingPlayer = true;
            }
            else
            {
                // The player cannot move because it is being blocked by a ball being blocked
                // by something else!
                startTime = 0;
                journeyLength = 0;
                this.path = null;
                movingPlayer = false;

                ballToMove = null;
            }

        }
        
    }

    public char getDirectionFromPlayer(Vector3 v1)
    {
        return new Coordinate(player.transform.position).getDirection(new Coordinate(v1));
    }
    
}
