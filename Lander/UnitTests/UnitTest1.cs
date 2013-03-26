using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LanderSimulator;

namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void SanityCheck()
        {
            // Check that the lander will crash with the default settings
            LanderSimulator.Environment environment = new LanderSimulator.Environment();
            Lander lander = new Lander(environment, 100, 0, 100);
            bool crashed = false;
            for (int i = 0; i < 100; i++)
            {
                if (lander.Update() == LanderStatus.Crashed)
                {
                    crashed = true;
                    break;
                }
            }
            Assert.IsTrue(crashed);
        }
    }
}
