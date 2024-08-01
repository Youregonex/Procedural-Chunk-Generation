using UnityEngine;
using System.Collections;

public abstract class FlashOnHit : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] protected Material _flashMaterial;
    [SerializeField] protected float _flashDuration = .125f;

    [Header("Debug Fields")]
    [SerializeField] protected SpriteRenderer _spriteRenderer;
    [SerializeField] protected Material _initialMaterial;

    protected void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _initialMaterial = _spriteRenderer.material;
    }

    protected IEnumerator FlashSprite()
    {
        _spriteRenderer.material = _flashMaterial;

        yield return new WaitForSeconds(_flashDuration);

        _spriteRenderer.material = _initialMaterial;
    }
}
