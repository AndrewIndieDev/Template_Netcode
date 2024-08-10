using UnityEngine;
using UnityEngine.UI;

public class ImmediateDeselect : MonoBehaviour
{
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => { button.OnDeselect(null); });
    }
}
