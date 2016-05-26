using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PathFinding;
using System;

/*
 Highlights the path on the grid.
 */
public class GridHighlighter : MonoBehaviour {

	public static GridHighlighter Instance { get; private set; }
    void Awake()
    {
        Instance = this;
    }

    private List<GameObject> activeCoordinates;
    private List<GameObject> platforms;

    void Start()
    {
        this.platforms = GetComponent<GameObjectContainer>().platforms;
    }

    public void clearCoordinates()
    {
        if (activeCoordinates!= null)
        {
            if (BallMovementMode)
            {
                //Deactivate grid elements that aren't on the movement path
                List<GameObject> platformsToDeactivate = this.platforms.FindAll(x => activeCoordinates.Contains(x));
                foreach (GameObject platformToDeactivate in platformsToDeactivate)
                {
                    platformToDeactivate.GetComponent<Renderer>().material = MaterialContainer.Instance.DeactiveMaterial;
                }
            }

            foreach (GameObject platform in activeCoordinates)
            {
                if (platform.name.Contains("keyplatform"))
                {
                    platform.GetComponent<Renderer>().material = MaterialContainer.Instance.KeyFloorMaterial;
                }
                else
                {
                    platform.GetComponent<Renderer>().material = MaterialContainer.Instance.FloorMaterial;
                }

            }
            
        }

        activeCoordinates = null;
    }
    
    public void setPlatforms(List<GameObject> platforms, Material material)
    {
        clearCoordinates();
        this.activeCoordinates = platforms;
        
        foreach (GameObject platform in platforms)
        {
            setPlatform(platform, material);
        }

    }

    public void setPlatform(GameObject platform, Material material)
    {
        if (!platform.name.Contains("keyplatform"))
        {
            platform.GetComponent<Renderer>().material = material;
        }
    }

    public bool BallMovementMode { get; set; }

    private GameObject ballMovementModeTargetBall;
    
    public GameObject BallMovementModeTargetBall
    {
        get { return ballMovementModeTargetBall; }
        set {
            // If we are removing the targetball - ensure it is shown
            if (value == null && ballMovementModeTargetBall != null)
            {
                ballMovementModeTargetBall.GetComponent<Renderer>().enabled = true;
            }
            ballMovementModeTargetBall = value;
        }
    }

}
