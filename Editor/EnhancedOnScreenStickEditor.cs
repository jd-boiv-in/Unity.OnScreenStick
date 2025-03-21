using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace EnhancedOnScreenControls.Editor
{
    [CustomEditor(typeof(EnhancedOnScreenStick))]
    public class EnhancedOnScreenStickEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();

            root.Add(new PropertyField(serializedObject.FindProperty("internalControlPath"))
            {
                label = "Control Path"
            });

            AddSpace(root, 6f);

            AddPropertyField(root, "stickType");
            AddPropertyField(root, "axisOptions");
            AddPropertyField(root, "movementRange");
            AddPropertyField(root, "deadZone");
            AddPropertyField(root, "showOnlyWhenPressed");
            
            AddSpace(root, 6f);

            AddPropertyField(root, "Fade");
            AddPropertyField(root, "Alpha");
            AddPropertyField(root, "FadeDelay");
            AddPropertyField(root, "FadeShowSpeed");
            AddPropertyField(root, "FadeHideSpeed");
            AddPropertyField(root, "LerpFactor");
            
            AddSpace(root, 6f);

            AddPropertyField(root, "background");
            AddPropertyField(root, "handle");

#if REWIRED
            AddSpace(root, 6f);
            AddPropertyField(root, "AxisHorizontal");
            AddPropertyField(root, "AxisVertical");
            AddPropertyField(root, "AxisHorizontalId");
            AddPropertyField(root, "AxisVerticalId");
#endif
            
            return root;
        }

        void AddPropertyField(VisualElement root, string propertyName)
        {
            root.Add(new PropertyField(serializedObject.FindProperty(propertyName)));
        }

        void AddSpace(VisualElement root, float space)
        {
            root.Add(new VisualElement() { style = { height = space } });
        }
    }
}