using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ButtonManager : MonoBehaviour
{
    public void Escape()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_ANDROID
        Application.Quit();
#endif
    }
    public void FluidActivation()
    {
        GameObject fluid = GameObject.Find("Fluid");
        fluid.SetActive(false);
        fluid.SetActive(true);
    }
}
