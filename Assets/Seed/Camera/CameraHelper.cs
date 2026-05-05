using UnityEngine;

namespace Seed.CameraSystem
{
    public static class CameraHelper
    {
        private static Camera _cached;

        public static Camera MainCamera
        {
            get
            {
                if (_cached == null) _cached = Camera.main;
                return _cached;
            }
        }

        public static bool IsOnScreen(this GameObject go)
        {
            if (go == null) return false;
            return IsWorldPointOnScreen(go.transform.position, out _);
        }

        public static bool IsOnScreen(this Vector3 worldPoint, out Vector3 viewportPoint)
        {
            return IsWorldPointOnScreen(worldPoint, out viewportPoint);
        }

        public static void PlaceAtWorldPoint(RectTransform rect, Vector3 worldPos)
        {
            if (rect == null) return;
            Camera cam = MainCamera;
            if (cam == null) return;
            rect.position = cam.WorldToScreenPoint(worldPos);
        }

        private static bool IsWorldPointOnScreen(Vector3 worldPoint, out Vector3 viewportPoint)
        {
            Camera cam = MainCamera;
            if (cam == null)
            {
                viewportPoint = default;
                return true;
            }

            viewportPoint = cam.WorldToViewportPoint(worldPoint);
            return viewportPoint is { z: > 0f, x: >= 0f and <= 1f, y: >= 0f and <= 1f };
        }
    }
}
