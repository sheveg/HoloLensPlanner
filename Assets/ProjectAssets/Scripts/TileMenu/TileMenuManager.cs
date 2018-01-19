using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using System.IO;
using UnityEngine.UI;
using System.Linq;

namespace HoloLensPlanner
{
    /// <summary>
    /// Possible states of <see cref="TileMenuManager"/>. 
    /// </summary>
    public enum TileMenuState
    {
        /// <summary>
        /// Corresponds to <see cref="TileMenuListView"/> 
        /// </summary>
        ListView,
        /// <summary>
        /// Corresponds to <see cref="TileMenuDetailView"/> 
        /// </summary>
        DetailView,
        /// <summary>
        /// Corresponds to <see cref="TileMenuDetailView"/> in creation mode. 
        /// </summary>
        NewTileView,
        /// <summary>
        /// Corresponds to <see cref="TextureListView"/>
        /// </summary>
        TextureListView
    }

    /// <summary>
    /// TileMenuManager is the central unit handling the TileMenu. It works together with <see cref="TileMenuListView"/> and <see cref="TileMenuDetailView"/>.  
    /// </summary>
    public class TileMenuManager : SingleInstance<TileMenuManager>
    {
        #region Editor variables

        /// <summary>
        /// Button to switch to the next element.
        /// </summary>
        [SerializeField]
        private Button ShowNextButton;

        /// <summary>
        /// Button to switch to the previous element.
        /// </summary>
        [SerializeField]
        private Button ShowPreviousButton;

        /// <summary>
        /// Button to switch to list view.
        /// </summary>
        [SerializeField]
        private Button ShowListViewButton;

        /// <summary>
        /// Button to switch to detail view.
        /// </summary>
        [SerializeField]
        private Button ShowDetailViewButton;

        /// <summary>
        /// Button to create a new tile.
        /// </summary>
        [SerializeField]
        private Button NewTileButton;

        /// <summary>
        /// Button to hide this view.
        /// </summary>
        [SerializeField]
        private Button CloseButton;

        #endregion // Editor variables

        public List<TileData> SavedTiles { get { return m_SavedTiles; } }

        // cached tiles
        private List<TileData> m_SavedTiles = new List<TileData>();

        // Make the list view the default state
        private TileMenuState m_State = TileMenuState.ListView;

        private void Start()
        {
            loadTiles();
            TileMenuListView.Instance.CreatePages(m_SavedTiles);

            // assign the button calls
            ShowNextButton.onClick.AddListener(showNext);
            ShowPreviousButton.onClick.AddListener(showPrevious);
            ShowDetailViewButton.onClick.AddListener(showDetailView);
            ShowListViewButton.onClick.AddListener(showListView);
            NewTileButton.onClick.AddListener(createTile);
            CloseButton.onClick.AddListener( () => Hide(true));

            TileMenuListView.Instance.gameObject.SetActive(true);
            TileMenuDetailView.Instance.gameObject.SetActive(false);

            //Hide();
        }

        /// <summary>
        /// Switches to detail view to the given tile.
        /// </summary>
        /// <param name="tileIndex"></param>
        public void ShowDetailView(int tileIndex)
        {
            // nothing to do if already in detal view
            if (m_State == TileMenuState.DetailView)
                return;

            m_State = TileMenuState.DetailView;
            TileMenuListView.Instance.Hide();
            TileMenuDetailView.Instance.Show(tileIndex);
        }

        /// <summary>
        /// Switches to list view to the given page.
        /// </summary>
        /// <param name="pageIndex"></param>
        public void ShowListView(int pageIndex)
        {
            // nothing to do if already in list view
            if (m_State == TileMenuState.ListView)
                return;

            m_State = TileMenuState.ListView;
            TileMenuDetailView.Instance.Hide();
            TileMenuListView.Instance.Show(pageIndex);
        }

        /// <summary>
        /// Shows the TileMenu.
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Hides the TileMenu.
        /// </summary>
        public void Hide(bool showMainMenu = false)
        {
            gameObject.SetActive(false);
            if (showMainMenu)
                MainMenuManager.Instance.Show();
        }

        /// <summary>
        /// Adds the given tile to the saved tiles.
        /// </summary>
        /// <param name="tile"></param>
        public void AddToCachedTiles(TileData tile)
        {
            m_SavedTiles.Add(tile);
        }

        /// <summary>
        /// Loads the tiles into <see cref="m_SavedTiles"/>. 
        /// </summary>
        private void loadTiles()
        {
            // first get all saved tile files
            string[] savedTilesPaths = Directory.GetFiles(GlobalSettings.Instance.PathLibrary.SavedTilesPath, "*." + GlobalSettings.Instance.PathLibrary.SavedTilesType);
            foreach (var savedTilePath in savedTilesPaths)
            {
                string tileName = Path.GetFileNameWithoutExtension(savedTilePath);
                var tile = new TileData();
                tile.LoadFromJson(tileName);
                m_SavedTiles.Add(tile);
            }
        }

        /// <summary>
        /// Shows the next element.
        /// </summary>
        private void showNext()
        {
            switch (m_State)
            {
                case TileMenuState.ListView:
                    TileMenuListView.Instance.ShowNextPage();
                    break;
                case TileMenuState.DetailView:
                    TileMenuDetailView.Instance.ShowNextTile();
                    break;
                default:
                    printStateError();
                    break;
            }
        }

        /// <summary>
        /// Shows the previous element.
        /// </summary>
        private void showPrevious()
        {
            switch (m_State)
            {
                case TileMenuState.ListView:
                    TileMenuListView.Instance.ShowPreviousPage();
                    break;
                case TileMenuState.DetailView:
                    TileMenuDetailView.Instance.ShowPreviousTile();
                    break;
                default:
                    printStateError();
                    break;
            }
        }
        
        /// <summary>
        /// Shows the detail view with the first tile in the curret page.
        /// </summary>
        private void showDetailView()
        {
            // nothing to do if already in detal view
            if (m_State == TileMenuState.DetailView)
                return;

            m_State = TileMenuState.DetailView;
            TileMenuListView.Instance.Hide();


            int tileIndex = TileMenuListView.Instance.CurrentPage * ObjectPage.MaxObjectsCount;
            TileMenuDetailView.Instance.Show(tileIndex);
        }

        /// <summary>
        /// Shows the list view on the page where the last current tile was.
        /// </summary>
        private void showListView()
        {
            // nothing to do if already in list view
            if (m_State == TileMenuState.ListView)
                return;

            if (m_State != TileMenuState.NewTileView)
            {
                TileMenuListView.Instance.UpdatePages(m_SavedTiles);
            }

            m_State = TileMenuState.ListView;
            TileMenuDetailView.Instance.Hide();

            int pageIndex = Mathf.CeilToInt(TileMenuDetailView.Instance.CurrentTile / ObjectPage.MaxObjectsCount);
            TileMenuListView.Instance.Show(pageIndex);
        }

        public void showTextureListView()
        {
            m_State = TileMenuState.TextureListView;
            TileMenuDetailView.Instance.gameObject.SetActive(false);
            TileMenuListView.Instance.destroyPages();
            TextureListView.Instance.CreatePages();

            TextureListView.Instance.gameObject.SetActive(true);
        }

        public void acceptTexture(int textureIndex)
        {
            m_State = TileMenuState.DetailView;

            TextureListView.Instance.destroyPages();
            TextureListView.Instance.gameObject.SetActive(false);
            
            TileMenuDetailView.Instance.updateTexture(textureIndex);
            TileMenuDetailView.Instance.gameObject.SetActive(true);

            TileMenuListView.Instance.CreatePages(m_SavedTiles);
        }

        /// <summary>
        /// Calls the <see cref="TileMenuDetailView"/> to create a new tile. 
        /// </summary>
        private void createTile()
        {
            // nothing to do if already in new tile view
            if (m_State == TileMenuState.NewTileView)
            {
                return;
            }
            else if(m_State == TileMenuState.ListView)
            {
                TileMenuListView.Instance.Hide();
            }

            m_State = TileMenuState.NewTileView;
            TileMenuDetailView.Instance.CreateTile();
        }

        /// <summary>
        /// Prints a default state error.
        /// </summary>
        private void printStateError()
        {
            Debug.Log("Case " + m_State + " not implemented!");
        }
    }
}


