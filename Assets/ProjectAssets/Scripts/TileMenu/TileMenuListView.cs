using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using UnityEngine.UI;

namespace HoloLensPlanner.TEST
{
    /// <summary>
    /// TileMenuListView is responsible for handling input from the list view buttons and showing the saved tiles in a list display.
    /// </summary>
    public class TileMenuListView : SingleInstance<TileMenuListView>
    {
        [SerializeField]
        private ObjectPage ObjectPagePrefab;

        [SerializeField]
        private Transform PageParent;

        [SerializeField]
        private Text PageText;

        // List of needed pages to show the saved tiles
        private List<ObjectPage> m_ObjectPages = new List<ObjectPage>();

        public int CurrentPage { get; private set; }

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
                fillPages(tiles);

            if (neededPages > 1)
            {
                for (int i = 1; i < m_ObjectPages.Count; i++)
                {
                    m_ObjectPages[i].gameObject.SetActive(false);
                }
            }
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
            int wrappedPageIndex = wrapPageIndex(pageIndex);
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

        /// <summary>
        /// Fills the pages with tile textures.
        /// </summary>
        /// <param name="tiles"></param>
        private void fillPages(List<TileData> tiles)
        {
            // we go through each object holder of each page
            for (int i = 0; i < m_ObjectPages.Count; i++)
            {
                for (int j = 0; j < m_ObjectPages[i].Objects.Count; j++)
                {
                    int tileIndex = i * ObjectPage.MaxObjectsCount + j;
                    // as long as we did not load all tiles we load up the texture
                    if (tileIndex < tiles.Count)
                    {
                        m_ObjectPages[i].Objects[j].name = tiles[tileIndex].Name;
                        m_ObjectPages[i].Objects[j].ObjectImage.texture = GlobalSettings.Instance.TextureLibrary.Textures[tiles[tileIndex].TextureIndex];
                        m_ObjectPages[i].Objects[j].ObjectImage.GetComponent<Button>().onClick.AddListener(() => TileMenuManager.Instance.ShowDetailView(tileIndex));
                    }
                    // otherwise we make the object template not visible
                    else
                    {
                        m_ObjectPages[i].Objects[j].gameObject.SetActive(false);
                    }
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

        /// <summary>
        /// In Unity "%" is the mathematical rem not mod, they behave differentely for different signs of a,b so we need to transform rem to mod.
        /// Source: https://answers.unity.com/questions/358574/modulus-operator-substitute-for-c.html
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private int wrapPageIndex(int index)
        {
            if (m_ObjectPages.Count == 0)
            {
                Debug.Log("First tile. If not check code in TileMenuListView.cs.");
                return (index % 1 + 1) % 1;
            }
            else
            {
                return (index % m_ObjectPages.Count + m_ObjectPages.Count) % m_ObjectPages.Count;
            }
        }
    }
}


