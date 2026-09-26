namespace proyecto2.Models
{
    public class LibrosDeCategoria
    {
        public string NombreCategoria { get; }
        public ListaLibros Libros { get; }

        public LibrosDeCategoria(string nombreCategoria, ListaLibros libros)
        {
            NombreCategoria = nombreCategoria;
            Libros = libros;
        }
    }
}
