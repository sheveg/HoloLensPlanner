using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using UnityEngine.UI;
using HoloLensPlanner.GazeResponse;
using System;
using HoloLensPlanner;

namespace HoloLensPlanner.TEST
{
    /// <summary>
    /// TileMenuDetailView is responsible for handling input from detail view buttons and showing the saved tiles in a detail view.
    /// </summary>
    public class TileMenuDetailView : SingleInstance<TileMenuDetailView>
    {
        // Editor buttons
        [SerializeField]
        private Button TextureButton;

        [SerializeField]
        private Button WidthButton;

        [SerializeField]
        private Button HeightButton;

        [SerializeField]
        private Button JointSizeButton;

        [SerializeField]
        private Button ThicknessButton;

        [SerializeField]
        private Text TileCountText;

        [SerializeField]
        private Button EditButton;

        [SerializeField]
        private Button SaveTileButton;

        [SerializeField]
        private Button NameButton;

        private Button selectedButton;

        /// <summary>
        /// In Edit mode we can change the values of the current tile.
        /// </summary>
        private bool m_EditMode = false;

        /// <summary>
        /// Index of current tile of the saved tiles.
        /// </summary>
        public int CurrentTile { get; private set; }

        private int CurrentTextureIndex;

        private Guid CurrentGuid;

        private void Start()
        {
            EditButton.onClick.AddListener(enableEditing);
            SaveTileButton.onClick.AddListener(SaveTile);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Shows the detail view with the current tile.
        /// </summary>
        /// <param name="tileIndex"></param>
        public void Show(int tileIndex)
        {
            gameObject.SetActive(true);
            if (!TileCountText.gameObject.activeSelf)
            {
                TileCountText.gameObject.SetActive(true);
            }
            CurrentTile = wrapTileIndex(tileIndex);

            //Save GUID and textureIndex of tile
            //Both needed to correctly store tile later
            CurrentTextureIndex = TileMenuManager.Instance.SavedTiles[CurrentTile].TextureIndex;
            CurrentGuid = TileMenuManager.Instance.SavedTiles[CurrentTile].Guid;

            loadCurrentTile();
            if(m_EditMode)
                disableEditing();
            updateTileIndexText();
        }

        public void NewTile()
        {
            if (gameObject.activeSelf == false)
            {
                gameObject.SetActive(true);
            }

            //Hide tile count
            TileCountText.gameObject.SetActive(false);

            WidthButton.GetComponentInChildren<Text>().text = TileData.DefaultWidthInCM.ToString("n0") + " cm";
            HeightButton.GetComponentInChildren<Text>().text = TileData.DefaultHeightInCM.ToString("n0") + " cm";

            float jointSizeInM = TileDimensionsLibrary.GetJointThickness(TileData.DefaultJointThickness);
            JointSizeButton.GetComponentInChildren<Text>().text = (jointSizeInM * 1000).ToString("n0") + " mm";

            float thicknessInM = TileDimensionsLibrary.GetTileThickness(TileData.DefaultTileThickness);
            ThicknessButton.GetComponentInChildren<Text>().text = (thicknessInM * 1000).ToString("n0") + " mm";

            TextureButton.GetComponent<RawImage>().texture = GlobalSettings.Instance.TextureLibrary.Textures[TileData.DefaultTextureIndex];
            CurrentTextureIndex = TileData.DefaultTextureIndex;
            CurrentGuid = Guid.Empty;
        }

        public void ShowNextTile()
        {
            Show(CurrentTile + 1);
        }

        public void ShowPreviousTile()
        {
            Show(CurrentTile - 1);
        }

        private void loadCurrentTile()
        {
            loadTexture();
            loadWidth();
            loadHeight();
            loadJointSize();
            loadThickness();
            loadName();
        }

        private void disableEditing()
        {
            m_EditMode = false;
            disableButton(TextureButton);
            disableButton(WidthButton);
            disableButton(HeightButton);
            disableButton(JointSizeButton);
            disableButton(ThicknessButton);
            disableButton(NameButton);
        }

        private void enableEditing()
        {
            if (m_EditMode == false)
            {
                m_EditMode = true;
                enableButton(TextureButton);
                enableButton(WidthButton);
                enableButton(HeightButton);
                enableButton(JointSizeButton);
                enableButton(ThicknessButton);
                enableButton(NameButton);
            }
            else if (m_EditMode == true)
            {
                m_EditMode = false;
                disableButton(TextureButton);
                disableButton(WidthButton);
                disableButton(HeightButton);
                disableButton(JointSizeButton);
                disableButton(ThicknessButton);
                disableButton(NameButton);
            }
        }

        private void edit(Button b)
        {
            selectedButton = b;

            if (b.Equals(HeightButton) || b.Equals(WidthButton))
            {                
                KeyboardWithNumbers.Instance.FollowWithOffset(b.transform, 0.01f);
                KeyboardWithNumbers.Instance.PresentKeyboard(KeyboardWithNumbers.LayoutType.Number);

                //Now done by simpleTagalong script
                //KeyboardNumbers.Instance.gameObject.transform.position = b.transform.position - 0.4f * b.transform.forward;

                KeyboardWithNumbers.Instance.OnTextSubmitted += acceptKeyboardInput;
            }
            else if (b.Equals(NameButton))
            {
                KeyboardWithNumbers.Instance.FollowWithOffset(b.transform, 0.01f);
                KeyboardWithNumbers.Instance.PresentKeyboard(KeyboardWithNumbers.LayoutType.Number);
                KeyboardWithNumbers.Instance.OnTextSubmitted += acceptKeyboardInput;
            }
            else if (b.Equals(TextureButton))
            {

                this.gameObject.SetActive(false);
                TextureListView.Instance.CreatePages();
                TextureListView.Instance.Show(Mathf.CeilToInt(CurrentTextureIndex / ObjectPage.MaxObjectsCount));
            }
        }

        private void acceptKeyboardInput(object sender, EventArgs e)
        {
            if (selectedButton.Equals(HeightButton))
            {
                HeightButton.GetComponentInChildren<Text>().text = KeyboardWithNumbers.Instance.InputField.text + " cm";                
            }
            else if (selectedButton.Equals(WidthButton))
            {
                WidthButton.GetComponentInChildren<Text>().text = KeyboardWithNumbers.Instance.InputField.text + " cm";
            }
            else if (selectedButton.Equals(NameButton))
            {
                NameButton.GetComponentInChildren<Text>().text = KeyboardWithNumbers.Instance.InputField.text;
            }
            KeyboardWithNumbers.Instance.OnTextSubmitted -= acceptKeyboardInput;
            KeyboardWithNumbers.Instance.StopFollow();
        }

        private void loadTexture()
        {
            //Texture2D tileTexture = GlobalSettings.Instance.TextureLibrary.Textures[TileMenuManager.Instance.SavedTiles[CurrentTile].TextureIndex];
            //Sprite tileSprite = Sprite.Create(
            //    tileTexture,
            //    new Rect(0, 0, tileTexture.width, tileTexture.height),
            //    new Vector2(0.5f, 0.5f));
            TextureButton.GetComponent<RawImage>().texture = GlobalSettings.Instance.TextureLibrary.Textures[TileMenuManager.Instance.SavedTiles[CurrentTile].TextureIndex];
        }

        private void loadWidth()
        {
            float widthInCM = TileMenuManager.Instance.SavedTiles[CurrentTile].Width;
            WidthButton.GetComponentInChildren<Text>().text = (widthInCM).ToString("n0") + " cm";
        }

        private void loadHeight()
        {
            float heightInCM = TileMenuManager.Instance.SavedTiles[CurrentTile].Height;
            HeightButton.GetComponentInChildren<Text>().text = (heightInCM).ToString("n0") + " cm";
        }

        private void loadJointSize()
        {
            float jointSizeInM = TileDimensionsLibrary.GetJointThickness(TileMenuManager.Instance.SavedTiles[CurrentTile].JointThickness);
            JointSizeButton.GetComponentInChildren<Text>().text = (jointSizeInM * 1000).ToString("n0") + " mm";
        }

        private void loadThickness()
        {
            float thicknessInM = TileDimensionsLibrary.GetTileThickness(TileMenuManager.Instance.SavedTiles[CurrentTile].TileThickness);
            ThicknessButton.GetComponentInChildren<Text>().text = (thicknessInM * 1000).ToString("n0") + " mm";
        }

        private void loadName()
        {
            string name = TileMenuManager.Instance.SavedTiles[CurrentTile].Name;
            NameButton.GetComponentInChildren<Text>().text = name;
        }

        private void disableButton(Button b)
        {
            b.enabled = false;
            GazeResponder gazeRes;
            if ( ( gazeRes = b.gameObject.GetComponent<GazeResponder>() ) != null )
                gazeRes.enabled = false;
            Outline o;
            if ((o = b.GetComponent<Outline>()) != null)
                o.enabled = false;
        }

        private void enableButton(Button b)
        {
            b.enabled = true;
            GazeResponder gazeRes;
            if ((gazeRes = b.gameObject.GetComponent<GazeResponder>()) != null)
                gazeRes.enabled = true;
            Outline o;
            if ((o = b.GetComponent<Outline>()) != null)
                o.enabled = true;

            b.onClick.AddListener(delegate { edit(b); });

        }

        private void updateTileIndexText()
        {
            TileCountText.text = string.Format("{0} / {1}", CurrentTile + 1, TileMenuManager.Instance.SavedTiles.Count);
        }

        private int wrapTileIndex(int tileIndex)
        {
            return (tileIndex % TileMenuManager.Instance.SavedTiles.Count + TileMenuManager.Instance.SavedTiles.Count) % TileMenuManager.Instance.SavedTiles.Count;
        }

        public void SaveTile()
        {
            //if CurrentGuid == null it's a new tile
            if (CurrentGuid.ToString().Equals("00000000-0000-0000-0000-000000000000"))
            {
                //TODO: add name
                TileData tile = new TileData(TileDimensionsLibrary.StringToFloat(HeightButton.GetComponentInChildren<Text>().text),
                    TileDimensionsLibrary.StringToFloat(WidthButton.GetComponentInChildren<Text>().text),
                    TileDimensionsLibrary.StringToFloat(ThicknessButton.GetComponentInChildren<Text>().text),
                    TileDimensionsLibrary.StringToFloat(JointSizeButton.GetComponentInChildren<Text>().text),
                    CurrentTextureIndex, 
                    NameButton.GetComponentInChildren<Text>().text,
                    Guid.NewGuid());
                tile.SaveToJson();
                TileMenuManager.Instance.addToCachedTiles(tile);
                CurrentTile = TileMenuManager.Instance.SavedTiles.Count;
            }
            //if CurrentGuid is set, save old tile
            else
            {
                //TODO: add name
                TileData tile = new TileData(TileDimensionsLibrary.StringToFloat(HeightButton.GetComponentInChildren<Text>().text),
                    TileDimensionsLibrary.StringToFloat(WidthButton.GetComponentInChildren<Text>().text),
                    TileDimensionsLibrary.StringToFloat(ThicknessButton.GetComponentInChildren<Text>().text),
                    TileDimensionsLibrary.StringToFloat(JointSizeButton.GetComponentInChildren<Text>().text),
                    CurrentTextureIndex,
                    NameButton.GetComponentInChildren<Text>().text, 
                    CurrentGuid);
                tile.SaveToJson();
                TileMenuManager.Instance.updateCachedTiles(tile);               
            }
            //TODO: Delete two lines if scene should change to "place tiles" not go back to ListView
            TileMenuListView.Instance.CreatePages(TileMenuManager.Instance.SavedTiles);
            TileMenuManager.Instance.ShowListView(Mathf.CeilToInt(CurrentTile / ObjectPage.MaxObjectsCount));
        }
    }
}
