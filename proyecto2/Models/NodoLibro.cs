namespace proyecto2.Models
{
    public class NodoLibro
    {
        public Libro Libro { get; }
        public NodoLibro? Izquierdo { get; internal set; }
        public NodoLibro? Derecho { get; internal set; }
        public int Altura { get; internal set; }

        public NodoLibro(Libro libro)
        {
            Libro = libro;
            Izquierdo = null;
            Derecho = null;
            Altura = 1;
        }

        public bool EsHoja()
        {
            return Izquierdo == null && Derecho == null;
        }
    }
}