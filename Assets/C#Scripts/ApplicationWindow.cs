using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Controllers;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class ApplicationWindow : MonoBehaviour
{
    private OverAllManager _overAllManager;
    [SerializeField] private GameObject _appImage;
    [SerializeField] private GameObject _appStartMessage;
    private Animator _animator;
    private OverAllManager.State _previousState; //以前の状態
    private OverAllManager.State _state; //今の状態

    private Vector2 _size;

    private string _gameName; //ゲームタイトル
    private string _exeFileName; //Fileの場所
    private string _gameImage; //ゲームのメインイメージ
    private OverAllManager.GameCategory _gameCategory; //ゲームカテゴリ

    Process _proc;

    private bool _exitedFlag;
    private bool _executeFlag;

    private void Update()
    {
        //Debug.Log("<color=red>Exited:" + _exitedFlag + "</color>");
        if (_exitedFlag)
        {
            _animator.SetBool("isStart", false);
            _exitedFlag = false;
        }

        if (OverAllManager.MenuType != OverAllManager.MenuTypes.Home) return;
        if (_previousState == OverAllManager.State.Unselect && _state == OverAllManager.State.Select)
        {
            //非選択状態から選択状態への遷移
            _animator.SetBool("SelectToUnselect", false);
            _animator.SetBool("UnselectToSelect", true);
            _previousState = _state;
        }
        else if (_previousState == OverAllManager.State.Select && _state == OverAllManager.State.Unselect)
        {
            //選択状態から非選択状態への遷移
            _animator.SetBool("UnselectToSelect", false);
            _animator.SetBool("SelectToUnselect", true);
            _previousState = _state;
        }

        if (Input.GetButtonDown("Submit"))
        {
            Debug.Log("Pressed Ok");
            if (_state == OverAllManager.State.Select) Execute();
        }
    }

    public void Initialize(OverAllManager overAllManager, string gameName, string exeFileName, string imageFileName,
        OverAllManager.GameCategory gameCategory, OverAllManager.State state)
    {
        _overAllManager = overAllManager;
        _gameName = gameName;
        _exeFileName = exeFileName;
        _gameImage = imageFileName;
        _gameCategory = gameCategory;
        _state = state;
        _size = transform.localScale;

        _animator = GetComponent<Animator>();

        Image image = GetComponentInChildren<Image>();
        image.sprite = SpriteEditor.SpriteFromFile(imageFileName);

        _exitedFlag = false;
        _executeFlag = false;

        SetUpProc();
//        _proc.Start();
//        ForceExit();
    }

    public void MoveWindow(Vector2 vector2)
    {
        transform.localPosition = vector2;
        //DebugLogPosition();
    }

    private void SetUpProc()
    {
        _proc = new Process();
        _proc.StartInfo.FileName = _exeFileName;
        _proc.EnableRaisingEvents = true;
    }

    public void Execute()
    {
        if (_executeFlag) return;
        if (_proc == null) SetUpProc();

        _executeFlag = true;
        _exitedFlag = false;

        _animator.SetBool("isStart", _executeFlag);

        _proc.Exited += Exited;
        bool executeResult = _proc.Start();
        Debug.Log($"execute : {executeResult}");
    }

    private void Exited(object sender, EventArgs e)
    {
        _executeFlag = false;
        _exitedFlag = true;
        //Debug.Log("アプリケーションの終了を検知");
    }

    private void ForceExit()
    {
        if (!_proc.CloseMainWindow()) _proc.Kill();

        _proc.Close();
        _proc.Dispose();
        Debug.Log("初期の自動起動後の自動終了");
    }

    private void DebugLogPosition()
    {
        Debug.Log("<color=blue>" + _gameName + "Pos:" + transform.position + "</color>");
        Debug.Log("<color=blue>" + _gameName + "LocalPos:" + transform.localPosition + "</color>");
    }

    public GameObject AppImage
    {
        get { return _appImage; }
    }

    public GameObject AppStartMessage
    {
        get { return _appStartMessage; }
    }

    public string GameName
    {
        get { return _gameName; }
    }

    public string GameImage
    {
        get { return _gameImage; }
    }

    public OverAllManager.GameCategory GameCategory
    {
        get { return _gameCategory; }
    }

    public Vector2 Size
    {
        get { return _size; }
    }

    public OverAllManager.State State
    {
        get { return _state; }
        set
        {
            _previousState = this._state;
            _state = value;
        }
    }

    public bool ExecuteFlag
    {
        get { return _executeFlag; }
    }
}