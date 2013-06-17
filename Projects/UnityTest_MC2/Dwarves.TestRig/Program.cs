// ----------------------------------------------------------------------------
// <copyright file="Program.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.TestRig
{
    using System.Threading;

    /// <summary>
    /// The main class.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The application entry point.
        /// </summary>
        /// <param name="args">The application args.</param>
        private static void Main(string[] args)
        {
            ////ITest test = new CreateTerrainTest();
            ITest test = new DigTerrainTest();

            while (true)
            {
                test.Update();
                Thread.Sleep(1);
            }
        }
    }
}