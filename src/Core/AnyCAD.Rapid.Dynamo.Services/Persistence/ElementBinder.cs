﻿using AnyCAD.Foundation;
using DynamoServices;

namespace AnyCAD.Rapid.Dynamo.Services.Persistence
{
    /// <summary>
    /// ElementBinder
    /// </summary>
    internal class ElementBinder
    {
        // Dynamo use a hard-code slot id for now, we have to use the same one instead of generating a new one
        // private const string ANYCAD_TRACE_ID = "{E416DDA1-9CD0-47D2-83D7-BA07A96F99FA}-ANYCAD";
        private const string ANYCAD_TRACE_ID = "{0459D869-0C72-447F-96D8-08A7FB92214B}-REVIT";

        public static ElementId GetElementIdFromTrace(Document document)
        {
            //Get the element ID that was cached in the callsite
            ElementId id = ElementId.InvalidId;
            var traceData = TraceUtils.GetTraceData(ANYCAD_TRACE_ID);
            if (!string.IsNullOrEmpty(traceData))
            {                
                ulong result;
                bool success = ulong.TryParse(traceData, out result);

                if (success)
                {
                    id = new(result);
                    Console.WriteLine("Conversion successful: " + result);
                }               
            }
            id ??= ElementId.InvalidId; // There was no usable data in the trace cache
            return id;
        }

        /// <summary>
        /// Get the element associated with the current operation from trace
        /// null if there is no object, or it's of the wrong type etc.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Element? GetElementFromTrace(Document document)
        {
            var elementId = GetElementIdFromTrace(document);

            if (!elementId.IsValid())
                return null;           

            return document.FindElement(elementId);
        }

        /// <summary>
        /// Set the element associated with the current operation from trace
        /// null if there is no object, or it's of the wrong type etc.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static void SetElementForTrace(ElementId elementId)
        {
            if (elementId == null || !elementId.IsValid())
                return;

            // if we're mutating the current Element id, that means we need to 
            // clean up the old object

            // Set the element ID cached in the callsite
            TraceUtils.SetTraceData(ANYCAD_TRACE_ID, elementId.ToString());
        }
    }
}
