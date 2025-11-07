using UnityEngine;
using UnityEngine.InputSystem;

public class Freecam : MonoBehaviour
{
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference lookAction;
    [SerializeField] private float travelSpeed;
    [SerializeField] private float rotationSpeed = 5f;
    private float rotationY;
    private float rotationX;
    private Vector2 movementVector2d;
    private Vector3 movementVector3d;
    private Vector2 rotationVector;

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
        movementVector3d = movementVector3d * travelSpeed * Time.deltaTime;
        transform.position += movementVector3d;
    }

    private void Rotate()
    {
        rotationY += rotationVector.x * rotationSpeed * Time.deltaTime;
        rotationX -= rotationVector.y * rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(rotationX, rotationY, 0);
    }
}
