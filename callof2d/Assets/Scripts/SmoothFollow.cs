using UnityEngine;
using System.Collections;

public class SmoothFollow : MonoBehaviour
{
    // http://answers.unity3d.com/questions/29183/2d-camera-smooth-follow.html
    // http://stackoverflow.com/questions/27752833/unity3d-2d-camera-to-centre-on-player-but-never-exceed-map-bounds

    public float interpVelocity;
    public float minDistance;
    public float followDistance;
    public GameObject target;
    public Vector3 offset;
    Vector3 targetPos;

    private GameManager GM;
    private Rect cameraBounds;

    // Use this for initialization
    void Start()
    {
        GM = GameObject.Find("GameManager").GetComponent<GameManager>();
        targetPos = transform.position;

        SetBounds();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target)
        {
            Vector3 posNoZ = transform.position;
            posNoZ.z = target.transform.position.z;

            Vector3 targetDirection = (target.transform.position - posNoZ);

            interpVelocity = targetDirection.magnitude * 25f;

            targetPos = transform.position + (targetDirection.normalized * interpVelocity * Time.deltaTime);

            transform.position = Vector3.Lerp(transform.position, targetPos + offset, 0.25f);
            //transform.position = new Vector3 (target.transform.position.x, target.transform.position.y, transform.position.z); // Hard followw

            ConstrainBounds();
            //Debug.Log(transform.position.ToString());
        }
    }

    void SetBounds() {

        // Find the worldspace dimensions of the sprite and restrict the camera's field of vision to them

        Camera cam = GetComponent<Camera>();
        float camVertExtent = 2 * cam.orthographicSize;     // Full camera worldspace height
        camVertExtent += 2;     // Fix whitespace
        float camHorzExtent = cam.aspect * camVertExtent;   // Full camera worldspace width
        //Debug.Log(camVertExtent); 
        //Debug.Log(camHorzExtent); 

        GameObject bounds = GameObject.Find("Background");
        Rect boundsRect = bounds.GetComponent<SpriteRenderer>().sprite.rect;                // Pixel dimensions of the sprite
        float pixelConversion = bounds.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit; // Used to convert pixels to worldspace units
        //Debug.Log(bounds.transform.position.ToString());
        //Debug.Log(boundsRect.ToString()); 
        //Debug.Log(pixelConversion); 

        // -27.4, -16.9
        // 92.5, 73.4
        float boundsWidth = (boundsRect.width / pixelConversion) * bounds.transform.localScale.x;   // Full width of sprite in worldspace units
        float boundsHeight = (boundsRect.height / pixelConversion) * bounds.transform.localScale.y; // Full height of sprite in worldspace units
        //Debug.Log(boundsWidth);
        //Debug.Log(boundsHeight);

        float minX = bounds.transform.position.x - 0.5f * boundsWidth;  // Left point of sprite in worldspace
        float minY = bounds.transform.position.y - 0.5f * boundsHeight; // Bottom point of sprite in worldspace
        //Debug.Log(minX);
        //Debug.Log(minY);

        float leftCamBound = minX + camHorzExtent;      // Left point adjusted to camera worldspace width
        float bottomCamBound = minY + camVertExtent;    // Bottom point adjusted to camera worldspace height
        //Debug.Log(leftCamBound);
        //Debug.Log(bottomCamBound);

        cameraBounds = new Rect(
            leftCamBound, 
            bottomCamBound, 
            boundsWidth - 2 * camHorzExtent, 
            boundsHeight - 2 * camVertExtent);

        //Debug.Log(cameraBounds.ToString());
    }

    void ConstrainBounds()
    {
        float camX = Mathf.Clamp(transform.position.x, cameraBounds.xMin, cameraBounds.xMax);
        float camY = Mathf.Clamp(transform.position.y, cameraBounds.yMin, cameraBounds.yMax);
        
        transform.position = new Vector3(camX, camY, transform.position.z);
    }
}
