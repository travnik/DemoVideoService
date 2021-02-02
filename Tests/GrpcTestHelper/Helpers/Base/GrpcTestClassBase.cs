using Grpc.Net.Client;
using GrpcTestHelper.Helpers.Contexts;

namespace GrpcTestHelper.Helpers.Base
{
    public class GrpcTestClassBase<TEntryPoint> where TEntryPoint : class
    {
        protected GrpcApplicationFactory<TEntryPoint> ApplicationFactory;
        protected GrpcChannel Channel;

        public GrpcTestClassBase()
        {
            ApplicationFactory = new GrpcApplicationFactory<TEntryPoint>();
            Channel = ApplicationFactory.CreateGrpcChannel();
        }

        protected TService GetRequiredService<TService>()
        {
            return ApplicationFactory.GetRequiredService<TService>();
        }
    }
}