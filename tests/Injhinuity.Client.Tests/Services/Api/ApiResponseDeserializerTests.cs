using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Injhinuity.Client.Core.Exceptions;
using Injhinuity.Client.Model.Domain;
using Injhinuity.Client.Services.Api;
using Injhinuity.Client.Services.Mappers;
using NSubstitute;
using Xunit;

namespace Injhinuity.Client.Tests.Services.Api
{
    public class ApiResponseDeserializerTests
    {
        private static readonly IFixture Fixture = new Fixture();
        private readonly IApiReponseDeserializer _subject;

        private readonly HttpResponseMessage _httpResponseMessage = new HttpResponseMessage();

        private readonly Command _command = Fixture.Create<Command>();
        private readonly IEnumerable<Command> _commands = Fixture.CreateMany<Command>();

        private readonly IInjhinuityMapper _mapper;

        public ApiResponseDeserializerTests()
        {
            _mapper = Substitute.For<IInjhinuityMapper>();
            _httpResponseMessage = new HttpResponseMessage();

            _subject = new ApiResponseDeserializer(_mapper);
        }

        [Fact]
        public void DeserializeAsync_WhenCalledWithEmptyContent_ThenThrowsException()
        {
            _httpResponseMessage.Content = new StringContent("");

            Func<Task> act = async () => await _subject.DeserializeAsync<object>(_httpResponseMessage);

            act.Should().Throw<InjhinuityException>().WithMessage("Deserialized result is null.");
        }

        [Fact]
        public async Task DeserializeAsync_WhenCalledWithContent_ThenReturnDeserializedObject()
        {
            _httpResponseMessage.Content = new StringContent("{\"aaaa\":\"aaaa\"}");

            var result = await _subject.DeserializeAsync<object>(_httpResponseMessage);

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task DeserializeAndAdaptAsync_WhenCalled_ThenDeserializesAndCastsToProperType()
        {
            _httpResponseMessage.Content = new StringContent("{\"aaaa\":\"aaaa\"}");

            _mapper.Map<object, Command>(Arg.Any<object>()).Returns(_command);

            var result = await _subject.DeserializeAndAdaptAsync<object, Command>(_httpResponseMessage);

            result.Should().NotBeNull();
            result.Should().BeOfType<Command>();
        }

        [Fact]
        public async Task DeserializeAndAdaptEnumerableAsync_WhenCalled_ThenDeserializesAndCastsToProperType()
        {
            _httpResponseMessage.Content = new StringContent("[\"aaaa\"]");

            _mapper.Map<IEnumerable<object>, IEnumerable<Command>>(Arg.Any<IEnumerable<object>>()).Returns(_commands);

            var result = await _subject.DeserializeAndAdaptEnumerableAsync<object, Command>(_httpResponseMessage);

            result.Should().NotBeEmpty();
            result.Should().AllBeOfType<Command>();
        }
    }
}
