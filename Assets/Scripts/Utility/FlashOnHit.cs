using UnityEngine;
using System.Collections;

public class FlashOnHit : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private Material _flashMaterial;
    [SerializeField] private float _flashDuration = .125f;

    [Header("Debug Fields")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Material _initialMaterial;
    [SerializeField] private AgentHealthSystem _healthSystem;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _initialMaterial = _spriteRenderer.material;
        _healthSystem = transform.root.GetComponent<AgentHealthSystem>();
    }

    private void Start()
    {
        _healthSystem.OnDamageTaken += HealthSystem_OnDamageTaken;
    }

    private void HealthSystem_OnDamageTaken(DamageStruct obj)
    {
        StopAllCoroutines();

        StartCoroutine(FlashSprite());
    }


    private IEnumerator FlashSprite()
    {
        _spriteRenderer.material = _flashMaterial;

        yield return new WaitForSeconds(_flashDuration);

        _spriteRenderer.material = _initialMaterial;
    }
}
