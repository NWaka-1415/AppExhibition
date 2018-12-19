using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Windows.Forms;
using UnityEngine.EventSystems;
using Button = UnityEngine.UI.Button;
using Screen = UnityEngine.Screen;

public class OverAllManager : MonoBehaviour
{
    //EventSystem
    [SerializeField] private EventSystem _eventSystem; //イベントシステム　入力受付停止などに用いる

    //GameObjects
    [SerializeField] private GameObject _appWindowsPrefabs; //アプリケーションウィンドウのプレハブ
    [SerializeField] private GameObject _gameTitleObject; //ゲームタイトルを表示するテキストオブジェクト
    [SerializeField] private GameObject _selectingGameCategoryObject; //選択中のゲームカテゴリを表示するテキストオブジェクト
    [SerializeField] private GameObject _backGround; //バックグラウンドオブジェクト（ただの背景）
    [SerializeField] private GameObject _home; //Home画面のオブジェクトをまとめるからのオブジェクト
    [SerializeField] private GameObject _settingsPanel; //Settings画面のパネル
    [SerializeField] private GameObject _addAppPanel; //Add画面のパネル
    [SerializeField] private GameObject _deleteAppPanel; //Delete画面のパネル
    [SerializeField] private GameObject _fileSelectButtonText; //アプリケーションファイル選択ダイアログオープンボタンオブジェクト in Add
    [SerializeField] private GameObject _imageFileSelectButtonText; //イメージファイル選択ダイアログオープンボタンオブジェクト in Add
    [SerializeField] private GameObject _titleNameEnter; //ゲームタイトルインプットフィールドオブジェクト in Add
    [SerializeField] private GameObject _gameCategoryDropdown; //ゲームカテゴリドロップダウンオブジェクト in Add

    //Buttons
    [SerializeField] private Button _firstSelectButtonOnSetting; //最初に選択されているボタン in Setting
    [SerializeField] private Button _firstSelectButtonOnAdd; //最初に選択されているボタン in Add
    [SerializeField] private Button _firstSelectButtonOnDelete; //最初に選択されているボタン in Delete

    //Animators
    [SerializeField] private Animator _gameCategoryAnimator; //ゲームカテゴリBackGroundのコンポーネント＜アニメーター＞

    //Sprites
    [SerializeField] private Sprite[] _gameCategoryIconSprites; //ゲームカテゴリの各種アイコンのSprite

    //Images
    [SerializeField] private Image[] _gameCategoryIcons; //ゲームカテゴリのアイコンオブジェクトのコンポーネントs＜Image＞

    private AppWindowsManager _appWindowsManager;

    private List<App> _apps;

    /*
     * 設定でセットされた新しいゲーム（アプリ）の情報
     */
    private string _setFileName;
    private string _setImageFileName;
    private string _setGameTitle;
    private GameCategory _setGameCategory;

    //==========================================-=-=
    private static MenuTypes _menuType;
    private Vector2 _prevDPad;

    private bool _openDialogFlag; //Dialogを開いていたらTrue おそらくもう不要

    private int _selectingGameCategory; //選ばれているゲームカテゴリ
    private int _previousSelectingGameCategory; //前フレームで選ばれていたゲームカテゴリ

    class App
    {
        private readonly string _gameName;
        private readonly string _fileName;
        private readonly string _imageFileName;
        private readonly GameCategory _gameCategory;

        public App(string gameName, string fileName, string imageFileName, GameCategory gameCategory)
        {
            _gameName = gameName;
            _fileName = fileName;
            _imageFileName = imageFileName;
            _gameCategory = gameCategory;
        }

        public string GameName
        {
            get { return _gameName; }
        }

        public string FileName
        {
            get { return _fileName; }
        }

        public string ImageFileName
        {
            get { return _imageFileName; }
        }

        public GameCategory GameCategory
        {
            get { return _gameCategory; }
        }
    }

    // Use this for initialization
    void Start()
    {
        Screen.fullScreen = true;
        Screen.SetResolution(1024, 576, FullScreenMode.FullScreenWindow);

        LoadApplication();

        _appWindowsManager = gameObject.GetComponent<AppWindowsManager>();
        _appWindowsManager.Initialize();

        foreach (App app in _apps)
        {
            CreateAppWindow(app);
            Debug.Log("AppName:" + app.GameName);
        }

        _appWindowsManager.Initialize();

        _prevDPad = Vector2.zero;

        _openDialogFlag = false;

        for (int i = 0; i < Enum.GetNames(typeof(GameCategory)).Length; i++)
        {
            SetDropDownMenu(i);
        }

        _settingsPanel.SetActive(false);

        _setFileName = "";
        _setGameTitle = "";
        _setGameCategory = GameCategory.Action;

        _selectingGameCategory = -1;
        _previousSelectingGameCategory = _selectingGameCategory;
    }

    private void Update()
    {
        if (_menuType == MenuTypes.Home)
        {
            _gameTitleObject.GetComponent<Text>().text = _appWindowsManager.GetSelectAppTitle();
        }

        if (Input.GetButtonDown("Cancel") && !_openDialogFlag)
        {
            Debug.Log("Pressed Cancel");
            BackTo();
        }
        else if (Input.GetButtonDown("Y"))
        {
            Debug.Log("Pressed Y");
            if (_menuType == MenuTypes.Home)
            {
                _home.SetActive(false);
                _settingsPanel.SetActive(true);
                _appWindowsManager.ResetAppInstants();
                _firstSelectButtonOnSetting.Select();
                _menuType = MenuTypes.Settings;
            }
        }
        else if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("Pressed Fire1");
            //RB
            if (_menuType == MenuTypes.Home) _selectingGameCategory = ChangeSelectingGameCategory(-1);
        }
        else if (Input.GetButtonDown("Fire2"))
        {
            Debug.Log("Pressed Fire2");
            //LB
            if (_menuType == MenuTypes.Home) _selectingGameCategory = ChangeSelectingGameCategory(1);
        }
        else if (Input.GetButtonDown("Menu"))
        {
            if (_menuType == MenuTypes.Home) _menuType = MenuTypes.Menu;
        }

        if (_menuType == MenuTypes.Home)
        {
            if (_selectingGameCategory != _previousSelectingGameCategory)
            {
                _previousSelectingGameCategory = _selectingGameCategory;

                _appWindowsManager.ResetAppInstants(); //既存のパネルを消し

                //情報を新たに追加しなおし
                if (_selectingGameCategory == -1)
                {
                    //All
                    foreach (App app in _apps)
                    {
                        CreateAppWindow(app);
                    }
                }
                else
                {
                    foreach (App app in _apps)
                    {
                        if (ExchangeGameCategoryFromInt(_selectingGameCategory) == app.GameCategory)
                        {
                            CreateAppWindow(app);
                        }
                    }
                }

                AnimationGameCategory(); //選択中のゲームカテゴリの変更アニメーションをしつつ

                _appWindowsManager.Initialize(); //ウィンドウパネルを初期化

                _selectingGameCategoryObject.GetComponent<Text>().text = _selectingGameCategory != -1
                    ? ExchangeGameCategoryFromInt(_selectingGameCategory).ToString()
                    : "All";
            }
        }

        /*
         * -------------------------------------------------------------
         */
        if (_menuType == MenuTypes.Home) return; //Homeであれば処理を終了


        Vector2 dPad = new Vector2(Input.GetAxisRaw("Horizontal_DPad"), Input.GetAxisRaw("Vertical_DPad"));

        if (EventSystem.current.currentSelectedGameObject != null)
        {
            Navigation nav = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>().navigation;
            if (Input.GetButtonDown("Up") || (dPad.y > 0 && Mathf.Abs(_prevDPad.y - dPad.y) > 0))
            {
                Debug.Log("<color=red>Axis Horizontal:" + dPad.y + "</color>");
                if (nav.selectOnUp != null) nav.selectOnUp.Select();
            }
            else if (Input.GetButtonDown("Down") || dPad.y < 0 && Mathf.Abs(_prevDPad.y - dPad.y) > 0)
            {
                Debug.Log("<color=red>Axis Horizontal:" + dPad.y + "</color>");
                if (nav.selectOnDown != null) nav.selectOnDown.Select();
            }
        }


        _prevDPad = dPad;
    }

    void SetDropDownMenu(int value)
    {
        GameCategory gameCategory;
        gameCategory = ExchangeGameCategoryFromInt(value);
        _gameCategoryDropdown.GetComponent<Dropdown>().options[value].text = gameCategory.ToString();
    }

    void CreateAppWindow(App app)
    {
        GameObject instantiateApplication = Instantiate(_appWindowsPrefabs, _home.transform);
        instantiateApplication.GetComponent<ApplicationWindow>().Initialize(this, app.GameName, app.FileName,
            app.ImageFileName, app.GameCategory, State.Unselect);
        _appWindowsManager.AppWidowInstants = instantiateApplication;
    }

    void BackTo()
    {
        switch (_menuType)
        {
            case MenuTypes.Add:
                _addAppPanel.SetActive(false);
                _deleteAppPanel.SetActive(false);
                _home.SetActive(false);
                _settingsPanel.SetActive(true);
                _firstSelectButtonOnSetting.Select();
                _menuType = MenuTypes.Settings;
                ClearAddAppInfo();
                break;
            case MenuTypes.Delete:
                _home.SetActive(false);
                _deleteAppPanel.SetActive(false);
                _addAppPanel.SetActive(false);
                _settingsPanel.SetActive(true);
                _firstSelectButtonOnSetting.Select();
                _menuType = MenuTypes.Settings;
                break;
            case MenuTypes.Settings:
                foreach (App app in _apps)
                {
                    CreateAppWindow(app);
                }

                _appWindowsManager.Initialize();
                _settingsPanel.SetActive(false);
                _addAppPanel.SetActive(false);
                _deleteAppPanel.SetActive(false);
                _home.SetActive(true);
                _menuType = MenuTypes.Home;
                break;
            case MenuTypes.Menu:
                _menuType = MenuTypes.Home;
                break;
        }
    }

    void ClearAddAppInfo()
    {
        _setFileName = "";
        _setImageFileName = "";
        _setGameTitle = "";
        _setGameCategory = GameCategory.Action;
        _fileSelectButtonText.GetComponent<Text>().text = "ゲームのexeファイルを選択してください";
        _imageFileSelectButtonText.GetComponent<Text>().text = "ゲームアイコンのpngファイルを選択してください";
        _titleNameEnter.GetComponent<InputField>().text = "";
        //DropDownのやつも元に戻すこと
        _gameCategoryDropdown.GetComponent<Dropdown>().value = 0;
    }

    int ChangeSelectingGameCategory(int change)
    {
        int gameCategory = _selectingGameCategory;
        gameCategory += change;
        if (gameCategory < -1)
        {
            //-1はALL
            gameCategory = Enum.GetNames(typeof(GameCategory)).Length - 1;
        }
        else if (gameCategory > Enum.GetNames(typeof(GameCategory)).Length - 1)
        {
            gameCategory = -1;
        }

        return gameCategory;
    }

    void LoadApplication()
    {
        _apps = new List<App>();

        string[] gameNames = PlayerPrefsX.GetStringArray("AppNames");
        string[] gameFileNames = PlayerPrefsX.GetStringArray("AppFileNames");
        string[] gameImageFileNames = PlayerPrefsX.GetStringArray("AppImageFileNames");
        int[] gameCategories = PlayerPrefsX.GetIntArray("GameCategories");

        if (gameNames.Length == 0 || gameFileNames.Length == 0 || gameCategories.Length == 0) return;

        for (int i = 0; i < gameNames.Length; i++)
        {
            _apps.Add(new App(gameNames[i], gameFileNames[i], gameImageFileNames[i],
                ExchangeGameCategoryFromInt(gameCategories[i])));
        }
    }

    void SaveApplication()
    {
        string[] gameNames = new string[_apps.Count];
        string[] gameFileNames = new string[_apps.Count];
        string[] gameImageFileNames = new string[_apps.Count];
        int[] gameCategories = new int[_apps.Count];

        int i = 0;

        foreach (App app in _apps)
        {
            gameNames[i] = app.GameName;
            gameFileNames[i] = app.FileName;
            gameImageFileNames[i] = app.ImageFileName;
            gameCategories[i] = (int) app.GameCategory;
            i++;
        }

        PlayerPrefsX.SetStringArray("AppNames", gameNames);
        PlayerPrefsX.SetStringArray("AppFileNames", gameFileNames);
        PlayerPrefsX.SetStringArray("AppImageFileNames", gameImageFileNames);
        PlayerPrefsX.SetIntArray("GameCategories", gameCategories);
    }

    public void MoveToAddApp()
    {
        _firstSelectButtonOnAdd.Select();
        _settingsPanel.SetActive(false);
        _addAppPanel.SetActive(true);
        _menuType = MenuTypes.Add;
    }

    public void MoveToDeleteApp()
    {
        _firstSelectButtonOnDelete.Select();
        _settingsPanel.SetActive(false);
        _deleteAppPanel.SetActive(true);
        _menuType = MenuTypes.Delete;
    }

    public void CreateApplication()
    {
        //決定ボタンを押されたときの処理
        _setGameTitle = _titleNameEnter.GetComponent<InputField>().text;
        if (_setFileName == "" || _setGameTitle == "")
        {
            //不正な音を鳴らす
            Debug.Log(_setFileName + ", " + _setGameTitle);
            return;
        }

        //ゲームカテゴリをセット
        _setGameCategory = ExchangeGameCategoryFromInt(_gameCategoryDropdown.GetComponent<Dropdown>().value);
        _apps.Add(new App(_setGameTitle, _setFileName, _setImageFileName, _setGameCategory));

        //作成したデータの保存
        SaveApplication();

        //設定画面に戻る
        BackTo();
    }

    public void OpenFile()
    {
        if (_openDialogFlag) return;
        //Fileのパスを取得
        _setFileName = "";

        OpenFileDialog openFileDialog = new OpenFileDialog();

        openFileDialog.Filter = "exeファイル|*.exe";

        openFileDialog.CheckFileExists = false;

        _eventSystem.enabled = false;
        _openDialogFlag = true;

        openFileDialog.ShowDialog();

        _eventSystem.enabled = true;
        _openDialogFlag = false;
        GameObject.Find("FileSelectButton").GetComponent<Button>().Select();

        Debug.Log("ShowDialog Off");

        //Debug.Log(_setFileName);
        if (openFileDialog.FileName == null)
        {
            Debug.Log("null");
            _setFileName = "";
            _fileSelectButtonText.GetComponent<Text>().text = "ゲームのexeファイルを選択してください";
        }
        else
        {
            Debug.Log("Not null");
            Debug.Log(openFileDialog.FileName);
            if (openFileDialog.FileName == "")
            {
                _setFileName = "";
                _fileSelectButtonText.GetComponent<Text>().text = "ゲームのexeファイルを選択してください";
            }
            else
            {
                _setFileName = openFileDialog.FileName;
                Debug.Log("SetFile:" + _setFileName);
                _fileSelectButtonText.GetComponent<Text>().text = _setFileName;
            }
        }

        openFileDialog.Reset();
    }

    public void OpenImageFile()
    {
        if (_openDialogFlag) return;
        _setImageFileName = "";

        OpenFileDialog openFileDialog1 = new OpenFileDialog();

        openFileDialog1.Filter = "pngファイル|*.png";

        openFileDialog1.CheckFileExists = false;

        _openDialogFlag = true;
        _eventSystem.enabled = false;

        openFileDialog1.ShowDialog();

        _eventSystem.enabled = true;
        _openDialogFlag = false;

        GameObject.Find("ImageFileSelectButton").GetComponent<Button>().Select();

        if (openFileDialog1.FileName == null)
        {
            Debug.Log("null");
            _setImageFileName = "";
            _imageFileSelectButtonText.GetComponent<Text>().text = "ゲームアイコンのpngファイルを選択してください";
        }
        else
        {
            Debug.Log("not null");
            if (openFileDialog1.FileName == "")
            {
                _setImageFileName = "";
                _imageFileSelectButtonText.GetComponent<Text>().text = "ゲームアイコンのpngファイルを選択してください";
            }
            else
            {
                _setImageFileName = openFileDialog1.FileName;
                Debug.Log("setImg:" + _setImageFileName);
                _imageFileSelectButtonText.GetComponent<Text>().text = _setImageFileName;
            }
        }

        openFileDialog1.Reset();
    }

    public void OnValueChanged(string result)
    {
        //InputEnter
        Debug.Log(result);
        _setGameTitle = result;
    }

/*
 * 未使用
    public void OnValueChanged(Dropdown dropdown)
    {
        Debug.Log("Result:" + dropdown.value);
        //DropDownの値の変化時
        _setGameCategory = ExchangeGameCategoryFromInt(dropdown.value);
    }
*/
    public GameCategory ExchangeGameCategoryFromInt(int gameCategory)
    {
        switch (gameCategory)
        {
            case 0:
                return GameCategory.Action;
            case 1:
                return GameCategory.Shooting;
            case 2:
                return GameCategory.Music;
            case 3:
                return GameCategory.Application;
            case 4:
                return GameCategory.Others;
            default:
                return GameCategory.Action;
        }
    }

    public void DeleteApplication()
    {
        _apps = new List<App>();
        SaveApplication();
        BackTo();
    }

    public void AnimationGameCategory()
    {
        _gameCategoryAnimator.SetTrigger("Anim");
    }

    public GameObject BackGround
    {
        get { return _backGround; }
    }

    public GameObject GameTitleObject
    {
        get { return _gameTitleObject; }
    }

    public int SelectingGameCategory
    {
        get { return _selectingGameCategory; }
    }

    public static MenuTypes MenuType
    {
        get { return _menuType; }
    }

    public enum GameCategory
    {
        Action = 0,
        Shooting = 1,
        Music = 2,
        Application = 4,
        Others = 3
    }

    public enum State
    {
        Select,
        Unselect
    }

    public enum MenuTypes
    {
        Home,
        Settings,
        Add,
        Delete,
        Menu
    }
}