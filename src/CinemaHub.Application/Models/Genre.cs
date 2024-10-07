using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaHub.Application.Models;

public class Genre
{
    [Key, Column(Order = 0)]
    public Guid MovieId { get; set; }

    [Key, Column(Order = 1)]
    [Required]
    public string Name { get; set; }

    public Movie Movie { get; set; }  // Navigation property to the Movie
}