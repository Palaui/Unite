using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Unite
{
    public static class Mouse
    {
        // Variables
        #region Variables

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

        /// <summary> Assigns a new cursor to be used by default. </summary>
        /// <param name="tex"> Texture of the cursor. </param>
        /// <param name="offset"></param>
        public static void SetDefaultCursor(Texture2D tex, Vector2 offset)
        {
            cursorTexture = tex;
            cursorOffset = offset;
            Cursor.SetCursor(cursorTexture, cursorOffset, CursorMode.Auto);
        }

        /// <summary> Changed the cursor to the one assigned as a default. </summary>
        public static void ApplyDefaultCursor()
        {
            Cursor.SetCursor(cursorTexture, cursorOffset, CursorMode.Auto);
        }

        /// <summary> Fires a raycast againts physic objects from the camera to the mouse pointing location. </summary>
        /// <returns> The impact of the raycast. </returns>
        public static RaycastHit PhysicRaycast()
        {
            RaycastHit impact;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out impact))
                return impact;

             return new RaycastHit();
        }

        /// <summary> Fires a raycast againts physic objects from the camera to the mouse pointing location. </summary>
        /// <param name="impact"> - out param - The impact of the raycast. </param>
        /// <returns> If there was a successful impact. </returns>
        public static bool PhysicRaycast(out RaycastHit impact)
        {
            impact = new RaycastHit();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out impact))
                return true;

            return false;
        }

        /// <summary> Fires a raycast against interface objects from the camera to the mouse pointing location. </summary>
        /// <param name="raycaster"> Graphic raycaster we are firing against. </param>
        /// <returns> Results of the impacts. </returns>
        public static List<RaycastResult> GraphicRaycast(GraphicRaycaster raycaster)
        {
            PointerEventData data = new PointerEventData(null);
            List<RaycastResult> results = new List<RaycastResult>();
            data.position = Location;
            raycaster.Raycast(data, results);
            return results;
        }
        /// <summary> Fires a raycast against interface objects from the camera to the mouse pointing location. </summary>
        /// <param name="raycaster"> Graphic raycaster we are firing against. </param>
        /// <param name="impacts"> - out param - The impact of the raycast. </param>
        /// <returns> If there was a successful impact. </returns>
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

        /// <summary>
        /// Fires a raycast againts physic and interface objects from the camera to the mouse pointing location.
        /// </summary>
        /// <param name="raycaster"> Graphic raycaster we are firing against. </param>
        /// <param name="impacts"> - out param - The impact of the raycast. </param>
        /// <param name="impact"> - out param - The impact of the raycast. </param>
        /// <returns> If there was a successful impact. </returns>
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
