namespace proyecto2.Models
{
    public class DatosAyuda
    {
        public string Nombre { get; }
        public string Carne { get; }
        public string Seccion { get; }
        public string EnlaceDocumentacion { get; }

        public DatosAyuda(string nombre, string carne, string seccion, string enlaceDocumentacion)
        {
            Nombre = nombre;
            Carne = carne;
            Seccion = seccion;
            EnlaceDocumentacion = enlaceDocumentacion;
        }
    }
}
