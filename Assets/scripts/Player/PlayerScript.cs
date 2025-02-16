using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : playerAbility
{
    [Header("Transform References")]
    [SerializeField] private Transform movementOrientation;
    [SerializeField] private Transform characterMesh;

    [Header("Movement")]
    [SerializeField] private float speed;
    [SerializeField] private float gravitationalAcceleration;
    [SerializeField] private float jumpForce;
    [Space(10.0f)]
    [SerializeField, Range(0.0f, 1.0f)] private float lookForwardThreshold;
    [SerializeField] private float lookForwardSpeed;
    [SerializeField] GameObject steppingObj;

    [Header("Physics")]
    [SerializeField] private bool isGround;
    private float horizontalInput;
    private float verticalInput;
    private Vector3 planeVelocity;
    public bool jumpFlag = false;
    public bool flyFlag = false;
    private bool runFlag = false;
    public bool attackFlag = false;
    public bool staCoolDown = false;

    private CharacterController m_characterController;
    private GroundChecker m_groundChecker;
    private Animator animator;
    private Vector3 velocity;
    private Vector3 lastFixedPosition;
    private Quaternion lastFixedRotation;
    private Vector3 nextFixedPosition;
    private Quaternion nextFixedRotation;
    
    private static PlayerScript player;
    public static PlayerScript GetInstance()
    {
        return player;
    }

    // Start is called before the first frame update
    void Start()
    {
        player = this;
        m_characterController = GetComponent<CharacterController>();
        m_groundChecker = GetComponentInChildren<GroundChecker>();
        animator = GetComponentInChildren<Animator>();
        velocity = new Vector3(0, 0, 0);
        lastFixedPosition = transform.position;
        lastFixedRotation = transform.rotation;
        nextFixedPosition = transform.position;
        nextFixedRotation = transform.rotation;

        horizontalInput = 0.0f;
        verticalInput = 0.0f;
        this.HpSum(100);
    }

    // Update is called once per frame
    void Update()
    {
        isGround = m_groundChecker.IsGrounded();
        //animator.SetBool("isGrounded", isGround);

        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        // Input feild

        //attack
        if (Input.GetMouseButtonDown(0))
        {
            //animator.SetBool("attack", true);
            //attack code
        }

        //jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (m_groundChecker.IsGrounded())
            {
                jumpFlag = true;
                //animator.SetBool("jump", true);
            }
        }

        if (jumpFlag == false)
        {
            //animator.SetBool("jump", false);
        }

        planeVelocity = GetXZVelocity(horizontalInput, 0);
        if (planeVelocity.magnitude == 0) runFlag = false;

        float interpolationAlpha = (Time.time - Time.fixedTime) / Time.fixedDeltaTime;
        m_characterController.Move(velocity * Time.deltaTime/*Vector3.Lerp(lastFixedPosition, nextFixedPosition, interpolationAlpha) - transform.position*/);
        characterMesh.rotation = Quaternion.Slerp(lastFixedRotation, nextFixedRotation, interpolationAlpha);





        //UI
        hpSlider.value = hp / maxHp;
        spSlider.value = sp / maxSp;
        
    }

    private void FixedUpdate()
    {
        //if (!IsOwner) return;

        lastFixedPosition = nextFixedPosition;
        lastFixedRotation = nextFixedRotation;

        Vector3 planeVelocity = GetXZVelocity(horizontalInput, 0);
        float yVelocity = GetYVelocity();
        velocity = new Vector3(planeVelocity.x, yVelocity, planeVelocity.z);

        if (m_groundChecker.IsGrounded()
            && steppingObj != null)
        {
            if (steppingObj.TryGetComponent(out Rigidbody rb))
                velocity += rb.linearVelocity;
        }

        if (planeVelocity.magnitude / speed >= lookForwardThreshold)
        {
            nextFixedRotation = Quaternion.Slerp(characterMesh.rotation, Quaternion.LookRotation(planeVelocity), lookForwardSpeed * Time.fixedDeltaTime);
        }
        /*animator.SetFloat("fall", yVelocity);

        if (planeVelocity.magnitude != 0 && !runFlag) animator.SetBool("walk", true);
        else animator.SetBool("walk", false);
        */
        nextFixedPosition += velocity * Time.fixedDeltaTime;
    }

    private void OnTriggerStay(Collider other)
    {
        if (m_groundChecker.IsGrounded() && other.tag == "Ground")
        {
            steppingObj = other.gameObject;
        }

    }

    private Vector3 GetXZVelocity(float horizontalInput, float verticalInput)
    {
        Vector3 moveVelocity = movementOrientation.forward * verticalInput + movementOrientation.right * horizontalInput;
        Vector3 moveDirection = moveVelocity.normalized;
        float moveSpeed;
        if (!runFlag)
        {
            moveSpeed = Mathf.Min(moveVelocity.magnitude, 1.0f) * speed;
        }
        else
        {
            moveSpeed = Mathf.Min(moveVelocity.magnitude, 1.0f) * (speed * 2);
        }

        return moveDirection * moveSpeed;
    }

    /// <remarks>
    /// This function must be called only in FixedUpdate()
    /// </remarks>
    private float GetYVelocity()
    {

        if (!isGround)
        {
            return velocity.y - gravitationalAcceleration * Time.fixedDeltaTime;
        }

        if (jumpFlag)
        {
            jumpFlag = false;
            return velocity.y + jumpForce;
        }
        else
        {
            return 0f;//Mathf.Max(0.0f, velocity.y);
        };
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.rigidbody)
        {
            hit.rigidbody.AddForce(velocity / hit.rigidbody.mass);
        }
    }
}
