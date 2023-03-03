using Mapster;

namespace QuickApi.Mapper
{
    /// <summary>
    /// https://medium.com/@M-S-2/enjoy-using-mapster-in-net-6-2d3f287a0989
    /// </summary>
    /// <typeparam name="TDto"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class BaseDto<TDto, TEntity> : IRegister
        where TDto : class, new()
        where TEntity : class, new()
    {

        public TEntity ToEntity()
        {
            return this.Adapt<TEntity>();
        }

        public TEntity ToEntity(TEntity entity)
        {
            return (this as TDto).Adapt(entity);
        }

        public static TDto FromEntity(TEntity entity)
        {
            return entity.Adapt<TDto>();
        }


        private TypeAdapterConfig Config { get; set; }

        public virtual void AddCustomMappings() { }

        /// <summary>
        /// DTO映射到Entity，如果要双向，第一个先加TwoWays()
        /// https://github.com/MapsterMapper/Mapster/wiki/Two-ways
        /// </summary>
        /// <returns></returns>
        protected TypeAdapterSetter<TDto, TEntity> SetCustomMappings()
            => Config.ForType<TDto, TEntity>();

        protected TypeAdapterSetter<TEntity, TDto> SetCustomMappingsInverse()
            => Config.ForType<TEntity, TDto>();

        public void Register(TypeAdapterConfig config)
        {
            Config = config;
            AddCustomMappings();
        }
    }
}