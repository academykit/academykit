// <copyright file="CurrentUser.cs" company="Vurilo Nepal Pvt. Ltd.">
// Copyright (c) Vurilo Nepal Pvt. Ltd.. All rights reserved.
// </copyright>

namespace Lingtren.Api.Common
{
    using Lingtren.Domain.Enums;

    public class CurrentUser
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string MobileNumber { get; set; }

        public UserRole Role { get; set; }
    }
}
