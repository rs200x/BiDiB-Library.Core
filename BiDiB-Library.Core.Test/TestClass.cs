using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using org.bidib.netbidibc.core.Models;

namespace org.bidib.netbidibc.core.Test
{
    [TestClass]
    public abstract class TestClass<TTarget> : TestClass
    {
        public TTarget Target { get; set; }
    }

    [TestClass]
    public abstract class TestClass
    {
        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            OnTestInitialize();
        }

        protected virtual void OnTestInitialize()
        {

        }

        [TestCleanup]
        public void TestCleanup()
        {
            OnTestCleanup();
        }

        protected virtual void OnTestCleanup()
        {

        }

        public void ValidateFile(string filePath, string schema, string xsdFilePath)
        {
            File.Exists(filePath).Should().BeTrue();
            File.Exists(xsdFilePath).Should().BeTrue();

            StringBuilder errorString = new StringBuilder();
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Schemas.Add(schema, xsdFilePath);
                xmlDocument.Load(filePath);

                if (xmlDocument.DocumentElement != null && xmlDocument.DocumentElement.NamespaceURI == schema)
                {
                    xmlDocument.Validate((sender, args) =>
                    {
                        errorString.AppendLine(string.Format(CultureInfo.CurrentUICulture, "{0}: {1}", args.Severity, args.Message));
                    });
                }
                else
                {
                    errorString.AppendLine(string.Format(CultureInfo.CurrentUICulture, "no schema defined {0}", schema));
                }
            }
            catch (XmlSchemaValidationException ex)
            {
                errorString.AppendLine(string.Format(CultureInfo.CurrentUICulture, "no valid for schema {0}", ex.Message));

            }
            catch (Exception ex)
            {
                errorString.AppendLine(string.Format(CultureInfo.CurrentUICulture, "process error {0}", ex.Message));
            }

            string errorMessage = errorString.ToString();

            errorMessage.Should().BeNullOrEmpty();
        }

        public T LoadFromXmlFile<T>(string filePath) where T : class
        {
            T data = null;

            try
            {
                XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                namespaces.Add("ct", Namespaces.CommonTypesNamespaceUrl);
                namespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                using (XmlReader reader = XmlReader.Create(filePath))
                {
                    data = xmlSerializer.Deserialize(reader) as T;
                }
            }
            catch (Exception ex)
            {
                Assert.Fail("Content of '{0}' could not be deserialized into object of type '{1}' -> {2}", filePath, typeof(T), ex.Message);
            }

            return data;
        }

        public void SaveToXmlFile<T>(T instance, string filePath) where T : class
        {
            try
            {
                XmlWriterSettings settings = new XmlWriterSettings { Indent = true };
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                namespaces.Add("ct", Namespaces.CommonTypesNamespaceUrl);
                namespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
                using (XmlWriter writer = XmlWriter.Create(filePath, settings))
                {
                    xmlSerializer.Serialize(writer, instance, namespaces);
                }
            }
            catch (Exception ex)
            {
                Assert.Fail("Instance of type '{0}' could not be serialized to file '{1}' -> {2}", instance.GetType(), filePath, ex.Message);
            }
        }

        public byte[] GetBytes(string messageString, char separator = '-')
        {
            return Array.ConvertAll(messageString.Split(separator), s => Convert.ToByte(s, 16));
        }
    }
}