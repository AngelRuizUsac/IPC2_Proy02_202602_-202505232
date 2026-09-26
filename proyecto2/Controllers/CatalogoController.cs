using System.IO;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using proyecto2.Models;
using proyecto2.Services;

namespace proyecto2.Controllers
{
    public class CatalogoController : Controller
    {
        private readonly EstadoCatalogo estado;
        private readonly CargadorXml cargador;
        private readonly GeneradorReportes reportes;
        private readonly RenderizadorGraphviz graphviz;
        private readonly DatosAyuda ayuda;

        public CatalogoController(EstadoCatalogo estado, CargadorXml cargador, GeneradorReportes reportes, RenderizadorGraphviz graphviz, DatosAyuda ayuda)
        {
            this.estado = estado;
            this.cargador = cargador;
            this.reportes = reportes;
            this.graphviz = graphviz;
            this.ayuda = ayuda;
        }

        [HttpGet]
        public IActionResult Index()
        {
            lock (estado.Sincronizacion)
            {
                ViewData["Categorias"] = estado.Catalogo.Categorias.Listar();
                return View("Index", estado.Catalogo.Libros.ListarPorISBN());
            }
        }

        [HttpGet]
        public IActionResult Carga()
        {
            return View("Carga");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CargarXml(IFormFile? archivo)
        {
            if (archivo == null || archivo.Length == 0)
            {
                ViewData["Alerta"] = "Selecciona un archivo XML con contenido.";
                return Carga();
            }
            string contenido;

            using (StreamReader lector = new StreamReader(archivo.OpenReadStream()))
            {
                contenido = lector.ReadToEnd();
            }

            lock (estado.Sincronizacion)
            {
                int duplicados;
                try
                {
                    duplicados = cargador.Cargar(contenido, estado.Catalogo);
                }
                catch (XmlException)
                {
                    ViewData["Alerta"] = "El archivo no es un XML válido. Revisa su estructura.";
                    return Carga();
                }
                if (duplicados > 0)
                {
                    ViewData["Alerta"] = "Carga completada: se impidió registrar " + duplicados + " registro(s) duplicado(s) o inválido(s). Revisa nombres, ISBN y categorías. Los registros válidos se incorporaron y los existentes se conservaron.";
                    return Index();
                }
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AgregarCategoria(string nombre, string? nombrePadre = null)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                ViewData["Alerta"] = "No se registró la categoría: escribe un nombre.";
                return Categorias();
            }

            lock (estado.Sincronizacion)
            {
                if (nombrePadre != null && estado.Catalogo.Categorias.Buscar(nombrePadre) == null)
                {
                    ViewData["Alerta"] = "La categoría padre ya no existe. Selecciona otra.";
                    return Categorias();
                }
                if (estado.Catalogo.AgregarCategoria(nombre, nombrePadre) == null)
                {
                    ViewData["Alerta"] = "No se registró la categoría: ya existe una categoría con el nombre «" + nombre + "». Usa un nombre diferente.";
                    return Categorias();
                }
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RegistrarLibro(long? isbn, string titulo, string autor, string nombreCategoria)
        {
            if (!ModelState.IsValid || isbn == null || string.IsNullOrWhiteSpace(titulo) || string.IsNullOrWhiteSpace(autor) || string.IsNullOrWhiteSpace(nombreCategoria))
            {
                ViewData["Alerta"] = "Completa los campos y escribe un ISBN entero válido.";
                return Index();
            }
            lock (estado.Sincronizacion)
            {
                if (estado.Catalogo.Categorias.Buscar(nombreCategoria) == null)
                {
                    ViewData["Alerta"] = "La categoría ya no existe. Selecciona otra.";
                    return Index();
                }
                if (estado.Catalogo.RegistrarLibro(isbn.Value, titulo, autor, nombreCategoria) == null)
                {
                    ViewData["Alerta"] = "No se registró el libro: el ISBN " + isbn + " ya existe. Usa un ISBN diferente.";
                    return Index();
                }
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarLibro(long? isbn)
        {
            lock (estado.Sincronizacion)
            {
                if (!ModelState.IsValid || isbn == null || !estado.Catalogo.EliminarLibro(isbn.Value))
                {
                    ViewData["Alerta"] = "No se eliminó el libro: el ISBN es inválido o el libro ya no existe.";
                    return Index();
                }
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult BuscarLibro(long? isbn)
        {
            lock (estado.Sincronizacion)
            {
                return View("Libro", ModelState.IsValid && isbn != null ? estado.Catalogo.BuscarLibro(isbn.Value) : null);
            }
        }

        [HttpGet]
        public IActionResult Menor()
        {
            lock (estado.Sincronizacion)
            {
                return View("Libro", estado.Catalogo.ObtenerLibroMenor());
            }
        }

        [HttpGet]
        public IActionResult Mayor()
        {
            lock (estado.Sincronizacion)
            {
                return View("Libro", estado.Catalogo.ObtenerLibroMayor());
            }
        }

        [HttpGet]
        public IActionResult LibrosCategoria(string nombreCategoria)
        {
            lock (estado.Sincronizacion)
            {
                ArbolLibros? arbol = estado.Catalogo.ObtenerLibrosCategoria(nombreCategoria);
                if (arbol == null)
                {
                    ViewData["Alerta"] = "La categoría no existe.";
                    return Categorias();
                }
                ListaLibros libros = arbol.ListarPorISBN();
                return View("LibrosCategoria", new LibrosDeCategoria(nombreCategoria, libros));
            }
        }

        [HttpGet]
        public IActionResult ReporteCategorias(string? nombreCategoria = null)
        {
            string dot;

            lock (estado.Sincronizacion)
            {
                if (nombreCategoria == null)
                {
                    dot = reportes.GenerarCategorias(estado.Catalogo.Categorias);
                }
                else
                {
                    Categoria? categoria = estado.Catalogo.Categorias.Buscar(nombreCategoria);
                    if (categoria == null)
                    {
                        ViewData["Alerta"] = "La categoría no existe.";
                        return Categorias();
                    }
                    dot = reportes.GenerarSubcategorias(categoria);
                }
            }

            return Content(graphviz.RenderizarSvg(dot), "image/svg+xml; charset=utf-8");
        }

        [HttpGet]
        public IActionResult ReporteLibros(string? nombreCategoria = null)
        {
            string dot;

            lock (estado.Sincronizacion)
            {
                ArbolLibros? libros = nombreCategoria == null ? estado.Catalogo.Libros : estado.Catalogo.ObtenerLibrosCategoria(nombreCategoria);
                if (libros == null)
                {
                    ViewData["Alerta"] = "La categoría no existe.";
                    return Categorias();
                }
                dot = reportes.GenerarLibros(libros);
            }

            return Content(graphviz.RenderizarSvg(dot), "image/svg+xml; charset=utf-8");
        }

        [HttpGet]
        public IActionResult Ayuda()
        {
            return View("Ayuda", ayuda);
        }

        [HttpGet]
        public IActionResult Categorias()
        {
            lock (estado.Sincronizacion)
            {
                return View("Categorias", estado.Catalogo.Categorias.Listar());
            }
        }

        [HttpGet]
        public IActionResult BuscarTitulo(string titulo)
        {
            lock (estado.Sincronizacion)
            {
                ViewData["TituloBuscado"] = titulo;
                return View("Titulos", estado.Catalogo.BuscarTitulo(titulo));
            }
        }

        [HttpGet]
        public IActionResult Documentacion()
        {
            string ruta = Path.Combine(System.AppContext.BaseDirectory, "documentacion", "Ensayo.pdf");
            return PhysicalFile(ruta, "application/pdf");
        }
    }
}
