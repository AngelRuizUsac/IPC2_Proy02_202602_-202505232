using Microsoft.Extensions.DependencyInjection;
using proyecto2.Models;
using proyecto2.Services;

namespace proyecto2.Configuracion
{
    public static class RegistroCatalogo
    {
        public static void Registrar(IServiceCollection servicios, string rutaGraphviz, string nombre, string seccion, string enlaceDocumentacion)
        {
            servicios.AddSingleton(typeof(EstadoCatalogo));
            servicios.AddSingleton(typeof(CargadorXml));
            servicios.AddSingleton(typeof(GeneradorReportes));
            servicios.AddSingleton(typeof(RenderizadorGraphviz), new RenderizadorGraphviz(rutaGraphviz));
            servicios.AddSingleton(typeof(DatosAyuda), new DatosAyuda(nombre, "202505232", seccion, enlaceDocumentacion));
        }
    }
}
