namespace Auditable.Tests
{
    using Infrastructure;
    using Machine.Specifications;

    public class ResetTheClock : ICleanupAfterEveryContextInAssembly
    {
        public void AfterContextCleanup()
        {
            SystemDateTime.Reset();
        }
    }
}