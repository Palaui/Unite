using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Unite
{
    public static class Mouse
    {
        // Properties
        #region Properties

        private static Texture2D cursorTexture = null;
        private static Vector2 cursorOffset = Vector2.zero;

        #endregion

        // Properties
        #region Properties

        public static Vector2 Location
        {
            get { return Input.mousePosition; }
        }

        #endregion

        // Public Static
        #region Public Static

        public static void SetDefaultCursor(Texture2D tex, Vector2 offset)
        {
            cursorTexture = tex;
            cursorOffset = offset;
            Cursor.SetCursor(cursorTexture, cursorOffset, CursorMode.Auto);
        }

        public static void ApplyDefaultCursor()
        {
            Cursor.SetCursor(cursorTexture, cursorOffset, CursorMode.Auto);
        }

        public static RaycastHit PhysicRaycast()
        {
            RaycastHit impact;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out impact))
                return impact;

             return new RaycastHit();
        }

        public static bool PhysicRaycast(out RaycastHit impact)
        {
            impact = new RaycastHit();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out impact))
                return true;

            return false;
        }

        public static List<RaycastResult> GraphicRaycast(GraphicRaycaster raycaster)
        {
            PointerEventData data = new PointerEventData(null);
            List<RaycastResult> results = new List<RaycastResult>();
            data.position = Location;
            raycaster.Raycast(data, results);
            return results;
        }

        public static bool GraphicRaycast(GraphicRaycaster raycaster, out List<RaycastResult> impacts)
        {
            impacts = new List<RaycastResult>();
            PointerEventData data = new PointerEventData(null);
            data.position = Location;
            raycaster.Raycast(data, impacts);
            if (impacts.Count > 0)
                return true;

            return false;
        }

        public static bool FullRaycast(GraphicRaycaster raycaster, out List<RaycastResult> impacts, out RaycastHit impact)
        {
            impacts = GraphicRaycast(raycaster);
            impact = PhysicRaycast();

            if (impacts.Count > 0 || impact.transform != null)
                return true;

            return false;
        }

        #endregion

    }
}
