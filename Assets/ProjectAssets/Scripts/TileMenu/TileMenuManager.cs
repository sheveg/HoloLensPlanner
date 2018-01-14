using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using System.IO;
using UnityEngine.UI;
using System.Linq;

namespace HoloLensPlanner.TEST
{
    public enum TileMenuState
    {
        ListView,
        DetailView,
        NewTileView
    }

    public class TileMenuManager : SingleInstance<TileMenuManager>
    {
        [SerializeField]
        private Button ShowNextButton;

        [SerializeField]
        private Button ShowPreviousButton;

        [SerializeField]
        private Button ShowListViewButton;

        [SerializeField]
        private Button ShowDetailViewButton;

        [SerializeField]
        private Button NewTileButton;

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

            NewTileButton.onClick.AddListener(CreateTile);

            TileMenuListView.Instance.gameObject.SetActive(true);
            TileMenuDetailView.Instance.gameObject.SetActive(false);
        }

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

        private void showListView()
        {
            // nothing to do if already in list view
            if (m_State == TileMenuState.ListView)
                return;

            m_State = TileMenuState.ListView;
            TileMenuDetailView.Instance.Hide();

            int pageIndex = Mathf.CeilToInt(TileMenuDetailView.Instance.CurrentTile / ObjectPage.MaxObjectsCount);
            TileMenuListView.Instance.Show(pageIndex);
        }

        private void CreateTile()
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

            TileMenuDetailView.Instance.NewTile();
        }

        private void printStateError()
        {
            Debug.Log("Case " + m_State + " not implemented!");
        }

        public void addToCachedTiles(TileData tile)
        {
            m_SavedTiles.Add(tile);
        }

        public void updateCachedTiles(TileData tile)
        {
            TileData tmp_tile = m_SavedTiles.Where(t => t.Guid == tile.Guid).FirstOrDefault();
            if (tmp_tile != null)
            {
                tmp_tile = tile;
            }
        }
    }
}


