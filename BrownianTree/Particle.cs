using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace BrownianTree
{
    class Particle
    {
        public Point position { get; set; }
        public ParticleType pType { get; set; }

        public enum ParticleType
        {
            freeParticle,
            fixedParticle
        };

        public Particle()
        {
        }

        public Particle(int x, int y, ParticleType type)
        {
            Point p = new Point(x, y);
            this.position = p;
            this.pType = type;
        }
    }
}
