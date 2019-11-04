using UnityEngine;
using UnityEngine.UI;

namespace Controllers
{
    public class PasswordController : MonoBehaviour
    {
        private static PasswordController _instance = null;

        public static PasswordController Instance => _instance;

        [SerializeField] private InputField passwordSetInputField = null;
        [SerializeField] private InputField passwordSetCheckInputField = null;
        [SerializeField] private InputField passwordCheckInputField = null;
        [SerializeField] private Button okButton = null;
        [SerializeField] private Button okSetButton = null;

        private static bool _setPassword;

        public static bool SetPassword => _setPassword;

        private static string _password;

        public static string Password
        {
            get { return _password; }
            set
            {
                if (_setPassword) return;
                if (value == "") return;
                _password = value;
                _setPassword = true;
            }
        }

        private void Awake()
        {
            if (_instance == null) _instance = this;
            else if (_instance != this) Destroy(gameObject);
            _setPassword = false;
        }

        private void Start()
        {
            okButton.onClick.AddListener(OnclickPasswordCheck);
            okSetButton.onClick.AddListener(OnclickSetPassword);
        }

        public void ResetPassCheckField()
        {
            passwordCheckInputField.text = "";
        }

        /// <summary>
        /// パスワードの登録を行う
        /// </summary>
        private void OnclickSetPassword()
        {
            if (passwordSetInputField.text == passwordSetCheckInputField.text && passwordSetInputField.text != "")
            {
                //確認用と一致した際の挙動
                _password = passwordSetInputField.text;
                OverAllManager.Instance.MoveToHomeFromWelcomes();
            }
            else
            {
                //確認用と不意一致の際の挙動
                passwordSetCheckInputField.text = "";
            }
        }

        /// <summary>
        /// パスワード入力後OK(続ける)によって呼ばれる
        /// パスワードのチェックを行う
        /// </summary>
        private void OnclickPasswordCheck()
        {
            string pass = passwordCheckInputField.text;
            if (pass == _password)
            {
                //パスワードが正しかった際の挙動
                OverAllManager.Instance.CheckPass();
            }
            else
            {
                //パスワードが間違っていた際の挙動
                passwordCheckInputField.text = "";
            }
        }
    }
}