using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lander.Model;

namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void SanityCheck()
        {
            // Check that the lander will crash with the default settings
            Lander.Model.Environment environment = new Lander.Model.Environment();
            Lander.Model.Lander lander = new Lander.Model.Lander(environment, 100, 0, 100);
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
