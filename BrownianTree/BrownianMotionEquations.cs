using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrownianTree
{
    class BrownianMotionEquations
    {
        /*
         * 2D -> 2 sets of coupled equations of motion, one for x and one for y.
         * All variables must be in SI units
         * ---------------------------------------------------------------------
         * x-direction:
         * 
         * (1)   u = dx/dt
         * (2)   du/dt = n_x(t) + beta * u
         * 
         * beta = 1 / relaxTime
         * 
         * n_x(t) = G_x * Sqrt((Pi * S) / deltat)
         * 
         * deltat = simulation timestep
         * S = (2*k*T*beta)/(Pi*m)
         * m = mass of particle
         * T = temperature
         * k = Boltzmann's constant
         * 
         * Before calculating G generate 2 uniform random numbers U1 and U2 between 0 and 1 (Random.NextDouble())
         * 
         * G_x = Sqrt(-2*ln(U1))*Cos(2*Pi*U2)
         * 
         * Use a forward Euler scheme to solve (1) after using a 4th order Runge-Kutta scheme to solve (2)
         * ------------------------------------------------------------------------------------------------------
         * y-direction:
         * 
         * (1)  v = dy/dt
         * (2)  dv/dt = n_y(t) + beta * v
         * 
         * n_y(t) = G_y * Sqrt((Pi * S) / deltat)
         * 
         * G_y = Sqrt(-2*ln(U1))*Sin(2*Pi*U2)
         * 
         */

        public enum SolutionDimension
        {
            X,
            Y
        }

        public static double BrownianSolution(double mass, double Temp, double relaxTime, double timeStep, SolutionDimension XorY)
        {
            /*************************************************************************************************
             * !!Change return type to a vector since will need to return both the velocity and the position!!
             **************************************************************************************************/ 
            double result = 0;
            Random random = new Random();

            //Calculate G
            double U1, U2, G;
            U1 = random.NextDouble();
            U2 = random.NextDouble();
            if(XorY == SolutionDimension.X)
                G = Math.Sqrt(-2 * (Math.Log(U1))) * Math.Cos(2 * Math.PI * U2);
            else
                G = Math.Sqrt(-2 * (Math.Log(U1))) * Math.Sin(2 * Math.PI * U2);

            //Calculate S
            double S;
            double k = 1.3806488E-23;
            S = (2 * k * Temp) / (Math.PI * mass * relaxTime);

            //Calculate n_x(t)
            double n = G * Math.Sqrt((Math.PI * S) / timeStep);

            //perform 4th order Runge-Kutta scheme on (2)
            //perform forward Euler scheme on (1)
            return result;
        }
    }
}
