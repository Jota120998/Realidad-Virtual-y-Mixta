using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private float embedDistance = 0.1f;
    private Vector3 _storedForward;

    void Start()
    {
        //Destroy(gameObject, lifeTime);
    }

    void FixedUpdate()
    {
        if (_rigidBody.linearVelocity.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_rigidBody.linearVelocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        
        _storedForward = transform.forward;
        
        transform.position += _storedForward * embedDistance;

        _rigidBody.isKinematic = true; 
        transform.parent = collision.transform; 

    }
    
}