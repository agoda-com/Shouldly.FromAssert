namespace Shouldly.FromAssert.Tests.Infrastructure;

/// <summary>Wraps a test body in a compilable document.</summary>
internal static class TestSource
{
    public static string InTestMethod(string body, string signature = "public void TestMethod()") => $@"
using NUnit.Framework;using System.Collections.Generic;using System;using System.Threading.Tasks;
using Shouldly;
namespace TestNamespace
{{
    public class TestClass
    {{
        [Test]
        {signature}
        {{
            {body}
        }}
    }}
}}";

    /// <summary>Nullable context, with a <c>report</c> local whose <c>Error</c> is a <c>string?</c>.</summary>
    public static string InNullableReportTest(string body) => $@"#nullable enable
#pragma warning disable CS1591
using NUnit.Framework;using System.Collections.Generic;
using Shouldly;
namespace TestNamespace
{{
    public class Report
    {{
        public string? Error {{ get; set; }}
        public string Name {{ get; set; }} = """";
        public List<string> Tags {{ get; }} = new List<string>();
    }}

    public class TestClass
    {{
        [Test]
        public void TestMethod()
        {{
            var report = new Report {{ Error = GetError() }};
            {body}
        }}

        private static string? GetError() => ""no route to host"";
    }}
}}";

    /// <summary>Nullable context, with a <c>sample</c> field of many shapes and async helpers.</summary>
    public static string InSampleTest(string body, string signature = "public void TestMethod()") => $@"#nullable enable
#pragma warning disable CS1591
using NUnit.Framework;using System;using System.Collections.Generic;using System.Linq;using System.Threading.Tasks;
using Shouldly;
namespace TestNamespace
{{
    public class Sample
    {{
        public double Ratio {{ get; set; }} = 0.8;
        public TimeSpan? Median {{ get; set; }} = TimeSpan.FromHours(1);
        public DateTimeOffset From {{ get; set; }}
        public DateTimeOffset To {{ get; set; }}
        public string Body {{ get; set; }} = """";
        public List<string> Lines {{ get; }} = new List<string>();
        public List<int> Numbers {{ get; }} = new List<int>();
        public Dictionary<string, string> Errors {{ get; }} = new Dictionary<string, string>();
        public IReadOnlyDictionary<string, string> Failures => Errors;
        public Task<byte[]> ReadAsByteArrayAsync() => Task.FromResult(new byte[] {{ 1 }});
    }}

    public class TestClass
    {{
        private readonly Sample sample = new Sample();

        [Test]
        {signature}
        {{
            {body}
        }}

        private static Task<string> GetStatusAsync() => Task.FromResult(""Ordered"");
        private static Task RefuseAsync() => Task.FromException(new InvalidOperationException());
    }}
}}";
}
