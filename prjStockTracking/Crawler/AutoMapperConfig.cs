using AutoMapper;
using prjStockTracking.Crawler.AutoMapperProfile;

namespace prjStockTracking.Crawler
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