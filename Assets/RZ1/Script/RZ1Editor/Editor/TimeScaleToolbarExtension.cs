using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

[Overlay(typeof(SceneView), "TimeScale Controller")]
public class TimeScaleOverlay : Overlay
{
    public override VisualElement CreatePanelContent()
    {
        var root = new VisualElement();
        root.style.flexDirection = FlexDirection.Row;
        root.style.alignItems = Align.Center;
        root.style.paddingLeft = 4;
        root.style.paddingRight = 4;

        // 初期値（Time.timeScale を 0～10 にスケーリング）
        int initial = Mathf.RoundToInt(Time.timeScale * 10f);
        var valueLabel = new Label(initial.ToString())
        {
            style =
            {
                unityTextAlign = TextAnchor.MiddleRight,
                width = 24
            }
        };

        var slider = new SliderInt(0, 10)
        {
            value = initial,
            style =
            {
                width = 120
            }
        };

        slider.RegisterValueChangedCallback(evt =>
        {
            Time.timeScale = evt.newValue / 10f;
            valueLabel.text = evt.newValue.ToString();
        });

        root.Add(valueLabel);
        root.Add(slider);
        return root;
    }
}