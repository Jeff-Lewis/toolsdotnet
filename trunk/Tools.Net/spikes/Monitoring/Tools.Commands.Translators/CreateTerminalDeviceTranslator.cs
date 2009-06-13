﻿using System;
using Tools.Commands.Implementation;
using Tools.Commands.Implementation.IF1.Ctd;
using Tools.Core.Utils;
using System.Collections.Generic;

namespace Tools.Commands.Translators
{
    public class CreateTerminalDeviceTranslator : ICommand2MessageTranslator
    {

        #region ICommand2MessageTranslator Members

        public MessageShim TranslateToShim(GenericCommand command)
        {
            CreateTerminalDevice ctd = new CreateTerminalDevice();

            ctd.req = new req();

            ctd.req.reqId = command.ReqId.ToString();

            CreateTerminalDevice createTD = new CreateTerminalDevice();


            bool baseMPSet = false;
            //TODO: (SD) can't use extension methods
            foreach (MarketingPackage mp in command.MarketingPackages)
            {
                // Base package is discriminated by the BASE MPType
                if (mp.MPType.ToUpper() == "BASE")
                {
                    baseMPSet = true;

                    baseMP bmp = new baseMP { id = mp.MPId.ToString() };

                    List<@params> parameters = new List<@params>();

                    foreach (PackageParameter pp in mp.Parameters)
                    {
                        parameters.Add(new @params
                        {
                            code = pp.ParamCode,
                            productCode = pp.ProductCode,
                            value = pp.Value
                        });



                    }

                    bmp.@params = parameters.ToArray();

                    ctd.req.baseMP = bmp;
                }
            }

            return new MessageShim
            {
                CorrelationId = command.ReqId.ToString(),
                Text = SerializationUtility.Serialize2String(ctd.req)
            };

            //for (Iterator<MPInstance> iter = request.getMarketingPackages().iterator(); iter.hasNext();) {
            //    mpInstance = iter.next();
            //    if (mpInstance instanceof BaseMPInstance) {
            //        baseMP.setId(new BigInteger(String.valueOf(mpInstance.getMarketingPackageId())));
            //        for (Param param : mpInstance.getParams()) {
            //            com.telekomsrbija.foris.commandtypes.alltypes.Params p = 
            //                new com.telekomsrbija.foris.commandtypes.alltypes.Params();
            //            p.setCode(param.getPk().getParamCode());
            //            p.setProductCode(param.getPk().getProductCode());
            //            p.setValue(param.getValue());

            //            baseMP.getParams().add(p);
            //        }
            //    }

            //    /*if (mpInstance instanceof AddonInstance) {
            //        AddOnMPs addon = new AddOnMPs();
            //        addon.setId(new BigInteger(String.valueOf(mpInstance.getMarketingPackageId())));
            //        for (Param param : mpInstance.getParams()) {
            //            com.telekomsrbija.foris.commandtypes.alltypes.Params p = 
            //                new com.telekomsrbija.foris.commandtypes.alltypes.Params();
            //            p.setCode(param.getPk().getParamCode());
            //            p.setProductCode(param.getPk().getProductCode());
            //            p.setValue(param.getValue());

            //            addon.getParams().add(p);
            //        }
            //        req.getAddOnMPs().add(addon);
            //    }*/
            //}


            //// treba dodati preslikavanje za params
            //// treba dodati i req.getAddonMPs - ovo proveriti kako ce ici
            //req.setBaseMP(baseMP);
            //req.setContractEnd(
            //        BasicConverter.timestampToXMLGregorianCalendar(request.getContractEndDate()));
            //req.setMonthlyLimit(request.getMonthlyLimit());
            //req.setReqId(request.getRequestId().toString());
            //req.setReqTIme(
            //        BasicConverter.timestampToXMLGregorianCalendar(request.getRequestTime()));
            //req.setTDtype(String.valueOf(request.getTdType().toInt()));
            //req.setP2p(request.getP2P());

            //TDelements tdElements = new TDelements();
            //if (request.getTdElements().getIccid() != null) {
            //    Mobile mobile = new Mobile();
            //    mobile.setICCID(request.getTdElements().getIccid());
            //    mobile.setPhoneNumber(request.getTdElements().getPhoneNumber());
            //    mobile.setProfile(request.getTdElements().getProfile());
            //    mobile.setShortNumber(request.getTdElements().getShortNumber());

            //    tdElements.setMobile(mobile);
            //}

            //req.setTDelements(tdElements);

            //req.setTISCustomerId(request.getTisCustomerId());
            //req.setTISTDid(request.getTisTerminalDeviceId());
            //req.setTISwalletId(request.getTisWalletId());

            //createTD.setReq(req);

            //out.add("payload", req);
            //out.add("id", req.getReqId());
            //out.add("commandName", createTD.getClass().getSimpleName());
        }

        #endregion
    }
}