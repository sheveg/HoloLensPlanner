using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using UnityEngine.UI;
using HoloLensPlanner.GazeResponse;

namespace HoloLensPlanner.TEST
{
    /// <summary>
    /// TileMenuDetailView is responsible for handling input from detail view buttons and showing the saved tiles in a detail view.
    /// </summary>
    public class TileMenuDetailView : Singleton<TileMenuDetailView>
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

        /// <summary>
        /// In Edit mode we can change the values of the current tile.
        /// </summary>
        private bool m_EditMode = false;

        /// <summary>
        /// Index of current tile of the saved tiles.
        /// </summary>
        public int CurrentTile { get; private set; }

        private void Start()
        {
            EditButton.onClick.AddListener(enableEditing);
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
            CurrentTile = wrapTileIndex(tileIndex);
            loadCurrentTile();
            if(m_EditMode)
                disableEditing();
            updateTileIndexText();
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
        }

        private void disableEditing()
        {
            m_EditMode = false;
            disableButton(TextureButton);
            disableButton(WidthButton);
            disableButton(HeightButton);
            disableButton(JointSizeButton);
            disableButton(ThicknessButton);
        }

        private void enableEditing()
        {
            m_EditMode = true;
            enableButton(TextureButton);
            enableButton(WidthButton);
            enableButton(HeightButton);
            enableButton(JointSizeButton);
            enableButton(ThicknessButton);
        }

        private void loadTexture()
        {
            Texture2D tileTexture = GlobalSettings.Instance.TextureLibrary.Textures[TileMenuManager.Instance.SavedTiles[CurrentTile].TextureIndex];
            Sprite tileSprite = Sprite.Create(
                tileTexture,
                new Rect(0, 0, tileTexture.width, tileTexture.height),
                new Vector2(0.5f, 0.5f));
            TextureButton.image.sprite = tileSprite;
        }

        private void loadWidth()
        {
            float widthInM = TileMenuManager.Instance.SavedTiles[CurrentTile].Width;
            WidthButton.GetComponentInChildren<Text>().text = (widthInM * 100).ToString("n0") + " cm";
        }

        private void loadHeight()
        {
            float heightInM = TileMenuManager.Instance.SavedTiles[CurrentTile].Height;
            HeightButton.GetComponentInChildren<Text>().text = (heightInM * 100).ToString("n0") + " cm";
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
        }

        private void updateTileIndexText()
        {
            TileCountText.text = string.Format("{0} / {1}", CurrentTile + 1, TileMenuManager.Instance.SavedTiles.Count);
        }

        private int wrapTileIndex(int tileIndex)
        {
            return (tileIndex % TileMenuManager.Instance.SavedTiles.Count + TileMenuManager.Instance.SavedTiles.Count) % TileMenuManager.Instance.SavedTiles.Count;
        }

    }
}
