﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Xml;

using FluentAssertions;

using Nancy.Hosting.Self;

using Newtonsoft.Json;

using NUnit.Framework;

using Polenter.Serialization;

namespace James.ServiceStubs.UnitTests
{
    [TestFixture]
    public class ConfiguredModuleTests
    {
        private ServiceStubsHost _host;

        [OneTimeSetUp]
        public void SetUp()
        {
            _host = new ServiceStubsHost("http://localhost:1234");
            _host.Start();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _host.Dispose();
        }

        [Test]
        public void given_template_with_ref_to_body_elements_when_posting_xml_should_return_proper_response()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:1234/");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

                var person = new Person {FirstName = "Todd", LastName = "Meinershagen", Birthdate = 21.January(1970)};
                var result = client.PostAsync("api/People/xml", person, new XmlMediaTypeFormatter()).Result;
                var content = result.Content.ReadAsStringAsync().Result;
                var age = DateTime.Now.Year - person.Birthdate.Year;

                var expected = @"<?xml version=""1.0"" encoding=""utf-8""?>
<response
  firstName=""Todd""
  lastName=""Meinershagen""
  age=""" + age + @""">
</response>";

                content.Should().Be(expected);
            }
        }

        [Test]
        public void given_template_with_ref_to_body_elements_when_posting_json_should_return_proper_response()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:1234/");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var person = new Person {FirstName = "Todd", LastName = "Meinershagen", Birthdate = 21.January(1970)};
                var result = client.PostAsync("api/People/json", person, new JsonMediaTypeFormatter()).Result;
                var content = result.Content.ReadAsStringAsync().Result;
                var age = DateTime.Now.Year - person.Birthdate.Year;

                var expected = @"{
  ""firstName"": ""Todd"",
  ""lastName"": ""Meinershagen"",
  ""age"": """ + age + @"""
}";

                content.Should().Be(expected);
            }
        }

        public class Person
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public DateTime Birthdate { get; set; }
        }
    }
}
