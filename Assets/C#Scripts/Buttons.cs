using System.Collections;
using System.Collections.Generic;
using Controllers;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Buttons : MonoBehaviour
{
    private Enums.State _previousState;
    [SerializeField] private Enums.State _state;
    [SerializeField] private Enums.MenuType _menuTypes;
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
        if (OverAllManager.Instance.CurrentMenuType != this._menuTypes)
            if (_previousState != _state)
            {
                switch (_state)
                {
                    case Enums.State.Select:
                        _image.color = _color;
                        break;
                    case Enums.State.Unselect:
                        _image.color = _colorInSelect;
                        break;
                }
            }

        if (_state == Enums.State.Select)
        {
            if (Input.GetButtonDown("Ok"))
            {
            }
        }
    }

    public Enums.State State
    {
        get { return _state; }
        set
        {
            _previousState = _state;
            _state = value;
        }
    }
}