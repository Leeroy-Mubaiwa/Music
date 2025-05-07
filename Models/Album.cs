using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Music.Models;

[Table("albums")]
public partial class Album
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("title")]
    [StringLength(255)]
    [Unicode(false)]
    public string Title { get; set; } = null!;

    [Column("artist_id")]
    [StringLength(450)]
    public string ArtistId { get; set; } = null!;

    [Column("release_date", TypeName = "datetime")]
    public DateTime ReleaseDate { get; set; }

    [ForeignKey("ArtistId")]
    [InverseProperty("Albums")]
    public virtual User Artist { get; set; } = null!;

    [InverseProperty("Album")]
    public virtual ICollection<Track> Tracks { get; set; } = new List<Track>();
}
