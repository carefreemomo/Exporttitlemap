using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ExportTitleMap
{
    public class MapPoint
    {
        private double x = 0;
        private double y = 0;

        public MapPoint()
        {

        }

        public MapPoint(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public double X
        {
            get => x;
            set
            {
                x = value;
            }
        }

        public double Y
        {
            get => y;
            set
            {
                y = value;
            }
        }

        public MapPoint Multiply(double factor)
        {
            return new MapPoint(this.x * factor, this.y * factor);
        }

        public MapPoint Divide(double factor)
        {
            return new MapPoint(this.x / factor, this.y / factor);
        }

        public void Round()
        {
            this.x = Math.Round(this.x);
            this.y = Math.Round(this.y);
        }

        public void Floor()
        {
            this.x = Math.Floor(this.x);
            this.y = Math.Floor(this.y);
        }
        public void Ceil()
        {
            this.x = Math.Ceiling(this.x);
            this.y = Math.Ceiling(this.y);
        }
    }
}