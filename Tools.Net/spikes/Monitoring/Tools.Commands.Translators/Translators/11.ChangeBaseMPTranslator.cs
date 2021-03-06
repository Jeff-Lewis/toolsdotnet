﻿using System;
using Tools.Commands.Implementation;
using Tools.Core.Utils;
using System.Collections.Generic;
using Tools.Core.Asserts;
using System.Globalization;
using System.Xml.Schema;
using System.Xml;
using System.IO;

using Tools.Commands.Implementation.IF1.ChangeBaseMP;

namespace Tools.Commands.Translators
{
    public class ChangeBaseMPTranslator : TranslatorBase
    {
        public ChangeBaseMPTranslator()
        {
            Schemas.Add("http://www.tibco.com/schemas/SDPRO_Observer/Observer/SharedResources/XSD/IF1/AllTypes.xsd", AppDomain.CurrentDomain.BaseDirectory + @"\IF1\xsd\AllTypes.xsd");
            Schemas.Add("http://www.tibco.com/schemas/SDPRO_Observer/Observer/SharedResources/XSD/IF1/ChangeBaseMP.xsd", AppDomain.CurrentDomain.BaseDirectory + @"\IF1\xsd\ChangeBaseMP.xsd");
        }
        #region ICommand2MessageTranslator Members

        public override MessageShim TranslateToShim(GenericCommand command)
        {
            ChangeBaseMP cBMP = new ChangeBaseMP();
            req req = new req();
            cBMP.req = req;

            #region Reference code for mapping

            //req.setTISTdid(request.getTisTerminalDeviceId());
            //req.setPhoneNumber(request.getTdElements().getPhoneNumber());
            //req.setReqId(request.getRequestId().toString());
            //req.setReqTime(
            //BasicConverter.timestampToXMLGregorianCalendar(request.getRequestTime()));

            #endregion

            req.reqId = command.ReqId.ToString();
            req.reqTime = command.ReqTime;
            req.phoneNumber = command.PhoneNumber;
            req.TISTDid = command.TisTDId.ToString();

            if (ErrorTrap.AddAssertion(command.MarketingPackages.Count == 1, "There should be exactly one base marketing package in this command.") &&
                ErrorTrap.AddAssertion(command.MarketingPackages[0].MPType.ToUpper() == "BASE",
                "One and only BASE marketing package is allowed for this command."))
            {
                MarketingPackage mp = command.MarketingPackages[0];

                if (mp.MPId.HasValue)
                {
                    BaseMP bmp = new BaseMP { id = mp.MPId.Value.ToString() };

                    List<@params> parameters = new List<@params>();

                    foreach (PackageParameter pp in mp.Parameters)
                    {
                        // Skip parameters with product code of N/A. That is taken from Milorad's code.
                        if (pp.ProductCode.ToUpper() == "N/A")
                            continue;

                        parameters.Add(new @params
                        {
                            code = (pp.ParamCode == "N/A") ? String.Empty : pp.ParamCode,
                            productCode = pp.ProductCode,
                            value = pp.Value
                        });
                    }

                    bmp.@params = parameters.ToArray();

                    req.BaseMP = bmp;
                }
                else
                {
                    ErrorTrap.AddAssertion(false, String.Format("Marketing package ID is missing (MP_ID command field is null! External mp_instance_id is {0}.", mp.MPInstanceId));
                }
            }


            #region Reference code
            //com.telekomsrbija.foris.commandtypes.changebasemp.BaseMP baseMP = 
            //    new com.telekomsrbija.foris.commandtypes.changebasemp.BaseMP();

            //for (MPInstance mpInstance : request.getMarketingPackages()) {
            //    BaseMPInstance b = (BaseMPInstance) mpInstance;
            //    baseMP.setId(new BigInteger(b.getMarketingPackageId().toString()));

            //    for (Param param : b.getParams()) {
            //        com.telekomsrbija.foris.commandtypes.alltypes.Params p = 
            //            new com.telekomsrbija.foris.commandtypes.alltypes.Params();
            //        p.setCode((param.getPk().getParamCode().equals("N/A") 
            //                ? "" : param.getPk().getParamCode()));
            //        p.setProductCode(param.getPk().getProductCode());
            //        p.setValue(param.getValue());

            //        baseMP.getParams().add(p);
            //    }
            //}

            //req.setBaseMP(baseMP);

            //changeBaseMP.setReq(req); 
            #endregion

            return new MessageShim
            {
                CorrelationId = command.ReqId.ToString(),
                Text = PrepareAndWrapMessageText(req)
            };
        }

        #endregion
    }
}