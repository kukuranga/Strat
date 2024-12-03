using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildDetailsMenuController : MonoBehaviour
{
    public Project.Build.Commands.BuildData buildData;
    public Text display;

    private void Start() {
        display.text = $"{buildData.branchName}:{buildData.buildNumber}\n{buildData.sha}";
    }
}
