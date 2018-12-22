using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AppWindowsManager : MonoBehaviour
{
    //アプリウィンドウオブジェクトを管理するクラス
    private List<GameObject> _appWidowInstants;
    private int _selectedNumber;
    private int _previousSelectNumber;
    private OverAllManager.GameCategory _previousGameCategory;
    private static readonly Vector2 DefaultPos = new Vector2(175f, -89.55002f);
    private const float SelectBetweenUnselectRight = 155f;
    private const float UnselectBetweenUnselect = 92.5f;
    private bool _isInit = false;

    private bool _isExecute;

    private Vector2 _prevDPad = new Vector2(0f, 0f);

    // Update is called once per frame
    void Update()
    {
        if (OverAllManager.MenuType != OverAllManager.MenuTypes.Home) return;
        //Homeじゃなければ処理終了
        if (_appWidowInstants.Count == 0) return;

        if (_appWidowInstants[_selectedNumber].GetComponent<ApplicationWindow>().GameCategory != _previousGameCategory)
        {
            //ゲームカテゴリの表示を変更(未完，本来はアイコンも変更)
            _previousGameCategory = _appWidowInstants[_selectedNumber].GetComponent<ApplicationWindow>().GameCategory;
        }

        //アプリが起動したときの動作
        if (_appWidowInstants[_selectedNumber].GetComponent<ApplicationWindow>().ExecuteFlag)
        {
            _isExecute = true;
            //背景OFF
            GetComponent<OverAllManager>().BackGround.SetActive(false);
            //ゲームタイトルOFF
            GetComponent<OverAllManager>().GameTitleObject.GetComponent<Text>().enabled = false;
            //ゲームカテゴリOFF
            GetComponent<OverAllManager>().SelectingGameCategoryObject.SetActive(false);
            for (int i = 0; i < 2; i++)
            {
                GetComponent<OverAllManager>().SetImageAlpha(i, 0f);
            }

            foreach (GameObject appWidowInstant in _appWidowInstants)
            {
                if (!appWidowInstant.GetComponent<ApplicationWindow>().ExecuteFlag)
                {
                    appWidowInstant.GetComponent<ApplicationWindow>().AppImage.GetComponent<Image>().color =
                        new Color(255f, 255f, 255f, 0f);
                }
            }
        }
        else
        {
            _isExecute = false;
            //背景ON
            GetComponent<OverAllManager>().BackGround.SetActive(true);
            //ゲームタイトルON
            GetComponent<OverAllManager>().GameTitleObject.GetComponent<Text>().enabled = true;
            //ゲームカテゴリON
            GetComponent<OverAllManager>().SelectingGameCategoryObject.SetActive(true);
            for (int i = 0; i < 2; i++)
            {
                GetComponent<OverAllManager>().SetImageAlpha(i, 255f);
            }

            foreach (GameObject appWidowInstant in _appWidowInstants)
            {
                appWidowInstant.GetComponent<ApplicationWindow>().AppImage.GetComponent<Image>().color =
                    new Color(255f, 255f, 255f, 255f);
            }
        }

        if (_isExecute) return;
        Vector2 dPad = new Vector2(Input.GetAxisRaw("Horizontal_DPad"), Input.GetAxisRaw("Vertical_DPad"));

        if (Input.GetButtonDown("Left") || (dPad.x < 0 && Mathf.Abs(_prevDPad.x - dPad.x) > 0))
        {
            Debug.Log("<color=red>Axis Horizontal:" + dPad.x + "</color>");
            _selectedNumber--;
        }
        else if (Input.GetButtonDown("Right") || dPad.x > 0 && Mathf.Abs(_prevDPad.x - dPad.x) > 0)
        {
            Debug.Log("<color=red>Axis Horizontal:" + dPad.x + "</color>");
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
            _appWidowInstants[_previousSelectNumber].GetComponent<ApplicationWindow>()
                .State = OverAllManager.State.Unselect;
            //今回選択状態になったものを選択状態に
            _appWidowInstants[_selectedNumber].GetComponent<ApplicationWindow>()
                .State = OverAllManager.State.Select;

            MoveAppWindow();

            _previousSelectNumber = _selectedNumber;
        }
    }

    public void Initialize()
    {
        if (!_isInit)
        {
            _appWidowInstants = new List<GameObject>();
            _isInit = true;
        }

        if (_appWidowInstants.Count == 0) return;

        _appWidowInstants[0].GetComponent<ApplicationWindow>().State = OverAllManager.State.Select;
        _appWidowInstants[0].GetComponent<ApplicationWindow>().MoveWindow(DefaultPos);

        _previousSelectNumber = 0;
        _selectedNumber = 0;

        _previousGameCategory =
            _appWidowInstants[_selectedNumber].GetComponent<ApplicationWindow>().GameCategory;

        for (int i = 1; i < _appWidowInstants.Count; i++)
        {
            _appWidowInstants[i].GetComponent<ApplicationWindow>().State = OverAllManager.State.Select;
            _appWidowInstants[i].GetComponent<ApplicationWindow>().State = OverAllManager.State.Unselect;
            _appWidowInstants[i].GetComponent<ApplicationWindow>().MoveWindow(
                DefaultPos + new Vector2(SelectBetweenUnselectRight + UnselectBetweenUnselect * (i - 1), 0f));
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

        _appWidowInstants = new List<GameObject>();
    }

    void MoveAppWindow()
    {
        //Debug.Log("SelectNumber:" + _selectedNumber);
        //選択状態のアプリケーションウィンドウポジションを設定
        _appWidowInstants[_selectedNumber].GetComponent<ApplicationWindow>().MoveWindow(DefaultPos);

        int j = 0;
        for (int i = _selectedNumber - 1; i >= 0; i--)
        {
            //選択状態パネルの左側
            _appWidowInstants[i].GetComponent<ApplicationWindow>().MoveWindow(
                DefaultPos - new Vector2(UnselectBetweenUnselect * (j + 1), 0f));
            j++;
            //Debug.Log(_appWidowInstants[i].transform.localPosition);
        }

        j = 0;
        for (int i = _selectedNumber + 1; i < _appWidowInstants.Count; i++)
        {
            //選択状態パネルの右側
            _appWidowInstants[i].GetComponent<ApplicationWindow>().MoveWindow(
                DefaultPos + new Vector2(SelectBetweenUnselectRight + UnselectBetweenUnselect * j, 0f));
            //Debug.Log("後半,i=" + i + "LocalPos:" + _appWidowInstants[i].transform.localPosition);
            j++;
        }
    }

    public GameObject AppWidowInstants
    {
        set { _appWidowInstants.Add(value); }
    }

    public string GetSelectAppTitle()
    {
        return _appWidowInstants.Count == 0
            ? ""
            : " " + _appWidowInstants[_selectedNumber].GetComponent<ApplicationWindow>().GameName;
    }

    public int GetSelectAppNumber()
    {
        return _selectedNumber;
    }
}