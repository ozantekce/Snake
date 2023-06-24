using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameArea : MonoBehaviour
{

    public Sprite cellDefaultSprite;
    public bool dontRender;

    private bool rendering = true;

    [SerializeField]
    private NameSpritePair[] _nameSpritePairs;

    private Dictionary<CellType, Sprite> _nameSpriteDic;


    private Vector2 _cellSize;
    private SpriteRenderer[][] _cellRenderers;

    private void Awake()
    {
        _nameSpriteDic = new Dictionary<CellType, Sprite>();
        for (int i = 0; i < _nameSpritePairs.Length; i++)
        {
            _nameSpriteDic.Add(_nameSpritePairs[i].name, _nameSpritePairs[i].sprite);
        }
    }


    private void Start()
    {
        /*
        if (dontRender)
        {
            for (int i = 0; i < _cellRenderers.Length; i++)
            {
                for(int j =0; j< _cellRenderers.Length; j++)
                {
                    _cellRenderers[i][j].enabled = false;
                }
            }
        }*/
    }


    public void CreateGameArea(int gameSize)
    {


        _cellSize.x = 1f / gameSize;
        _cellSize.y = 1f / gameSize;
        Debug.Log(_cellSize);

        Vector2 startPosition = new Vector2(-0.5f + _cellSize.x / 2, +0.5f - _cellSize.y / 2);
        Debug.Log(startPosition);

        Vector2 rowDelta = new Vector2(0, -_cellSize.y);
        Vector2 colDelta = new Vector2(_cellSize.x, 0);

        _cellRenderers = new SpriteRenderer[gameSize][];
        for (int i = 0; i < gameSize; i++)
        {
            Vector2 rowStartPos = startPosition + i * rowDelta;
            _cellRenderers[i] = new SpriteRenderer[gameSize];
            for (int j = 0; j < gameSize; j++)
            {
                Vector2 pos = rowStartPos + j * colDelta;
                GameObject tempGO = new GameObject(i + ", " + j);
                tempGO.transform.SetParent(transform);
                tempGO.transform.localPosition = pos;
                tempGO.transform.localScale = _cellSize;
                SpriteRenderer spriteRenderer = tempGO.AddComponent<SpriteRenderer>();
                _cellRenderers[i][j] = spriteRenderer;
                if (cellDefaultSprite != null)
                {
                    spriteRenderer.sprite = cellDefaultSprite;
                    spriteRenderer.sortingOrder = 1;
                }

            }

        }

    }


    public void Render(Vector2Int pos, CellType partType)
    {
        if (dontRender) return;
        Sprite sp = _nameSpriteDic[partType];

        _cellRenderers[pos.x][pos.y].sprite = sp;

    }


    public Vector3 GetRealPosition(Vector2Int pos)
    {
        return _cellRenderers[pos.x][pos.y].transform.position;
    }



    [System.Serializable]
    private struct NameSpritePair
    {
        public CellType name;
        public Sprite sprite;
    }




}
