﻿using UnityEngine;
using HoloToolkit.Unity;
using UnityEngine.UI;
using HoloLensPlanner.GazeResponse;
using System;
using HoloLensPlanner.Utilities;

namespace HoloLensPlanner
{
    /// <summary>
    /// TileMenuDetailView is responsible for handling input from detail view buttons and showing the saved tiles in a detail view.
    /// </summary>
    public class TileMenuDetailView : SingleInstance<TileMenuDetailView>
    {
        #region Editor variables

        /// <summary>
        /// Button to change the texture of the current tile.
        /// </summary>
        [SerializeField]
        private Button TextureButton;

        /// <summary>
        /// Button to change the tile width.
        /// </summary>
        [SerializeField]
        private Button WidthButton;

        /// <summary>
        /// Button to change the tile height.
        /// </summary>
        [SerializeField]
        private Button HeightButton;

        /// <summary>
        /// Button to change the joint size between tiles.
        /// </summary>
        [SerializeField]
        private Button JointSizeButton;

        /// <summary>
        /// Button to change the tile thickness.
        /// </summary>
        [SerializeField]
        private Button ThicknessButton;
        
        /// <summary>
        /// Button to change the name.
        /// </summary>
        [SerializeField]
        private Button NameButton;

        /// <summary>
        /// Text to show the tile index of the current tile.
        /// </summary>
        [SerializeField]
        private Text TileCountText;

        /// <summary>
        /// Button which turns the edit mode on and off.
        /// </summary>
        [SerializeField]
        private Button EditButton;

        /// <summary>
        /// Button to accept the edited values.
        /// </summary>
        [SerializeField]
        private Button AcceptEditButton;

        /// <summary>
        /// Button to decline the edited values.
        /// </summary>
        [SerializeField]
        private Button CancelEditButton;

        /// <summary>
        /// Background which is active in edit mode. This background covers all buttons which can not be clicked when in edit mode, e.g. button to add a new tile.
        /// </summary>
        [SerializeField]
        private Image EditBackground;

        /// <summary>
        /// To indicate that the user is in creation mode we add a header on top of the UI.
        /// </summary>
        [SerializeField]
        private Image CreationHeader;

        /// <summary>
        /// Button to place the current tile on a floor if there is one.
        /// </summary>
        [SerializeField]
        private Button PlaceTileButton;

        #endregion // Editor variables

        #region Cached variables

        /// <summary>
        /// Index of current tile of the saved tiles.
        /// </summary>
        public int CurrentTile { get; private set; }

        /// <summary>
        /// In Edit mode we can change the values of the current tile.
        /// </summary>
        private bool m_EditMode = false;

        private Button m_SelectedButton;

        public int CurrentTextureIndex { get; private set; }

        private Guid m_CurrentGuid;

        /// <summary>
        /// Component for blending. Canvasgroup has an alpha value which blends the whole UI.
        /// </summary>
        private CanvasGroup m_CanvasGroup;

        /// <summary>
        /// When we show/hide the menu we simply enable/disable following the camera and blend it out/in.
        /// </summary>
        private SimpleTagalong m_TagAlong;

        private const float m_HideAlpha = 0.2f;

        #endregion // Cached variables

        #region MonoBehaviour methods

        private void Start()
        {
            Debug.Log("updateTileIndexText");
            WidthButton.onClick.AddListener(editWidth);
            HeightButton.onClick.AddListener(editHeight);
            NameButton.onClick.AddListener(editName);
            JointSizeButton.onClick.AddListener(editJointSize);
            ThicknessButton.onClick.AddListener(editThickness);
            TextureButton.onClick.AddListener(editTexture);
            
            EditButton.onClick.AddListener(enableEditing);
            // canceling the edit mode restores the values of the current tile
            CancelEditButton.onClick.AddListener(disableEditing);
            CancelEditButton.onClick.AddListener(() => TileMenuManager.Instance.ShowDetailView(CurrentTile));
            // accepting the edit mode saves the values to the current tile
            AcceptEditButton.onClick.AddListener(disableEditing);
            AcceptEditButton.onClick.AddListener(saveTile);
            PlaceTileButton.onClick.AddListener(placeTiles);

            m_CanvasGroup = GetComponent<CanvasGroup>();
            m_TagAlong = GetComponent<SimpleTagalong>();
        }

        #endregion // Monobehaviour methods

        #region Public methods

        /// <summary>
        /// Shows the detail view with the current tile.
        /// </summary>
        /// <param name="tileIndex"></param>
        public void Show(int tileIndex)
        {
            Debug.Log("updateTileIndexText");
            gameObject.SetActive(true);
            if (!TileCountText.gameObject.activeSelf)
            {
                TileCountText.gameObject.SetActive(true);
            }
            CurrentTile = MathUtility.WrapArrayIndex(tileIndex, TileMenuManager.Instance.SavedTiles.Count);

            //Save GUID and textureIndex of tile
            //Both needed to correctly store tile later
            CurrentTextureIndex = TileMenuManager.Instance.SavedTiles[CurrentTile].TextureIndex;
            m_CurrentGuid = TileMenuManager.Instance.SavedTiles[CurrentTile].Guid;

            loadCurrentTile();
            if (m_EditMode)
                disableEditing();
            updateTileIndexText();
        }

        /// <summary>
        /// Hides this view.
        /// </summary>
        public void Hide()
        {
            Debug.Log("updateTileIndexText");
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Shows the next tile in the tile list.
        /// </summary>
        public void ShowNextTile()
        {
            Debug.Log("updateTileIndexText");
            Show(CurrentTile + 1);
        }

        /// <summary>
        /// Shows the previous tile in the tile list.
        /// </summary>
        public void ShowPreviousTile()
        {
            Debug.Log("updateTileIndexText");
            Show(CurrentTile - 1);
        }

        /// <summary>
        /// Creates a new tile with default values loaded from <see cref="TileData"/>. 
        /// </summary>
        public void CreateTile()
        {
            Debug.Log("updateTileIndexText");
            if (gameObject.activeSelf == false)
            {
                gameObject.SetActive(true);
            }

            //Hide tile count
            TileCountText.gameObject.SetActive(false);

            WidthButton.GetComponentInChildren<Text>().text = TileData.DefaultWidthInCM.ToString("n0") + " cm";
            HeightButton.GetComponentInChildren<Text>().text = TileData.DefaultHeightInCM.ToString("n0") + " cm";
            JointSizeButton.GetComponentInChildren<Text>().text = TileData.DefaultJointThicknessInMM.ToString("n0") + " mm";
            ThicknessButton.GetComponentInChildren<Text>().text = TileData.DefaultTileThicknessInMM.ToString("n0") + " mm";
            NameButton.GetComponentInChildren<Text>().text = TileData.DefaultName;

            TextureButton.GetComponent<RawImage>().texture = GlobalSettings.Instance.TextureLibrary.Textures[TileData.DefaultTextureIndex];
            CurrentTextureIndex = TileData.DefaultTextureIndex;
            m_CurrentGuid = Guid.Empty;
            enableEditing();
            CreationHeader.gameObject.SetActive(true);
        }

        public void updateTexture(int textureIndex)
        {
            Debug.Log("updateTileIndexText");
            CurrentTextureIndex = textureIndex;
            TextureButton.GetComponent<RawImage>().texture = GlobalSettings.Instance.TextureLibrary.Textures[textureIndex];
        }

        /// <summary>
        /// Saves the current tile and adds it to the cached tiles if it is a new one.
        /// </summary>
        public void saveTile()
        {
            Debug.Log("updateTileIndexText");
            var tile = getTileInformationFromButtons();
            if (tile.Guid == Guid.Empty)
            {
                tile.Guid = Guid.NewGuid();
                TileMenuManager.Instance.AddToCachedTiles(tile);
                TileMenuListView.Instance.UpdatePages(TileMenuManager.Instance.SavedTiles);
                CurrentTile = TileMenuManager.Instance.SavedTiles.Count - 1;
                TileMenuManager.Instance.ShowDetailView(CurrentTile);
            }
            else
            {
                TileMenuManager.Instance.SavedTiles[CurrentTile] = tile;
            }
            tile.SaveToJson();
        }

        #endregion // Public methods

        #region Private internal methods

        private void placeTiles()
        {
            Debug.Log("updateTileIndexText");
            if (RoomManager.Instance.Floor != null)
            {
                MenuHub.Instance.ShowMenu(MainMenuManager.Instance.gameObject);
                TilesGenerator.Instance.CreateTileFloor(TileMenuManager.Instance.SavedTiles[CurrentTile]);
            }
            else
            {
                TextManager.Instance.ShowWarning("You need to create a floor first!");
            }
        }

        /// <summary>
        /// Creates a <see cref="TileData"/> from the buttons information. 
        /// </summary>
        private TileData getTileInformationFromButtons()
        {
            Debug.Log("updateTileIndexText");
            TileData tile = new TileData(
                ParserUtility.StringToFloat(HeightButton.GetComponentInChildren<Text>().text) / 100f,
                ParserUtility.StringToFloat(WidthButton.GetComponentInChildren<Text>().text) / 100f,
                ParserUtility.StringToFloat(ThicknessButton.GetComponentInChildren<Text>().text) / 1000f,
                ParserUtility.StringToFloat(JointSizeButton.GetComponentInChildren<Text>().text) / 1000f,
                CurrentTextureIndex,
                NameButton.GetComponentInChildren<Text>().text,
                m_CurrentGuid);
            return tile;
        }

        /// <summary>
        /// Loads the tile given by the current tile index.
        /// </summary>
        private void loadCurrentTile()
        {
            Debug.Log("updateTileIndexText");
            loadTexture();
            loadWidth();
            loadHeight();
            loadJointSize();
            loadThickness();
            loadName();
        }

        #region loading methods

        private void loadTexture()
        {
            Debug.Log("updateTileIndexText");
            TextureButton.GetComponent<RawImage>().texture = GlobalSettings.Instance.TextureLibrary.Textures[TileMenuManager.Instance.SavedTiles[CurrentTile].TextureIndex];
        }

        private void loadWidth()
        {
            Debug.Log("updateTileIndexText");
            float widthInCM = TileMenuManager.Instance.SavedTiles[CurrentTile].Width * 100;
            WidthButton.GetComponentInChildren<Text>().text = (widthInCM).ToString("n0") + " cm";
        }

        private void loadHeight()
        {
            Debug.Log("updateTileIndexText");
            float heightInCM = TileMenuManager.Instance.SavedTiles[CurrentTile].Height * 100;
            HeightButton.GetComponentInChildren<Text>().text = (heightInCM).ToString("n0") + " cm";
        }

        private void loadJointSize()
        {
            Debug.Log("updateTileIndexText");
            JointSizeButton.GetComponentInChildren<Text>().text = (TileMenuManager.Instance.SavedTiles[CurrentTile].JointSize * 1000).ToString("n0") + " mm";
        }

        private void loadThickness()
        {
            Debug.Log("updateTileIndexText");
            ThicknessButton.GetComponentInChildren<Text>().text = (TileMenuManager.Instance.SavedTiles[CurrentTile].TileThickness * 1000).ToString("n0") + " mm";
        }

        private void loadName()
        {
            Debug.Log("updateTileIndexText");
            string name = TileMenuManager.Instance.SavedTiles[CurrentTile].Name;
            NameButton.GetComponentInChildren<Text>().text = name;
        }

        #endregion // loading methods

        /// <summary>
        /// Changes the edit mode to the opposite value.
        /// </summary>
        private void changeEditMode()
        {
            Debug.Log("updateTileIndexText");
            if (!m_EditMode)
            {
                enableEditing();
            }
            else
            {
                disableEditing();
            }
        }

        /// <summary>
        /// Enables editing by enabling input for each button.
        /// </summary>
        private void enableEditing()
        {
            Debug.Log("updateTileIndexText");
            m_EditMode = true;
            enableButton(TextureButton);
            enableButton(WidthButton);
            enableButton(HeightButton);
            enableButton(JointSizeButton);
            enableButton(ThicknessButton);
            enableButton(NameButton);
            EditButton.gameObject.SetActive(false);
            PlaceTileButton.gameObject.SetActive(false);
            AcceptEditButton.gameObject.SetActive(true);
            CancelEditButton.gameObject.SetActive(true);
            EditBackground.gameObject.SetActive(true);
        }

        /// <summary>
        /// Disables editing by disabling input for each button.
        /// </summary>
        private void disableEditing()
        {
            Debug.Log("updateTileIndexText");
            m_EditMode = false;
            disableButton(TextureButton);
            disableButton(WidthButton);
            disableButton(HeightButton);
            disableButton(JointSizeButton);
            disableButton(ThicknessButton);
            disableButton(NameButton);
            EditButton.gameObject.SetActive(true);
            PlaceTileButton.gameObject.SetActive(true);
            AcceptEditButton.gameObject.SetActive(false);
            CancelEditButton.gameObject.SetActive(false);
            EditBackground.gameObject.SetActive(false);
            CreationHeader.gameObject.SetActive(false);
        }

        /// <summary>
        /// Enables the given button and activates the <see cref="GazeResponder"/> component and outline if it has any. 
        /// </summary>
        /// <param name="button"></param>
        private void enableButton(Button button)
        {
            Debug.Log("updateTileIndexText");
            button.interactable = true;
            GazeResponder gazeRes;
            if ((gazeRes = button.gameObject.GetComponent<GazeResponder>()) != null)
                gazeRes.enabled = true;
            CustomOutline o;
            if ((o = button.GetComponent<CustomOutline>()) != null)
                o.enabled = true;
        }

        /// <summary>
        /// Disables the given button and deactivates the <see cref="GazeResponder"/> component and outline if it has any. 
        /// </summary>
        /// <param name="button"></param>
        private void disableButton(Button button)
        {
            Debug.Log("updateTileIndexText");
            button.interactable = false;
            GazeResponder gazeRes;
            if ((gazeRes = button.gameObject.GetComponent<GazeResponder>()) != null)
                gazeRes.enabled = false;
            CustomOutline o;
            if ((o = button.GetComponent<CustomOutline>()) != null)
                o.enabled = false;
        }

        private void editWidth()
        {
            Debug.Log("updateTileIndexText");
            showNumbersKeyboard();
            KeyboardWithNumbers.Instance.OnTextSubmitted += acceptWidthFromKeyboard;
            KeyboardWithNumbers.Instance.OnClosed += closeKeyboard;
        }

        private void acceptWidthFromKeyboard(object sender, EventArgs e)
        {
            Debug.Log("updateTileIndexText");
            KeyboardWithNumbers.Instance.OnTextSubmitted -= acceptWidthFromKeyboard;
            if(!string.IsNullOrEmpty(KeyboardWithNumbers.Instance.InputField.text))
                WidthButton.GetComponentInChildren<Text>().text = KeyboardWithNumbers.Instance.InputField.text + " cm";
            KeyboardWithNumbers.Instance.OnClosed -= closeKeyboard;
        }

        private void editHeight()
        {
            Debug.Log("updateTileIndexText");
            showNumbersKeyboard();
            KeyboardWithNumbers.Instance.OnTextSubmitted += acceptHeightFromKeyboard;
            KeyboardWithNumbers.Instance.OnClosed += closeKeyboard;
        }

        private void acceptHeightFromKeyboard(object sender, EventArgs e)
        {
            Debug.Log("updateTileIndexText");
            KeyboardWithNumbers.Instance.OnTextSubmitted -= acceptHeightFromKeyboard;
            if (!string.IsNullOrEmpty(KeyboardWithNumbers.Instance.InputField.text))
                HeightButton.GetComponentInChildren<Text>().text = KeyboardWithNumbers.Instance.InputField.text + " cm";
            KeyboardWithNumbers.Instance.OnClosed -= closeKeyboard;
        }

        private void editThickness()
        {
            Debug.Log("updateTileIndexText");
            showNumbersKeyboard();
            KeyboardWithNumbers.Instance.OnTextSubmitted += acceptThicknessFromKeyboard;
            KeyboardWithNumbers.Instance.OnClosed += closeKeyboard;
        }

        private void acceptThicknessFromKeyboard(object sender, EventArgs e)
        {
            Debug.Log("updateTileIndexText");
            KeyboardWithNumbers.Instance.OnTextSubmitted -= acceptThicknessFromKeyboard;
            if (!string.IsNullOrEmpty(KeyboardWithNumbers.Instance.InputField.text))
                ThicknessButton.GetComponentInChildren<Text>().text = KeyboardWithNumbers.Instance.InputField.text + " mm";
            KeyboardWithNumbers.Instance.OnClosed -= closeKeyboard;
        }

        private void editJointSize()
        {
            Debug.Log("updateTileIndexText");
            showNumbersKeyboard();
            KeyboardWithNumbers.Instance.OnTextSubmitted += acceptJointSizeFromKeyboard;
            KeyboardWithNumbers.Instance.OnClosed += closeKeyboard;
        }

        private void acceptJointSizeFromKeyboard(object sender, EventArgs e)
        {
            Debug.Log("updateTileIndexText");
            KeyboardWithNumbers.Instance.OnTextSubmitted -= acceptJointSizeFromKeyboard;
            if (!string.IsNullOrEmpty(KeyboardWithNumbers.Instance.InputField.text))
                JointSizeButton.GetComponentInChildren<Text>().text = KeyboardWithNumbers.Instance.InputField.text + " mm";
            KeyboardWithNumbers.Instance.OnClosed -= closeKeyboard;
        }

        private void editName()
        {
            Debug.Log("updateTileIndexText");
            showAlphaKeyboard();
            KeyboardWithNumbers.Instance.OnTextSubmitted += acceptNameFromKeyboard;
            KeyboardWithNumbers.Instance.OnClosed += closeKeyboard;
        }

        private void acceptNameFromKeyboard(object sender, EventArgs e)
        {
            Debug.Log("updateTileIndexText");
            KeyboardWithNumbers.Instance.OnTextSubmitted -= acceptNameFromKeyboard;
            if (!string.IsNullOrEmpty(KeyboardWithNumbers.Instance.InputField.text))
                NameButton.GetComponentInChildren<Text>().text = KeyboardWithNumbers.Instance.InputField.text;
            KeyboardWithNumbers.Instance.OnClosed -= closeKeyboard;
        }

        private void closeKeyboard(object sender, EventArgs e)
        {
            Debug.Log("updateTileIndexText");
            KeyboardWithNumbers.Instance.OnTextSubmitted -= closeKeyboard;
            KeyboardWithNumbers.Instance.OnTextSubmitted -= acceptWidthFromKeyboard;
            KeyboardWithNumbers.Instance.OnTextSubmitted -= acceptHeightFromKeyboard;
            KeyboardWithNumbers.Instance.OnTextSubmitted -= acceptNameFromKeyboard;
            KeyboardWithNumbers.Instance.OnTextSubmitted -= acceptJointSizeFromKeyboard;
            KeyboardWithNumbers.Instance.OnTextSubmitted -= acceptThicknessFromKeyboard;
        }

        private void editTexture()
        {
            Debug.Log("updateTileIndexText");
            TileMenuManager.Instance.showTextureListView();
        }

        /// <summary>
        /// Enables the numbers keyboard.
        /// </summary>
        private void showNumbersKeyboard()
        {
            Debug.Log("updateTileIndexText");
            KeyboardWithNumbers.Instance.FollowWithOffset(transform, 0.01f);
            KeyboardWithNumbers.Instance.PresentKeyboard(KeyboardWithNumbers.LayoutType.Number);
        }

        /// <summary>
        /// Enables the alphanumeric keyboard.
        /// </summary>
        private void showAlphaKeyboard()
        {
            Debug.Log("updateTileIndexText");
            KeyboardWithNumbers.Instance.FollowWithOffset(transform, 0.01f);
            KeyboardWithNumbers.Instance.PresentKeyboard(KeyboardWithNumbers.LayoutType.Alpha);
        }

        private void acceptKeyboardInput(object sender, EventArgs e)
        {
            Debug.Log("updateTileIndexText");
            if (m_SelectedButton.Equals(HeightButton))
            {
                HeightButton.GetComponentInChildren<Text>().text = KeyboardWithNumbers.Instance.InputField.text + " cm";                
            }
            else if (m_SelectedButton.Equals(WidthButton))
            {
                WidthButton.GetComponentInChildren<Text>().text = KeyboardWithNumbers.Instance.InputField.text + " cm";
            }
            else if (m_SelectedButton.Equals(NameButton))
            {
                NameButton.GetComponentInChildren<Text>().text = KeyboardWithNumbers.Instance.InputField.text;
            }
            KeyboardWithNumbers.Instance.OnTextSubmitted -= acceptKeyboardInput;
            KeyboardWithNumbers.Instance.StopFollow();
        }

        private void updateTileIndexText()
        {
            Debug.Log("updateTileIndexText");
            TileCountText.text = string.Format("{0} / {1}", CurrentTile + 1, TileMenuManager.Instance.SavedTiles.Count);
        }

        #endregion // Private internal methods
    }
}
