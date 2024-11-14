using UnityEngine;

public class MazeSlider : MonoBehaviour
{
    public float slideSpeed;
    public Rigidbody2D gameCamera;
    public Rigidbody2D playerUI;
    public Transform cameraTransform;
    public Transform playerTransform;
    public Transform kucingTransform;
    public Transform suaraTransform;
    public Transform playerUITransform;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameCamera.linearVelocity = new Vector2(slideSpeed, 0);
        playerUI.linearVelocity = new Vector2(slideSpeed, 0);
    }
    // Update is called once per frame
    void Update()
    {

    }
    public void Teleport(float distance) {
        cameraTransform.position = new Vector3(cameraTransform.position.x - distance, cameraTransform.position.y, cameraTransform.position.z);
        playerTransform.position = new Vector3(playerTransform.position.x - distance, playerTransform.position.y, playerTransform.position.z);
        kucingTransform.position = new Vector3(kucingTransform.position.x - distance, kucingTransform.position.y, kucingTransform.position.z);
        suaraTransform.position = new Vector3(suaraTransform.position.x - distance, suaraTransform.position.y, suaraTransform.position.z);
        playerUITransform.position = new Vector3(playerUITransform.position.x - distance, playerUITransform.position.y, playerUITransform.position.z);
    }
}
