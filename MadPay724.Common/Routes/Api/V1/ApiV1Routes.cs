﻿
namespace MadPay724.Common.Routes.V1.Api
{
    public static class ApiV1Routes
    {
        public const string BaseApi = "pg";

        #region Pay
        public static class Pay
        {
            //Post
            public const string PaySend = BaseApi + "/pay";
        }
        #endregion

    }
}