using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class ProgressBar : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("GameObject/UI/Linear Progress Bar")]
    public static void AddLinearProgressBar()
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>("UI/Linear Progress Bar"), 
            Selection.activeGameObject.transform, false);
    }
    [MenuItem("GameObject/UI/Radial Progress Bar")]
    public static void AddRadialProgressBar()
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>("UI/Radial Progress Bar"), 
            Selection.activeGameObject.transform, false);
    }
#endif
    
    #region Variables

    public float minimum;
    public float maximum;
    public float current;

    public Image mask;
    public Image fill;

    public enum ColorMode
    {
        Color,
        Gradient
    }
    public ColorMode colorMode;

    public Color color;
    public Gradient gradient;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        GetCurrentFill();
    }

    private void GetCurrentFill()
    {
        float currentOffset = current - minimum;
        float maximumOffset = maximum - minimum;
        float fillAmount = currentOffset / maximumOffset;
        mask.fillAmount = fillAmount;

        switch (colorMode)
        {
            case ColorMode.Color:
                fill.color = color;
                break;
            case ColorMode.Gradient:
                fill.color = gradient.Evaluate(fillAmount);
                break;
            default:
                break;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ProgressBar))]
public class ProgressBarEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ProgressBar bar = (ProgressBar) target;

        bar.minimum = EditorGUILayout.FloatField("Minimum", bar.minimum);
        bar.maximum = EditorGUILayout.FloatField("Maximum", bar.maximum);
        bar.current = EditorGUILayout.FloatField("Current", bar.current);

        bar.mask = (Image)EditorGUILayout.ObjectField("Mask", bar.mask, typeof(Image));
        bar.fill = (Image)EditorGUILayout.ObjectField("Mask", bar.fill, typeof(Image));
        
        bar.colorMode = (ProgressBar.ColorMode)EditorGUILayout.EnumPopup("Color Mode", bar.colorMode);
        switch (bar.colorMode)
        {
            case ProgressBar.ColorMode.Color:
                bar.color = EditorGUILayout.ColorField("Color", bar.color);
                break;
            case ProgressBar.ColorMode.Gradient:
                bar.gradient = EditorGUILayout.GradientField("Gradient", bar.gradient);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
#endif
