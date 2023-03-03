// MIT 许可证
//
// 版权 (c) 2020-present 百小僧, 百签科技（广东）有限公司 和所有贡献者
//
// 特此免费授予任何获得本软件副本和相关文档文件（下称“软件”）的人不受限制地处置该软件的权利，
// 包括不受限制地使用、复制、修改、合并、发布、分发、转授许可和/或出售该软件副本，
// 以及再授权被配发了本软件的人如上的权利，须在下列条件下：
//
// 上述版权声明和本许可声明应包含在该软件的所有副本或实质成分中。
//
// 本软件是“如此”提供的，没有任何形式的明示或暗示的保证，包括但不限于对适销性、特定用途的适用性和不侵权的保证。
// 在任何情况下，作者或版权持有人都不对任何索赔、损害或其他责任负责，无论这些追责来自合同、侵权或其它行为中，
// 还是产生于、源于或有关于本软件以及本软件的使用或其它处置。

using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace QuickApi.UnitOfWork
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class UnitOfWorkAttribute : Attribute, IAsyncActionFilter, IOrderedFilter
    {

        /// <summary>
        /// 事务隔离级别 仅限于关系型数据库
        /// </summary>
        public IsolationLevel TransactionIsolationLevel { get; set; } = IsolationLevel.ReadCommitted;

        /// <summary>
        /// 事务超时时间
        /// </summary>
        /// <remarks>单位秒</remarks>
        public int TransactionTimeout { get; set; } = 60;

        /// <summary>
        /// 过滤器排序 越小越先执行
        /// </summary>
        private const int FilterOrder = 9999;

        /// <summary>
        /// 排序属性
        /// </summary>
        public int Order => FilterOrder;

        /// <summary>
        /// 拦截请求
        /// </summary>
        /// <param name="context">动作方法上下文</param>
        /// <param name="next">中间件委托</param>
        /// <returns></returns>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // 获取动作方法描述器
            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            var method = actionDescriptor.MethodInfo;

            // 创建分布式环境事务
            (var transactionScope, var logger) = CreateTransactionScope(context);

            try
            {
                // 开始事务
                (var _unitOfWork,var unitOfWorkAttribute)= await BeginTransactionAsync(context, method);

                // 获取执行 Action 结果
                var resultContext = await next();

                // 提交事务
                await CommitTransactionAsync(context, _unitOfWork, unitOfWorkAttribute, resultContext);

                // 提交分布式环境事务
                if (resultContext.Exception == null)
                {
                    transactionScope?.Complete();
                }
                else
                {
                    logger.LogError(resultContext.Exception, "Transaction Failed.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Transaction Failed.");

                throw;
            }
            finally
            {
                transactionScope?.Dispose();
            }
        }

        /// <summary>
        /// 创建事务
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private (TransactionScope, ILogger<UnitOfWorkAttribute>) CreateTransactionScope(FilterContext context)
        {
            // 是否启用分布式环境事务
            var transactionScope = new TransactionScope();

            // 创建日志记录器
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<UnitOfWorkAttribute>>();

            return (transactionScope, logger);
        }

        /// <summary>
        /// 开始事务
        /// </summary>
        /// <param name="context"></param>
        /// <param name="method"></param>
        /// <param name="_unitOfWork"></param>
        /// <param name="unitOfWorkAttribute"></param>
        private static async Task<(IUnitOfWork,UnitOfWorkAttribute)> BeginTransactionAsync(FilterContext context, MethodInfo method)
        {
            // 解析工作单元服务
            var _unitOfWork = context.HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();

            // 获取工作单元特性
            var unitOfWorkAttribute = method.GetCustomAttribute<UnitOfWorkAttribute>();

            // 调用开启事务方法
            await _unitOfWork.BeginTransactionAsync(context, unitOfWorkAttribute);

            return (_unitOfWork, unitOfWorkAttribute);
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        /// <param name="context"></param>
        /// <param name="_unitOfWork"></param>
        /// <param name="unitOfWorkAttribute"></param>
        /// <param name="resultContext"></param>
        private static async Task CommitTransactionAsync(FilterContext context, IUnitOfWork _unitOfWork,
            UnitOfWorkAttribute unitOfWorkAttribute, FilterContext resultContext)
        {
            // 获取动态结果上下文
            dynamic dynamicResultContext = resultContext;

            if (dynamicResultContext.Exception == null)
            {
                // 调用提交事务方法
                await _unitOfWork.CommitTransactionAsync(resultContext, unitOfWorkAttribute);
            }
            else
            {
                // 调用回滚事务方法
                await _unitOfWork.RollbackTransactionAsync(resultContext, unitOfWorkAttribute);
            }

            // 调用执行完毕方法
            _unitOfWork.OnCompleted(context, resultContext);
            
        }
    }
}