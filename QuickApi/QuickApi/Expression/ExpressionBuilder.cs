using System;
using System.Linq;
using System.Linq.Expressions;

namespace QuickApi.Expression
{
    /// <summary>
    /// 表达式创建
    /// </summary>
    public static class ExpressionBuilder
    {
        public static Expression<Func<T, bool>> True<T>()
        {
            return f => true;
        }

        public static Expression<Func<T, bool>> False<T>() 
        {
            return f => false;
        }
        
        /// <summary>
        /// 获取排序表达式 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, object>> OrderBy<T>(Expression<Func<T, object>>? func=null) 
        {
             return func;
        }
      
        private static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second,
            Func<System.Linq.Expressions.Expression, System.Linq.Expressions.Expression,
                System.Linq.Expressions.Expression> merge)
        {
            // build parameter map (from parameters of second to parameters of first)  
            var map = first.Parameters.Select((f, i) => new {f, s = second.Parameters[i]})
                .ToDictionary(p => p.s, p => p.f);

            // replace parameters in the second lambda expression with parameters from the first  
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

            // apply composition of lambda expression bodies to parameters from the first expression   
            return System.Linq.Expressions.Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }

        /// <summary>
        /// 扩展 必须使用T=T.And(...)方式,而不是在原对象上改变
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second)
        {
            return first.Compose(second, System.Linq.Expressions.Expression.AndAlso);
        }
        /// <summary>
        /// 根据条件扩展
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> AndIf<T>(this Expression<Func<T, bool>> first
            ,bool cond,
            Expression<Func<T, bool>> second)
        {
            if(cond)
                return first.Compose(second, System.Linq.Expressions.Expression.AndAlso);
            else
            {
                return first;
            }
        }
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second)
        {
            return first.Compose(second, System.Linq.Expressions.Expression.OrElse);
        }
    }
}