namespace MovieHub.Contracts.Responses
{
    public class GenreDto
    {
        public Guid MovieId { get; set; }
        public int GenreId { get; set; }
    }
    public class GenreResponse
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<GenreDto> Genres { get; set; }
    }
}