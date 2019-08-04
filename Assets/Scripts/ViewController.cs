using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ViewController : MonoBehaviour
{
    [SerializeField]
    private GameObject _movieInfoElementPrefab;

    [SerializeField]
    private Transform _contentPaneTransform;

    private RectTransform _contentPaneRectTransform;
    private RectTransform _movieInfoElementRectTransform;


    public static ViewController Instance { get; private set; }

    private List<GameObject> _miElementList = new List<GameObject>();

    private void Awake()
    {
        if (Instance != this && Instance != null)
        {
            Destroy(gameObject);
        }

        Instance = this;

        _contentPaneRectTransform = _contentPaneTransform.GetComponent<RectTransform>();
        _movieInfoElementRectTransform = _movieInfoElementPrefab.GetComponent<RectTransform>();
    }

    public void UpdateUI(List<MovieInfo> movieInfo)
    {
        
        if (movieInfo == null || movieInfo.Count == 0)
        {
            Debug.LogError("movieInfo null");
            return;
        }

        if (_contentPaneTransform == null)
        {
            Debug.LogError("Set content pane transform reference");
            return;
        }

        for (int i = _miElementList.Count - 1; i >=0 ; i--)
        {
            Destroy(_miElementList[i]);
        }

        _miElementList.Clear();

        for (int i = 0; i < movieInfo.Count; i++)
        {
            for (int j = 0; j < movieInfo[i].Search.Count; j++)
            {
                _miElementList.Add(Instantiate(_movieInfoElementPrefab, _contentPaneTransform));

                MovieInfoElement miElement = _miElementList[_miElementList.Count - 1].GetComponent<MovieInfoElement>();

                if (miElement != null)
                {
                    miElement.TextField.text = movieInfo[i].Search[j].Title;
                    GetRemotePoster(miElement.RawImage, movieInfo[i].Search[j].Poster);
                }
                else
                {
                    Debug.LogError("The prefab is missing the movieinfoelmeent component");
                }
            }

        }

        _contentPaneRectTransform.sizeDelta = new Vector2(_contentPaneRectTransform.sizeDelta.x, _miElementList.Count * _movieInfoElementRectTransform.sizeDelta.y);
    }

    private async void GetRemotePoster(RawImage rawImage, string posterURL)
    {
        Texture2D texture = await DoAsyncPull(posterURL);
        rawImage.texture = texture;
    }

    private static async Task<Texture2D> DoAsyncPull(string posterURL)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(posterURL))
        {
            var asyncOp = www.SendWebRequest();

            while (!asyncOp.isDone)
            {
                await Task.Delay(1000 / 30);
            }

            if (www.isHttpError || www.isNetworkError)
            {
                Debug.Log(www.error);
                return null;
            }
            else
            {
                return ((DownloadHandlerTexture)www.downloadHandler).texture;
            }
        }
    }
}
