using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using System.Collections;

public class CharacterMovement : NetworkBehaviour
{
    [SerializeField] private CharacterController myController;
    [SerializeField] private Transform camPivot;
    [SerializeField] private Camera myCamera;
    [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private float sprintSpeed = 20f;
    [SerializeField] private float crouchSpeed = 5f;
    [SerializeField] private float gravity = 30f;
    [SerializeField] private Vector3 crouchCameraPosition;
    [SerializeField] private Vector3 baseCameraPosition;
    [SerializeField] private float crouchTransitionSpeed = 1f;
    [SerializeField] private Vector3 crouchScale;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference lookAction;
    [SerializeField] private float intervalBetweenNoise = 0.5f;
    [SerializeField] private GameObject noisePrefab;
    private float rotationY;
    private float rotationX;
    private Vector2 movementVector2d;
    private Vector3 movementVector3d;
    private Vector2 rotationVector;
    private float verticalVelocity;
    private MovementState movementState = MovementState.Walking;
    private float timerBetweenNoise;
    private GameObject noiseInstance;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner)
        {
            GetComponent<CharacterMovement>().enabled = false;
            Destroy(myCamera.gameObject);
            return;
        }
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        GetComponent<MeshRenderer>().enabled = false;
    }

    private void Update()
    {
        movementVector2d = moveAction.action.ReadValue<Vector2>();
        CustomDebug.Instance.UIDebugLog(movementVector2d.ToString());
        Move();

        rotationVector = lookAction.action.ReadValue<Vector2>();
        Rotate();
    }

    private void Move()
    {
        timerBetweenNoise += Time.deltaTime;
        NoiseBehaviour noiseInstanceBehaviour = null;
        if (timerBetweenNoise >= intervalBetweenNoise)
        {
            noiseInstance = Instantiate(noisePrefab, transform.position, Quaternion.identity);
            noiseInstanceBehaviour = noiseInstance.GetComponent<NoiseBehaviour>();
            timerBetweenNoise = 0.0f;
        }
        movementVector3d = transform.forward * movementVector2d.y + transform.right * movementVector2d.x;
        switch (movementState)
        {
            case MovementState.Walking:
                movementVector3d = movementVector3d * walkSpeed * Time.deltaTime;
                if (noiseInstance != null && noiseInstanceBehaviour != null)
                {
                    noiseInstanceBehaviour.Activate(1);
                }
                break;
            case MovementState.Running:
                movementVector3d = movementVector3d * sprintSpeed * Time.deltaTime;
                if (noiseInstance != null && noiseInstanceBehaviour != null)
                {
                    //noiseInstance.transform.localScale *= 2;
                    noiseInstanceBehaviour.Activate(2);
                }
                break;
            case MovementState.Crouching:
                movementVector3d = movementVector3d * crouchSpeed * Time.deltaTime;
                if (noiseInstance != null && noiseInstanceBehaviour != null)
                {
                    //noiseInstance.transform.localScale *= 0.5f;
                    noiseInstanceBehaviour.Activate(0.5f);
                }
                break;
            default:
                movementVector3d = movementVector3d * walkSpeed * Time.deltaTime;
                break;
        }
        myController.Move(movementVector3d);

        verticalVelocity = verticalVelocity - gravity * Time.deltaTime;
        myController.Move(new Vector3(0, verticalVelocity, 0)*Time.deltaTime);

        noiseInstance = null;
    }

    private void Rotate()
    {
        rotationY += rotationVector.x * rotationSpeed * Time.deltaTime;
        rotationX -= rotationVector.y * rotationSpeed * Time.deltaTime;
        rotationX = Mathf.Clamp(rotationX, -80, 80);
        transform.rotation = Quaternion.Euler(0, rotationY, 0);
        camPivot.localRotation = Quaternion.Euler(rotationX, 0, 0);
    }

    public void Sprint(InputAction.CallbackContext ctx)
    {
        if (ctx.started && movementState != MovementState.Crouching)
        {
            movementState = MovementState.Running;
        }
        else if (ctx.canceled && movementState != MovementState.Crouching)
        {
            movementState = MovementState.Walking;
        }
    }

    public void Crouch(InputAction.CallbackContext ctx)
    {
        if (ctx.started && movementState != MovementState.Running)
        {
            movementState = MovementState.Crouching;
            transform.localScale -= crouchScale;
            StopAllCoroutines();
            StartCoroutine(CrouchSmoothTransition(crouchCameraPosition));
        }
        else if (ctx.canceled && movementState != MovementState.Running)
        {
            movementState = MovementState.Walking;
            transform.localScale += crouchScale;
            StopAllCoroutines();
            StartCoroutine(CrouchSmoothTransition(baseCameraPosition));
        }
    }

    IEnumerator CrouchSmoothTransition(Vector3 targetPosition)
    {
        while (camPivot.localPosition != targetPosition)
        {
            camPivot.localPosition = Vector3.MoveTowards(camPivot.localPosition, targetPosition, crouchTransitionSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    public void Jump(InputAction.CallbackContext ctx)
    {
        if (ctx.started && myController.isGrounded)
        {
            verticalVelocity = jumpForce;
            movementState = MovementState.Jumping;
        }
    }
}

public enum MovementState
{
    Walking,
    Running,
    Crouching,
    Jumping
}
