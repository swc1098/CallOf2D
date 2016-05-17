using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//It is common to create a class to contain all of your
//extension methods. This class must be static.
public static class Extensions
{
    //Even though they are used like normal methods, extension
    //methods must be declared static. Notice that the first
    //parameter has the 'this' keyword followed by a Transform
    //variable. This variable denotes which class the extension
    //method becomes a part of.

    public static Dictionary<string, GameObject> idToObject = new Dictionary<string, GameObject>();
    public static GameManager GM = GameObject.Find("GameManager").GetComponent<GameManager>();

    public static string GenerateID()
    {
        GM.IDCount += 1;
        return GM.SocketID + GM.IDCount;
    }

    public static void StoreID(this GameObject obj, string ID)
    {
        idToObject.Add(ID, obj);   
    }

    public static void RemoveID(this GameObject obj, string ID)
    {
        idToObject.Remove(ID);
    }

}