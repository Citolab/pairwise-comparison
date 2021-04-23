using System;
using Cito.Cat.Core.Interfaces;
using Cito.Cat.Core.Models;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;

namespace Cito.Cat.Service
{
    public static class CatHelper
    {
        public static void CheckCatProcessorType(CatOptions catOptions)
        {
            var typeName =
                $"{catOptions.ProcessorAssemblyName}.{catOptions.ProcessorTypeName}, {catOptions.ProcessorAssemblyName}";
            var catProcessorType = Type.GetType(typeName);
            if (catProcessorType == null)
            {
                throw new Exception($"CatProcessor with type name {catOptions.ProcessorTypeName} could not be found.");
            }
        }

        public static ICatProcessor GetProcessor(CatOptions catOptions, string sectionConfiguration,
            ILoggerFactory loggerFactory = null, IAsyncDocumentSession asyncDocumentSession = null)
        {
            var typeName =
                $"{catOptions.ProcessorAssemblyName}.{catOptions.ProcessorTypeName}, {catOptions.ProcessorAssemblyName}";
            var catProcessorType = Type.GetType(typeName);
            if (catProcessorType == null)
            {
                throw new Exception($"CatProcessor with type name {catOptions.ProcessorTypeName} could not be found.");
            }

            var catProcessor = (ICatProcessor) Activator.CreateInstance(catProcessorType, sectionConfiguration,
                loggerFactory, asyncDocumentSession);
            return catProcessor;
        }
    }
}