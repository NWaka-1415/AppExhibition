using System.Collections.Generic;
using Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers
{
    public class AppWindowsManager : MonoBehaviour
    {
        //アプリウィンドウオブジェクトを管理するクラス
        private List<ApplicationWindow> _appWidowInstants;
        private int _selectedNumber;
        private int _previousSelectNumber;
        private Enums.GameCategory _previousGameCategory;
        private static readonly Vector2 DefaultPos = new Vector2(175f, -89.55002f);
        private const float SelectBetweenUnselectRight = 155f;
        private const float UnselectBetweenUnselect = 92.5f;
        private bool _isInit = false;

        private bool _isExecute;

        private Vector2 _prevDPad = new Vector2(0f, 0f);

        // Update is called once per frame
        void Update()
        {
            if (OverAllManager.Instance.CurrentMenuType != Enums.MenuType.Home) return;
            //Homeじゃなければ処理終了
            if (_appWidowInstants.Count == 0) return;

            if (_appWidowInstants[_selectedNumber].GameCategory != _previousGameCategory)
            {
                //ゲームカテゴリの表示を変更(未完，本来はアイコンも変更)
                _previousGameCategory = _appWidowInstants[_selectedNumber].GameCategory;
                OverAllManager.Instance.SetCategoryIcon(0,
                    OverAllManager.Instance.ExchangeIntFromGameCategory(_previousGameCategory));
            }

            //アプリが起動したときの動作
            if (_appWidowInstants[_selectedNumber].ExecuteFlag)
            {
                _isExecute = true;
                //背景OFF
                OverAllManager.Instance.BackGround.SetActive(false);
                //ゲームタイトルOFF
                OverAllManager.Instance.GameTitleObject.GetComponent<Text>().enabled = false;
                //ゲームカテゴリOFF
                OverAllManager.Instance.SelectingGameCategoryObject.SetActive(false);
                for (int i = 0; i < 2; i++)
                {
                    OverAllManager.Instance.SetImageAlpha(i, 0f);
                }

                foreach (ApplicationWindow appWidowInstant in _appWidowInstants)
                {
                    if (!appWidowInstant.ExecuteFlag)
                    {
                        appWidowInstant.AppImage.GetComponent<Image>().color =
                            new Color(255f, 255f, 255f, 0f);
                    }
                }
            }
            else
            {
                _isExecute = false;
                //背景ON
                OverAllManager.Instance.BackGround.SetActive(true);
                //ゲームタイトルON
                OverAllManager.Instance.GameTitleObject.GetComponent<Text>().enabled = true;
                //ゲームカテゴリON
                OverAllManager.Instance.SelectingGameCategoryObject.SetActive(true);
                for (int i = 0; i < 2; i++)
                {
                    OverAllManager.Instance.SetImageAlpha(i, 255f);
                }

                foreach (ApplicationWindow appWidowInstant in _appWidowInstants)
                {
                    appWidowInstant.AppImage.GetComponent<Image>().color =
                        new Color(255f, 255f, 255f, 255f);
                }
            }

            if (_isExecute) return;
            Vector2 dPad = new Vector2(Input.GetAxisRaw("Horizontal_DPad"), Input.GetAxisRaw("Vertical_DPad"));

            if (Input.GetButtonDown("Left") || (dPad.x < 0 && Mathf.Abs(_prevDPad.x - dPad.x) > 0))
            {
                Debug.Log("<color=red>Axis Horizontal:" + dPad.x + "</color>");
                AudioController.Instance.Play(AudioController.AudioPattern.Move);
                _selectedNumber--;
            }
            else if (Input.GetButtonDown("Right") || dPad.x > 0 && Mathf.Abs(_prevDPad.x - dPad.x) > 0)
            {
                Debug.Log("<color=red>Axis Horizontal:" + dPad.x + "</color>");
                AudioController.Instance.Play(AudioController.AudioPattern.Move);
                _selectedNumber++;
            }

            _prevDPad = dPad;

            if (_selectedNumber < 0)
            {
                _selectedNumber = _appWidowInstants.Count - 1;
            }
            else if (_selectedNumber >= _appWidowInstants.Count)
            {
                _selectedNumber = 0;
            }

            if (_selectedNumber != _previousSelectNumber)
            {
                //以前選択状態だったものを非選択状態に
                _appWidowInstants[_previousSelectNumber].State = Enums.State.Unselect;
                //今回選択状態になったものを選択状態に
                _appWidowInstants[_selectedNumber].State = Enums.State.Select;

                MoveAppWindow();

                _previousSelectNumber = _selectedNumber;
            }
        }

        public void Initialize()
        {
            if (!_isInit)
            {
                _appWidowInstants = new List<ApplicationWindow>();
                _isInit = true;
            }

            if (_appWidowInstants.Count == 0) return;

            _appWidowInstants[0].State = Enums.State.Select;
            _appWidowInstants[0].MoveWindow(DefaultPos);

            _previousSelectNumber = 0;
            _selectedNumber = 0;

            _previousGameCategory = _appWidowInstants[_selectedNumber].GameCategory;
            OverAllManager.Instance.SetCategoryIcon(0,
                OverAllManager.Instance.ExchangeIntFromGameCategory(_previousGameCategory));

            for (int i = 1; i < _appWidowInstants.Count; i++)
            {
                _appWidowInstants[i].State = Enums.State.Select;
                _appWidowInstants[i].State = Enums.State.Unselect;
                _appWidowInstants[i].MoveWindow(DefaultPos +
                                                new Vector2(
                                                    SelectBetweenUnselectRight + UnselectBetweenUnselect * (i - 1),
                                                    0f));
                //Debug.Log(_appWidowInstants[i].transform.localPosition.x + "=" + DefaultPos.x + "+" +
                //SelectBetweenUnselectRight + "+" + UnselectBetweenUnselect + "*" + (i - 1));
            }
        }

        public void ResetAppInstants()
        {
            for (int i = 0; i < _appWidowInstants.Count; i++)
            {
                Destroy(_appWidowInstants[i]);
            }

            _appWidowInstants = new List<ApplicationWindow>();
        }

        void MoveAppWindow()
        {
            //Debug.Log("SelectNumber:" + _selectedNumber);
            //選択状態のアプリケーションウィンドウポジションを設定
            _appWidowInstants[_selectedNumber].MoveWindow(DefaultPos);

            int j = 0;
            for (int i = _selectedNumber - 1; i >= 0; i--)
            {
                //選択状態パネルの左側
                _appWidowInstants[i].MoveWindow(DefaultPos - new Vector2(UnselectBetweenUnselect * (j + 1), 0f));
                j++;
                //Debug.Log(_appWidowInstants[i].transform.localPosition);
            }

            j = 0;
            for (int i = _selectedNumber + 1; i < _appWidowInstants.Count; i++)
            {
                //選択状態パネルの右側
                _appWidowInstants[i]
                    .MoveWindow(DefaultPos + new Vector2(SelectBetweenUnselectRight + UnselectBetweenUnselect * j, 0f));
                //Debug.Log("後半,i=" + i + "LocalPos:" + _appWidowInstants[i].transform.localPosition);
                j++;
            }
        }

        public ApplicationWindow AppWidowInstants
        {
            set => _appWidowInstants.Add(value);
        }

        public bool IsExecute => _isExecute;

        public string GetSelectAppTitle()
        {
            return _appWidowInstants.Count == 0
                ? ""
                : " " + _appWidowInstants[_selectedNumber].GameName;
        }

        public int GetSelectAppNumber()
        {
            return _selectedNumber;
        }

        public string GetSelectAppImageFileName()
        {
            //選択中のアプリケーションのイメージファイル名を返す
            if (_appWidowInstants.Count == 0) return "";
            foreach (ApplicationWindow appWidowInstant in _appWidowInstants)
            {
                if (appWidowInstant.State == Enums.State.Select)
                    return appWidowInstant.GameImage;
            }

            return "";
        }

        public string GetSelectAppArgFileName()
        {
            if (_appWidowInstants.Count == 0) return "";
            foreach (ApplicationWindow appWidowInstant in _appWidowInstants)
            {
                if (appWidowInstant.State == State.Select)
                    return appWidowInstant.GameImage;
            }

            return "";
        }
    }
}