using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using System.Collections;

public class CharacterMovement : NetworkBehaviour
{
    [SerializeField] private CharacterController myController;
    [SerializeField] private Camera myCamera;
    [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private float sprintSpeed = 20f;
    [SerializeField] private float crouchSpeed = 5f;
    [SerializeField] private Vector3 crouchCameraPosition;
    [SerializeField] private Vector3 baseCameraPosition;
    [SerializeField] private float crouchTransitionSpeed = 1f;
    [SerializeField] private Vector3 crouchScale;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference lookAction;
    private float rotationY;
    private float rotationX;
    private Vector2 movementVector2d;
    private Vector3 movementVector3d;
    private Vector2 rotationVector;
    private MovementState movementState = MovementState.Walking;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner)
        {
            GetComponent<CharacterMovement>().enabled = false;
            return;
        }
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        GetComponent<MeshRenderer>().enabled = false;
    }

    private void Update()
    {
        movementVector2d = moveAction.action.ReadValue<Vector2>();
        Move();

        rotationVector = lookAction.action.ReadValue<Vector2>();
        Rotate();
    }

    private void Move()
    {
        movementVector3d = transform.forward * movementVector2d.y + transform.right * movementVector2d.x;
        switch (movementState)
        {
            case MovementState.Walking:
                movementVector3d = movementVector3d * walkSpeed * Time.deltaTime;
                break;
            case MovementState.Running:
                movementVector3d = movementVector3d * sprintSpeed * Time.deltaTime;
                break;
            case MovementState.Crouching:
                movementVector3d = movementVector3d * crouchSpeed * Time.deltaTime;
                break;
            default:
                movementVector3d = movementVector3d * walkSpeed * Time.deltaTime;
                break;
        }
        myController.Move(movementVector3d);
    }

    private void Rotate()
    {
        rotationY += rotationVector.x * rotationSpeed * Time.deltaTime;
        rotationX -= rotationVector.y * rotationSpeed * Time.deltaTime;
        rotationX = Mathf.Clamp(rotationX, -80, 80);
        transform.rotation = Quaternion.Euler(0, rotationY, 0);
        myCamera.gameObject.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
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
        while (myCamera.transform.localPosition != targetPosition)
        {
            myCamera.transform.localPosition = Vector3.MoveTowards(myCamera.transform.localPosition, targetPosition, crouchTransitionSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }
}

public enum MovementState
{
    Walking,
    Running,
    Crouching
}
