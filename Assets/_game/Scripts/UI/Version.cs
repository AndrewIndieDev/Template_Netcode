using TMPro;
using UnityEngine;

public class Version : MonoBehaviour
{
    [SerializeField] private TMP_Text version;

    void Start()
    {
        version.text = $"v{Application.version}";
    }
}
