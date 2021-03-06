﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Tools.Core;
using Tools.Core.Utils;

namespace Tools.Logging
{
    public class LogDataXPathFormatter : IXPathFormatter
    {
        #region IXPathFormatter Members

        public virtual XPathNavigator Format(object data)
        {
            if (data is XPathNavigator) return data as XPathNavigator; // Somebody already did the job

            var sData = data as String;

            if (!String.IsNullOrEmpty(sData))
            {
                return new XPathDocument(new StringReader(
                                             CombineTraceStringForMessageOnly(sData))).CreateNavigator();
            }

            var exEntry = data as Exception;

            if (exEntry != null)
            {
                var sb = new StringBuilder();
                var exInfo = new StringBuilder();

                using (XmlWriter xWriter = XmlWriter.Create(exInfo,
                                                            new XmlWriterSettings
                                                                {
                                                                    OmitXmlDeclaration = false,
                                                                    ConformanceLevel = ConformanceLevel.Fragment
                                                                }))
                {
                    AddExceptionToTraceString(xWriter, exEntry);
                    // TODO: (SD) Fix to provide proper innerException xml formatting
                    sb.Append("<TraceRecord xmlns=\"http://schemas.microsoft.com/2004/10/E2ETraceEvent/TraceRecord\">").
                        Append("<TraceIdentifier>http://code.google.com/p/tools/log.aspx</TraceIdentifier>").
                        Append("<Description>").Append(exEntry.Message).Append("</Description>").
                        Append("<Exception>").Append(exEntry.ToString()).
                        Append("</Exception>").
                        Append("</TraceRecord>");

                    return new XPathDocument(new StringReader(sb.ToString())).CreateNavigator();
                }
            }
            if (data != null)
            {
                return new XPathDocument(new StringReader(
                                             CombineTraceStringForMessageOnly(data.ToString()))).CreateNavigator();
            }
            return null;
        }

        #endregion

        #region Methods - helper

        protected static string CombineTraceStringForMessageOnly(string message)
        {
            var sb = new StringBuilder();
            sb.Append("<TraceRecord xmlns=\"http://schemas.microsoft.com/2004/10/E2ETraceEvent/TraceRecord\">").
                Append("<TraceIdentifier>http://code.google.com/p/tools/log.aspx</TraceIdentifier>").
                Append("<Description>").Append(XmlUtility.Encode(message)).
                Append("</Description></TraceRecord>");
            return sb.ToString();
        }

        private void AddExceptionToTraceString(XmlWriter xml, Exception exception)
        {
            xml.WriteElementString("ExceptionType", XmlUtility.Encode(exception.GetType().AssemblyQualifiedName));
            xml.WriteElementString("Message", XmlUtility.Encode(exception.Message));
            xml.WriteElementString("StackTrace", XmlUtility.Encode(StackTraceString(exception)));
            xml.WriteElementString("ExceptionString", XmlUtility.Encode(exception.ToString()));

            var exception2 = exception as Win32Exception;

            if (exception2 != null)
            {
                xml.WriteElementString("NativeErrorCode",
                                       exception2.NativeErrorCode.ToString("X", CultureInfo.InvariantCulture));
            }
            if ((exception.Data != null) && (exception.Data.Count > 0))
            {
                xml.WriteStartElement("DataItems");
                foreach (object obj2 in exception.Data.Keys)
                {
                    xml.WriteStartElement("Data");
                    xml.WriteElementString("Key", XmlUtility.Encode(obj2.ToString()));
                    xml.WriteElementString("Value", XmlUtility.Encode(exception.Data[obj2].ToString()));
                    xml.WriteEndElement();
                }
                xml.WriteEndElement();
            }
            if (exception.InnerException != null)
            {
                xml.WriteStartElement("InnerException");
                AddExceptionToTraceString(xml, exception.InnerException);
                xml.WriteEndElement();
            }
        }

        private string StackTraceString(Exception exception)
        {
            string stackTrace = exception.StackTrace;
            if (!string.IsNullOrEmpty(stackTrace))
            {
                return stackTrace;
            }
            StackFrame[] frames = new StackTrace(false).GetFrames();
            int skipFrames = 0;
            bool flag = false;
            foreach (StackFrame frame in frames)
            {
                string str3;
                string name = frame.GetMethod().Name;
                if (((str3 = name) != null) &&
                    (((str3 == "StackTraceString") || (str3 == "AddExceptionToTraceString")) ||
                     (((str3 == "BuildTrace") || (str3 == "TraceEvent")) || (str3 == "TraceException"))))
                {
                    skipFrames++;
                }
                else if (name.StartsWith("ThrowHelper", StringComparison.Ordinal))
                {
                    skipFrames++;
                }
                else
                {
                    flag = true;
                }
                if (flag)
                {
                    break;
                }
            }
            var trace = new StackTrace(skipFrames, false);
            return trace.ToString();
        }

        #endregion
    }
}