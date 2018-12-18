using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
            _animator.SetTrigger("UnselectToSelect");
            _previousState = _state;
        }
        else if (_previousState == OverAllManager.State.Select && _state == OverAllManager.State.Unselect)
        {
            //選択状態から非選択状態への遷移
            _animator.SetTrigger("SelectToUnselect");
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
        image.sprite = SpriteFromFile(imageFileName);

        _exitedFlag = false;
        _executeFlag = false;
    }

    public void MoveWindow(Vector2 vector2)
    {
        transform.localPosition = vector2;
        DebugLogPosition();
    }

    public void Execute()
    {
        _proc = new Process();
        _proc.StartInfo.FileName = _exeFileName;
        _proc.EnableRaisingEvents = true;

        _executeFlag = true;
        _exitedFlag = false;

        _animator.SetBool("isStart", _executeFlag);

        _proc.Exited += Exited;
        _proc.Start();
    }

    private void Exited(object sender, EventArgs e)
    {
        _executeFlag = false;
        _exitedFlag = true;
        //Debug.Log("アプリケーションの終了を検知");
    }

    private Texture2D Texture2DFromFile(string path)
    {
        Texture2D texture = null;
        if (File.Exists(path))
        {
            //byte取得
            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            BinaryReader bin = new BinaryReader(fileStream);
            byte[] readBinary = bin.ReadBytes((int) bin.BaseStream.Length);
            bin.Close();
            fileStream.Dispose();
            fileStream = null;
            if (readBinary != null)
            {
                //横サイズ
                int pos = 16;
                int width = 0;
                for (int i = 0; i < 4; i++)
                {
                    width = width * 256 + readBinary[pos++];
                }

                //縦サイズ
                int height = 0;
                for (int i = 0; i < 4; i++)
                {
                    height = height * 256 + readBinary[pos++];
                }

                //byteからTexture2D作成
                texture = new Texture2D(width, height);
                texture.LoadImage(readBinary);
            }

            readBinary = null;
        }

        return texture;
    }

    private Sprite SpriteFromTexture2D(Texture2D texture)
    {
        Sprite sprite = null;
        if (texture)
        {
            //Texture2DからSprite作成
            sprite = Sprite.Create(texture, new UnityEngine.Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }

        return sprite;
    }

    private Sprite SpriteFromFile(string path)
    {
        Sprite sprite = null;
        Texture2D texture = Texture2DFromFile(path);
        if (texture)
        {
            //Texture2DからSprite作成
            sprite = SpriteFromTexture2D(texture);
        }

        texture = null;
        return sprite;
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