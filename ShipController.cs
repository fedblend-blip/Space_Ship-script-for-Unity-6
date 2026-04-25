using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public class ShipController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 0f; // Speed of the ship
    [SerializeField] private float rotationSpeed = 90f;// Rotation speed of the ship
    [SerializeField] private float horizontalRotationLimit = 0f; // Maximum horizontal rotation angle
    [SerializeField] public Rigidbody rigidBody; // Reference to the ship's Rigidbody component
    [Header("VFX Settings")]
    public List<GameObject> vfx = new List<GameObject>(); // List to hold the bullets fired by the ship
    [Header("Landing Settings")]
    public float landingSpeed = 20f; // Speed at which the ship descends when landing
    public LayerMask ground; // Layer mask to identify the ground for landing
    public float rayDistance = 100f; // Distance for the raycast to check for ground during landing
    public float groundOffset = 1f; // Offset to keep the ship above the ground when landing
    public bool isLanding = false; // Flag to indicate if the ship is currently landing
    public Transform groundCheck;
    public float groundDistance = 0.2f;
    bool isGrounded;
    [Header("Liftoff Settings")]
    public float liftHeight = 5f;
    public float duration = 2f;
    private bool isLifted = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>(); // Get the Rigidbody component attached to the ship
        rigidBody.useGravity = true; // Enable gravity for the ship at the start
        foreach (GameObject vfx in vfx)
        {
            vfx.SetActive(false); // Deactivate all VFX GameObjects in the list at the start
        }
        
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reload the current scene when the R key is pressed
        }
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, ground); // Check if the ship is grounded by performing a sphere check at the groundCheck position with the specified radius and layer mask
        
        if (Input.GetKeyDown(KeyCode.E))
        {
           
             rigidBody.useGravity = false; // Toggle gravity on or off when the E key is pressed
             StartLift(); // Start the liftoff process when the E key is pressed
            
               
            
        }
        if (speed > 5f)
        {

            foreach (GameObject vfx in vfx)
            {
                vfx.SetActive(true); // Activate all VFX GameObjects in the list when speed exceeds 5
            }
        }
        if (speed > 0)
        {
            isLanding = false; // Set landing flag to false when speed is greater than 0
        }
        
    }
    private void ToLand()
    {
        RaycastHit hit; // Variable to store the result of the raycast
        if (Physics.Raycast(transform.position + Vector3.up * 2f, Vector3.down, out hit, rayDistance, ground) && isLanding == true)
        {
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * landingSpeed);

            // --- 2. Плавное приземление (Position) ---
            Vector3 targetPosition = hit.point + Vector3.up * groundOffset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * landingSpeed);
        }

    }
    void FixedUpdate()
    {
        Turn();
        Vector3 forwardMovement = transform.forward * speed * Time.deltaTime; // Calculate forward movement based on the ship's forward direction and input// Handle
        rigidBody.MovePosition(rigidBody.position + forwardMovement); // Move the ship using Rigidbody's MovePosition for smooth physics-based movement
        Move();
        if (Input.GetKey(KeyCode.Q) && speed == 0f)
        {
            isLanding = true; // Set landing flag to true when Q is pressed and speed is 0
            ToLand(); // Call the ToLand method to handle landing behavior when Q is pressed and speed is 0
            rigidBody.useGravity = true; // Enable gravity when landing
        }


    }
    private void Turn()
    {
        float horizontalInput = Input.GetAxis("Horizontal"); // Get horizontal input (A/D or Left/Right arrow keys)
        float verticalInput = Input.GetAxis("Vertical"); // Get vertical input (W/S or Up/Down arrow keys)
        transform.Rotate(Vector3.up, horizontalInput * horizontalRotationLimit * Time.deltaTime); // Rotate the ship around the Y-axis based on horizontal input
        transform.Rotate(Vector3.right, verticalInput * rotationSpeed * Time.deltaTime); // Rotate the ship around the X-axis based on vertical input
        transform.Rotate(Vector3.forward, horizontalInput * -rotationSpeed * Time.deltaTime); // Rotate the ship around the Z-axis based on horizontal input for banking effect
    }
    private void Move()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed += 1f; // Increase speed when Left Shift is pressed
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            speed = -5f; // Decrease speed when Left Control is pressed
            if (speed < 0f)
            {
                speed = 0f; // Ensure speed does not go below 0
            }

        }

    }

    public void StartLift()
    {
        if (!isLifted)
        {
            StartCoroutine(LiftObject());
            isLifted = true;
        }
    }


    IEnumerator LiftObject()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.up * liftHeight;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // Плавно интерполируем позицию
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null; // Ждем следующего кадра
        }

        transform.position = endPos; // Гарантируем конечную позицию
    }
}

