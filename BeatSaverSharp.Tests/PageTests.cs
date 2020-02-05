using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static BeatSaverSharp.Tests.Utils;

namespace BeatSaverSharp.Tests
{
    [TestClass]
    public class PageTests
    {
        #region Method Tests
        [TestMethod]
        public async Task Pagination()
        {
            var page0 = await Client.Hot(0);
            Assert.IsNull(page0.PreviousPage);
            Assert.IsNotNull(page0.NextPage);
            Assert.IsNotNull(page0.LastPage);

            var page1 = await page0.FetchNextPage();
            Assert.AreEqual(page1.PreviousPage, 0);
            Assert.AreEqual(page1.NextPage, 2);
        }

        [TestMethod]
        public async Task ManualPagination()
        {
            var task1 = Client.Latest(0);
            var task2 = Client.Latest(1);
            var task3 = Client.Latest(2);

            var pages = await Task.WhenAll(task1, task2, task3);
            var page0 = pages[0];
            var page1 = pages[1];
            var page2 = pages[2];

            Assert.IsNull(page0.PreviousPage);
            Assert.IsNotNull(page0.NextPage);
            Assert.IsNotNull(page0.LastPage);

            Assert.AreEqual(page1.PreviousPage, 0);
            Assert.AreEqual(page1.NextPage, 2);

            Assert.AreEqual(page2.PreviousPage, 1);
            Assert.AreEqual(page2.NextPage, 3);

            var lastPage = await Client.Latest((uint)page0.LastPage);
            Assert.IsNotNull(lastPage.PreviousPage);
            Assert.IsNull(lastPage.NextPage);

        }
        #endregion
    }
}
