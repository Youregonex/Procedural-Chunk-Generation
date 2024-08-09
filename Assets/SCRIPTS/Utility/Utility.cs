using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace Youregone.Utilities
{
    public static class Utility
    {

        public static bool InRange(float range, Vector2 v1, Vector2 v2)
        {
            var dx = v1.x - v2.x;
            var dy = v1.y - v2.y;

            return dx * dx + dy * dy < range * range;
        }

        public static bool PointerOverUIObject()
        {
            PointerEventData eventDataCurrentPosition = new(EventSystem.current);
            eventDataCurrentPosition.position = Mouse.current.position.ReadValue();
            List<RaycastResult> results = new();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

            return results.Count > 0;
        }

        public static Vector2 GetMouseGridPosition()
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            return new Vector2(Mathf.Round(mousePosition.x), Mathf.Round(mousePosition.y));
        }
    }
}
