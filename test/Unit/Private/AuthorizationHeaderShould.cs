using Amazon.Util;
using AWS.SignatureVersion4.Private;
using AWS.SignatureVersion4.TestSuite;
using Shouldly;
using Xunit;

namespace AWS.SignatureVersion4.Unit.Private
{
    public class AuthorizationHeaderShould : IClassFixture<TestSuiteContext>
    {
        private readonly TestSuiteContext context;

        public AuthorizationHeaderShould(TestSuiteContext context)
        {
            this.context = context;
        }

        [Theory]
        [InlineData("get-header-key-duplicate")]
        [InlineData("get-header-value-multiline")]
        [InlineData("get-header-value-order")]
        [InlineData("get-header-value-trim")]
        [InlineData("get-unreserved")]
        [InlineData("get-utf8")]
        [InlineData("get-vanilla")]
        [InlineData("get-vanilla-empty-query-key")]
        [InlineData("get-vanilla-query")]
        [InlineData("get-vanilla-query-order-key")]
        [InlineData("get-vanilla-query-order-key-case")]
        [InlineData("get-vanilla-query-order-value")]
        [InlineData("get-vanilla-query-unreserved")]
        [InlineData("get-vanilla-utf8-query")]
        [InlineData("normalize-path", "get-relative")]
        [InlineData("normalize-path", "get-relative-relative")]
        [InlineData("normalize-path", "get-slash")]
        [InlineData("normalize-path", "get-slash-dot-slash")]
        [InlineData("normalize-path", "get-slashes")]
        [InlineData("normalize-path", "get-slash-pointless-dot")]
        [InlineData("normalize-path", "get-space")]
        [InlineData("post-header-key-case")]
        [InlineData("post-header-key-sort")]
        [InlineData("post-header-value-case")]
        [InlineData("post-sts-token", "post-sts-header-after")]
        [InlineData("post-sts-token", "post-sts-header-before")]
        [InlineData("post-vanilla")]
        [InlineData("post-vanilla-empty-query-value")]
        [InlineData("post-vanilla-query")]
        [InlineData("post-x-www-form-urlencoded", Skip = SkipReasons.PlausibleSignedHeadersTestSuiteError)]
        [InlineData("post-x-www-form-urlencoded-parameters", Skip = SkipReasons.RedundantContentTypeCharset)]
        public void PassTestSuite(params string[] scenarioName)
        {
            // Arrange
            var scenario = context.LoadScenario(scenarioName);

            // Add header 'X-Amz-Date' since the algorithm at this point expects it on the request
            scenario.Request.AddHeader(HeaderKeys.XAmzDateHeader, context.UtcNow.ToIso8601BasicDateTime());

            // Act
            var authorizationHeader = AuthorizationHeader.Build(
                context.UtcNow,
                context.RegionName,
                context.ServiceName,
                context.Credentials,
                scenario.ExpectedSignedHeaders,
                scenario.ExpectedCredentialScope,
                scenario.ExpectedStringToSign);

            // Assert
            authorizationHeader.ShouldBe(scenario.ExpectedAuthorizationHeader);
        }
    }
}