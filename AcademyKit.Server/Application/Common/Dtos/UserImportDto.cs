﻿namespace AcademyKit.Application.Common.Dtos
{
    using CsvHelper.Configuration.Attributes;

    public class UserImportDto
    {
        [Name("FirstName")]
        public string FirstName { get; set; }

        [Name("MiddleName")]
        public string MiddleName { get; set; }

        [Name("LastName")]
        public string LastName { get; set; }

        [Name("Email")]
        public string Email { get; set; }

        [Name("MobileNumber")]
        public string MobileNumber { get; set; }

        [Name("Role")]
        public string Role { get; set; }

        [Name("Designation")]
        public string Designation { get; set; }

        [Name("Department")]
        public string Department { get; set; }
    }
}
