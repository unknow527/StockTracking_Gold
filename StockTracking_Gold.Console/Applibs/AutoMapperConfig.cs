using AutoMapper;
using StockTracking_Gold.Ap.Applibs.AutoMapperProfile;

namespace StockTracking_Gold.Ap.Applibs
{
    public static class AutoMapperConfig
    {
        private static IMapper mapper;

        public static IMapper Mapper
        {
            get
            {
                if (mapper == null)
                {
                    Register();
                }

                return mapper;
            }
        }

        /// <summary>
        /// 註冊automapper設定
        /// </summary>
        public static void Register()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AllowNullCollections = true;
                cfg.AddProfile<StockInfoProfile>();
            });

            mapper = config.CreateMapper();
        }
    }
}
