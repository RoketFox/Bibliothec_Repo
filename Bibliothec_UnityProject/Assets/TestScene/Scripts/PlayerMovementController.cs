using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementController : MonoBehaviour
{
    #region Variables
    private InputMaster Actions;
    public VariableJoystick joystick;
    public TouchField field;

    public GameObject model;
    private GameObject cam;

    private Rigidbody rb;

    [Header("Gravity settings")]
    [SerializeField]
    private Transform[] groundChecks = new Transform[5];
    [SerializeField]
    private LayerMask walckbleLayers;
    [SerializeField]
    private LayerMask groundLayers;
    [SerializeField]
    private float turnSpeed = 2f;
    [SerializeField]
    private float rotationToSurfaceSpeed = 10f;
    [SerializeField]
    private float gravityForce = 9.8f;
    [SerializeField]
    private float bottomOffset = 1;
    [SerializeField]
    private float collisionRadius = 0.5f;

    Vector3 groundDir = Vector3.zero;

    [Header("Player settings")]
    [SerializeField]
    private float playerSpeed = 15f;
    [SerializeField]
    private float maxSpeed = 20;
    [SerializeField]
    private bool canJump = true;
    [SerializeField]
    private float jumpForce = 2;



    [Header("Camera settings")]
    [SerializeField]
    private float camRotSpeed = 10f;
    [SerializeField]
    private float horizontalSensitivity = 30.0f;
    [SerializeField]
    private float verticalSensitivity = 30.0f;
    Vector3 cameraRotation;

    float delta;
    #endregion

    #region Enable/Disable
    private void OnEnable()
    {
        Actions.Enable();
    }

    private void OnDisable()
    {
        Actions.Disable();
    }
    #endregion

    #region Standart Voids
    //###########################(Awake)############################
    private void Awake()
    {
        Actions = new InputMaster();

        delta = Time.deltaTime;
        groundDir = transform.up;
        cam = GameObject.FindGameObjectWithTag("MainCamera");
        groundChecks[0] = GameObject.Find("Check1").transform;
        groundChecks[1] = GameObject.Find("Check2").transform;
        groundChecks[2] = GameObject.Find("Check3").transform;
        rb = GetComponent<Rigidbody>();

        Actions.PlayerMovement.Jump.performed += _ => Jump();
    }

    //###########################(Start)############################
    void Start()
    {
        rb.useGravity = false;
        rb.freezeRotation = true;
    }

    //###########################(Update)###########################
    private void Update()
    {

    }

    //#########################(FixedUpdate)########################
    void FixedUpdate()
    {
        Look();
        Rotater();
        Gravity();
        Move();
    }
    #endregion

    #region Voids
    void Look()
    {
        Vector2 GetCameraRotation = field.TouchDist;
        float RotationX = horizontalSensitivity * GetCameraRotation.x * Time.deltaTime;
        float RotationY = verticalSensitivity * GetCameraRotation.y * Time.deltaTime;

        cameraRotation.x -= RotationY;
        cameraRotation.y += RotationX;

        if (cameraRotation.x > 85) cameraRotation.x = 85;
        if (cameraRotation.x < -85) cameraRotation.x = -85;

        cam.transform.localRotation = Quaternion.Lerp(cam.transform.localRotation, Quaternion.Euler(cameraRotation), delta * camRotSpeed);
        model.transform.localRotation = Quaternion.Lerp(model.transform.localRotation, Quaternion.Euler(0, cameraRotation.y, 0), delta * camRotSpeed);
    }

    public void Jump()
    {
        StartCoroutine(CanJump());
    }
    public bool Grounded()
    {
        Vector3 Pos = transform.position + (-transform.up * bottomOffset);
        Collider[] hitColliders = Physics.OverlapSphere(Pos, collisionRadius, groundLayers);
        if (hitColliders.Length > 0)
        {
            return true;
        }

        return false;
    }

#region Forces
    void Gravity()
    {
        rb.AddForce(-transform.up * gravityForce * delta, ForceMode.Impulse);
    }

    
    void Move()
    {
        Vector2 pos = Vector2.zero;

        if (joystick.Direction != Vector2.zero)
        {
            pos = joystick.Direction;
        }
        else
        {
            pos = Actions.PlayerMovement.Move.ReadValue<Vector2>();
        }
        rb.AddForce(model.transform.forward * pos.y * playerSpeed * delta, ForceMode.VelocityChange);
        rb.AddForce(model.transform.right * pos.x * playerSpeed * delta, ForceMode.VelocityChange);

        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }

#endregion

#region Rotater
    void Rotater()
    {
        groundDir = Vector3.Lerp(groundDir, FloorAngleCheck(), delta * rotationToSurfaceSpeed);
        RotateSelf(FloorAngleCheck(), delta, rotationToSurfaceSpeed);
        RotateMesh(delta, transform.forward, turnSpeed);
    }
    Vector3 FloorAngleCheck()
    {
        int rayRange = 10;

        RaycastHit HitFront;
        RaycastHit HitCentre;
        RaycastHit HitBack;
        RaycastHit HitRight;
        RaycastHit HitLeft;

        Physics.Raycast(groundChecks[0].position, -groundChecks[0].transform.up, out HitFront, rayRange, walckbleLayers);
        Debug.DrawRay(groundChecks[0].position, -groundChecks[0].transform.up * 10, Color.red);

        Physics.Raycast(groundChecks[1].position, -groundChecks[1].transform.up, out HitCentre, 10f, walckbleLayers);
        Debug.DrawRay(groundChecks[1].position, -groundChecks[1].transform.up * 10, Color.red);

        Physics.Raycast(groundChecks[2].position, -groundChecks[2].transform.up, out HitBack, 10f, walckbleLayers);
        Debug.DrawRay(groundChecks[2].position, -groundChecks[2].transform.up * 10, Color.red);

        Physics.Raycast(groundChecks[3].position, -groundChecks[3].transform.up, out HitRight, 10f, walckbleLayers);
        Debug.DrawRay(groundChecks[3].position, -groundChecks[3].transform.up * 10, Color.red);

        Physics.Raycast(groundChecks[4].position, -groundChecks[4].transform.up, out HitLeft, 10f, walckbleLayers);
        Debug.DrawRay(groundChecks[4].position, -groundChecks[4].transform.up * 10, Color.red);

        Vector3 HitDir = transform.up;

        if (HitFront.transform != null)
        {
            HitDir += HitFront.normal;
        }
        if (HitCentre.transform != null)
        {
            HitDir += HitCentre.normal;
        }
        if (HitBack.transform != null)
        {
            HitDir += HitBack.normal;
        }
        if (HitRight.transform != null)
        {
            HitDir += HitRight.normal;
        }
        if (HitLeft.transform != null)
        {
            HitDir += HitLeft.normal;
        }

        return HitDir.normalized;
    }
    void RotateSelf(Vector3 Direction, float delta, float rotSpeed)
    {
        Vector3 LerpDir = Vector3.Lerp(transform.up, Direction, delta * rotSpeed);
        transform.rotation = Quaternion.FromToRotation(transform.up, LerpDir) * transform.rotation;
    }
    void RotateMesh(float delta, Vector3 LookDiract, float speed)
    {
        Quaternion SlerpRot = Quaternion.LookRotation(LookDiract, transform.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, SlerpRot, speed * delta);
    }
#endregion
#endregion

#region Coroutines

    IEnumerator CanJump()
    {
        if(Grounded())
        {
            rb.AddForce(model.transform.up * jumpForce, ForceMode.Impulse);
        }

        yield return null;
    }
    #endregion

}
