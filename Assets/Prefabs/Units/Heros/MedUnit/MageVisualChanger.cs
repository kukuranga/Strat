using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class MageVisualChanger : MonoBehaviour
{
    public GameObject _Shell;
    public VisualEffect _OuterSphere;
    public VisualEffect _MiddleSphere;
    public VisualEffect _InnerSphere;

    private Color _OuterSphereBaseColor;
    private Color _MiddleSphereBaseColor;
    private Color _InnerSphereBaseColor;

    private void Start()
    {
        _OuterSphereBaseColor = _OuterSphere.GetVector4("Particle color");
        _MiddleSphereBaseColor = _MiddleSphere.GetVector4("Particle color");
        _InnerSphereBaseColor = _InnerSphere.GetVector4("Particle color");
    }

    public void TakeDamage()
    {
        StartCoroutine(LerpColor(Color.red, Color.red, Color.red, 3));
    }

    public void PlaceCrystal()
    {
        StartCoroutine(LerpColor(Color.blue, Color.blue, Color.blue, 3));
    }

    private IEnumerator LerpColor(Color _OuterSphereColor, Color _MiddleSphereColor, Color _InnerSphereColor, float _Time)
    {
        _Shell.SetActive(false);
        _OuterSphere.SetVector4("Particle color", _OuterSphereColor);
        _MiddleSphere.SetVector4("Particle color", _MiddleSphereColor);
        _InnerSphere.SetVector4("Particle color", _InnerSphereColor);

        yield return new WaitForSeconds(_Time);

        _Shell.SetActive(true);
        _OuterSphere.SetVector4("Particle color", _OuterSphereBaseColor);
        _MiddleSphere.SetVector4("Particle color", _MiddleSphereBaseColor);
        _InnerSphere.SetVector4("Particle color", _InnerSphereBaseColor);

        yield return null;
    }
}
