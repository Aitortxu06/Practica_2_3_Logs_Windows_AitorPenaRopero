using System;
using System.Diagnostics;
using System.IO;

namespace ScriptCSVToXML
{
    public class Logger
    {
        private readonly string _source;
        private readonly string _logName;

        // Constructor: Inicializa el EventLog
        public Logger(string source = "ScriptCSVToXML_Aitor", string logName = "Application")
        {
            _source = source;
            _logName = logName;

            try
            {
                if (!EventLog.SourceExists(_source))
                {
                    EventLog.CreateEventSource(_source, _logName);
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[AVISO SISTEMA] No se pudo verificar/crear el origen de eventos (ejecuta como Administrador): {ex.Message}");
                Console.ResetColor();
            }
        }

        // Función principal que pinta en pantalla y genera el Log
        public void EscribirLog(string mensaje, EventLogEntryType tipo, int id)
        {
            // 1. Pintar en pantalla con color según el tipo
            switch (tipo)
            {
                case EventLogEntryType.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case EventLogEntryType.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
            }

            Console.WriteLine($"[LOG ID:{id} - {tipo}] {mensaje}");
            Console.ResetColor();

            // 2. Escribir en el Visor de Eventos de Windows
            try
            {
                EventLog.WriteEntry(_source, mensaje, tipo, id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR VISOR EVENTOS] No se pudo escribir el log en Windows: {ex.Message}");
            }
        }

        // --- FUNCIONES ---

        // 1. LogInicio -> Origen, Destino, Filtro y Tipo de Conversión
        public void LogInicio(string rutaOrigen, string rutaDestino, string filtro, string tipoConversion)
        {
            string mensaje = $"INICIO DE EXPORTACIÓN\n" +
                             $" - Tipo de conversión: {tipoConversion}\n" +
                             $" - Filtro aplicado: {filtro}\n" +
                             $" - Fichero Origen: {Path.GetFullPath(rutaOrigen)}\n" +
                             $" - Fichero Destino: {Path.GetFullPath(rutaDestino)}";

            EscribirLog(mensaje, EventLogEntryType.Information, 100);
        }

        // 2. LogElemento -> Log de lectura/exportación de película
        public void LogElemento(string title, string genre, string premiere, string runtime, string imdbScore, string language)
        {
            string mensaje = $"ELEMENTO PROCESADO -> Title: {title} | Genre: {genre} | Premiere: {premiere} | Runtime: {runtime} | IMDB: {imdbScore} | Language: {language}";

            EscribirLog(mensaje, EventLogEntryType.Information, 200);
        }

        // 3. LogFin -> Log de fin de exportación
        public void LogFin(string rutaDestino, int totalExportadas)
        {
            string mensaje = $"FIN DE EXPORTACIÓN\n" +
                             $" - Total películas exportadas: {totalExportadas}\n" +
                             $" - Nuevo fichero generado en: {Path.GetFullPath(rutaDestino)}";

            EscribirLog(mensaje, EventLogEntryType.Information, 300);
        }

        // 4. LogAviso y LogError
        public void LogAviso(string mensajeAviso)
        {
            EscribirLog($"AVISO: {mensajeAviso}", EventLogEntryType.Warning, 400);
        }

        public void LogError(string mensajeError)
        {
            EscribirLog($"ERROR: {mensajeError}", EventLogEntryType.Error, 500);
        }
    }
}