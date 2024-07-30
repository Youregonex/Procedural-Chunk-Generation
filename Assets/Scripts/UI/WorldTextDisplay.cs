using UnityEngine;

public class WorldTextDisplay : MonoBehaviour
{
    public static WorldTextDisplay Instance;

    [SerializeField] private DamagePopup _damagePopupPrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            DisplayDamagePopup(Vector2.zero, 10);
        }
    }

    public void DisplayDamagePopup(Vector2 position, int damage)
    {
        DamagePopup damagePopup = Instantiate(_damagePopupPrefab, position, Quaternion.identity);

        damagePopup.InitializeDamagePopup(damage);
    }
}
