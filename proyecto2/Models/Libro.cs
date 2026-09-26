namespace proyecto2.Models
{
    public class Libro
    {
        public long ISBN { get; }
        public string Titulo { get; }
        public string Autor { get; }
        public Categoria Categoria { get; }

        public Libro(long isbn, string titulo, string autor, Categoria categoria)
        {
            ISBN = isbn;
            Titulo = titulo;
            Autor = autor;
            Categoria = categoria;
        }
    }
}
