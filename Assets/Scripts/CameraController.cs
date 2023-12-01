using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float speed = 3.5f;
    private float X;
    private float Y;
    [SerializeField]
    private GameObject target;
    [SerializeField]
    private float minTargetHight = 0;
    private Camera cam;
    [SerializeField]
    private LayerMask targetLayer;
    [SerializeField]
    private bool invertControls = true;

    private bool holding = false;
    private float rayDistance = 0;

    private void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, targetLayer))
            {
                Transform objectHit = hit.transform;

                if (objectHit == target.transform)
                {
                    //we hit the target
                    holding = true;
                    rayDistance = Vector3.Distance(cam.transform.position, target.transform.position);
                }
            }
        }


        if (Input.GetMouseButton(0))
        {
            if (holding)
            {
                Vector3 newPos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, rayDistance));
                target.transform.position = new Vector3(newPos.x, Mathf.Max(newPos.y, minTargetHight), newPos.z);
            }
            else
            {
                transform.Rotate(new Vector3(Input.GetAxis("Mouse Y") * speed * (invertControls ? -1 : 1), -Input.GetAxis("Mouse X") * speed * (invertControls?-1:1), 0));
                X = transform.rotation.eulerAngles.x;
                Y = transform.rotation.eulerAngles.y;
                transform.rotation = Quaternion.Euler(X, Y, 0);
            }


        }
        else
        {
            holding = false;
        }
    }
}
