using UnityEngine;
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

        private int m_CurrentTextureIndex;

        private Guid m_CurrentGuid;

        #endregion // Cached variables

        #region MonoBehaviour methods

        private void Start()
        {
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
        }

        #endregion // Monobehaviour methods

        #region Public methods

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
            CurrentTile = MathUtility.WrapArrayIndex(tileIndex, TileMenuManager.Instance.SavedTiles.Count);

            //Save GUID and textureIndex of tile
            //Both needed to correctly store tile later
            m_CurrentTextureIndex = TileMenuManager.Instance.SavedTiles[CurrentTile].TextureIndex;
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
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Shows the next tile in the tile list.
        /// </summary>
        public void ShowNextTile()
        {
            Show(CurrentTile + 1);
        }

        /// <summary>
        /// Shows the previous tile in the tile list.
        /// </summary>
        public void ShowPreviousTile()
        {
            Show(CurrentTile - 1);
        }

        /// <summary>
        /// Creates a new tile with default values loaded from <see cref="TileData"/>. 
        /// </summary>
        public void CreateTile()
        {
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
            m_CurrentTextureIndex = TileData.DefaultTextureIndex;
            m_CurrentGuid = Guid.Empty;
            enableEditing();
            CreationHeader.gameObject.SetActive(true);
        }

        public void updateTexture(int textureIndex)
        {
            TextureButton.GetComponent<RawImage>().texture = GlobalSettings.Instance.TextureLibrary.Textures[textureIndex];
        }

        #endregion // Public methods

        #region Private internal methods

        /// <summary>
        /// Saves the current tile and adds it to the cached tiles if it is a new one.
        /// </summary>
        private void saveTile()
        {
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

        /// <summary>
        /// Creates a <see cref="TileData"/> from the buttons information. 
        /// </summary>
        private TileData getTileInformationFromButtons()
        {
            TileData tile = new TileData(
                ParserUtility.StringToFloat(HeightButton.GetComponentInChildren<Text>().text) / 100f,
                ParserUtility.StringToFloat(WidthButton.GetComponentInChildren<Text>().text) / 100f,
                ParserUtility.StringToFloat(ThicknessButton.GetComponentInChildren<Text>().text) / 1000f,
                ParserUtility.StringToFloat(JointSizeButton.GetComponentInChildren<Text>().text) / 1000f,
                m_CurrentTextureIndex,
                NameButton.GetComponentInChildren<Text>().text,
                m_CurrentGuid);
            return tile;
        }

        /// <summary>
        /// Loads the tile given by the current tile index.
        /// </summary>
        private void loadCurrentTile()
        {
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
            TextureButton.GetComponent<RawImage>().texture = GlobalSettings.Instance.TextureLibrary.Textures[TileMenuManager.Instance.SavedTiles[CurrentTile].TextureIndex];
        }

        private void loadWidth()
        {
            float widthInCM = TileMenuManager.Instance.SavedTiles[CurrentTile].Width * 100;
            WidthButton.GetComponentInChildren<Text>().text = (widthInCM).ToString("n0") + " cm";
        }

        private void loadHeight()
        {
            float heightInCM = TileMenuManager.Instance.SavedTiles[CurrentTile].Height * 100;
            HeightButton.GetComponentInChildren<Text>().text = (heightInCM).ToString("n0") + " cm";
        }

        private void loadJointSize()
        {
            JointSizeButton.GetComponentInChildren<Text>().text = (TileMenuManager.Instance.SavedTiles[CurrentTile].JointSize * 1000).ToString("n0") + " mm";
        }

        private void loadThickness()
        {
            ThicknessButton.GetComponentInChildren<Text>().text = (TileMenuManager.Instance.SavedTiles[CurrentTile].TileThickness * 1000).ToString("n0") + " mm";
        }

        private void loadName()
        {
            string name = TileMenuManager.Instance.SavedTiles[CurrentTile].Name;
            NameButton.GetComponentInChildren<Text>().text = name;
        }

        #endregion // loading methods

        /// <summary>
        /// Changes the edit mode to the opposite value.
        /// </summary>
        private void changeEditMode()
        {
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
            button.enabled = true;
            GazeResponder gazeRes;
            if ((gazeRes = button.gameObject.GetComponent<GazeResponder>()) != null)
                gazeRes.enabled = true;
            Outline o;
            if ((o = button.GetComponent<Outline>()) != null)
                o.enabled = true;
        }

        /// <summary>
        /// Disables the given button and deactivates the <see cref="GazeResponder"/> component and outline if it has any. 
        /// </summary>
        /// <param name="button"></param>
        private void disableButton(Button button)
        {
            button.enabled = false;
            GazeResponder gazeRes;
            if ((gazeRes = button.gameObject.GetComponent<GazeResponder>()) != null)
                gazeRes.enabled = false;
            Outline o;
            if ((o = button.GetComponent<Outline>()) != null)
                o.enabled = false;
        }

        private void editWidth()
        {
            showNumbersKeyboard();
            KeyboardWithNumbers.Instance.OnTextSubmitted += acceptWidthFromKeyboard;
        }

        private void acceptWidthFromKeyboard(object sender, EventArgs e)
        {
            KeyboardWithNumbers.Instance.OnTextSubmitted -= acceptWidthFromKeyboard;
            if(!string.IsNullOrEmpty(KeyboardWithNumbers.Instance.InputField.text))
                WidthButton.GetComponentInChildren<Text>().text = KeyboardWithNumbers.Instance.InputField.text + " cm";
        }

        private void editHeight()
        {
            showNumbersKeyboard();
            KeyboardWithNumbers.Instance.OnTextSubmitted += acceptHeightFromKeyboard;
        }

        private void acceptHeightFromKeyboard(object sender, EventArgs e)
        {
            KeyboardWithNumbers.Instance.OnTextSubmitted -= acceptHeightFromKeyboard;
            if (!string.IsNullOrEmpty(KeyboardWithNumbers.Instance.InputField.text))
                HeightButton.GetComponentInChildren<Text>().text = KeyboardWithNumbers.Instance.InputField.text + " cm";
        }

        private void editThickness()
        {
            showNumbersKeyboard();
            KeyboardWithNumbers.Instance.OnTextSubmitted += acceptThicknessFromKeyboard;
        }

        private void acceptThicknessFromKeyboard(object sender, EventArgs e)
        {
            KeyboardWithNumbers.Instance.OnTextSubmitted -= acceptThicknessFromKeyboard;
            if (!string.IsNullOrEmpty(KeyboardWithNumbers.Instance.InputField.text))
                ThicknessButton.GetComponentInChildren<Text>().text = KeyboardWithNumbers.Instance.InputField.text + " mm";
        }

        private void editJointSize()
        {
            showNumbersKeyboard();
            KeyboardWithNumbers.Instance.OnTextSubmitted += acceptJointSizeFromKeyboard;
        }

        private void acceptJointSizeFromKeyboard(object sender, EventArgs e)
        {
            KeyboardWithNumbers.Instance.OnTextSubmitted -= acceptJointSizeFromKeyboard;
            if (!string.IsNullOrEmpty(KeyboardWithNumbers.Instance.InputField.text))
                JointSizeButton.GetComponentInChildren<Text>().text = KeyboardWithNumbers.Instance.InputField.text + " mm";
        }

        private void editName()
        {
            showAlphaKeyboard();
            KeyboardWithNumbers.Instance.OnTextSubmitted += acceptNameFromKeyboard;
        }

        private void acceptNameFromKeyboard(object sender, EventArgs e)
        {
            KeyboardWithNumbers.Instance.OnTextSubmitted -= acceptNameFromKeyboard;
            if (!string.IsNullOrEmpty(KeyboardWithNumbers.Instance.InputField.text))
                NameButton.GetComponentInChildren<Text>().text = KeyboardWithNumbers.Instance.InputField.text;
        }

        private void editTexture()
        {
            TileMenuManager.Instance.showTextureListView();
        }

        /// <summary>
        /// Enables the numbers keyboard.
        /// </summary>
        private void showNumbersKeyboard()
        {
            KeyboardWithNumbers.Instance.FollowWithOffset(transform, 0.01f);
            KeyboardWithNumbers.Instance.PresentKeyboard(KeyboardWithNumbers.LayoutType.Number);
        }

        /// <summary>
        /// Enables the alphanumeric keyboard.
        /// </summary>
        private void showAlphaKeyboard()
        {
            KeyboardWithNumbers.Instance.FollowWithOffset(transform, 0.01f);
            KeyboardWithNumbers.Instance.PresentKeyboard(KeyboardWithNumbers.LayoutType.Alpha);
        }


        private void editButtonInformation(Button button)
        {
            m_SelectedButton = button;

            if (button.Equals(HeightButton) || button.Equals(WidthButton))
            {                
                KeyboardWithNumbers.Instance.FollowWithOffset(button.transform, 0.01f);
                KeyboardWithNumbers.Instance.PresentKeyboard(KeyboardWithNumbers.LayoutType.Number);

                //Now done by simpleTagalong script
                //KeyboardNumbers.Instance.gameObject.transform.position = b.transform.position - 0.4f * b.transform.forward;

                KeyboardWithNumbers.Instance.OnTextSubmitted += acceptKeyboardInput;
            }
            else if (button.Equals(NameButton))
            {
                KeyboardWithNumbers.Instance.FollowWithOffset(button.transform, 0.01f);
                KeyboardWithNumbers.Instance.PresentKeyboard(KeyboardWithNumbers.LayoutType.Alpha);
                KeyboardWithNumbers.Instance.OnTextSubmitted += acceptKeyboardInput;
            }
            else if (button.Equals(TextureButton))
            {

                this.gameObject.SetActive(false);
                TextureListView.Instance.CreatePages();
                TextureListView.Instance.Show(Mathf.CeilToInt(m_CurrentTextureIndex / ObjectPage.MaxObjectsCount));
            }
        }

        private void acceptKeyboardInput(object sender, EventArgs e)
        {
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
            TileCountText.text = string.Format("{0} / {1}", CurrentTile + 1, TileMenuManager.Instance.SavedTiles.Count);
        }

        #endregion // Private internal methods
    }
}
