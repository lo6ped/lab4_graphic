
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Lab4_Maksimenko
{
    public class Lab4Maksimenko : Form
    {
        // Температуры за 31 день марта 
        private readonly int[] temperatures =
        {
            -8, -1, 1, 5, -2, -6, -10, 2, 9, 0,
            18, 18, 10, 12, 4, -8, 3, 3, 19, 10,
            -2, -6, 0, 4, 5, 4, 12, 16, 8, 5, 7
        };

        private Panel graphPanel;

        public Lab4Maksimenko()
        {
            Text = "Лабораторная работа №4 — Вариант 10";
            ClientSize = new Size(1200, 800);

            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = true;

            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;

            graphPanel = new Panel();
            graphPanel.Dock = DockStyle.Fill;
            graphPanel.BackColor = Color.White;
            graphPanel.Paint += GraphPanel_Paint;

            Controls.Add(graphPanel);
        }

        private void GraphPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint =
                System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            int width = graphPanel.ClientSize.Width;
            int height = graphPanel.ClientSize.Height;

            float left = 75;
            float right = width - 35;
            float top = 65;
            float bottom = height - 75;

            float graphWidth = right - left;
            float graphHeight = bottom - top;

            float minTemp = -12;
            float maxTemp = 22;

            float GetY(float temp)
            {
                return bottom -
                    (temp - minTemp) /
                    (maxTemp - minTemp) * graphHeight;
            }

            float GetX(int day)
            {
                return left + (day - 1) *
                    graphWidth / (temperatures.Length - 1);
            }


            using (Font titleFont = new Font(
                "Arial", 18, FontStyle.Bold))
            using (Brush titleBrush = new SolidBrush(Color.Black))
            {
                g.DrawString(
                    "График температур за март — вариант 10",
                    titleFont,
                    titleBrush,
                    260,
                    15);
            }


            using (Pen gridPen = new Pen(Color.LightGray, 1))
            using (Font axisFont = new Font("Arial", 9))
            using (Brush axisBrush = new SolidBrush(Color.Black))
            {
                for (int temp = -10; temp <= 20; temp += 5)
                {
                    float y = GetY(temp);

                    g.DrawLine(gridPen, left, y, right, y);

                    g.DrawString(
                        temp.ToString(),
                        axisFont,
                        axisBrush,
                        30,
                        y - 8);
                }

                // Подписи дней
                for (int day = 1; day <= 31; day++)
                {
                    float x = GetX(day);

                    if (day % 2 == 1 || day == 31)
                    {
                        g.DrawString(
                            day.ToString(),
                            axisFont,
                            axisBrush,
                            x - 8,
                            bottom + 10);
                    }
                }
            }

            // ======================================
            // ОСИ
            // ======================================

            using (Pen axisPen = new Pen(Color.Black, 1.5f))
            {
                // Вертикальная ось
                g.DrawLine(axisPen, left, top, left, bottom);

                // Горизонтальная ось температуры 0
                g.DrawLine(
                    axisPen,
                    left,
                    GetY(0),
                    right,
                    GetY(0));

                // Нижняя ось дней
                g.DrawLine(axisPen, left, bottom, right, bottom);
            }

            // ======================================
            // ПОДПИСИ ОСЕЙ
            // ======================================

            using (Font axisTitleFont = new Font(
                "Arial", 10, FontStyle.Bold))
            using (Brush axisTitleBrush = new SolidBrush(Color.Black))
            {
                g.DrawString(
                    "Температура, °C",
                    axisTitleFont,
                    axisTitleBrush,
                    5,
                    35);

                g.DrawString(
                    "День марта",
                    axisTitleFont,
                    axisTitleBrush,
                    right - 80,
                    bottom + 35);
            }

            // ======================================
            // РИСОВАНИЕ СЕГМЕНТОВ ГРАФИКА
            // ======================================

            for (int i = 0; i < temperatures.Length - 1; i++)
            {
                float x1 = GetX(i + 1);
                float y1 = GetY(temperatures[i]);

                float x2 = GetX(i + 2);
                float y2 = GetY(temperatures[i + 1]);

                DrawTemperatureSegment(
                    g,
                    x1, y1,
                    x2, y2,
                    temperatures[i],
                    temperatures[i + 1]);
            }

            // ======================================
            // ТОЧКИ ТЕМПЕРАТУР
            // ======================================

            using (Brush pointBrush = new SolidBrush(Color.Black))
            using (Font valueFont = new Font("Arial", 8))
            {
                for (int i = 0; i < temperatures.Length; i++)
                {
                    float x = GetX(i + 1);
                    float y = GetY(temperatures[i]);

                    g.FillEllipse(pointBrush, x - 3, y - 3, 6, 6);

                    // Подпись значения около точки
                    string value = temperatures[i].ToString();

                    g.DrawString(
                        value,
                        valueFont,
                        pointBrush,
                        x - 8,
                        y - 17);
                }
            }

            // Рамка графика
            using (Pen borderPen = new Pen(Color.Gray, 1))
            {
                g.DrawRectangle(
                    borderPen,
                    left,
                    top,
                    graphWidth,
                    graphHeight);
            }
        }

        // Рисование отдельного температурного отрезка
        private void DrawTemperatureSegment(
            Graphics g,
            float x1, float y1,
            float x2, float y2,
            float temp1, float temp2)
        {
            // Пороговые температуры, в которых меняется стиль
            float[] thresholds = { -5, 0, 15 };

            // Собираем точки разделения отрезка
            float[] cuts = new float[5];
            int count = 0;

            cuts[count++] = 0;

            for (int i = 0; i < thresholds.Length; i++)
            {
                float threshold = thresholds[i];

                if ((temp1 < threshold && temp2 > threshold) ||
                    (temp1 > threshold && temp2 < threshold))
                {
                    float t = (threshold - temp1) /
                        (temp2 - temp1);

                    cuts[count++] = t;
                }
            }

            cuts[count++] = 1;

            // Сортируем точки разделения
            Array.Sort(cuts, 0, count);

            // Каждый участок рисуется собственным массивом из 2 точек
            for (int i = 0; i < count - 1; i++)
            {
                float tStart = cuts[i];
                float tEnd = cuts[i + 1];

                float startX = x1 + (x2 - x1) * tStart;
                float startY = y1 + (y2 - y1) * tStart;

                float endX = x1 + (x2 - x1) * tEnd;
                float endY = y1 + (y2 - y1) * tEnd;

                // Температура в середине участка
                float middleT = (tStart + tEnd) / 2;
                float middleTemp =
                    temp1 + (temp2 - temp1) * middleT;

                PointF[] segment =
                {
                    new PointF(startX, startY),
                    new PointF(endX, endY)
                };

                using (Pen pen = CreateTemperaturePen(middleTemp))
                {
                    g.DrawLines(pen, segment);
                }
            }
        }

        // Выбор пера в зависимости от температуры
        private Pen CreateTemperaturePen(float temperature)
        {
            // Выше 15 градусов — широкая сплошная линия
            if (temperature > 15)
            {
                return new Pen(Color.DarkRed, 4);
            }

            // Ниже -5 градусов — широкая пунктирная линия
            if (temperature < -5)
            {
                Pen pen = new Pen(Color.DarkBlue, 4);
                pen.DashStyle = DashStyle.Dash;
                return pen;
            }

            // Ниже 0 — обычная сплошная линия
            if (temperature < 0)
            {
                return new Pen(Color.Black, 2);
            }

            // Выше 0 до 15 — комбинированная линия
            Pen combinedPen = new Pen(Color.DarkGreen, 4);

            combinedPen.CompoundArray = new float[]
            {
                0.0f, 0.15f,
                0.35f, 0.65f,
                0.85f, 1.0f
            };

            return combinedPen;
        }
    }
}
