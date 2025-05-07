public class AudDResponse
{
    public string Status { get; set; }
    public AudDResult Result { get; set; }
}

public class AudDResult
{
    public string Artist { get; set; }
    public string Title { get; set; }
    public string Album { get; set; }
    public string ReleaseDate { get; set; }
    public string Label { get; set; }
    public string SongLink { get; set; }
    public AppleMusicData AppleMusic { get; set; }
    public SpotifyData Spotify { get; set; }
}

public class AppleMusicData
{
    public string Url { get; set; }
    public string Name { get; set; }
    public string ArtistName { get; set; }
    public string AlbumName { get; set; }
}

public class SpotifyData
{
    public string Name { get; set; }
    public string Uri { get; set; }
}
