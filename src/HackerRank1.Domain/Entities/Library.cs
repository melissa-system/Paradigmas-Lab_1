namespace LibraryService.WebAPI.Data
{
    public class Library
    {
        // Id sigue la convencion de claves de EF Core (propiedad "Id"),
        // por lo que no requiere el atributo [Key] de DataAnnotations.
        // Esto mantiene la entidad de Domain libre de dependencias de EF Core / persistencia.
        public int Id { get; set; }

        public string Name { get; set; }

        public string Location { get; set; }
    }
}
