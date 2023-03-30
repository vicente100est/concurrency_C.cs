using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinForms.Models;

namespace WinForms
{
    public partial class Form1 : Form
    {
        private string _apiURL;
        private HttpClient _httpClient;
        public Form1()
        {
            InitializeComponent();
            _apiURL = "https://localhost:44374/";
            _httpClient = new HttpClient();
        }

        private async void Iniciar_Click(object sender, EventArgs e)
        {
            loadingGif.Visible = true;
            pgProcesamiento.Visible = true;

            var reportarProgreso = new Progress<int>(ReportarProgresoTarjetas);

            var tarjetas = await ObtenerTarjetasDeCredito(200);
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            //await Esperar(); //Si no usamos await no va a esperar que termine la tarea
            //var nombre = txtInput.Text;
            try
            {
                await ProcesarTarjetas(tarjetas, reportarProgreso);
                //var saludo = await ObtenerSaludo(nombre);
                //MessageBox.Show(saludo);
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show(ex.Message);
            }

            MessageBox.Show($"Operacion finalizada en {stopwatch.ElapsedMilliseconds / 1000.0} segundos.");

            loadingGif.Visible = false;
            pgProcesamiento.Visible = false;
        }

        private void ReportarProgresoTarjetas(int porcentaje)
        {
            pgProcesamiento.Value = porcentaje;
        }

        private async Task ProcesarTarjetas(List<string> tarjetas, IProgress<int> progress = null)
        {
            var semaforo = new SemaphoreSlim(30);

            var tareas = new List<Task<HttpResponseMessage>>();

            var indice = 0;

            tareas = tarjetas.Select(async tarjeta =>
            {
                var json = JsonConvert.SerializeObject(tarjeta);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                await semaforo.WaitAsync();
                try
                {
                    var tareaInterna =  await _httpClient.PostAsync($"{_apiURL}api/tarjeta", content);

                    //Reportar en un bucle
                    //if (progress != null)
                    //{
                    //    indice++;
                    //    var porcentaje = (double)indice / tarjetas.Count;
                    //    porcentaje = porcentaje * 100;
                    //    var porcentajeInt = (int)Math.Round(porcentaje, 0);
                    //    progress.Report(porcentajeInt);
                    //}

                    return tareaInterna;
                }
                finally
                {
                    semaforo.Release();
                }
            }).ToList();

            var respuestasTareas = Task.WhenAll(tareas);

            //Reportar cada x tiempo
            if (progress != null)
            {
                while (await Task.WhenAny(respuestasTareas, Task.Delay(3000)) != respuestasTareas)
                {
                    var tareasCompletadas = tareas.Where(x => x.IsCompleted).Count();

                    var porcentaje = (double)tareasCompletadas / tarjetas.Count;
                    porcentaje = porcentaje * 100;
                    var porcentajeInt = (int)Math.Round(porcentaje, 0);
                    progress.Report(porcentajeInt);
                }
            }



            var respuestas = await respuestasTareas;

            var tarjetasRechazadas = new List<string>();

            foreach (var respuesta in respuestas)
            {
                var contenido = await respuesta.Content.ReadAsStringAsync();
                var respuestaTarjeta = JsonConvert.DeserializeObject<RespuestaTarjeta>(contenido);

                if (!respuestaTarjeta.Aprobada)
                {
                    tarjetasRechazadas.Add(respuestaTarjeta.Tarjeta);
                }
            }

            foreach(var tarjeta in tarjetasRechazadas)
            {
                Console.WriteLine(tarjeta);
            }

            //Creando nuestra propia tarea

            //var tareas = new List<Task>();
            //await Task.Run(() =>
            //{
            //    foreach (var tarjeta in tarjetas)
            //    {
            //        var json = JsonConvert.SerializeObject(tarjeta);
            //        var content = new StringContent(json, Encoding.UTF8, "application/json");
            //        var respuestaTask = _httpClient.PostAsync($"{_apiURL}api/tarjeta", content);
            //        tareas.Add(respuestaTask);
            //    }
            //});

            await Task.WhenAll(tareas);
        }

        private async Task<List<string>> ObtenerTarjetasDeCredito(int cantidadTarjetas)
        {
            return await Task.Run(() =>
            {
                var tarjetas = new List<string>();

                for (int i = 0; i < cantidadTarjetas; i++)
                {
                    //Genera strings
                    tarjetas.Add(i.ToString().PadLeft(16, '0'));
                }

                return tarjetas;
            });
        }

        private async Task Esperar()
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private async Task<string> ObtenerSaludo(string nombre)
        {
            using (var respuesta = await _httpClient.GetAsync($"{_apiURL}api/saludar/{nombre}"))
            {
                respuesta.EnsureSuccessStatusCode();
                var saludo = await respuesta.Content.ReadAsStringAsync();
                return saludo;
            }
        }

        private void loadingGif_Click(object sender, EventArgs e)
        {

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }
    }
}
