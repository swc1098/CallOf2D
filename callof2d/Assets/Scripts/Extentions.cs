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

    public static Dictionary<int, GameObject> idToObject = new Dictionary<int, GameObject>();

    public static int GenerateID()
    {
        int ID;

        // Generate Unique ID
        do
        {
            ID = Random.Range(1, int.MaxValue);
        } while (idToObject.ContainsKey(ID));

        return ID;
    }

    public static void StoreID(this GameObject obj, int ID)
    {
        idToObject.Add(ID, obj);   
    }

    public static void RemoveID(this GameObject obj, int ID)
    {
        idToObject.Remove(ID);
    }

}