using System;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Connections;
using OzonEdu.MerchandiseService.Domain.Services.Interfaces;
using OzonEdu.MerchandiseService.Grpc;

namespace OzonEdu.MerchandiseService.GrpcServices
{
    public class MerchandiseGrpcService : MerchandiseServiceGrpc.MerchandiseServiceGrpcBase
    {
        private readonly IMerchandiseService _merchandiseService;
        
        public MerchandiseGrpcService(IMerchandiseService merchandiseService)
        {
            _merchandiseService = merchandiseService;
        }

        public override async Task GetAll(
            Empty request,
            IServerStreamWriter<GetAllMerchResponse> responseStream,
            ServerCallContext context)
        {
            var allMerch = await _merchandiseService.GetAllMerch(context.CancellationToken);
            foreach (var merch in allMerch.Data)
            {
                if (context.CancellationToken.IsCancellationRequested)
                {
                    throw new ConnectionAbortedException("Aborted by cancellation token");
                }

                await responseStream.WriteAsync(new GetAllMerchResponse()
                {
                    Id = merch.Id.ToString(),
                    Name = merch.Name
                });
            }
        }

        public override async Task<MerchInfoResponse> GetInfo(
            MerchInfoRequest request,
            ServerCallContext context)
        {
            var merchInfo = await _merchandiseService.GetMerchInfo(
                Guid.Parse(request.Id),
                context.CancellationToken);
            
            return new MerchInfoResponse()
            {
                Description = merchInfo.Description,
                Name = merchInfo.Name
            };
        }
    }
}