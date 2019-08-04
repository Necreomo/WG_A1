using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class QueryController : MonoBehaviour
{

    private const string QUERY = "http://www.omdbapi.com/?apikey={0}&s={1}&page={2}";
    private const string APIKEY = "6367da97";

    public static QueryController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);

        }

        Instance = this;
    }

    public void HandleQuery(Text searchText)
    {
        int pageNumber = 1;

        bool failCallback = false;

        List<MovieInfo> movieInfoList = new List<MovieInfo>();

        while (!failCallback)
        {

            // Debug.Log(string.Format(QUERY, APIKEY, searchText.text, pageNumber.ToString()));

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format(QUERY, APIKEY, searchText.text, pageNumber.ToString()));
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            StreamReader streamReader = new StreamReader(response.GetResponseStream());

            string jsonResult = streamReader.ReadToEnd();

            Debug.Log(jsonResult);

            MovieInfo movieInfo = JsonUtility.FromJson<MovieInfo>(jsonResult);

            if (movieInfo.Response)
            {
                movieInfoList.Add(movieInfo);

            }
            else
            {
                failCallback = true;

            }
            pageNumber++;
        }

        ViewController.Instance.UpdateUI(movieInfoList);
    }

}
