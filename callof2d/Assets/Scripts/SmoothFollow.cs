using UnityEngine;
using System.Collections;

public class SmoothFollow : MonoBehaviour
{
    // http://answers.unity3d.com/questions/29183/2d-camera-smooth-follow.html

    public float interpVelocity;
    public float minDistance;
    public float followDistance;
    public GameObject target;
    public Vector3 offset;
    Vector3 targetPos;

    //private GameManager GM;

    // Use this for initialization
    void Start()
    {
        //GM = GameObject.Find("GameManager").GetComponent<GameManager>();
        targetPos = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target)
        {
            Vector3 posNoZ = transform.position;
            posNoZ.z = target.transform.position.z;

            Vector3 targetDirection = (target.transform.position - posNoZ);

            interpVelocity = targetDirection.magnitude * 20f;

            targetPos = transform.position + (targetDirection.normalized * interpVelocity * Time.deltaTime);

            transform.position = Vector3.Lerp(transform.position, targetPos + offset, 0.25f);

        }
    }
}
