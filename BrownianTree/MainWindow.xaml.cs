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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Random rand;
        DispatcherTimer timer;

        Color blue = Color.FromRgb(0, 0, 255);
        Color green = Color.FromRgb(0, 255, 0);
        Color red = Color.FromRgb(255, 0, 0);

        List<Particle> particleList;
        Particle seed;
        int particleGridWidth;//number of particles across the X-axis of the grid
        int i, j, fixedParticleCount;
        Rectangle seedParticle;

        //initial conditions for brownian motion
        double mass; //mass of particle
        double Temp; //Temperature of system in Kelvin
        double relaxTime; //relaxation time for the system
        double prevXVelocity;
        double prevYVelocity;
        double timeStep; //timestep for solution incrementation

        public MainWindow()
        {
            InitializeComponent();
            rand = new Random();

            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = TimeSpan.FromMilliseconds(100);

            tbXseed.Text = string.Format("{0}", rand.Next(371, 379));
            tbYseed.Text = string.Format("{0}", rand.Next(380, 388));
            tbNumParticles.Text = "20";

            //draw the initalisation particle
            Particle p = new Particle(Convert.ToInt32(tbXseed.Text), Convert.ToInt32(tbYseed.Text), 0, 0, Particle.ParticleType.fixedParticle);
            seedParticle = CreateRectangle(red);
            PaintCanvas.Children.Add(seedParticle);
            Canvas.SetTop(seedParticle, p.position.Y - 2);
            Canvas.SetLeft(seedParticle, p.position.X - 2);
        }

        void timer_Tick(object sender, EventArgs e)
        {
            fixedParticleCount = 0;

            //move particles and convert colliding particles into fixed particles
            foreach (var particle in particleList)
            {
                //if particle is not fixed randomly move it
                if (particle.pType == Particle.ParticleType.freeParticle)
                {
                    //calculate new x position and assign updated velocity to prevXVelocity
                    Vector xSolutionVector = BrownianMotionEquations.BrownianSolution(mass, Temp, relaxTime, particle.position.X, particle.velocity.X, timeStep, BrownianMotionEquations.SolutionDimension.X);                    

                    //calculate new y position and assign updated velocity to prevYVelocity
                    Vector ySolutionVector = BrownianMotionEquations.BrownianSolution(mass, Temp, relaxTime, particle.position.Y, particle.velocity.Y, timeStep, BrownianMotionEquations.SolutionDimension.Y);                    

                    //Point p = new Point(particle.position.X + rand.Next(-3, 4), particle.position.Y + rand.Next(-3, 4));
                    Point p = new Point(xSolutionVector.X, ySolutionVector.X);
                    Vector v = new Vector(xSolutionVector.Y, ySolutionVector.Y);
                    particle.position = p;
                    particle.velocity = v;
                }
                else continue;
                //check for collision, if collision occurs then make particle fixed
                foreach (var otherParticle in particleList)
                {
                    if (otherParticle.pType == Particle.ParticleType.fixedParticle)
                        if (particle.position.X > otherParticle.position.X - 4 && particle.position.X < otherParticle.position.X + 4) //is particle within the width of the fixed particle rectangle?
                            if (particle.position.Y > otherParticle.position.Y - 4 && particle.position.Y < otherParticle.position.Y + 4)//is particle within the height of the fixed particle rectangle?
                                particle.pType = Particle.ParticleType.fixedParticle;
                }
            }

            //erase the canvas
            PaintCanvas.Children.Clear();

            //redraw the canvas
            foreach (var particle in particleList)
            {
                Rectangle rect;
                if (particle.pType == Particle.ParticleType.freeParticle)
                    rect = CreateRectangle(blue);
                else
                {
                    rect = CreateRectangle(red);
                    fixedParticleCount += 1;
                }
                PaintCanvas.Children.Add(rect);
                Canvas.SetTop(rect, particle.position.Y - 2);
                Canvas.SetLeft(rect, particle.position.X - 2);
            }

            //determine if the simulation is over
            if (fixedParticleCount == particleList.Count)
                timer.Stop();
        }

        private void btCreate_Click(object sender, RoutedEventArgs e)
        {
            int particleDistance = 5; //initial distance between particles
            particleList = new List<Particle>();
            particleGridWidth = Convert.ToInt32(tbNumParticles.Text);
            int firstParticleX = (((int)PaintCanvas.ActualWidth - (particleDistance * particleGridWidth)) / 2);
            int firstParticleY = (((int)PaintCanvas.ActualHeight - (particleDistance * (particleGridWidth + 20))) / 2);

            //set the seed particle
            seed = new Particle(Convert.ToInt32(tbXseed.Text), Convert.ToInt32(tbYseed.Text), 0, 0, Particle.ParticleType.fixedParticle);
            particleList.Add(seed);

            //fill the list of particles with non-fixed particles
            for (i = firstParticleX; i < (firstParticleX + particleGridWidth * particleDistance); i += particleDistance)
                for (j = firstParticleY; j < (firstParticleY + (particleDistance * (particleGridWidth + 20))); j += particleDistance)
                {
                    if (i > (Convert.ToInt32(tbXseed.Text) - 5) && i < (Convert.ToInt32(tbXseed.Text) + 5) && j > (Convert.ToInt32(tbYseed.Text) - 5) && j < (Convert.ToInt32(tbYseed.Text) + 5))
                        continue;
                    Particle p = new Particle((i + rand.Next(-2, 3)), (j + rand.Next(-2, 3)), 0, 0, Particle.ParticleType.freeParticle);
                    particleList.Add(p);
                }

            //render the initial scene
            foreach (var particle in particleList)
            {
                Rectangle rect;
                if (particle.pType == Particle.ParticleType.freeParticle)
                    rect = CreateRectangle(blue);
                else rect = CreateRectangle(red);
                PaintCanvas.Children.Add(rect);
                Canvas.SetTop(rect, particle.position.Y - 2);
                Canvas.SetLeft(rect, particle.position.X - 2);
            }

            //set initial conditions for brownian motion calculations
            mass = 2.325E-27;
            Temp = 300;
            relaxTime = 1;
            timeStep = (double)(1.0 / 30.0);

            //start timer
            timer.Start();
        }

        //create a rectangle
        public Rectangle CreateRectangle(Color colour)
        {
            SolidColorBrush fillBrush = new SolidColorBrush(colour);

            return new Rectangle()
            {
                Height = 4,
                Width = 4,
                Fill = fillBrush
            };
        }
    }
}
