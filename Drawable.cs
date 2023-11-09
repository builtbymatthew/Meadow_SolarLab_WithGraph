using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Graphics.Converters;


namespace SolarLabRight2023
{
    public class LineDrawable : BaseGraphData, IDrawable
    {
        
        //necessary values and instantances are created here
        private const int numberOfGraphs = 2;
        private string[] colorName = new string[numberOfGraphs] {"Blue","White" };
        ColorTypeConverter converter = new ColorTypeConverter();
        private int[] lineWidth = new int[numberOfGraphs] { 1, 1 };
        public BaseGraphData[] baseGraphs = new BaseGraphData[numberOfGraphs];

        //default contructor
        public LineDrawable() : base()
        {
            //for the number of graphs I set, create a new graph with the parameters passed to baseGraphs[]
            for (int i = 0; i < numberOfGraphs; i++)
            {
                baseGraphs[i] = new BaseGraphData
                (
                    Yaxis = 0,
                    Xaxis = 0,
                    lineColor: (Color)(converter.ConvertFromInvariantString(colorName[i])),
                    lineSize: lineWidth[i],
                    newGraph: true
                );


            }
        }

        //draw the desired graphs on the canvas 
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            for (int graphIndex = 0; graphIndex < baseGraphs.Length; graphIndex++)
            {
                Rect baseGraphRect = new(dirtyRect.X, dirtyRect.Y, dirtyRect.Width, dirtyRect.Height);
                DrawLineGraph(canvas, baseGraphRect, baseGraphs[graphIndex]);
                DrawBarGraph(canvas, baseGraphRect, baseGraphs[graphIndex], graphIndex);
            }
        }

        //draw the desired bar graph with the passed parameters 
        private void DrawBarGraph(ICanvas canvas, Rect baseGraphRect, BaseGraphData baseGraphData, int graphIndex)
        {
            int barWidth = 10;
            int lineGraphWidth = 600;
            int barGraphLocation = lineGraphWidth + barWidth / 2 + graphIndex * barWidth;
            int graphHeight = 500;
            canvas.StrokeSize = barWidth;
            canvas.DrawLine(barGraphLocation, graphHeight, barGraphLocation, baseGraphData.Yaxis);
        }

        //draw the desired line graph with the passed parameters 
        private void DrawLineGraph(ICanvas canvas, Rect baseGraphRect, BaseGraphData baseGraphData)
        {
            if (baseGraphData.Xaxis < 2)
            {
                baseGraphData.pointArray[baseGraphData.Xaxis] = baseGraphData.Yaxis;
                baseGraphData.Xaxis++;
                return;
            }
            else if (baseGraphData.Xaxis < 1000)
            {
                baseGraphData.pointArray[baseGraphData.Xaxis] = baseGraphData.Yaxis;
                baseGraphData.Xaxis++;
            }
            else
            {

                for (int i = 0; i < 999; i++)
                {
                    baseGraphData.pointArray[i] = baseGraphData.pointArray[i + 1];
                }
                baseGraphData.pointArray[999] = baseGraphData.Yaxis;


            }
            for (int i = 0; i < baseGraphData.Xaxis - 1; i++)
            {
                canvas.StrokeColor = baseGraphData.lineColor;
                canvas.StrokeSize = baseGraphData.lineSize;
                canvas.DrawLine(i, baseGraphData.pointArray[i], i + 1, baseGraphData.pointArray[i + 1]);
            }
        }
    }
}
