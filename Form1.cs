using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Device.Location;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace WeatherApp
{
    public partial class Form1 : Form
    {
        public double lon;
        public double lat;
        public String city;
        public double unitC;
        public String labelText;
        public bool useF = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            lat = e.Position.Location.Latitude;
            lon = e.Position.Location.Longitude;
            button1.Text = "Текущая локация";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            GeoCoordinateWatcher watcher = new GeoCoordinateWatcher();
            watcher.PositionChanged += watcher_PositionChanged;
            watcher.Start();
            String queue = "https://api.openweathermap.org/data/2.5/weather?lat=" 
                + lat
                + "&lon=" 
                + lon
                + "&APPID=1499f87b39982a746c16f0c3ff09b18b";
            fill(queue);
        }

        public async void fill(String queue)
        {
            WebRequest request = WebRequest.Create(queue);
            request.Method = "POST";
            request.ContentType = "application/x-www-urlencoded";
            try
            {
                WebResponse response = await request.GetResponseAsync();
                string answer = string.Empty;
                using (Stream s = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(s))
                    {
                        answer = await reader.ReadToEndAsync();
                    }
                }
                response.Close();

                OpenWeather.OpenWeather oW = JsonConvert.DeserializeObject<OpenWeather.OpenWeather>(answer);
                city = oW.name;
                label1.Text = this.city;
                setUnitC(oW.main.temp);
                panel1.BackgroundImage = oW.weather[0].Icon;
                label3.Text = oW.weather[0].main;
                label5.Text = oW.weather[0].description;
                if (useF)
                {
                    double unitF = unitC;
                    unitF *= 1.8;
                    unitF += 32;
                    label6.Text = "Средняя температура: " + unitF.ToString("0.##") + "°F";
                }
                else {
                    label6.Text = "Средняя температура: " + unitC.ToString("0.##") + "°С";
                }
                label4.Text = "Влажность: " + oW.main.humidity.ToString() + "%";
                label7.Text = "Давление: " + ((int)oW.main.pressure).ToString() + "мм рт.ст.";
                label8.Text = "Скорость: " + oW.wind.speed.ToString() + "м/с";
                label9.Text = "Направление: " + oW.wind.deg.ToString() + "°";
            } catch (System.Net.WebException e)
            {
                MessageBox.Show(e.Message + "Город не найден");
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            String queue = "https://api.openweathermap.org/data/2.5/weather?lat=" + lat + "&lon=" + lon + "&APPID=1499f87b39982a746c16f0c3ff09b18b";
            fill(queue);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            String newCity;
            newCity = this.city;
            textBox1.Clear();
            String queue = "https://api.openweathermap.org/data/2.5/weather?q=" + newCity + "&APPID=1499f87b39982a746c16f0c3ff09b18b&lang=ru";
            fill(queue);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            city = textBox1.Text;
        }

        public void setUnitC(double c) {
            unitC = c;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            useF = false;
            labelText = "Средняя температура: " + unitC.ToString("0.##") + "°С";
            label6.Text = labelText;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            useF = true;
            double unitF = unitC;
            unitF *= 1.8;
            unitF += 32;
            labelText = "Средняя температура: " + unitF.ToString("0.##") + "°F";
            label6.Text = labelText;
        }
    }
}
