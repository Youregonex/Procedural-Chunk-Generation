using UnityEngine;
using TMPro;
using DG.Tweening;

public class DamagePopup : MonoBehaviour
{
    [Header("Base Settings")]
    [SerializeField] private float _popupLifetime = .75f;
    [SerializeField] private float _popupMoveTime = .5f;
    [SerializeField] private float _popupMoveDistance = .5f;

    [Header("Fade Settings")]
    [SerializeField] private Color _fadeInColor;
    [SerializeField] private Color _fadeOutColor;
    [SerializeField] private float _popupFadeTime = .3f;

    [Header("Scale Settings")]
    [SerializeField] private Vector2 _startScale = new Vector2(.75f, .75f);
    [SerializeField] private Vector2 _targetScale = Vector2.one;
    [SerializeField] private float _popupScaleTime = .3f;

    private TextMeshPro _popupText;
    private Sequence _damagePopupSequence;

    public void InitializeDamagePopup(int damage)
    {
        _popupText = GetComponent<TextMeshPro>();
        _popupText.text = damage.ToString();

        transform.localScale = _startScale;

        _fadeInColor = _popupText.color;
        _fadeOutColor = _popupText.color;
        _fadeOutColor.a = 0f;

        _popupText.color = _fadeOutColor;

        _damagePopupSequence = DOTween.Sequence();

        _damagePopupSequence
            .Append(_popupText.DOColor(_fadeInColor, _popupFadeTime))
            .Append(transform.DOScale(_targetScale, _popupScaleTime))
            .Append(transform.DOMove((Vector2)transform.position + (Vector2.up * _popupMoveDistance), _popupMoveTime))
            .Append(_popupText.DOColor(_fadeOutColor, _popupFadeTime))
            .OnComplete(() =>
            {
                Destroy(gameObject);
            }).SetDelay(_popupLifetime);

        _damagePopupSequence.Play();
    }
}
