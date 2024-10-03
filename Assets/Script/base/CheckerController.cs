using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class CheckerController : MonoBehaviour
{
    public bool Updated;
    bool change;
    float Scale;
    
    [Header("Place")]
    public int ID;
    public List<GameSetup.ColumnClass> columnTarget = new List<GameSetup.ColumnClass>();
    public GameSetup.ColumnClass columnStart;
    public bool finished;
    
    [Header("Fix")]
    public bool Fixing;
    public Vector3 FixTarget;
    public float FixSpeed;
    public bool canFix;
    public float distanceToFix;
    public float multipy;
    
    [Header("Drag")]
    public bool canDrag;
    public bool isDragging;
    public float touchDistance;

    [Header("Audio")]
    public List<AudioSource> PlaceAudio;
    public bool played;
    public bool FirstAudio;
    public bool kickplayed;

    public enum side
    {
        White,
        Black
    }
    [Header("Side")]
    public side Side;

    [Header("HighLight")]
    public bool HighLight;
    public SpriteRenderer highlightSprite;

    [Header("Rotation")]
    public bool Stand;
    private bool isStand;

    [Header("Sprite")]
    public SpriteRenderer spriteRender;
    public SpriteRenderer finishSprite;
    
    [Header("Art")]
    public List<SpriteClass> checkerSprite;

    [System.Serializable]
    public class SpriteClass
    {
        public Sprite[] sprite;
        public Color[] color;
    }
    
    PauseController pauseController;
    GameSetup GameSetup;
    private SortingGroup _sortingGroup;
    private PlayerInfo _playerInfo;

    void Start()
    {
        GameSetup = FindObjectOfType<GameSetup>();
        pauseController = FindObjectOfType<PauseController>();
        _sortingGroup = GetComponent<SortingGroup>();
        _playerInfo = FindObjectOfType<PlayerInfo>();

        FirstAudio = true;
        kickplayed = true;
        HighLight = false;
        Scale = 1;
    }

    void Update()
    {
        if (isDragging)
            Scale = Mathf.Lerp(Scale,1.3f, 10 * Time.deltaTime);
        else
            Scale = Mathf.Lerp(Scale, 1, 10 * Time.deltaTime);
        
        transform.localScale = new Vector3(Scale, Scale, Scale);

        HighLight = canDrag && !isDragging && !GameSetup.disableCheckersHighLight && GameSetup.Roll.playerTurn && ((GameSetup.GeneralOptions.Online && !pauseController.Pause) || GameSetup.GeneralOptions.AI);

        if (!Updated && GameSetup.Updated)
        {
            SetSpriteSide();
            Updated = true;
        }

        distanceToFix = Vector3.Distance(transform.position, FixTarget);

        if (!Updated) { return; }
        
        Drag();
        Fix();
        setFinish();
        PlayPlaceAudio();
        SetHighLight();

        if (canDrag) Touch();
    }

    public void SetLayerOrder(string type)
    {
        if (_sortingGroup == null)
            _sortingGroup = GetComponent<SortingGroup>();
            
        switch (type)
        {
            case "place":
                if (_sortingGroup != null) _sortingGroup.sortingOrder = GetPlaceID();
                break;
            
            case "top":
                if (_sortingGroup != null) _sortingGroup.sortingOrder = 40;
                break;
        }
    }

    int GetPlaceID()
    {
        for (int i = columnStart.Place.Count-1; i >= 0; i--)
        {
            if (columnStart.Place[i].Checker == this)
            {
                return i;
            }
        }

        return -1;
    }

    void SetSpriteSide()
    {
        finishSprite.gameObject.SetActive(false);
        spriteRender.gameObject.SetActive(true);
        
        int type = 0;
        
        switch (_playerInfo.PlayerData.boardType)
        {
            case GameSetup.ArtClass.ArtType.type1: type = 0;
                break;
            
            case GameSetup.ArtClass.ArtType.type2: type = 1;
                break;
            
            case GameSetup.ArtClass.ArtType.type3: type = 2;
                break;
        }

        switch (Side)
        {
            case side.White:
            {
                if (_playerInfo.PlayerData.checkerSide == GameSetup.sides.Black && GameSetup.GeneralOptions.playerSide == GameSetup.sides.White)
                {
                    spriteRender.sprite = checkerSprite[type].sprite[1];
                    finishSprite.color = checkerSprite[type].color[1];
                }
                else
                {
                    spriteRender.sprite = checkerSprite[type].sprite[0];
                    finishSprite.color = checkerSprite[type].color[0];
                }
            }
                break;
            
            case side.Black:
            {
                if (_playerInfo.PlayerData.checkerSide == GameSetup.sides.Black && GameSetup.GeneralOptions.playerSide == GameSetup.sides.White)
                {
                    spriteRender.sprite = checkerSprite[type].sprite[0];
                    finishSprite.color = checkerSprite[type].color[0];
                }
                else
                {
                    spriteRender.sprite = checkerSprite[type].sprite[1];
                    finishSprite.color = checkerSprite[type].color[1];
                }
            }
                break;
        }
    }

    void Touch()
    {
        if (Input.GetMouseButtonDown(0) && GameSetup.GetTouchNearChecker() == this)
            TouchDrag();
    }

    public void TouchDrag()
    {
        if (canDrag && ((GameSetup.GeneralOptions.Online && !pauseController.Pause) || GameSetup.GeneralOptions.AI))
        {
            isDragging = true;
            Fixing = false;
            GameSetup.touchNearChecker = this;
        }
    }

    void PlayPlaceAudio()
    {
        if (distanceToFix <= 3)
        {
            if (!played)
            {
                int play = 0;
                int canplay = 1;

                if (FirstAudio)
                {
                    play = Random.Range(0, PlaceAudio.Count - 4);
                    canplay = Random.Range(1, 3);
                }
                else
                    play = Random.Range(8, PlaceAudio.Count);

                if (canplay == 1)
                    PlaceAudio[play].Play();

                played = true;
            }

            if (!kickplayed)
            {
                PlaceAudio[0].Play();
                kickplayed = true;
            }
        }
    }

    void SetHighLight()
    {
        float speed = 1;
        Color color = highlightSprite.color;
        if (HighLight)
        {
            if (color.a <= 0.1f)
                change = true;

            if (color.a >= 0.5f)
                change = false;

            if (change)
                color.a += speed * Time.deltaTime;
            else
                color.a -= speed * Time.deltaTime;
        }
        else
            color.a = 0;

        highlightSprite.color = color;
    }

    void setFinish()
    {
        if (Stand)
        {
            spriteRender.gameObject.SetActive(false);
            finishSprite.gameObject.SetActive(true);
            isStand = true;
        }
        else if (isStand)
        {
            SetSpriteSide();
            isStand = false;
        }
    }

    GameSetup.ColumnClass CheckColumnRange()
    {
        bool inRange = false;

        for (int i = 0; i < GameSetup.Column.Count + 2; i++)
        {
            Vector3 cubeMin = Vector3.zero;
            Vector3 cubeMax = Vector3.zero;
            Bounds cubeBounds = new Bounds();

            if (i < GameSetup.Column.Count)
            {
                cubeMin = GameSetup.Column[i].Pos - GameSetup.GeneralOptions.rangeSizeColumn / 2f;
                cubeMax = GameSetup.Column[i].Pos + GameSetup.GeneralOptions.rangeSizeColumn / 2f;
            }
            else
            {
                if (i == GameSetup.Column.Count + 1)
                {
                    cubeMin = (GameSetup.finishColumn[1].Pos + GameSetup.GeneralOptions.rangeDistance) - GameSetup.GeneralOptions.rangeSizeFinishColumn / 2f;
                    cubeMax = (GameSetup.finishColumn[1].Pos + GameSetup.GeneralOptions.rangeDistance) + GameSetup.GeneralOptions.rangeSizeFinishColumn / 2f;
                }
            }

            cubeBounds = new Bounds((cubeMin + cubeMax) / 2f, cubeMax - cubeMin);

            inRange = cubeBounds.Contains(transform.position);

            if (inRange)
            {
                if (i < GameSetup.Column.Count)
                    return GameSetup.Column[i];
                else
                {
                    if (i == GameSetup.Column.Count + 1)
                        return GameSetup.finishColumn[1];
                }
            }
        }

        return null;
    }

    void Fix()
    {
        if (Fixing && canFix)
        {
            float distance = Vector3.Distance(FixTarget, transform.position) + 0.5f;
            float speed = GameSetup.GeneralOptions.fixSpeed * multipy * distance * 0.3f;
            Vector3 pos = transform.position;

            
            Vector3 direction = (FixTarget - pos).normalized;
            pos += direction * Mathf.Min(speed * Time.deltaTime, Vector3.Distance(pos, FixTarget));

            transform.position = pos;
        }
    }

    public void CheckFinish(GameSetup.ColumnClass column)
    {
        if (column.ID == -1 || column.ID == 24)
        {
            finished = true;
            Stand = true;
        }
    }

    void Drag()
    {
        if (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) || GameSetup.GetTouchPosition() == Vector3.zero)
        {
            if (isDragging)
            {
                if (GameSetup.touchNearChecker == this) GameSetup.touchNearChecker = null;

                GameSetup.ColumnClass inRangeColumn = CheckColumnRange();

                if (inRangeColumn != null && columnTarget.Contains(inRangeColumn))
                {
                    multipy = 2;
                    UpdateRolls(inRangeColumn, true, true);
                    CheckFinish(inRangeColumn);
                    GameSetup.ResetCheckers();
                    GameSetup.PlaceChecker(this, inRangeColumn, true, columnStart,false);

                    if (GameSetup.Roll.Rolls.Count > 0) StartCoroutine(UpdateNaxtMove(GameSetup.GeneralOptions.delayUpdateNaxtMove, "player"));
                    else
                        GameSetup.playerDone(true);
                }
                else
                {
                    multipy = 3.2f;
                }
                
                Fixing = true;
                SetColumnHighLight(false);
                SetLayerOrder("place");

                isDragging = false;
            }
        }

        if (isDragging)
        {
            if (!canDrag || (GameSetup.GeneralOptions.Online && pauseController.Pause)) { isDragging = false; return; }

            Vector3 Pos = transform.position;
            Pos.x = GameSetup.GetTouchPosition().x;
            Pos.y = GameSetup.GetTouchPosition().y;
            Pos.z = 4;
            transform.position = Pos; 

            SetColumnHighLight(true);
            SetLayerOrder("top");
        }
    }

    public IEnumerator UpdateNaxtMove(float delay,string side)
    {
        GameSetup.ResetColumnHighLight();
        yield return new WaitForSeconds(delay * Time.deltaTime);

        if (side == "player")
            GameSetup.UsableChecker(GameSetup.GeneralOptions.PlayerLocation,GameSetup.GeneralOptions.playerSide,GameSetup.Roll.Rolls);
        else
        if (side == "opponent")
        {
            if (GameSetup.GeneralOptions.AI)
                StartCoroutine(GameSetup.AIAct(GameSetup.GeneralOptions.delayAIMove,"intelligent"));
        }
    }

    void SetColumnHighLight(bool active)
    {
        if (columnTarget.Count > 0)
        {
            foreach(GameSetup.ColumnClass column in columnTarget)
            {
                column.HighLight = active;
            }
        }

        GameSetup.disableCheckersHighLight = active;
    }

    public void UpdateRolls(GameSetup.ColumnClass column, bool createHistory, bool verify)
    {
        int changeDistance = Mathf.Abs(columnStart.ID - column.ID);

        List<int> indexesToRemove = new List<int>();

        bool oneByone = false;
        bool haveKickRoll = false;

        for (int i = 0; i < GameSetup.Roll.Rolls.Count; i++)
        {
            if (GameSetup.Roll.Rolls[i] == changeDistance)
            {
                indexesToRemove.Add(i);
                oneByone = true;
                break;
            }
        }

        if (!oneByone)
        {
            for (int i = 0; i < GameSetup.Roll.Rolls.Count; i++)
            {
                if (GameSetup.Roll.Rolls.Count == 2)
                {
                    if (GameSetup.Roll.Rolls[0] + GameSetup.Roll.Rolls[1] == changeDistance)
                    {
                        if (columnStart.BlockedRoll != 0 && columnStart.BlockedRoll == GameSetup.Roll.Rolls[0])
                        {
                            indexesToRemove.Add(1);
                            indexesToRemove.Add(0);
                            Debug.Log("have Block Roll");
                            haveKickRoll = true;
                        }
                        else
                        {
                            indexesToRemove.Add(0);
                            indexesToRemove.Add(1);
                        }
                    }

                    break;
                }

                if (GameSetup.Roll.Rolls.Count == 3)
                {
                    if (GameSetup.Roll.Rolls[0] + GameSetup.Roll.Rolls[1] == changeDistance)
                    {
                        indexesToRemove.Add(0);
                        indexesToRemove.Add(1);
                    }
                    else
                    if (GameSetup.Roll.Rolls[0] + GameSetup.Roll.Rolls[2] == changeDistance)
                    {
                        indexesToRemove.Add(0);
                        indexesToRemove.Add(2);
                    }
                    else
                    if (GameSetup.Roll.Rolls[1] + GameSetup.Roll.Rolls[2] == changeDistance)
                    {
                        indexesToRemove.Add(1);
                        indexesToRemove.Add(2);
                    }
                    else
                    if (GameSetup.Roll.Rolls[0] + GameSetup.Roll.Rolls[1] + GameSetup.Roll.Rolls[2] == changeDistance)
                    {
                        indexesToRemove.Add(0);
                        indexesToRemove.Add(1);
                        indexesToRemove.Add(2);
                    }

                    break;
                }

                if (GameSetup.Roll.Rolls.Count == 4)
                {
                    if (GameSetup.Roll.Rolls[0] + GameSetup.Roll.Rolls[1] == changeDistance)
                    {
                        indexesToRemove.Add(0);
                        indexesToRemove.Add(1);
                    }
                    else
                    if (GameSetup.Roll.Rolls[0] + GameSetup.Roll.Rolls[2] == changeDistance)
                    {
                        indexesToRemove.Add(0);
                        indexesToRemove.Add(2);
                    }
                    else
                    if (GameSetup.Roll.Rolls[0] + GameSetup.Roll.Rolls[3] == changeDistance)
                    {
                        indexesToRemove.Add(0);
                        indexesToRemove.Add(3);
                    }
                    else
                    if (GameSetup.Roll.Rolls[1] + GameSetup.Roll.Rolls[2] == changeDistance)
                    {
                        indexesToRemove.Add(1);
                        indexesToRemove.Add(2);
                    }
                    else
                    if (GameSetup.Roll.Rolls[1] + GameSetup.Roll.Rolls[3] == changeDistance)
                    {
                        indexesToRemove.Add(1);
                        indexesToRemove.Add(3);
                    }
                    else
                    if (GameSetup.Roll.Rolls[2] + GameSetup.Roll.Rolls[3] == changeDistance)
                    {
                        indexesToRemove.Add(2);
                        indexesToRemove.Add(3);
                    }
                    else
                    if (GameSetup.Roll.Rolls[0] + GameSetup.Roll.Rolls[1] + GameSetup.Roll.Rolls[2] == changeDistance)
                    {
                        indexesToRemove.Add(0);
                        indexesToRemove.Add(1);
                        indexesToRemove.Add(2);
                    }
                    else
                    if (GameSetup.Roll.Rolls[0] + GameSetup.Roll.Rolls[1] + GameSetup.Roll.Rolls[3] == changeDistance)
                    {
                        indexesToRemove.Add(0);
                        indexesToRemove.Add(1);
                        indexesToRemove.Add(3);
                    }
                    else
                    if (GameSetup.Roll.Rolls[0] + GameSetup.Roll.Rolls[2] + GameSetup.Roll.Rolls[3] == changeDistance)
                    {
                        indexesToRemove.Add(0);
                        indexesToRemove.Add(2);
                        indexesToRemove.Add(3);
                    }
                    else
                    if (GameSetup.Roll.Rolls[1] + GameSetup.Roll.Rolls[2] + GameSetup.Roll.Rolls[3] == changeDistance)
                    {
                        indexesToRemove.Add(1);
                        indexesToRemove.Add(2);
                        indexesToRemove.Add(3);
                    }
                    else
                    if (GameSetup.Roll.Rolls[0] + GameSetup.Roll.Rolls[1] + GameSetup.Roll.Rolls[2] + GameSetup.Roll.Rolls[3] == changeDistance)
                    {
                        indexesToRemove.Add(0);
                        indexesToRemove.Add(1);
                        indexesToRemove.Add(2);
                        indexesToRemove.Add(3);
                    }

                    break;
                }
            }
        }

        GameSetup.HistoryClass history = null;

        if (createHistory) // Create History
        {
            history = new GameSetup.HistoryClass();
            GameSetup.History.Add(history);
            history.checker = this;
            history.startColumn = columnStart;
            history.targetColumn = column;
        }

        if (indexesToRemove.Count > 0)
        {
            for (int j=0;j< indexesToRemove.Count;j++)
            {
                if (createHistory)
                    history.roll.Add(GameSetup.Roll.Rolls[indexesToRemove[j]]);
            }

            if (haveKickRoll)
            {
                for (int i = 0; i < indexesToRemove.Count; i++)
                {
                    if (GameSetup.Roll.Rolls.Contains(GameSetup.Roll.Rolls[indexesToRemove[i]]))
                    {
                        GameSetup.Roll.Rolls.RemoveAt(indexesToRemove[i]);
                    }
                    else
                    {
                        if (createHistory)
                            history.roll.Add(GameSetup.FindMaxRoll(GameSetup.Roll.Rolls));

                        GameSetup.Roll.Rolls.Remove(GameSetup.FindMaxRoll(GameSetup.Roll.Rolls));
                    }
                }
            }
            else
            {
                for (int i = indexesToRemove.Count - 1; i >= 0; i--)
                {
                    if (GameSetup.Roll.Rolls.Contains(GameSetup.Roll.Rolls[indexesToRemove[i]]))
                    {

                        GameSetup.Roll.Rolls.RemoveAt(indexesToRemove[i]);
                    }
                    else
                    {
                        if (createHistory)
                            history.roll.Add(GameSetup.FindMaxRoll(GameSetup.Roll.Rolls));

                        GameSetup.Roll.Rolls.Remove(GameSetup.FindMaxRoll(GameSetup.Roll.Rolls));
                    }
                }
            }
            
        }else
        {
            if (createHistory)
                history.roll.Add(GameSetup.FindMaxRoll(GameSetup.Roll.Rolls));

            GameSetup.Roll.Rolls.Remove(GameSetup.FindMaxRoll(GameSetup.Roll.Rolls));
        }

        if (createHistory && verify)
        {
            history.verify = true; // Verify Move
            GameSetup.Roll.moveNumber += 1;
        }


        if (GameSetup.Roll.Rolls.Count > 4)
        {
            for (int i = GameSetup.Roll.Rolls.Count - 1; i > 3; i--)
            {
                GameSetup.Roll.Rolls.Remove(GameSetup.Roll.Rolls[i]);
            }
        }
    }
}