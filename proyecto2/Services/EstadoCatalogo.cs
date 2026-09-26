using proyecto2.Models;

namespace proyecto2.Services
{
    public class EstadoCatalogo
    {
        internal Catalogo Catalogo { get; }
        internal object Sincronizacion { get; }

        public EstadoCatalogo()
        {
            Catalogo = new Catalogo();
            Sincronizacion = new object();
        }
    }
}
