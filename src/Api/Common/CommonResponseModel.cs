// <copyright file="CommonResponseModel.cs" company="Vurilo Nepal Pvt. Ltd.">
// Copyright (c) Vurilo Nepal Pvt. Ltd.. All rights reserved.
// </copyright>

namespace Lingtren.Api.Common
{
    public class CommonResponseModel
    {
        public bool Success { get; set; } = false;

        public string Message { get; set; } = string.Empty;
    }
}
