﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.42
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.CodeDom.Compiler;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;

[GeneratedCode("System.ServiceModel", "3.0.0.0")]
[ServiceContract(Namespace = "http://Dsi.Tools.Common.servicehost.wcf/test", ConfigurationName = "IStatusQuerable")]
public interface IStatusQuerable
{
    [OperationContract(Action = "http://Dsi.Tools.Common.servicehost.wcf/test/IStatusQuerable/QueryForStatus",
        ReplyAction = "http://Dsi.Tools.Common.servicehost.wcf/test/IStatusQuerable/QueryForStatusResponse")]
    string QueryForStatus();
}

[GeneratedCode("System.ServiceModel", "3.0.0.0")]
public interface IStatusQuerableChannel : IStatusQuerable, IClientChannel
{
}

[DebuggerStepThrough]
[GeneratedCode("System.ServiceModel", "3.0.0.0")]
public class StatusQuerableClient : ClientBase<IStatusQuerable>, IStatusQuerable
{
    public StatusQuerableClient()
    {
    }

    public StatusQuerableClient(string endpointConfigurationName) :
        base(endpointConfigurationName)
    {
    }

    public StatusQuerableClient(string endpointConfigurationName, string remoteAddress) :
        base(endpointConfigurationName, remoteAddress)
    {
    }

    public StatusQuerableClient(string endpointConfigurationName, EndpointAddress remoteAddress) :
        base(endpointConfigurationName, remoteAddress)
    {
    }

    public StatusQuerableClient(Binding binding, EndpointAddress remoteAddress) :
        base(binding, remoteAddress)
    {
    }

    #region IStatusQuerable Members

    public string QueryForStatus()
    {
        return base.Channel.QueryForStatus();
    }

    #endregion
}