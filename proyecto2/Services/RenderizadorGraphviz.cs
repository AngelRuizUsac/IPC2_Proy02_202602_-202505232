using System.Diagnostics;
using System.IO;
using System.Text;

namespace proyecto2.Services
{
    public class RenderizadorGraphviz
    {
        private readonly string rutaEjecutable;

        public RenderizadorGraphviz(string rutaEjecutable)
        {
            this.rutaEjecutable = rutaEjecutable;
        }

        public string RenderizarSvg(string contenidoDot)
        {
            string archivoTemporal = Path.GetTempFileName();
            File.WriteAllText(archivoTemporal, contenidoDot, new UTF8Encoding(false));
            ProcessStartInfo inicio = new ProcessStartInfo();
            inicio.FileName = rutaEjecutable;
            inicio.Arguments = "-Tsvg \"" + archivoTemporal + "\"";
            inicio.UseShellExecute = false;
            inicio.CreateNoWindow = true;
            inicio.RedirectStandardOutput = true;
            inicio.StandardOutputEncoding = Encoding.UTF8;

            string svg;

            using (Process proceso = Process.Start(inicio)!)
            {
                svg = proceso.StandardOutput.ReadToEnd();
                proceso.WaitForExit();
            }

            File.Delete(archivoTemporal);
            return svg;
        }
    }
}
