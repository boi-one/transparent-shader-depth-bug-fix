using UnityEngine;

public class GravityPlayer : MonoBehaviour
{
    [SerializeField] private float currentGravity;
    
    [SerializeField] private float groundDrag = 3.5f;
    [SerializeField] private float arialDrag = 0.25f;

    private Rigidbody _rigidBody;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        _rigidBody.drag = GetComponent<GroundChecker>().IsGrounded ? groundDrag : arialDrag;
        _rigidBody.AddForce(new Vector3(0, currentGravity, 0));
    }
}
