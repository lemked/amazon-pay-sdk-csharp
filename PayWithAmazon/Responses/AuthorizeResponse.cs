﻿using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace PayWithAmazon.Responses
{
    public class AuthorizeResponse
    {
        /// <summary>
        /// Documentation source https://payments.amazon.com/documentation/apireference/201752450
        /// </summary>

        public string xml;
        public string json;
        public IDictionary dictionary;
        public string authorizationId;

        /// <summary>
        /// AmazonOrderReferenceId returned only for AuthorizeOnBillingAgreement API call
        /// </summary>
        public string amazonOrderReferenceId;
        public string requestId;
        public string authorizationReferenceId;
        public string sellerAuthorizationNote;

        public decimal authorizationAmount;
        public decimal capturedAmount;
        public string capturedAmountCurrencyCode;
        public string authorizationAmountCurrencyCode;

        public decimal authorizationFee;
        public string authorizationFeeCurrencyCode;
        public string authorizationState;
        public List<string> captureId = new List<string>();

        public DateTime lastUpdateTimestamp;
        public DateTime expirationTimeStamp;
        public DateTime creationTimestamp;

        public string reasonCode;
        public string reasonDescription;

        public bool captureNow;
        public string softDescriptor;

        public string errorCode;
        public string errorMessage;
        public string parentKey;

        public bool success = false;
        private static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public AuthorizeResponse(string xml)
        {
            log4net.Config.XmlConfigurator.Configure();
            this.xml = xml;
            ResponseParser.SetXml(xml);
            this.json = ResponseParser.ToJson();
            this.dictionary = ResponseParser.ToDict();

            ErrorResponse errorResponse = new ErrorResponse(this.dictionary);
            if (errorResponse.IsSetErrorCode() && errorResponse.IsSetErrorMessage())
            {
                success = false;
                log.Debug("METHOD__AuthorizeResponse Constructor | MESSAGE__success:" + this.success);
                this.errorCode = errorResponse.GetErrorCode();
                log.Debug("METHOD__AuthorizeResponse Constructor | MESSAGE__errorCode:" + this.errorCode);
                this.errorMessage = errorResponse.GetErrorMessage();
                log.Debug("METHOD__AuthorizeResponse Constructor | MESSAGE__errorMessage:" + this.errorMessage);
                this.requestId = errorResponse.GetRequestId();
                log.Debug("METHOD__AuthorizeResponse Constructor | MESSAGE__RequestId:" + this.requestId);
            }
            else
            {
                success = true;
                log.Debug("METHOD__AuthorizeResponse Constructor | MESSAGE__success:" + this.success);
                ParseDictionaryToVariables(this.dictionary);
            }
        }

        private enum Operator
        {
            AmazonOrderReferenceId, AmazonAuthorizationId, RequestId, SellerAuthorizationNote, ExpirationTimestamp, CreationTimestamp, AuthorizationReferenceId, State, Amount,
            AuthorizationAmount, CapturedAmount, AuthorizationFee, CurrencyCode, ReasonCode, ReasonDescription, CaptureNow, SoftDescriptor, LastUpdateTimestamp, member
        }

        /// <summary>
        /// Flattening the Dictionary
        /// The input dictionary contains key value pairs in the below format
        /// Type 1. Key (string) , Value (string)
        /// Type 2. Key (string) , Value (Dictionary)
        /// The function will parse the dictionary values into respective class variables by directly jumping to to the switch case for Type 1 
        /// else it will recursively parse the inner dictionary for Type 2
        /// </summary>
        /// <param name="dictionary"></param>
        private void ParseDictionaryToVariables(IDictionary dictionary)
        {
            foreach (string strKey in dictionary.Keys)
            {
                // Obj is the value of the dictionary key. this could either be a string or a nested inner dictionary.
                object obj = dictionary[strKey];
                if (obj != null)
                {
                    // If obj is dictionary recursively parse it
                    if (obj is IDictionary)
                    {
                        parentKey = strKey;
                        ParseDictionaryToVariables((IDictionary)obj);
                    }
                    else
                    {
                        if (Enum.IsDefined(typeof(Operator), strKey))
                        {
                            switch ((Operator)Enum.Parse(typeof(Operator), strKey))
                            {
                                case Operator.AmazonOrderReferenceId:
                                    amazonOrderReferenceId = obj.ToString();
                                    log.Debug("METHOD__ParseDictionaryToVariables | MESSAGE__AmazonOrderReferenceId:" + this.amazonOrderReferenceId);
                                    break;
                                case Operator.AmazonAuthorizationId:
                                    authorizationId = obj.ToString();
                                    log.Debug("METHOD__ParseDictionaryToVariables | MESSAGE__AmazonAuthorizationId:" + this.authorizationId);
                                    break;
                                case Operator.RequestId:
                                    requestId = obj.ToString();
                                    log.Debug("METHOD__ParseDictionaryToVariables | MESSAGE__RequestId:" + this.requestId);
                                    break;
                                case Operator.SellerAuthorizationNote:
                                    sellerAuthorizationNote = obj.ToString();
                                    log.Debug("METHOD__ParseDictionaryToVariables | MESSAGE__SellerAuthorizationNote:" + this.sellerAuthorizationNote);
                                    break;
                                case Operator.ExpirationTimestamp:
                                    expirationTimeStamp = DateTime.Parse(obj.ToString());
                                    log.Debug("METHOD__ParseDictionaryToVariables | MESSAGE__ExpirationTimestamp:" + this.expirationTimeStamp);
                                    break;
                                case Operator.LastUpdateTimestamp:
                                    lastUpdateTimestamp = DateTime.Parse(obj.ToString());
                                    log.Debug("METHOD__ParseDictionaryToVariables | MESSAGE__LastUpdateTimestamp:" + this.lastUpdateTimestamp);
                                    break;
                                case Operator.CreationTimestamp:
                                    creationTimestamp = DateTime.Parse(obj.ToString());
                                    log.Debug("METHOD__ParseDictionaryToVariables | MESSAGE__CreationTimestamp:" + this.creationTimestamp);
                                    break;
                                case Operator.AuthorizationReferenceId:
                                    authorizationReferenceId = obj.ToString();
                                    log.Debug("METHOD__ParseDictionaryToVariables | MESSAGE__AuthorizationReferenceId:" + this.authorizationReferenceId);
                                    break;
                                case Operator.State:
                                    authorizationState = obj.ToString();
                                    log.Debug("METHOD__ParseDictionaryToVariables | MESSAGE__AuthorizationState:" + this.authorizationState);
                                    break;
                                case Operator.Amount:
                                    if (parentKey.Equals(Operator.AuthorizationAmount.ToString()))
                                    {
                                        authorizationAmount = decimal.Parse(obj.ToString());
                                        log.Debug("METHOD__ParseDictionaryToVariables | MESSAGE__AuthorizationAmount:" + this.authorizationAmount);
                                    }
                                    else if (parentKey.Equals(Operator.CapturedAmount.ToString()))
                                    {
                                        capturedAmount = decimal.Parse(obj.ToString());
                                        log.Debug("METHOD__ParseDictionaryToVariables | MESSAGE__CapturedAmount:" + this.capturedAmount);
                                    }
                                    else if (parentKey.Equals(Operator.AuthorizationFee.ToString()))
                                    {
                                        authorizationFee = decimal.Parse(obj.ToString());
                                        log.Debug("METHOD__ParseDictionaryToVariables | MESSAGE__AuthorizationFee:" + this.authorizationFee);
                                    }
                                    break;
                                case Operator.CurrencyCode:
                                    if (parentKey.Equals(Operator.AuthorizationAmount.ToString()))
                                    {
                                        authorizationAmountCurrencyCode = obj.ToString();
                                        log.Debug("METHOD__ParseDictionaryToVariables | MESSAGE__authorizationAmountCurrencyCode:" + this.authorizationAmountCurrencyCode);
                                    }
                                    else if (parentKey.Equals(Operator.CapturedAmount.ToString()))
                                    {
                                        capturedAmountCurrencyCode = obj.ToString();
                                        log.Debug("METHOD__ParseDictionaryToVariables | MESSAGE__capturedAmountCurrencyCode:" + this.capturedAmountCurrencyCode);
                                    }
                                    else if (parentKey.Equals(Operator.AuthorizationFee.ToString()))
                                    {
                                        authorizationFeeCurrencyCode = obj.ToString();
                                        log.Debug("METHOD__ParseDictionaryToVariables | MESSAGE__authorizationFeeCurrencyCode:" + this.authorizationFeeCurrencyCode);
                                    }
                                    break;
                                case Operator.ReasonCode:
                                    reasonCode = obj.ToString();
                                    log.Debug("METHOD__ParseDictionaryToVariables | MESSAGE__ReasonCode:" + this.reasonCode);
                                    break;
                                case Operator.ReasonDescription:
                                    reasonDescription = obj.ToString();
                                    log.Debug("METHOD__ParseDictionaryToVariables | MESSAGE__ReasonDescription:" + this.reasonDescription);
                                    break;
                                case Operator.CaptureNow:
                                    captureNow = bool.Parse(obj.ToString());
                                    log.Debug("METHOD__ParseDictionaryToVariables | MESSAGE__CaptureNow:" + this.captureNow);
                                    break;
                                case Operator.SoftDescriptor:
                                    softDescriptor = obj.ToString();
                                    log.Debug("METHOD__ParseDictionaryToVariables | MESSAGE__SoftDescriptor:" + this.softDescriptor);
                                    break;
                                case Operator.member:
                                    if (obj.GetType() == typeof(JArray))
                                    {
                                        JArray capIdArray = JArray.Parse(obj.ToString());
                                        foreach (string capId in capIdArray)
                                        {
                                            captureId.Add(capId);
                                            log.Debug("METHOD__ParseDictionaryToVariables | MESSAGE__CaptureId:" + capId);
                                        }
                                    }
                                    else
                                    {
                                        captureId.Add(obj.ToString());
                                        log.Debug("METHOD__ParseDictionaryToVariables | MESSAGE__CaptureId:" + obj.ToString());
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
        }

        public string GetAmazonOrderReferenceId()
        {
            return this.amazonOrderReferenceId;
        }
        public string GetAuthorizationId()
        {
            return this.authorizationId;
        }
        public string GetRequestId()
        {
            return this.requestId;
        }
        public string GetAuthorizationReferenceId()
        {
            return this.authorizationReferenceId;
        }
        public string GetSellerAuthorizationNote()
        {
            return this.sellerAuthorizationNote;
        }
        public decimal GetAuthorizationAmount()
        {
            return this.authorizationAmount;
        }
        public string GetAuthorizationAmountCurrencyCode()
        {
            return this.authorizationAmountCurrencyCode;
        }
        public string GetCapturedAmountCurrencyCode()
        {
            return this.capturedAmountCurrencyCode;
        }
        public decimal GetCapturedAmount()
        {
            return this.capturedAmount;
        }
        public decimal GetAuthorizationFee()
        {
            return this.authorizationFee;
        }
        public string GetAuthorizationFeeCurrencyCode()
        {
            return this.authorizationFeeCurrencyCode;
        }
        public string GetAuthorizationState()
        {
            return this.authorizationState;
        }
        public IList<string> GetCaptureIdList()
        {
            return this.captureId.AsReadOnly();
        }
        public DateTime GetLastUpdateTimestamp()
        {
            return this.lastUpdateTimestamp;
        }
        public DateTime GetExpirationTimestamp()
        {
            return this.expirationTimeStamp;
        }
        public DateTime GetCreationTimestamp()
        {
            return this.creationTimestamp;
        }
        public string GetReasonCode()
        {
            return this.reasonCode;
        }
        public string GetReasonDescription()
        {
            return this.reasonDescription;
        }
        public bool GetCaptureNow()
        {
            return this.captureNow;
        }
        public string GetSoftDescriptor()
        {
            return this.softDescriptor;
        }
        public bool GetSuccess()
        {
            return this.success;
        }
        public string GetErrorCode()
        {
            return this.errorCode;
        }
        public string GetErrorMessage()
        {
            return this.errorMessage;
        }
        public string GetJson()
        {
            return this.json;
        }
        public string GetXml()
        {
            return this.xml;
        }
        public IDictionary GetDictionary()
        {
            return this.dictionary;
        }
    }
}