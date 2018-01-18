using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using UnityEngine.UI;
using HoloLensPlanner.Utilities;

namespace HoloLensPlanner
{
    /// <summary>
    /// TileMenuListView is responsible for handling input from the list view buttons and showing the saved tiles in a list display.
    /// </summary>
    public class TileMenuListView : SingleInstance<TileMenuListView>
    {
        #region Editor variables

        /// <summary>
        /// Prefab for a default object page.
        /// </summary>
        [SerializeField]
        private ObjectPage ObjectPagePrefab;

        /// <summary>
        /// Transform parent of all spawned pages.
        /// </summary>
        [SerializeField]
        private Transform PageParent;

        /// <summary>
        /// Text component to show the curennt page.
        /// </summary>
        [SerializeField]
        private Text PageText;

        #endregion // Editor variables

        #region Cached variables

        /// <summary>
        /// Current page index the user is lookit at.
        /// </summary>
        public int CurrentPage { get; private set; }

        /// <summary>
        /// List of needed pages to show the saved tiles.
        /// </summary>
        private List<ObjectPage> m_ObjectPages = new List<ObjectPage>();

        #endregion // Cached variables

        #region Public methods

        /// <summary>
        /// Creates enough pages to load the given tiles and fills up the pages.
        /// </summary>
        /// <param name="tiles"></param>
        public void CreatePages(List<TileData> tiles)
        {
            // round up , 9 elements and max 6 objects per page => 2 pages
            int neededPages = Mathf.CeilToInt(tiles.Count / (float)ObjectPage.MaxObjectsCount);
            for (int i = 0; i < neededPages; i++)
            {
                var page = Instantiate(ObjectPagePrefab, PageParent);
                m_ObjectPages.Add(page);
            }
            updatePageText();
            // we need to create at least one page, even when there are no saved tiles, but we cannot fill pages in this case
            if (tiles.Count > 0)
                fillPagesInformation(tiles);
            // show the first page as the default page
            if (neededPages > 1)
            {
                for (int i = 1; i < m_ObjectPages.Count; i++)
                {
                    m_ObjectPages[i].gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Updates the page count and creates new pages if needed and fills them with tile information.
        /// </summary>
        /// <param name="tiles"></param>
        public void UpdatePages(List<TileData> tiles)
        {
            int neededPages = Mathf.CeilToInt(tiles.Count / (float)ObjectPage.MaxObjectsCount);
            int oldPageCount = m_ObjectPages.Count;
            if (m_ObjectPages.Count < neededPages)
            {
                for (int i = m_ObjectPages.Count; i < neededPages; i++)
                {
                    var page = Instantiate(ObjectPagePrefab, PageParent);
                    m_ObjectPages.Add(page);
                }
                updatePageText();              
                // show the last page if they are updated
                if (neededPages > 1)
                {
                    for (int i = 0; i < m_ObjectPages.Count - 1; i++)
                    {
                        m_ObjectPages[i].gameObject.SetActive(false);
                    }
                    m_ObjectPages[m_ObjectPages.Count - 1].gameObject.SetActive(true);
                }
            }
            // we start updating the information at the old last page
            fillPagesInformation(tiles, oldPageCount - 1);
        }

        /// <summary>
        /// Shows the next page in the page list.
        /// </summary>
        public void ShowNextPage()
        {
            // no pages or only one page => cannot show next page
            if (m_ObjectPages.Count < 2)
                return;

            ShowPage(CurrentPage + 1);
        }

        /// <summary>
        /// Shows the previous page in the page list.
        /// </summary>
        public void ShowPreviousPage()
        {
            // no pages or only one page => cannot show next page
            if (m_ObjectPages.Count < 2)
                return;

            ShowPage(CurrentPage - 1);
        }

        /// <summary>
        /// Shows the page given by the index.
        /// </summary>
        /// <param name="pageIndex"></param>
        public void ShowPage(int pageIndex)
        {
            int wrappedPageIndex = MathUtility.WrapArrayIndex(pageIndex, m_ObjectPages.Count);
            if (pageIndex == CurrentPage || wrappedPageIndex == CurrentPage)
                return;

            m_ObjectPages[CurrentPage].gameObject.SetActive(false);
            CurrentPage = wrappedPageIndex;
            m_ObjectPages[CurrentPage].gameObject.SetActive(true);
            updatePageText();
        }

        /// <summary>
        /// Hides this view.
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Shows this view.
        /// </summary>
        /// <param name="pageIndex"></param>
        public void Show(int pageIndex)
        {
            gameObject.SetActive(true);
            ShowPage(pageIndex);
        }

        public void destroyPages()
        {
            for (int i = 0; i < m_ObjectPages.Count; i++)
            {
                Destroy(m_ObjectPages[i].gameObject);
            }
        }

        #endregion // Public methods

        #region Private internal methods

        /// <summary>
        /// Fills the pages with tile textures.
        /// </summary>
        /// <param name="tiles"></param>
        private void fillPagesInformation(List<TileData> tiles, int startPage = 0)
        {
            // we go through each object holder of each page
            for (int i = startPage; i < m_ObjectPages.Count; i++)
            {
                for (int j = 0; j < m_ObjectPages[i].Objects.Count; j++)
                {
                    int tileIndex = i * ObjectPage.MaxObjectsCount + j;
                    bool activeState = false;
                    // as long as we did not load all tiles we load up the texture
                    if (tileIndex < tiles.Count)
                    {
                        m_ObjectPages[i].Objects[j].name = tiles[tileIndex].Name;
                        m_ObjectPages[i].Objects[j].ObjectImage.texture = GlobalSettings.Instance.TextureLibrary.Textures[tiles[tileIndex].TextureIndex];
                        m_ObjectPages[i].Objects[j].ObjectImage.GetComponent<Button>().onClick.AddListener(() => TileMenuManager.Instance.ShowDetailView(tileIndex));
                        activeState = true;
                    }
                    m_ObjectPages[i].Objects[j].gameObject.SetActive(activeState);
                }
            }
        }

        /// <summary>
        /// Updates the page count text to the current page variable.
        /// </summary>
        private void updatePageText()
        {
            PageText.text = string.Format("{0} / {1}", CurrentPage+1, m_ObjectPages.Count);
        }

        #endregion // Private internal methods
    }
}


