using UnityEngine;
using System.Collections;

public static class AssetManager {

    // Public Assets
    public static Sprite HealthBar = Resources.Load("Health", typeof(Sprite)) as Sprite;
    public static Material GreenMat = Resources.Load("Green", typeof(Material)) as Material;
    public static Material YellowMat = Resources.Load("Yellow", typeof(Material)) as Material;
    public static Material RedMat = Resources.Load("Red", typeof(Material)) as Material;

}
