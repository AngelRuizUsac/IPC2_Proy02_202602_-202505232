using Microsoft.AspNetCore.Mvc;
using proyecto2.Models;
using System.Diagnostics;

namespace proyecto2.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Catalogo");
        }

        public IActionResult Privacy()
        {
            return RedirectToAction("Index", "Catalogo");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            string identificador = HttpContext.TraceIdentifier;
            Activity? actividad = Activity.Current;
            if (actividad != null && actividad.Id != null)
            {
                identificador = actividad.Id;
            }
            ErrorViewModel error = new ErrorViewModel();
            error.RequestId = identificador;
            return View(error);
        }
    }
}
