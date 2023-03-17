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

            var tarjetas = await ObtenerTarjetasDeCredito(25000);
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            //await Esperar(); //Si no usamos await no va a esperar que termine la tarea
            //var nombre = txtInput.Text;
            try
            {
                await ProcesarTarjetas(tarjetas);
                //var saludo = await ObtenerSaludo(nombre);
                //MessageBox.Show(saludo);
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show(ex.Message);
            }

            MessageBox.Show($"Operacion finalizada en {stopwatch.ElapsedMilliseconds / 1000.0} segundos.");

            loadingGif.Visible = false;
        }

        private async Task ProcesarTarjetas(List<string> tarjetas)
        {
            var tareas = new List<Task>();

            //Creando nuestra propia tarea
            await Task.Run(() =>
            {
                foreach (var tarjeta in tarjetas)
                {
                    var json = JsonConvert.SerializeObject(tarjeta);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var respuestaTask = _httpClient.PostAsync($"{_apiURL}api/tarjeta", content);
                    tareas.Add(respuestaTask);
                }
            });

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
    }
}
