using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Music.Models;

[Table("tracks")]
public partial class Track
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [StringLength(255)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [Column("length")]
    public int Length { get; set; }

    [Column("unit_price")]
    public double UnitPrice { get; set; }

    [Column("album_id")]
    public int? AlbumId { get; set; }

    [Column("artist_id")]
    [StringLength(450)]
    public string ArtistId { get; set; } = null!;

    [Column("plays")]
    public int Plays { get; set; }

    [Column("approved")]
    public bool Approved { get; set; }
    [Column("track_url")]
    public string TrackUrl { get; set; }

    [Column("genre")]
    [StringLength(50)]
    [Unicode(false)]
    public string? Genre { get; set; }

    [ForeignKey("AlbumId")]
    [InverseProperty("Tracks")]
    public virtual Album? Album { get; set; }

    [ForeignKey("ArtistId")]
    [InverseProperty("Tracks")]
    public virtual User Artist { get; set; } = null!;

    [InverseProperty("Track")]
    public virtual ICollection<Detection> Detections { get; set; } = new List<Detection>();
}
