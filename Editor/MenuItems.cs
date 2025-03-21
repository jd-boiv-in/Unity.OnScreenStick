using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EnhancedOnScreenControls.Editor
{
    static class MenuItems
    {
        [MenuItem("GameObject/UI/Enhanced On-Screen Controls/On-Screen Stick")]
        public static void CreateOnScreenStick()
        {
            var path = "Packages/com.annulusgames.enhanced-on-screen-stick/Runtime/Prefabs/Enhanced On-Screen Stick.prefab";
            CreateUIItem(path, "Enhanced On-Screen Stick");
        }

        static void CreateUIItem(string assetPath, string objectName)
        {
            var obj = Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(assetPath));

            var canvas = Object.FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                canvas = new GameObject("Canvas").AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.gameObject.AddComponent<CanvasScaler>();
                canvas.gameObject.AddComponent<GraphicRaycaster>();

                if (Object.FindFirstObjectByType<EventSystem>() == null)
                {
                    _ = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
                }
            }

            obj.name = objectName;
            obj.transform.SetParent(canvas.transform);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;

            Undo.RegisterCreatedObjectUndo(obj, "Create " + objectName);
            Selection.activeGameObject = obj;
        }
    }
}