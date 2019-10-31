using System;
using System.Collections.Generic;
using System.Windows.Forms;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
using Cursor = UnityEngine.Cursor;
using Screen = UnityEngine.Screen;

namespace Controllers
{
    public class OverAllManager : MonoBehaviour
    {
        private static OverAllManager _instance = null;

        public static OverAllManager Instance => _instance;

        //EventSystem
        [SerializeField] private EventSystem _eventSystem; //イベントシステム　入力受付停止などに用いる

        //GameObjects
        [SerializeField] private GameObject _appWindowsPrefabs; //アプリケーションウィンドウのプレハブ
        [SerializeField] private GameObject _gameTitleObject; //ゲームタイトルを表示するテキストオブジェクト
        [SerializeField] private GameObject _gameCategoryIconObject; //選択されているゲームのカテゴリアイコン
        [SerializeField] private GameObject _selectingGameCategoryObject; //選択中のゲームカテゴリを表示するテキストオブジェクト
        [SerializeField] private GameObject _backGround; //バックグラウンドオブジェクト（ただの背景）
        [SerializeField] private GameObject _home; //Home画面のオブジェクトをまとめるからのオブジェクト
        [SerializeField] private GameObject _menu; //Home画面で表示されるメニューオブジェクト
        [SerializeField] private GameObject _settingsPanel; //Settings画面のパネル
        [SerializeField] private GameObject _addAppPanel; //Add画面のパネル
        [SerializeField] private GameObject _deleteAppPanel; //Delete画面のパネル
        [SerializeField] private GameObject _editPanel; //Edit画面のパネル
        [SerializeField] private GameObject _dialogPanel; //Dialog画面のパネル
        [SerializeField] private GameObject _passwordSetPanel = null;
        [SerializeField] private GameObject _passwordCheckPanel = null;
        [SerializeField] private GameObject _welcomePanel = null;
        [SerializeField] private GameObject _fileSelectButtonText; //アプリケーションファイル選択ダイアログオープンボタンオブジェクト in Add
        [SerializeField] private GameObject _imageFileSelectButtonText; //イメージファイル選択ダイアログオープンボタンオブジェクト in Add
        [SerializeField] private GameObject _titleNameEnter; //ゲームタイトルインプットフィールドオブジェクト in Add
        [SerializeField] private GameObject _gameCategoryDropdown; //ゲームカテゴリドロップダウンオブジェクト in Add

        [SerializeField]
        private GameObject _fileSelectButtonTextOnEdit; //Edit画面のアプリケーションファイル選択ダイアログオープンボタンオブジェクト in Edit

        [SerializeField]
        private GameObject _imageFileSelectButtonTextOnEdit; //Edit画面のイメージファイル選択ダイアログオープンボタンオブジェクト in Edit

        [SerializeField] private GameObject _titleNameEnterOnEdit; //Edit画面のタイトル入力フィード
        [SerializeField] private GameObject _gameCategoryDropDownOnEdit; //Edit画面のカテゴリードロップダウンオブジェクト

        //Buttons
        [SerializeField] private Button _firstSelectButtonOnSetting; //最初に選択されているボタン in Setting
        [SerializeField] private Button _firstSelectButtonOnAdd; //最初に選択されているボタン in Add
        [SerializeField] private Button _firstSelectButtonOnDelete; //最初に選択されているボタン in Delete
        [SerializeField] private Button _firstSelectButtonOnDialog; //最初に選択されているボタン in Dialog
        [SerializeField] private Button _firstSelectButtonOnEdit; //最初に選択されている in Edit
        [SerializeField] private Button nextButtonOnWelcome;
        [SerializeField] private InputField firstSelectOnPassSet = null;
        [SerializeField] private InputField firstSelectOnPassCheck = null;

        //Animators
        [SerializeField] private Animator _gameCategoryAnimator; //ゲームカテゴリBackGroundのコンポーネント＜アニメーター＞

        //Sprites
        [SerializeField] private Sprite[] _gameCategoryIconSprites; //ゲームカテゴリの各種アイコンのSprite

        //Images
        [SerializeField] private Image[] _gameCategoryIcons; //ゲームカテゴリのアイコンオブジェクトのコンポーネントs＜Image＞
        [SerializeField] private Image _dialogIcon; //ダイアログ画面上のゲームアイコンオブジェクトのコンポーネント＜Image＞
        [SerializeField] private Image _editIcon; //Edit画面のゲームアイコンオブジェクトのコンポーネント＜Image＞

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
        private bool _isMenuOpen; //メニューが開かれているか
        private bool _isEdit; //Editモードかどうか

        private int _selectingGameCategory; //選ばれているゲームカテゴリ
        private int _previousSelectingGameCategory; //前フレームで選ばれていたゲームカテゴリ

        private bool _isShowWelcomePage;

        private MenuTypes _willMenuType;

        class App
        {
            public App(string gameName, string fileName, string imageFileName, GameCategory gameCategory)
            {
                GameName = gameName;
                FileName = fileName;
                ImageFileName = imageFileName;
                GameCategory = gameCategory;
            }

            public string GameName { get; set; }

            public string FileName { get; set; }

            public string ImageFileName { get; set; }

            public GameCategory GameCategory { get; set; }
        }

        private void Awake()
        {
            if (_instance == null) _instance = this;
            else if (_instance != this) Destroy(gameObject);
            Cursor.visible = false;
        }

        // Use this for initialization
        void Start()
        {
            Screen.fullScreen = true;
            Screen.SetResolution(1024, 576, FullScreenMode.FullScreenWindow);

            LoadApplication();
            _menuType = _isShowWelcomePage ? MenuTypes.Welcome : MenuTypes.Home;
            switch (_menuType)
            {
                case MenuTypes.Welcome:
                    _welcomePanel.SetActive(true);
                    _home.SetActive(false);
                    nextButtonOnWelcome.Select();
                    break;
                case MenuTypes.Home:
                    _welcomePanel.SetActive(false);
                    _home.SetActive(true);
                    break;
                default:
                    break;
            }

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

            _isMenuOpen = false;
            _isEdit = false;
            _settingsPanel.SetActive(false);

            _setFileName = "";
            _setGameTitle = "";
            _setGameCategory = GameCategory.Action;

            _selectingGameCategory = -1;
            _previousSelectingGameCategory = _selectingGameCategory;

            SetCategoryIcon(1, _selectingGameCategory);
        }

        private void Update()
        {
            if (_appWindowsManager.IsExecute) return;

            if (_menuType == MenuTypes.Home)
            {
                _gameTitleObject.GetComponent<Text>().text = _appWindowsManager.GetSelectAppTitle();
                //Debug.Log(_apps.Count);
                _gameCategoryIconObject.SetActive(_apps.Count > 0);
            }

            if (Input.GetButtonDown("Cancel") && !_openDialogFlag)
            {
                Debug.Log("Pressed Cancel");
                AudioController.Instance.Play(AudioController.AudioPattern.Cancel);
                BackTo();
            }
            else if (Input.GetButtonDown("Ok")) AudioController.Instance.Play(AudioController.AudioPattern.Select);
            else if (Input.GetButtonDown("Y"))
            {
                Debug.Log("Pressed Y");
                if (_menuType == MenuTypes.Home)
                {
                    AudioController.Instance.Play(AudioController.AudioPattern.Select);
                    _home.SetActive(false);
                    _settingsPanel.SetActive(true);
                    //_appWindowsManager.ResetAppInstants();
                    _firstSelectButtonOnSetting.Select();
                    _firstSelectButtonOnSetting.OnSelect(null);
                    _menuType = MenuTypes.Settings;
                }
            }
            else if (Input.GetButtonDown("Fire1"))
            {
                Debug.Log("Pressed Fire1");
                //RB
                AudioController.Instance.Play(AudioController.AudioPattern.Move);
                if (_menuType == MenuTypes.Home) _selectingGameCategory = ChangeSelectingGameCategory(-1);
            }
            else if (Input.GetButtonDown("Fire2"))
            {
                Debug.Log("Pressed Fire2");
                //LB
                AudioController.Instance.Play(AudioController.AudioPattern.Move);
                if (_menuType == MenuTypes.Home) _selectingGameCategory = ChangeSelectingGameCategory(1);
            }
            else if (Input.GetButtonDown("Menu"))
            {
                if (_menuType == MenuTypes.Home && _apps.Count > 0)
                {
                    AudioController.Instance.Play(AudioController.AudioPattern.Open);
                    _menuType = MenuTypes.Menu;
                    MenuChange(); //Open
                }
            }

            if (_menuType == MenuTypes.Home)
            {
                /*
             * カテゴリー別タイトル入れ替え
             */
                if (_selectingGameCategory != _previousSelectingGameCategory)
                {
                    _previousSelectingGameCategory = _selectingGameCategory;

                    _appWindowsManager.ResetAppInstants(); //既存のパネルを消し

                    SetCategoryIcon(1, _selectingGameCategory);

                    int count = 0; //アプリ数キャッシュ

                    //情報を新たに追加しなおし
                    if (_selectingGameCategory == -1)
                    {
                        //All
                        foreach (App app in _apps)
                        {
                            count++;
                            CreateAppWindow(app);
                        }
                    }
                    else
                    {
                        foreach (App app in _apps)
                        {
                            if (ExchangeGameCategoryFromInt(_selectingGameCategory) == app.GameCategory)
                            {
                                count++;
                                CreateAppWindow(app);
                            }
                        }
                    }

                    _gameCategoryIcons[0].color =
                        count == 0 ? new Color(255f, 255f, 255f, 0f) : new Color(255f, 255f, 255f, 255f);

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
                    if (nav.selectOnUp != null)
                    {
                        nav.selectOnUp.Select();
                        AudioController.Instance.Play(AudioController.AudioPattern.Move);
                    }
                }
                else if (Input.GetButtonDown("Down") || dPad.y < 0 && Mathf.Abs(_prevDPad.y - dPad.y) > 0)
                {
                    Debug.Log("<color=red>Axis Horizontal:" + dPad.y + "</color>");
                    if (nav.selectOnDown != null)
                    {
                        nav.selectOnDown.Select();
                        AudioController.Instance.Play(AudioController.AudioPattern.Move);
                    }
                }
                else if (dPad.x < 0 && Mathf.Abs(_prevDPad.x - dPad.x) > 0)
                {
                    //Left
                    if (nav.selectOnLeft != null)
                    {
                        nav.selectOnLeft.Select();
                        AudioController.Instance.Play(AudioController.AudioPattern.Move);
                    }
                }
                else if (dPad.x > 0 && Mathf.Abs(_prevDPad.x - dPad.x) > 0)
                {
                    //Right
                    if (nav.selectOnRight != null)
                    {
                        nav.selectOnRight.Select();
                        AudioController.Instance.Play(AudioController.AudioPattern.Move);
                    }
                }
            }


            _prevDPad = dPad;
        }

        void SetDropDownMenu(int value)
        {
            GameCategory gameCategory = ExchangeGameCategoryFromInt(value);
            _gameCategoryDropdown.GetComponent<Dropdown>().options[value].text = gameCategory.ToString();
            _gameCategoryDropDownOnEdit.GetComponent<Dropdown>().options[value].text = gameCategory.ToString();
        }

        void CreateAppWindow(App app)
        {
            GameObject instantiateApplication = Instantiate(_appWindowsPrefabs, _home.transform);
            instantiateApplication.GetComponent<ApplicationWindow>().Initialize(this, app.GameName, app.FileName,
                app.ImageFileName, app.GameCategory, State.Unselect);
            _appWindowsManager.AppWidowInstants = instantiateApplication;
        }

        void MenuChange()
        {
            //メニューの開閉を行う
            _isMenuOpen = !_isMenuOpen;
            _menu.GetComponent<Animator>().SetBool("isMenuOpen", _isMenuOpen);
            if (_isMenuOpen)
            {
                //メニューが開かれたら
                Button deleteButtonComp = _menu.transform.Find("DeleteButton").GetComponent<Button>();
                deleteButtonComp.Select();
            }
            else
            {
                //メニューが閉じたらボタンのフォーカスをオフ
                EventSystem.current.SetSelectedGameObject(null);
            }
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
                    _firstSelectButtonOnSetting.OnSelect(null);
                    _menuType = MenuTypes.Settings;
                    ClearAddAppInfo();
                    break;
                case MenuTypes.Delete:
                    _home.SetActive(false);
                    _deleteAppPanel.SetActive(false);
                    _addAppPanel.SetActive(false);
                    _settingsPanel.SetActive(true);
                    _firstSelectButtonOnSetting.Select();
                    _firstSelectButtonOnSetting.OnSelect(null);
                    _menuType = MenuTypes.Settings;
                    break;
                case MenuTypes.Settings:
                    _appWindowsManager.ResetAppInstants();
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
                    MenuChange();
                    break;
                case MenuTypes.IndividualDelete:
                    //アプリケーション個別削除からホームに戻る
                    _appWindowsManager.ResetAppInstants();
                    foreach (App app in _apps)
                    {
                        CreateAppWindow(app);
                    }

                    _appWindowsManager.Initialize();
                    _menu.SetActive(true);
                    _home.SetActive(true);
                    _dialogPanel.SetActive(false);
                    _menuType = MenuTypes.Home;
                    break;
                case MenuTypes.Edit:
                    _appWindowsManager.ResetAppInstants();
                    foreach (App app in _apps)
                    {
                        CreateAppWindow(app);
                    }

                    _appWindowsManager.Initialize();
                    _editPanel.SetActive(false);
                    _isEdit = false;
                    _home.SetActive(true);
                    _menu.SetActive(true);
                    _menuType = MenuTypes.Home;
                    break;
                case MenuTypes.PasswordSet:
                    _passwordSetPanel.SetActive(false);
                    _welcomePanel.SetActive(true);
                    nextButtonOnWelcome.Select();
                    break;
                case MenuTypes.PasswordCheck:
                    _passwordCheckPanel.SetActive(false);
                    PasswordController.Instance.ResetPassCheckField();
                    switch (_willMenuType)
                    {
                        case MenuTypes.Add:
                        case MenuTypes.Delete:
                            _firstSelectButtonOnSetting.Select();
                            _firstSelectButtonOnSetting.OnSelect(null);
                            _settingsPanel.SetActive(true);
                            _menuType = MenuTypes.Settings;
                            break;
                        case MenuTypes.IndividualDelete:
                        case MenuTypes.Edit:
                            _appWindowsManager.ResetAppInstants();
                            foreach (App app in _apps)
                            {
                                CreateAppWindow(app);
                            }

                            _appWindowsManager.Initialize();
                            _home.SetActive(true);
                            _menu.SetActive(true);
                            _menuType = MenuTypes.Home;
                            break;
                    }

                    break;
                case MenuTypes.Welcome:
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
            PasswordController.Password = PlayerPrefs.GetString("Password", "");
            _isShowWelcomePage = !PasswordController.SetPassword;

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
            PlayerPrefs.SetString("Password", PasswordController.Password);
        }

        public void MoveToHomeFromWelcomes()
        {
            _menuType = MenuTypes.Home;
            _home.SetActive(true);
            _welcomePanel.SetActive(false);
            _passwordSetPanel.SetActive(false);
            SaveApplication();
        }

        public void SetMoveToAddApp()
        {
            _willMenuType = MenuTypes.Add;
            _menuType = MenuTypes.PasswordCheck;
            _settingsPanel.SetActive(false);
            _passwordCheckPanel.SetActive(true);
            firstSelectOnPassCheck.Select();
            firstSelectOnPassCheck.OnSelect(null);
        }

        private void MoveToAddApp()
        {
            _firstSelectButtonOnAdd.Select();
            _firstSelectButtonOnAdd.OnSelect(null);
            _settingsPanel.SetActive(false);
            _addAppPanel.SetActive(true);
            _menuType = MenuTypes.Add;
            _willMenuType = MenuTypes.None;
        }

        public void SetMoveToDeleteApp()
        {
            _willMenuType = MenuTypes.Delete;
            _menuType = MenuTypes.PasswordCheck;
            _settingsPanel.SetActive(false);
            _passwordCheckPanel.SetActive(true);
            firstSelectOnPassCheck.Select();
        }

        private void MoveToDeleteApp()
        {
            _firstSelectButtonOnDelete.Select();
            _firstSelectButtonOnDelete.OnSelect(null);
            _settingsPanel.SetActive(false);
            _deleteAppPanel.SetActive(true);
            _menuType = MenuTypes.Delete;
            _willMenuType = MenuTypes.None;
        }

        public void CheckPass()
        {
            _passwordCheckPanel.SetActive(false);
            PasswordController.Instance.ResetPassCheckField();
            switch (_willMenuType)
            {
                case MenuTypes.Add:
                    MoveToAddApp();
                    break;
                case MenuTypes.Delete:
                    MoveToDeleteApp();
                    break;
                case MenuTypes.IndividualDelete:
                    OpenDialogWindow();
                    break;
                case MenuTypes.Edit:
                    EditApplication();
                    break;
            }
        }

        public void DeleteCheckPass()
        {
            MenuChange();
            _menu.SetActive(false);
            _home.SetActive(false);
            _willMenuType = MenuTypes.IndividualDelete;
            _menuType = MenuTypes.PasswordCheck;
            _passwordCheckPanel.SetActive(true);
            firstSelectOnPassCheck.Select();
            firstSelectOnPassCheck.OnSelect(null);
        }

        public void OpenDialogWindow()
        {
            //Dialogを開く
            //以前の画面に応じてDialogの種類を決定
            switch (_menuType)
            {
                case MenuTypes.Menu:
                    //Menuから移ってくる場合はアプリ単体削除Dialog
                    MenuChange(); //isOpenをfalseに
                    _menu.SetActive(false);
                    _home.SetActive(false);
                    _dialogIcon.color = new Color(255f, 255f, 255f, 255f); //アプリ単体削除ダイアログの場合はゲームのアイコンを表示
                    _dialogIcon.sprite =
                        SpriteEditor.SpriteFromFile(_appWindowsManager.GetSelectAppImageFileName());
                    _dialogPanel.SetActive(true);
                    _menuType = MenuTypes.IndividualDelete;
                    break;
                case MenuTypes.PasswordCheck:
                    //アプリ単体削除Dialog
                    _menu.SetActive(false);
                    _home.SetActive(false);
                    _dialogIcon.color = new Color(255f, 255f, 255f, 255f); //アプリ単体削除ダイアログの場合はゲームのアイコンを表示
                    _dialogIcon.sprite =
                        SpriteEditor.SpriteFromFile(_appWindowsManager.GetSelectAppImageFileName());
                    _dialogPanel.SetActive(true);
                    _menuType = MenuTypes.IndividualDelete;
                    break;
                default:
                    break;
            }

            _firstSelectButtonOnDialog.Select();
            _firstSelectButtonOnDialog.OnSelect(null);
        }

        public void OnClickedDialogOkButton()
        {
            //Dialog画面のOKボタンがクリックされた際の挙動
            switch (_menuType)
            {
                case MenuTypes.IndividualDelete:
                    //選択中のアプリを削除
                    _apps.RemoveAt(_appWindowsManager.GetSelectAppNumber());
                    SaveApplication();
                    _appWindowsManager.ResetAppInstants();
                    foreach (App app in _apps)
                    {
                        CreateAppWindow(app);
                    }

                    _appWindowsManager.Initialize();
                    _menu.SetActive(true);
                    _home.SetActive(true);
                    _dialogPanel.SetActive(false);
                    _menuType = MenuTypes.Home;
                    break;
                default:
                    break;
            }
        }

        public void OnClickedDialogCancelButton()
        {
            //キャンセルによって，前の画面に戻るだけ
            BackTo();
        }

        public void OnclickWelcomeToSetPass()
        {
            _menuType = MenuTypes.PasswordSet;
            _welcomePanel.SetActive(false);
            _passwordSetPanel.SetActive(true);
            firstSelectOnPassSet.Select();
            firstSelectOnPassSet.OnSelect(null);
        }

        public void ChangeApplicationInfo()
        {
            _setGameTitle = _titleNameEnterOnEdit.GetComponent<InputField>().text;
            if (_setFileName == "" || _setGameTitle == "")
            {
                return;
            }

            //ゲームカテゴリをセット
            _setGameCategory = ExchangeGameCategoryFromInt(_gameCategoryDropDownOnEdit.GetComponent<Dropdown>().value);

            _apps[_appWindowsManager.GetSelectAppNumber()].FileName = _setFileName;
            _apps[_appWindowsManager.GetSelectAppNumber()].ImageFileName = _setImageFileName;
            _apps[_appWindowsManager.GetSelectAppNumber()].GameName = _setGameTitle;
            _apps[_appWindowsManager.GetSelectAppNumber()].GameCategory = _setGameCategory;
            SaveApplication();

            BackTo();
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
            Text fileSelectButtonText = _fileSelectButtonText.GetComponent<Text>();
            if (_isEdit) fileSelectButtonText = _fileSelectButtonTextOnEdit.GetComponent<Text>();
            //Fileのパスを取得
            _setFileName = "";

            Cursor.visible = true;

            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "exeファイル|*.exe";

            openFileDialog.CheckFileExists = false;

            _eventSystem.enabled = false;
            _openDialogFlag = true;

            openFileDialog.ShowDialog();

            _eventSystem.enabled = true;
            _openDialogFlag = false;
            if (!_isEdit)
            {
                _firstSelectButtonOnAdd.Select();
                _firstSelectButtonOnAdd.OnSelect(null);
            }
            else
            {
                _firstSelectButtonOnEdit.Select();
                _firstSelectButtonOnEdit.OnSelect(null);
            }
            //GameObject.Find("FileSelectButton").GetComponent<Button>().Select();

            Debug.Log("ShowDialog Off");
            Cursor.visible = false;

            //Debug.Log(_setFileName);
            if (_isEdit)
            {
                fileSelectButtonText.text = _apps[_appWindowsManager.GetSelectAppNumber()].FileName;
            }

            if (openFileDialog.FileName == null)
            {
                Debug.Log("null");
                _setFileName = "";
                fileSelectButtonText.text =
                    _isEdit ? _apps[_appWindowsManager.GetSelectAppNumber()].FileName : "ゲームのexeファイルを選択してください";
            }
            else
            {
                Debug.Log("Not null");
                Debug.Log(openFileDialog.FileName);
                if (openFileDialog.FileName == "")
                {
                    _setFileName = "";
                    fileSelectButtonText.text =
                        _isEdit ? _apps[_appWindowsManager.GetSelectAppNumber()].FileName : "ゲームのexeファイルを選択してください";
                }
                else
                {
                    _setFileName = openFileDialog.FileName;
                    Debug.Log("SetFile:" + _setFileName);
                    fileSelectButtonText.text = _setFileName;
                }
            }

            openFileDialog.Reset();
        }

        public void OpenImageFile()
        {
            if (_openDialogFlag) return;
            _setImageFileName = "";

            Cursor.visible = true;
            
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Filter = "pngファイル|*.png";

            openFileDialog1.CheckFileExists = false;

            _openDialogFlag = true;
            _eventSystem.enabled = false;

            openFileDialog1.ShowDialog();

            _eventSystem.enabled = true;
            _openDialogFlag = false;

            Text imageFileSelectButtonText = _imageFileSelectButtonText.GetComponent<Text>();
            if (_isEdit) imageFileSelectButtonText = _imageFileSelectButtonTextOnEdit.GetComponent<Text>();
            GameObject.Find("ImageFileSelectButton").GetComponent<Button>().Select();

            Cursor.visible = false;

            if (_isEdit) imageFileSelectButtonText.text = _appWindowsManager.GetSelectAppImageFileName();

            if (openFileDialog1.FileName == null)
            {
                Debug.Log("null");
                _setImageFileName = _isEdit ? _appWindowsManager.GetSelectAppImageFileName() : "";
                imageFileSelectButtonText.text =
                    _isEdit ? _appWindowsManager.GetSelectAppImageFileName() : "ゲームアイコンのpngファイルを選択してください";
            }
            else
            {
                Debug.Log("not null");
                if (openFileDialog1.FileName == "")
                {
                    _setImageFileName = _isEdit ? _appWindowsManager.GetSelectAppImageFileName() : "";
                    imageFileSelectButtonText.text =
                        _isEdit ? _appWindowsManager.GetSelectAppImageFileName() : "ゲームアイコンのpngファイルを選択してください";
                }
                else
                {
                    _setImageFileName = openFileDialog1.FileName;
                    Debug.Log("setImg:" + _setImageFileName);
                    imageFileSelectButtonText.text = _setImageFileName;
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

        public int ExchangeIntFromGameCategory(GameCategory gameCategory)
        {
            switch (gameCategory)
            {
                case GameCategory.Action:
                    return 0;
                case GameCategory.Shooting:
                    return 1;
                case GameCategory.Music:
                    return 2;
                case GameCategory.Application:
                    return 3;
                case GameCategory.Others:
                    return 4;
                default:
                    return 0;
            }
        }

        public void DeleteApplication()
        {
            _apps = new List<App>();
            SaveApplication();
            BackTo();
        }

        public void SetEditApplication()
        {
            MenuChange();
            _willMenuType = MenuTypes.Edit;
            _menuType = MenuTypes.PasswordCheck;
            _menu.SetActive(false);
            _home.SetActive(false);
            _passwordCheckPanel.SetActive(true);
            firstSelectOnPassCheck.Select();
            firstSelectOnPassCheck.OnSelect(null);
        }

        private void EditApplication()
        {
            //Edit画面を開く
//            MenuChange(); //isOpenをfalseに
            _menu.SetActive(false);
            _home.SetActive(false);
            _editPanel.SetActive(true);

            _isEdit = true;

            _editIcon.color = new Color(255f, 255f, 255f, 255f); //一応
            _editIcon.sprite =
                SpriteEditor.SpriteFromFile(_appWindowsManager.GetSelectAppImageFileName());
            _menuType = MenuTypes.Edit;

            _setFileName = _apps[_appWindowsManager.GetSelectAppNumber()].FileName;
            _setImageFileName = _appWindowsManager.GetSelectAppImageFileName();
            _setGameTitle = _appWindowsManager.GetSelectAppTitle();
            _setGameCategory = _apps[_appWindowsManager.GetSelectAppNumber()].GameCategory;
            _fileSelectButtonTextOnEdit.GetComponent<Text>().text = _setFileName;
            _imageFileSelectButtonTextOnEdit.GetComponent<Text>().text = _setImageFileName;
            _titleNameEnterOnEdit.GetComponent<InputField>().text = _setGameTitle;
            //DropDownのやつも元に戻すこと
            _gameCategoryDropDownOnEdit.GetComponent<Dropdown>().value =
                (int) _apps[_appWindowsManager.GetSelectAppNumber()].GameCategory;

            _firstSelectButtonOnEdit.Select();
            _firstSelectButtonOnEdit.OnSelect(null);
        }

        public void AnimationGameCategory()
        {
            _gameCategoryAnimator.SetTrigger("Anim");
        }

        public void SetCategoryIcon(int number, int gameCategory)
        {
            if (gameCategory == -1) gameCategory = 5;
            _gameCategoryIcons[number].sprite = _gameCategoryIconSprites[gameCategory];
        }

        public void SetImageAlpha(int number, float alpha)
        {
            _gameCategoryIcons[number].color = new Color(255f, 255f, 255f, alpha);
        }

        public GameObject SelectingGameCategoryObject => _selectingGameCategoryObject;

        public GameObject BackGround => _backGround;

        public GameObject GameTitleObject => _gameTitleObject;

        public int SelectingGameCategory => _selectingGameCategory;

        public static MenuTypes MenuType => _menuType;

        public enum GameCategory
        {
            Action,
            Shooting,
            Music,
            Application,
            Others
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
            Menu,
            IndividualDelete,
            Edit,
            PasswordSet,
            PasswordCheck,
            Welcome,
            None
        }
    }
}