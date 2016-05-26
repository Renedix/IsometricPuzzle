using UnityEngine;
using System.Collections;

/*
 Holds all the materials used by the game
 */
public class MaterialContainer : MonoBehaviour {

    public Material PlayerMaterial;
    public Material PlayerHighlightMaterial;

    public Material BallMaterial;
    public Material BallHighlightMaterial;
    public Material ShadowBallMaterial;

    public Material FloorMaterial;
    public Material SelectedFloorMaterial;
    public Material FloorHighlightMaterial;
    public Material KeyFloorMaterial;

    public Material DeactiveMaterial;

    public static MaterialContainer Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

}
