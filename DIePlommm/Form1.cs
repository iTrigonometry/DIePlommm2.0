﻿using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;



namespace DIePlommm
{
    public partial class Form1 : Form
    {
        /**
         * Переменные
         */
        TrafficIntensityEstimate tie; //отвечает за информацию которая будет появляться на экране
        Timer timer; //таймер который отвечает за обновление информации
        bool networkConnection; //позволит определять есть ли соеденение                


        /**
         * Отсюдава програма начинаеся
         */
        public static void Main()
        { 
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Form f = new Form1();
            Application.Run(f);
        }


       
        /**
         * Конструктор
         */
        public Form1()
        {
            //---------------------------------AutoGenerated---------------------------------------------
            InitializeComponent();

            //-------------------------------Экземпляр нашего класса-------------------------------------
            tie = new TrafficIntensityEstimate();

            //-----------------------------Выводим интерфейсы в ListBox----------------------------------
            listBox1.DataSource = tie.getNetworkInterfaces();

            //-------------------------------------------------------------------------------------------
            listBox1.SelectedIndexChanged += changeNetworkInterface;
            networkConnection = isNetworkConnection();
            turnOnZoom();

            //-------------------------------Определяем работу таймера-----------------------------------
            timer = new Timer();
            timer.Tick += outputData;
            changeInterval();
            timer.Start();
        }


        /**
         * Метод для постоянного обновления значения интенсивности трафика
         * Метод для Timer.Tick
         * Рисуется график и выводится текущее значение интенсивности трафика
         */
        private void outputData(object sender, EventArgs e)
        {
            if (networkConnection)
            {
                //------------------------ИНИЦИАЛИЩАЦИЯ ПЕРЕМЕННЫХ--------------------
                double bytes = tie.getInputTraffic();
                double kBit = Math.Round((bytes / 125), 3);
                double mBit = Math.Round((bytes / 125000), 3);


                //---------------------ВЫВОД ИНФОРМАЦИИ-------------------------------
                string outputTextBoxMbits = mBit.ToString() + "Mbit/sec";
                string outputTextBoxKbits = kBit.ToString() + "Kbit/sec";


                if (mBit <= 0.5)                {
                    textBox1.Text = outputTextBoxKbits;
                }
                else{
                    textBox1.Text = outputTextBoxMbits;
                }
                chartMbits.Series[0].Points.AddXY(DateTime.Now.ToShortTimeString(), mBit);
            }
            else
            {
                textBox1.Text = "NotConnection";
                networkConnection = isNetworkConnection();
            }
        }



        /**
         * Реагирует на изменения выбраного сетевого интерфейса в listBox1
         */
        private void changeNetworkInterface(object sender, EventArgs e)
        {
            tie.setNetworkInterface(listBox1.SelectedItem as String);
            chartMbits.Series[0].Points.Clear();

            networkConnection = isNetworkConnection();
        }



        /**
         * Метод проверяет есть ли подключение по данному сетевому интерфейсу.
         * Если нагрузка нулевая, то подключение отсутсвует
         */
       private bool isNetworkConnection()
        {
           if (tie.isNetworkConnection())
            {
                label2.ForeColor = Color.Green;
                label2.Text = "Connection";
                return true;

            }
            label2.ForeColor = Color.Red;
            label2.Text = "Not Connection";
            return false;
        }


        /**
         * Метод реагирует на нажатие кнопки buttonResetZoom
         * Убирает последний зум по X и Y
         */
        private void ButtonResetZoom_Click(object sender, EventArgs e)
        {
            chartMbits.ChartAreas[0].AxisY.ScaleView.ZoomReset();
            chartMbits.ChartAreas[0].AxisX.ScaleView.ZoomReset();
        }


        /**
         * Метод реагирует на нажатие клавиши buttonClearGraph
         * Очищает график
         */
        private void ButtonClearGraph_Click(object sender, EventArgs e)
        {
            chartMbits.Series[0].Points.Clear();
        }


        /**
         * Метод реагирует на изменение значения в numericUpDown1
         */
        private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            changeInterval();
        }


        /**
         * Метод для изменения интервала таймера в зависимости от значения в numericUpDown1
         * Так же выводит в labelIntervalError текст о том большой ли интервал был выбран. 
         * В моем понимании интервал больше 5 может стать большой проблемой в качестве полученных данных.
         * Неизвестно что произошло за эти 4 секунды. 3 это край край
         */
        private void changeInterval()
        {
            timer.Interval = (int)numericUpDown1.Value * 1000;


            if ((int)numericUpDown1.Value >= 4)
            {
                labelIntervalError.ForeColor = Color.Red;
                labelIntervalError.Text = "Данные могут быть некоректны";
            }
            else
            {
                labelIntervalError.ForeColor = Color.Green;
                labelIntervalError.Text = "Интервал в пределах разумного";
            }
        }

        /**
         * Метод который включает возможность масштабирования графика
         */
        private void turnOnZoom()
        {
            chartMbits.ChartAreas[0].CursorX.IsUserEnabled = true;
            chartMbits.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chartMbits.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            chartMbits.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;

            chartMbits.ChartAreas[0].CursorY.IsUserEnabled = true;
            chartMbits.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
            chartMbits.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
            chartMbits.ChartAreas[0].AxisY.ScrollBar.IsPositionedInside = true;
        }
    }
}
