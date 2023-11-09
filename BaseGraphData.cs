using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarLabRight2023
{
    //class created to store the data from the graphs you wish to create 
    public class BaseGraphData
    {
        //create public variables which can be referenced in other classes 
        public int Yaxis { get; set; } = 0;
        public int Xaxis { get; set; } = 0;
        public int[] pointArray { get; set; }
        public Color lineColor { get; set; }
        public int lineSize { get; set; }
        public bool newGraph { get; set; } = true;

        //default constructor
        public BaseGraphData()
        {

        }
        //constructor

        //public function that will set the next graps parameters with the parameters you pass to this function 
        public BaseGraphData(
            int Yaxis,
            int Xaxis,
            Color lineColor,
            int lineSize,
            bool newGraph)
        {

            this.Yaxis = Yaxis;
            this.Xaxis = Xaxis;
            this.lineColor = lineColor;
            pointArray = new int[1000];
            this.lineSize = lineSize;
            this.newGraph = newGraph;

        }
    }
}
