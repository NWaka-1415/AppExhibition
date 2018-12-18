using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Buttons : MonoBehaviour
{
    private OverAllManager.State _previousState;
    [SerializeField] private OverAllManager.State _state;
    [SerializeField] private OverAllManager.MenuTypes _menuTypes;
    [SerializeField] private Color _color;
    [SerializeField] private Color _colorInSelect;
    private Image _image;

    // Use this for initialization
    void Start()
    {
        _image = gameObject.GetComponent<Image>();
        _previousState = _state;
    }

    // Update is called once per frame
    void Update()
    {
        if (OverAllManager.MenuType != this._menuTypes)
            if (_previousState != _state)
            {
                switch (_state)
                {
                    case OverAllManager.State.Select:
                        _image.color = _color;
                        break;
                    case OverAllManager.State.Unselect:
                        _image.color = _colorInSelect;
                        break;
                }
            }

        if (_state == OverAllManager.State.Select)
        {
            if (Input.GetButtonDown("Ok"))
            {
            }
        }
    }

    public OverAllManager.State State
    {
        get { return _state; }
        set
        {
            _previousState = _state;
            _state = value;
        }
    }
}