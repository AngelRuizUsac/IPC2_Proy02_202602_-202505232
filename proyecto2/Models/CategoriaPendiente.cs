namespace proyecto2.Models
{
    internal class CategoriaPendiente
    {
        public string Nombre { get; }
        public string? NombrePadre { get; }
        public CategoriaPendiente? Siguiente { get; set; }

        public CategoriaPendiente(string nombre, string? nombrePadre)
        {
            Nombre = nombre;
            NombrePadre = nombrePadre;
            Siguiente = null;
        }
    }
}
