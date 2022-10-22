namespace org.bidib.netbidibc.core.Test
{
    public static class TestCategory
    {
        /// <summary>
        /// Defines component unit tests
        /// </summary>
        public const string UnitTest = "UnitTest";

        /// <summary>
        /// Defines tests which integrate with other/multiple components.
        /// </summary>
        public const string IntegrationTest = "IntegrationTest";

        /// <summary>
        /// Defines tests which integrate with other external components/systems.
        /// </summary>
        public const string SystemIntegrationTest = "SystemIntegrationTest";

        /// <summary>
        /// A manual test needs special attention and/or manual preparation of test data,
        /// e.g. database records. In some way it is an abuse of unit testing - it is 
        /// just a means to save work.
        /// 
        /// Manual tests should usually not appear in the test browser - comment out
        /// the [Test]-Attribute before checking in
        /// </summary>
        public const string ManualTest = "ManualTest";
    }
}