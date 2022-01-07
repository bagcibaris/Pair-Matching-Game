using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PictureManager : MonoBehaviour
{
    public Picture PicturePrefab;
    public Transform PicSpawnPosition;
    public Vector2 StartPosition = new Vector2(-2.15f, 3.62f);

    public enum GameState
    {   NoAction,
        MovingOnPositions,
        DeletingPuzzles,
        FlipBack,
        Checking,
        GameEnd
    };
    public enum PuzzleState
    {
        PuzzleRotating,
        CanRotate
    };

    public enum RevealedState
    {
        NoRevealed,
        OneRevealed,
        TwoRevealed
    };
    [HideInInspector]
    public GameState CurrentGameState;
    [HideInInspector]
    public PuzzleState CurrentPuzzleState;
    [HideInInspector]
    public RevealedState PuzzleRevealedNumber;

    [HideInInspector]
    public List<Picture> PictureList;

    private Vector2 _offset = new Vector2(1.42f, 1.52f);
    private Vector2 _offsetFor15Pairs = new Vector2(1.08f, 1.22f);
    private Vector2 _offsetFor20Pairs = new Vector2(1.08f, 1.0f);
    private Vector3 _newScaleDown = new Vector3(0.9f, 0.9f, 0.001f);


    private List<Material> _materialList = new List<Material>();
    private List<string> _texturePathList = new List<string>();
    private Material _firstMaterial;
    private string _firstTexturePath;

    private int _firstRevealedPic;
    private int _secondRevealedPic;
    private int _revealedPicNumber = 0;
    private int _picToDestroy1;
    private int _picToDestroy2;

    void Start()
    {
        CurrentGameState = GameState.NoAction;
        CurrentPuzzleState = PuzzleState.CanRotate;
        PuzzleRevealedNumber = RevealedState.NoRevealed;
        _revealedPicNumber = 0;
        _firstRevealedPic = -1;
        _secondRevealedPic = -1;

        LoadMaterials();

        if(GameSettings.Instance.GetPairNumber() == GameSettings.EPairNumber.E10Pairs)
        {
            CurrentGameState = GameState.MovingOnPositions;
            SpawnPictureMesh(4, 5, StartPosition, _offset, false);
            MovePicture(4, 5, StartPosition, _offset);
        }
        else if (GameSettings.Instance.GetPairNumber() == GameSettings.EPairNumber.E15Pairs)
        {
            CurrentGameState = GameState.MovingOnPositions;
            SpawnPictureMesh(5, 6, StartPosition, _offset, false);
            MovePicture(5, 6, StartPosition, _offsetFor15Pairs);
        }
        else if (GameSettings.Instance.GetPairNumber() == GameSettings.EPairNumber.E20Pairs)
        {
            CurrentGameState = GameState.MovingOnPositions;
            SpawnPictureMesh(5, 8, StartPosition, _offset, true);
            MovePicture(5, 8, StartPosition, _offsetFor20Pairs);
        }

    }

    public void CheckPicture()
    {
        CurrentGameState = GameState.Checking;
        _revealedPicNumber = 0;

        for (int id = 0; id < PictureList.Count; id++)
        {
            if(PictureList[id].Revealed && _revealedPicNumber < 2)
            {
                if(_revealedPicNumber == 0)
                {
                    _firstRevealedPic = id;
                    _revealedPicNumber++;
                }
                else if(_revealedPicNumber == 1) 
                {
                    _secondRevealedPic = id;
                    _revealedPicNumber++;
                }
            }
        }
        if ( _revealedPicNumber == 2)
        {
            if(PictureList[_firstRevealedPic].GetIndex() == PictureList[_secondRevealedPic].GetIndex() && _firstRevealedPic != _secondRevealedPic)
            {
                CurrentGameState = GameState.DeletingPuzzles;
                _picToDestroy1 = _firstRevealedPic;
                _picToDestroy2 = _secondRevealedPic;
            }
            else
            {
                CurrentGameState = GameState.FlipBack;
            }
        }
        CurrentPuzzleState = PictureManager.PuzzleState.CanRotate;

        if(CurrentGameState == GameState.Checking)
        {
            CurrentGameState = GameState.NoAction;
        }
    }

    private void DestroyPicture()
    {
        PuzzleRevealedNumber = RevealedState.NoRevealed;
        System.Threading.Thread.Sleep(200); // eþleþince delay
        PictureList[_picToDestroy1].Deactivate();
        PictureList[_picToDestroy2].Deactivate();
        _revealedPicNumber = 0;
        CurrentGameState = GameState.NoAction;
        CurrentPuzzleState = PuzzleState.CanRotate;
    }

    private void FlipBack()
    {
        System.Threading.Thread.Sleep(500); // To remove (testing delay)

        PictureList[_firstRevealedPic].FlipBack();
        PictureList[_secondRevealedPic].FlipBack();

        PictureList[_firstRevealedPic].Revealed = false;
        PictureList[_secondRevealedPic].Revealed = false;

        PuzzleRevealedNumber = RevealedState.NoRevealed;
        CurrentGameState = GameState.NoAction;
    }

    private void LoadMaterials()
    {
        var materialFilePath = GameSettings.Instance.GetMaterialDirectoryame();
        var textureFilePath = GameSettings.Instance.GetPuzzleCategoryTextureDirectoryName();
        var pairNumber = (int)GameSettings.Instance.GetPairNumber();
        const string matBaseName = "Pic";       //material name
        var firstMaterialName = "Back";

        for(var index = 1; index <= pairNumber; index++)
        {
            var currentFÝlePath = materialFilePath + matBaseName + index;
            Material mat = Resources.Load(currentFÝlePath, typeof(Material)) as Material;
            _materialList.Add(mat);

            var currentTextureFilePath = textureFilePath + matBaseName + index;
            _texturePathList.Add(currentTextureFilePath);
        }


        _firstTexturePath = textureFilePath + firstMaterialName;
        _firstMaterial = Resources.Load(materialFilePath + firstMaterialName, typeof(Material)) as Material;

    }


    void Update()
    {
        if(CurrentGameState == GameState.DeletingPuzzles)
        {
            if(CurrentPuzzleState == PuzzleState.CanRotate)
            {
                DestroyPicture();
            }
        }

        if(CurrentGameState == GameState.FlipBack)
        {
            if(CurrentPuzzleState == PuzzleState.CanRotate)
            {
                FlipBack();
            }
        }
    }

    private void SpawnPictureMesh(int rows, int columns, Vector2 Pos, Vector2 offset, bool scaleDown)
    {
        for(int col = 0; col < columns; col++)
        {
            for(int row = 0; row < rows; row++)
            {
                var tempPicture = (Picture)Instantiate(PicturePrefab, PicSpawnPosition.position, PicturePrefab.transform.rotation);

                if (scaleDown)
                {
                    tempPicture.transform.localScale = _newScaleDown;
                }

                tempPicture.name = tempPicture.name + "c" + col + "r" + row;
                PictureList.Add(tempPicture);
            }

        }

        ApplyTextures();

    }
        public void ApplyTextures()
    {
        var rndMatIndex = Random.Range(0, _materialList.Count);
        var AppliedTimes = new int[_materialList.Count];

        for(int i = 0; i < _materialList.Count; i++)
        {
            AppliedTimes[i] = 0;
        }

        foreach(var o in PictureList)
        {
            var randPrevious = rndMatIndex;
            var counter = 0;
            var forceMat = false;
            
            while(AppliedTimes[rndMatIndex] >= 2 || ((randPrevious == rndMatIndex) && !forceMat))
            {
                rndMatIndex = Random.Range(0, _materialList.Count);
                counter++;
                if(counter > 100)
                {
                    for(var j = 0; j < _materialList.Count; j++)
                    {
                        if(AppliedTimes[j] < 2)
                        {
                            rndMatIndex = j;
                            forceMat = true;
                        }
                    }
                    if(forceMat == false)
                    {
                        return;
                    }

                }

            }

            o.SetFirstMaterial(_firstMaterial, _firstTexturePath);
            o.ApplyFirstMaterial();
            o.SetSecondMaterial(_materialList[rndMatIndex], _texturePathList[rndMatIndex]);
            o.SetIndex(rndMatIndex);
            o.Revealed = false;
            // o.ApplySecondMaterial(); // test
            AppliedTimes[rndMatIndex] += 1;
            forceMat = false;
        }
        
    }
    private void MovePicture(int rows, int columns, Vector2 pos, Vector2 offset)
    {
        var index = 0;
        for(var col = 0; col < columns; col++)
        {
            for(int row = 0; row < rows; row++)
            {
                var targetPosition = new Vector3((pos.x + (offset.x * row)), (pos.y - (offset.y * col)), 0.0f);
                StartCoroutine(MoveToPosition(targetPosition, PictureList[index]));
                index++;
            }
        }
    }

    private IEnumerator MoveToPosition(Vector3 target, Picture obj)
    {
        var randomDis = 7;

        while(obj.transform.position != target)
        {
            obj.transform.position = Vector3.MoveTowards(obj.transform.position, target, randomDis * Time.deltaTime);
            yield return 0;
        }
    }
}
