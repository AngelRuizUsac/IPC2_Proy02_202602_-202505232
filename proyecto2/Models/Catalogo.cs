namespace proyecto2.Models
{
    public class Catalogo
    {
        public ArbolCategorias Categorias { get; }
        public ArbolLibros Libros { get; }
        private readonly ArbolTitulos titulos;

        public Catalogo()
        {
            Categorias = new ArbolCategorias();
            Libros = new ArbolLibros();
            titulos = new ArbolTitulos();
        }

        public Categoria? AgregarCategoria(string nombre, string? nombrePadre = null)
        {
            if (string.IsNullOrWhiteSpace(nombre) || Categorias.Buscar(nombre) != null) return null;
            if (nombrePadre != null && Categorias.Buscar(nombrePadre) == null) return null;
            return Categorias.Agregar(nombre, nombrePadre);
        }

        public Libro? RegistrarLibro(long isbn, string titulo, string autor, string nombreCategoria)
        {
            if (Libros.Buscar(isbn) != null) return null;
            Categoria? categoria = Categorias.Buscar(nombreCategoria);
            if (categoria == null || string.IsNullOrWhiteSpace(titulo) || string.IsNullOrWhiteSpace(autor)) return null;
            Libro libro = new Libro(isbn, titulo, autor, categoria);
            Libros.Insertar(libro);
            categoria.Libros.Insertar(libro);
            titulos.Insertar(libro);
            return libro;
        }

        public Libro? BuscarLibro(long isbn)
        {
            return Libros.Buscar(isbn);
        }

        public bool EliminarLibro(long isbn)
        {
            Libro? libro = Libros.Buscar(isbn);
            if (libro == null) return false;
            titulos.Eliminar(libro);
            libro.Categoria.Libros.Eliminar(isbn);
            Libros.Eliminar(isbn);
            return true;
        }

        public ListaLibros BuscarTitulo(string titulo)
        {
            return titulos.Buscar(titulo);
        }

        public Libro? ObtenerLibroMenor()
        {
            return Libros.ObtenerMenor();
        }

        public Libro? ObtenerLibroMayor()
        {
            return Libros.ObtenerMayor();
        }

        public ArbolLibros? ObtenerLibrosCategoria(string nombreCategoria)
        {
            return Categorias.Buscar(nombreCategoria)?.Libros;
        }
    }
}
