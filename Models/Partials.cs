using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Music.Models;

 
public partial class Track
{
    [NotMapped]
    public IFormFile AudioFile { get; set; }
}

