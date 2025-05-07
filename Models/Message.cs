using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Music.Models;

[Table("messages")]
public partial class Message
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("source_user")]
    [StringLength(450)]
    public string SourceUser { get; set; } = null!;

    [Column("target_user")]
    [StringLength(450)]
    public string TargetUser { get; set; } = null!;

    [Column("date_sent", TypeName = "datetime")]
    public DateTime DateSent { get; set; }

    [Column("message")]
    public string Message1 { get; set; } = null!;

    [ForeignKey("SourceUser")]
    [InverseProperty("MessageSourceUserNavigations")]
    public virtual User SourceUserNavigation { get; set; } = null!;

    [ForeignKey("TargetUser")]
    [InverseProperty("MessageTargetUserNavigations")]
    public virtual User TargetUserNavigation { get; set; } = null!;
}
