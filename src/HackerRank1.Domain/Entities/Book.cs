namespace LibraryService.WebAPI.Data
{
    public class Book
    {
        // Id sigue la convencion de claves de EF Core (propiedad "Id"),
        // por lo que no requiere el atributo [Key] de DataAnnotations.
        public int Id { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public int LibraryId { get; set; }
        public virtual Library Library { get; set; }
    }
}
