using UnityEngine;

public class RepositionCamera : MonoBehaviour
{
    Camera mainCamera;
    [SerializeField] Transform player;

    void Update()
    {
        moveCamera();
    }
    
    void Start()
    {
        mainCamera = Camera.main;
    }

    public void moveCamera()
    {
        float cameraPos = Mathf.Clamp(player.transform.position.y, 5, 100);
        
        mainCamera.transform.position = new Vector3(0, cameraPos, -10);
    }
}