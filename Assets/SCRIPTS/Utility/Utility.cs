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

        public static float[] FlattenArray(float[,] array)
        {
            int rows = array.GetLength(0);
            int cols = array.GetLength(1);

            float[] flatArray = new float[rows * cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    flatArray[i * cols + j] = array[i, j];
                }
            }

            return flatArray;
        }

        public static float[,] UnflattenArray(float[] array, int rows, int cols)
        {
            float[,] multiDimArray = new float[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    multiDimArray[i, j] = array[i * cols + j];
                }
            }

            return multiDimArray;
        }
    }
}
