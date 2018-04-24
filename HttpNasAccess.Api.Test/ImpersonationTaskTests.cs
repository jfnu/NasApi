using System;
using System.Collections.Generic;
using System.Linq;
using HttpNasAccess.Api.Impersonation.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttpNasAccess.Api.Test
{
    [TestClass]
    public class ImpersonationTaskTests
    {
        [TestMethod]
        public void TestListAllImpersonationTask()
        {
            var task = new ListAllImpersonationTask(@"\\nasv0034\WAPFileTest\GoodNas");
            var result = task.Execute();
            Assert.IsTrue(result.Any());
        }

        [TestMethod]
        public void TestListDateTimesImpersonationTask()
        {
            var task = new ListInfoImpersonationTask(@"\\nasv0034\WAPFileTest\GoodNas");
            var result = task.Execute();
            Assert.IsTrue(result.Any());
        }

        [TestMethod]
        public void TestCopyToAnotherNasImpersonationTask()
        {
            var filesToCopy = new List<string>
            {
                @"\\nasv0034\WAPFileTest\GoodNas\Folder1",
                @"\\nasv0034\WAPFileTest\GoodNas\APSET1761.JPG",
                @"\\nasv0034\WAPFileTest\GoodNas\IST databases 042415.xlsx"
            };
            var task = new CopyToAnotherNasImpersonationTask(filesToCopy,@"\\nasv0034\WAPFileTest\GoodNas\Folder2");
            var result = task.Execute();
            Assert.IsTrue(result);
        }
    }
}
