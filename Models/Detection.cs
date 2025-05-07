using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Music.Models;

[Table("detections")]
public partial class Detection
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("track_id")]
    public int TrackId { get; set; }

    [Column("date_detected", TypeName = "datetime")]
    public DateTime DateDetected { get; set; }

    [Column("artist_name")]
    [StringLength(950)]
    public string ArtistName { get; set; } = null!;

    [Column("song_name")]
    [StringLength(950)]
    public string SongName { get; set; } = null!;

    [Column("confidence")]
    public double Confidence { get; set; }

    [ForeignKey("TrackId")]
    [InverseProperty("Detections")]
    public virtual Track Track { get; set; } = null!;
}
