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

    public static void StoreID(this GameObject obj)
    {
        idToObject.Add(obj.GetInstanceID(), obj);
    }
}