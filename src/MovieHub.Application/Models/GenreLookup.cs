using System.ComponentModel.DataAnnotations;

namespace MovieHub.Application.Models
{
    public class GenreLookup
    {
        public int Id { get; init; }

        private string _name;

        [Required]
        [StringLength(30)]
        public string Name
        {
            get => _name;
            set => _name = NormalizeName(value);
        }

        private static string NormalizeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null or empty", nameof(name));
            }
            return name.Trim().ToLower();
        }
        public ICollection<Genre> Genres { get; set; } = new List<Genre>();
    }
    
}