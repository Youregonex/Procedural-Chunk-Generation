using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float _cameraSpeed = 5f;
    [SerializeField] private Transform _moveTowardsTransform;

    private Vector2 _movementDirection;

    private void Awake()
    {
        _movementDirection = (_moveTowardsTransform.position - transform.position).normalized;
    }

    private void Update()
    {
        transform.position +=  _cameraSpeed * Time.deltaTime * (Vector3)_movementDirection;
    }
}
