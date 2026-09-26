namespace proyecto2.Models
{
    public class NodoListaLibro
    {
        public Libro Libro { get; }
        public NodoListaLibro? Siguiente { get; internal set; }

        public NodoListaLibro(Libro libro)
        {
            Libro = libro;
            Siguiente = null;
        }
    }

    public class ListaLibros
    {
        public NodoListaLibro? Primero { get; private set; }
        private NodoListaLibro? ultimo;

        public ListaLibros()
        {
            Primero = null;
            ultimo = null;
        }

        public void Agregar(Libro libro)
        {
            NodoListaLibro nuevo = new NodoListaLibro(libro);

            if (Primero == null)
            {
                Primero = nuevo;
            }
            else
            {
                ultimo!.Siguiente = nuevo;
            }

            ultimo = nuevo;
        }
    }
}
