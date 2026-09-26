namespace proyecto2.Models
{
    public class NodoListaCategoria
    {
        public string Nombre { get; }
        public string? Padre { get; }
        public int Nivel { get; }
        public NodoListaCategoria? Siguiente { get; internal set; }

        public NodoListaCategoria(string nombre, string? padre, int nivel)
        {
            Nombre = nombre;
            Padre = padre;
            Nivel = nivel;
        }
    }

    public class ListaCategorias
    {
        public NodoListaCategoria? Primero { get; private set; }
        private NodoListaCategoria? ultimo;

        public void Agregar(string nombre, string? padre, int nivel)
        {
            NodoListaCategoria nuevo = new NodoListaCategoria(nombre, padre, nivel);
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
