using UnityEngine;
using PathFinding;
using System.Collections.Generic;

public class PlatformController : MonoBehaviour {

    PlayerController playerController;
    GameObject shadowBall;
    GameObjectContainer gameObjects;

    void Start()
    {
        playerController = GameObject.Find("Main Camera").GetComponent<PlayerController>();
        gameObjects = GameObject.Find("Main Camera").GetComponent<GameObjectContainer>();
        shadowBall = gameObjects.shadowBall;

    }

    void OnMouseOver()
    {
        GridHighlighter.Instance.clearCoordinates();

        List<GameObject> platformsToHighlight = null;

        if (GridHighlighter.Instance.BallMovementMode)
        {
            // If we are moving the ball
            // We only want to move on the ball coordinate path
            platformsToHighlight = playerController.getBallMovementModeCoordinates(GridHighlighter.Instance.BallMovementModeTargetBall.transform.position);

            // The platforms the user can move to now it is in movement mode
            GridHighlighter.Instance.setPlatforms(platformsToHighlight, MaterialContainer.Instance.FloorHighlightMaterial);

            // If the user is overing over a platform that they can move too..
            GameObject selectedPlatform = gameObjects.getPlatform(new Coordinate(this.transform.position));
            if (platformsToHighlight.Contains(selectedPlatform))
            {
                // Highlight the platform the cursor is on
                this.GetComponent<Renderer>().material = MaterialContainer.Instance.SelectedFloorMaterial;
                char dir = playerController.getDirectionFromPlayer(this.transform.position);

                // Set location
                Coordinate hoverCoordinate = new Coordinate(this.transform.position);
                Coordinate shadowBallCoordinate = new Coordinate(hoverCoordinate,dir);
                shadowBall.transform.position = shadowBallCoordinate.toVector3D(shadowBall.transform.position.y);

                // Enable shadowball
                shadowBall.SetActive(true);
                shadowBall.GetComponent<Renderer>().enabled = true;
                
                // Hide the ball the player is next to
                GridHighlighter.Instance.BallMovementModeTargetBall.GetComponent<Renderer>().enabled = false;

            }
            else
            {
                // Hide shadowball
                GridHighlighter.Instance.BallMovementModeTargetBall.GetComponent<Renderer>().enabled = true;
                shadowBall.SetActive(false);
                shadowBall.GetComponent<Renderer>().enabled = false;
                
            }
            
        }
        else
        {
            // Highlight the path
            platformsToHighlight = playerController.getProposedPath(this.transform.position);
            GridHighlighter.Instance.setPlatforms(platformsToHighlight, MaterialContainer.Instance.FloorHighlightMaterial);
            this.GetComponent<Renderer>().material = MaterialContainer.Instance.SelectedFloorMaterial;

            // Hide shadowball
            shadowBall.SetActive(false);
            shadowBall.GetComponent<Renderer>().enabled = false;
        }

    }

    void OnMouseEnter()
    {
        
    }

    void OnMouseExit()
    {
        GridHighlighter.Instance.clearCoordinates();
    }
}
