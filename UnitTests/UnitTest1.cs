using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LanderSimulator.Model;

namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void SanityCheck()
        {
            // Check that the lander will crash with the default settings
            LanderSimulator.Model.Environment environment = new LanderSimulator.Model.Environment();
            LanderSimulator.Model.Lander lander = new LanderSimulator.Model.Lander(environment, 100, 0, 100);
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
