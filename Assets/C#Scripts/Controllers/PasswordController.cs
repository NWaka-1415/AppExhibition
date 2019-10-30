using UnityEngine;
using UnityEngine.UI;

namespace Controllers
{
    public class PasswordController : MonoBehaviour
    {
        private static PasswordController _instance = null;

        public static PasswordController Instance
        {
            get { return _instance; }
        }

        [SerializeField] private InputField passwordSetInputField = null;
        [SerializeField] private InputField passwordSetCheckInputField = null;
        [SerializeField] private InputField passwordCheckInputField = null;
        [SerializeField] private Button okButton = null;
        [SerializeField] private Button cancelButton = null;

        private static string _password;

        public static string Password
        {
            get { return _password; }
        }

        private void Awake()
        {
            if (_instance == null) _instance = this;
            else if (_instance != this) Destroy(gameObject);
        }

        private void Start()
        {
            okButton.onClick.AddListener(OnclickPasswordCheck);
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
            }
            else
            {
                //パスワードが間違っていた際の挙動
                passwordCheckInputField.text = "";
            }
        }
    }
}