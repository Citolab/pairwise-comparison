using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Cito.Cat.Core.Interfaces;
using Cito.Cat.Core.Models;
using Cito.Cat.Service.Handlers;
using Cito.Cat.Service.Tests.Stubs;
using Ims.Cat.Models;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace Cito.Cat.Service.Tests
{
    [ExcludeFromCodeCoverage]
    public class PairwiseCatIntegrationTests : MyRavenTestDriver
    {
        private readonly IGoodTime _goodTime;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public PairwiseCatIntegrationTests()
        {
            _goodTime = new GoodTimeStub();
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                //IgnoreNullValues = true,
                Converters =
                {
                    new JsonStringEnumMemberConverter(JsonNamingPolicy.CamelCase)
                }
            };
            
            CatOptions = new CatOptions
            {
                ProcessorTypeName = "PairwiseCatProcessor",
                ProcessorAssemblyName = "Cito.Cat.Algorithms.Pairwise"
            };
        }

        [Test]
        public void InitializeSection()
        {
            // arrange
            var catSectionHandler = new CatSectionHandler(_goodTime, CatOptions, DocumentSession, NullLoggerFactory.Instance);
            var text = File.ReadAllText(@"testdata/pairwise/SectionInitRequest.json");
            var request =
                JsonSerializer.Deserialize<SectionDType>(text, _jsonSerializerOptions);

            // act
            var response = catSectionHandler.Initialize(request).Result;

            // assert
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.SectionIdentifier));
        }

        [Test]
        public void InitializeSession()
        {
            // arrange
            Console.WriteLine(Environment.CurrentDirectory);
            // init section
            var catSectionHandler = new CatSectionHandler(_goodTime, CatOptions, DocumentSession, NullLoggerFactory.Instance);
            var text = File.ReadAllText(@"testdata/pairwise/SectionInitRequest.json");
            var sectionInitRequest =
                JsonSerializer.Deserialize<SectionDType>(text, _jsonSerializerOptions);
            var sectionInitResponse = catSectionHandler.Initialize(sectionInitRequest).Result;

            var catSessionHandler = new CatSessionHandler(CatOptions, DocumentSession, NullLoggerFactory.Instance);
            text = File.ReadAllText(@"testdata/pairwise/SessionInitRequest.json");
            var sessionInitRequest = JsonSerializer.Deserialize<SessionDType>(text, _jsonSerializerOptions);

            // act
            var response = catSessionHandler.Initialize(sessionInitRequest, sectionInitResponse.SectionIdentifier)
                .Result;

            // assert
            //Assert.IsFalse(string.IsNullOrWhiteSpace(response.SectionIdentifier));
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.SessionIdentifier));
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.SessionState));
            Assert.IsNotNull(response.NextItems);
            Assert.IsNotEmpty(response.NextItems.ItemIdentifiers);
        }

        [Test]
        public void GetNextItems()
        {
            // arrange
            Console.WriteLine(Environment.CurrentDirectory);
            // init section
            var catSectionHandler = new CatSectionHandler(_goodTime, CatOptions, DocumentSession, NullLoggerFactory.Instance);
            var text = File.ReadAllText(@"testdata/pairwise/SectionInitRequest.json");
            var sectionInitRequest =
                JsonSerializer.Deserialize<SectionDType>(text, _jsonSerializerOptions);
            var sectionInitResponse = catSectionHandler.Initialize(sectionInitRequest).Result;

            var catSessionHandler = new CatSessionHandler(CatOptions, DocumentSession, NullLoggerFactory.Instance);
            text = File.ReadAllText(@"testdata/pairwise/SessionInitRequest.json");
            var sessionInitRequest = JsonSerializer.Deserialize<SessionDType>(text, _jsonSerializerOptions);

            var initResponse = catSessionHandler.Initialize(sessionInitRequest, sectionInitResponse.SectionIdentifier)
                .Result;

            text = File.ReadAllText(@"testdata/pairwise/NextItemsRequest.json");
            var nextItemsRequest = JsonSerializer.Deserialize<ResultsDType>(text, _jsonSerializerOptions);


            var nextItemsHandler = new NextItemsHandler(CatOptions, DocumentSession, NullLoggerFactory.Instance);

            // act
            var nextItemsResponse =
                nextItemsHandler.Get(nextItemsRequest, sectionInitResponse.SectionIdentifier).Result;

            // assert
            Assert.IsFalse(string.IsNullOrWhiteSpace(nextItemsResponse.SessionState));
            Assert.IsNotNull(nextItemsResponse.NextItems);
            Assert.IsNotEmpty(nextItemsResponse.NextItems.ItemIdentifiers);
        }
    }
}