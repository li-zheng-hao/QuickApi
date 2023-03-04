using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;

namespace QuickApi.JwtAuthorization;

public class AppJwtBearerEvents : JwtBearerEvents
    {
        private readonly JwtOptions _jwtOptions;
        public AppJwtBearerEvents(IOptionsSnapshot<JwtOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
 
        }

        /// <summary>
        /// 当收到请求时（此时还未获取到Token）
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <remarks>
        /// 可在此自定义Token获取方式，并将获取的Token赋值到 context.Token（记得将Scheme从字符串中移除）
        /// 只要我们赋值的Token既非Null也非Empty，那后续验证就会使用该Token
        /// </remarks>
        public override Task MessageReceived(MessageReceivedContext context)
        {
            Debug.WriteLine("-------------- MessageReceived Begin --------------");

            base.MessageReceived(context);
            if (context.Result != null)
            {
                return Task.CompletedTask;
            }

            Debug.WriteLine($"Scheme: {context.Scheme.Name}");

            #region 以下是自定义Token获取方式示例（实际上也是默认方式）

            string authorization = context.Request.Headers[HeaderNames.Authorization];
            if (string.IsNullOrEmpty(authorization))
            {
                context.NoResult();
                return Task.CompletedTask;
            }

            if (authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                context.Token = authorization["Bearer ".Length..].Trim();
            }

            if (string.IsNullOrEmpty(context.Token))
            {
                context.NoResult();
                return Task.CompletedTask;
            }

            #endregion

            Debug.WriteLine("-------------- MessageReceived End --------------");

            return Task.CompletedTask;
        }

        /// <summary>
        /// Token验证通过后
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task TokenValidated(TokenValidatedContext context)
        {
            Debug.WriteLine("-------------- TokenValidated Begin --------------");

            base.TokenValidated(context);
            if (context.Result != null)
            {
                return Task.CompletedTask;
            }

            Debug.WriteLine($"User Name: {context.Principal.Identity.Name}");
            Debug.WriteLine($"Scheme: {context.Scheme.Name}");

            var token = context.SecurityToken;
            Debug.WriteLine($"Token Id: {token.Id}");
            Debug.WriteLine($"Token Issuer: {token.Issuer}");
            Debug.WriteLine($"Token Valid From: {token.ValidFrom}");
            Debug.WriteLine($"Token Valid To: {token.ValidTo}");

            Debug.WriteLine($"Token SecurityKey: {token.SecurityKey}");

            if (token.SigningKey is SymmetricSecurityKey ssk)
            {
                Debug.WriteLine($"Token SigningKey: {_jwtOptions.Encoding.GetString(ssk.Key)}");
            }
            else
            {
                Debug.WriteLine($"Token SigningKey: {token.SigningKey}");
            }

            Debug.WriteLine("-------------- TokenValidated End --------------");

            return Task.CompletedTask;
        }

        /// <summary>
        /// 由于认证过程中抛出异常，导致的身份认证失败后
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task AuthenticationFailed(AuthenticationFailedContext context)
        {
            Debug.WriteLine("-------------- AuthenticationFailed Begin --------------");

            base.AuthenticationFailed(context);
            if (context.Result != null)
            {
                return Task.CompletedTask;
            }

            Debug.WriteLine($"Scheme: {context.Scheme.Name}");
            Debug.WriteLine($"Exception: {context.Exception}");

            Debug.WriteLine("-------------- AuthenticationFailed End --------------");

            return Task.CompletedTask;
        }

        /// <summary>
        /// 质询
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task Challenge(JwtBearerChallengeContext context)
        {
            Debug.WriteLine("-------------- Challenge Begin --------------");

            base.Challenge(context);
            if (context.Handled)
            {
                return Task.CompletedTask;
            }
            // 可以在这里添加自定义响应 比如加个Header
            // context.Response.Headers.Add("NewHeader", "NewValue");

            Debug.WriteLine($"Scheme: {context.Scheme.Name}");
            Debug.WriteLine($"Authenticate Failure: {context.AuthenticateFailure}");
            Debug.WriteLine($"Error: {context.Error}");
            Debug.WriteLine($"Error Description: {context.ErrorDescription}");
            Debug.WriteLine($"Error Uri: {context.ErrorUri}");

            Debug.WriteLine("-------------- Challenge End --------------");

            return Task.CompletedTask;
        }

        /// <summary>
        /// 禁止403
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task Forbidden(ForbiddenContext context)
        {
            Debug.WriteLine("-------------- Forbidden Begin --------------");

            base.Forbidden(context);
            if (context.Result != null)
            {
                context.Request.Headers.Add("NewHeader", "NewValue");
                return Task.CompletedTask;
            }
            Debug.WriteLine($"Scheme: {context.Scheme.Name}");

            Debug.WriteLine("-------------- Forbidden End --------------");

            return Task.CompletedTask;
        }
    }