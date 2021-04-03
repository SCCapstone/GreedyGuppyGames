using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 30f;
    public float panBorderThickness= 10f;
    public float scrollSpeed = 5000f;
    public float minY = 20f;
    public float maxY = 75f;
    public float minX = -65f;
    public float maxX = 10f;
    public float minZ = 0f;
    public float maxZ = 75f;



    // Update is called once per frame
    void Update()
    {
        // move camera up
        if(Input.GetKey("w") || Input.mousePosition.y >= Screen.height - panBorderThickness)
        {
            transform.Translate(Vector3.left * panSpeed * Time.deltaTime, Space.World);
        }
        // move camera down
        if(Input.GetKey("s") || Input.mousePosition.y <= panBorderThickness)
        {
            transform.Translate(Vector3.right * panSpeed * Time.deltaTime, Space.World);
        }
        // move camera right
        if(Input.GetKey("d") || Input.mousePosition.x >= Screen.width - panBorderThickness)
        {
            transform.Translate(Vector3.forward * panSpeed * Time.deltaTime, Space.World);
        }
        // move camera left
        if(Input.GetKey("a") || Input.mousePosition.x <= panBorderThickness)
        {
            transform.Translate(Vector3.back * panSpeed * Time.deltaTime, Space.World);
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Vector3 pos = transform.position;
   
        // use scroll wheel to zoom and then clamp to set y
        pos.y -= scroll * scrollSpeed * Time.deltaTime;
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        // equations make the clamp vary based on the position of the y
        float horizontalClampLeft = (pos.y-minY)/((maxY-minY)/(maxZ/2));
        float horizontalClampRight = maxZ - (pos.y-minY)/((maxY-minY)/(maxZ/2));
        float verticalClampMin = ((pos.y-minY)/((maxY-minY)/(maxX-minX))) + minX;

        // sets the x and z
        pos.z = Mathf.Clamp(pos.z, horizontalClampLeft, horizontalClampRight);
        pos.x = Mathf.Clamp(pos.x, verticalClampMin, maxX);

        transform.position = pos;

    }
}
