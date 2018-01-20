using HoloLensPlanner;
using HoloLensPlanner.GazeResponse;
using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TextureListView : Singleton<TextureListView> {

    [SerializeField]
    private ObjectPage ObjectPagePrefab;

    [SerializeField]
    private Transform PageParent;

    [SerializeField]
    private Text PageText;

    [SerializeField]
    private Button AcceptTextureButton;

    // List of needed pages to show the saved textures
    private List<ObjectPage> m_ObjectPages = new List<ObjectPage>();

    public int CurrentPage { get; private set; }

    private int selectedTextureIndex;

    public void Start()
    {
        AcceptTextureButton.onClick.AddListener(acceptTexture);
    }

    /// <summary>
    /// Creates enough pages to load the given textures and fills up the pages.
    /// </summary>
    /// <param name="tiles"></param>
    public void CreatePages()
    {
        AcceptTextureButton.gameObject.SetActive(true);
        List<Texture2D> textures = new List<Texture2D>();
        textures = GlobalSettings.Instance.TextureLibrary.Textures.OfType<Texture2D>().ToList();
        // round up , 9 elements and max 6 objects per page => 2 pages
        int neededPages = Mathf.CeilToInt(textures.Count / (float)ObjectPage.MaxObjectsCount);
        for (int i = 0; i < neededPages; i++)
        {
            var page = Instantiate(ObjectPagePrefab, PageParent);
            m_ObjectPages.Add(page);
        }
        updatePageText();
        // we need to create at least one page, even when there are no saved tiles, but we cannot fill pages in this case
        if (textures.Count > 0)
            fillPages(textures);

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
    /// Destroy pages of textures and hide acceptButton
    /// </summary>
    public void destroyPages()
    {
        AcceptTextureButton.gameObject.SetActive(false);
        for (int i = 0; i < m_ObjectPages.Count; i++)
        {
            Destroy(m_ObjectPages[i].gameObject);
        }
        m_ObjectPages.Clear();
    }

    /// <summary>
    /// Fills the pages with tile textures.
    /// </summary>
    /// <param name="textures"></param>
    private void fillPages(List<Texture2D> textures)
    {
        int page = 0;
        int texture = 0;
        // we go through each object holder of each page
        for (int i = 0; i < m_ObjectPages.Count; i++)
        {
            for (int j = 0; j < m_ObjectPages[i].Objects.Count; j++)
            {
                int textureIndex = i * ObjectPage.MaxObjectsCount + j;
                // as long as we did not load all tiles we load up the texture
                if (textureIndex < textures.Count)
                {
                    page = i;
                    texture = j;
                    m_ObjectPages[page].Objects[texture].ObjectImage.texture = textures[textureIndex];
                    m_ObjectPages[page].Objects[texture].ObjectImage.GetComponent<Button>().onClick.AddListener(delegate { markTextureAsSelected(textureIndex); });
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
        PageText.text = string.Format("{0} / {1}", CurrentPage + 1, m_ObjectPages.Count);
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

    private void markTextureAsSelected(int textureIndex)
    {
        selectedTextureIndex = textureIndex;
        //Delete outline for every object
        for (int i = 0; i < m_ObjectPages[CurrentPage].Objects.Count; i++)
        {
            m_ObjectPages[CurrentPage].Objects[i].gameObject.GetComponent<Outline>().enabled = false;
        }

        m_ObjectPages[CurrentPage].Objects[textureIndex % ObjectPage.MaxObjectsCount].gameObject.GetComponent<Outline>().enabled = true;
    }

    private void acceptTexture()
    {
        AcceptTextureButton.gameObject.SetActive(false);
        TileMenuManager.Instance.acceptTexture(selectedTextureIndex);
    }
}
