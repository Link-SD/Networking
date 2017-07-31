using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SortButton : MonoBehaviour {

    public HighScoreManager.SortType typeA;
    public HighScoreManager.SortType typeB;
    private HighScoreManager.SortType type;

    private Button button;
    [SerializeField]
    private Image _arrow;

    private Dropdown dropdown;

    // Use this for initialization
    void Start() {
        type = typeA;
        
        button = GetComponent<Button>();
        if (button != null) {
            button.onClick.AddListener(SwitchArrow);
            button.onClick.AddListener(delegate { HighScoreManager.Instance.Sort(type); });
        }
        dropdown = GetComponent<Dropdown>();

        if (dropdown != null) {
            dropdown.onValueChanged.AddListener(Sort);
        }
    }

    private void Sort(int arg0) {
        if (arg0 == 0) {
            type = typeA;
        } else {
            type = typeB;
        }
        HighScoreManager.Instance.Sort(type);
    }

    private void SwitchArrow() {
        Vector3 r = _arrow.transform.localScale;
        r.y *= -1;
        _arrow.transform.localScale = r;

        type = type == typeA ? type = typeB : type = typeA;
    }
}
