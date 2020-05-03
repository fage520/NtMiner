﻿using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class ClientDataBinaryController : ApiControllerBase, IClientDataBinaryController<HttpResponseMessage> {
        [Role.User]
        [HttpPost]
        public HttpResponseMessage QueryClients(QueryClientsRequest request) {
            QueryClientsResponse response;
            if (request == null) {
                response = ResponseBase.InvalidInput<QueryClientsResponse>("参数错误");
            }
            else {
                try {
                    var data = WebApiRoot.ClientDataSet.QueryClients(
                        User,
                        request,
                        out int total,
                        out List<CoinSnapshotData> latestSnapshots,
                        out int totalOnlineCount,
                        out int totalMiningCount) ?? new List<ClientData>();
                    response = QueryClientsResponse.Ok(data, total, latestSnapshots, totalMiningCount, totalOnlineCount);
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                    response = ResponseBase.ServerError<QueryClientsResponse>(e.Message);
                }
            }
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK) {
                Content = new ByteArrayContent(VirtualRoot.BinarySerializer.Serialize(response))
            };
            httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");
            return httpResponseMessage;
        }
    }
}