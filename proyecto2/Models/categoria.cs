namespace proyecto2.Models
{
    public class Categoria
    {
        public string Nombre { get; }
        public ArbolLibros Libros { get; }

        public Categoria? Padre { get; internal set; }
        public Categoria? PrimerHijo { get; internal set; }
        public Categoria? SiguienteHermano { get; internal set; }

        public Categoria(string nombre)
        {
            Nombre = nombre;
            Libros = new ArbolLibros();
            Padre = null;
            PrimerHijo = null;
            SiguienteHermano = null;
        }
    }
}
