using QuickApi.HttpResponse;

namespace QuickApi.Extensions
{
    public static class Check
    {
        /// <summary>
        /// 检查是否不为空
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="message"></param>
        /// <typeparam name="T"></typeparam>
        /// <exception cref="BusinessException"></exception>
        public static void NotNull<T>(T obj, string message = null) where T : class
        {
            if (obj == null)
            {
                throw new BusinessException(message ?? "参数不能为空");
            }
        }
        /// <summary>
        /// 条件为true则抛出异常
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="message"></param>
        /// <exception cref="BusinessException"></exception>
        public static void ThrowIf(bool condition, string message = null)
        {
            if (condition)
            {
                throw new BusinessException(message ?? "参数错误");
            }
        }
    }
}