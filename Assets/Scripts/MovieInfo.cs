
using System.Collections.Generic;


[System.Serializable]
public class MovieDetails
{
    public string Title;
    public string Year;
    public string imdbID;
    public string Type;
    public string Poster;
}

public class MovieInfo {

    public List<MovieDetails> Search;
    public int totalResults;
    public bool Response;

}
