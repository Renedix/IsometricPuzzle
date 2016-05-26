﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PathFinding;

public class BallController : MonoBehaviour {

    Material unhighlightedMaterial;
    public Material highlightMaterial;
    PlayerController playerController;
    GameObjectContainer gameObjects;

    void Start()
    {
        playerController = GameObject.Find("Main Camera").GetComponent<PlayerController>();
        gameObjects = GameObject.Find("Main Camera").GetComponent<GameObjectContainer>();
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
                this.GetComponent<Renderer>().material = MaterialContainer.Instance.BallHighlightMaterial;
            }

        }
        else
        {
            this.GetComponent<Renderer>().material = MaterialContainer.Instance.BallHighlightMaterial;
        }

    }

    void OnMouseEnter()
    {

    }

    void OnMouseExit()
    {
        GridHighlighter.Instance.clearCoordinates();
        this.GetComponent<Renderer>().material = MaterialContainer.Instance.BallMaterial;
    }

}
