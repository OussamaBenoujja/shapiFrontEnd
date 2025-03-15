using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float sensitivityX = 15f;
    public float sensitivityY = 15f;
    public Transform playerBody;
    
    private float rotationX = 0f;
    
    void Start()
    {
        // Lock cursor to game window
        Cursor.lockState = CursorLockMode.Locked;
        
        // If no player body is assigned, try to find the parent
        if (playerBody == null)
            playerBody = transform.parent;
    }
    
    void Update()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * sensitivityX;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivityY;
        
        // Handle vertical rotation (camera)
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);
        transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        
        // Handle horizontal rotation (player body)
        if (playerBody != null)
            playerBody.Rotate(Vector3.up * mouseX);
    }
    
    void OnApplicationFocus(bool focused)
    {
        if (focused)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.None;
    }
}