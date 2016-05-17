using UnityEngine;
using System.Collections;

public class SpawnPoint : MonoBehaviour
{

    public bool free;

    // Use this for initialization
    void Start()
    {
        free = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            free = false;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            free = true;
        }
    }
}
