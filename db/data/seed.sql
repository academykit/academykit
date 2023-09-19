USE `standalone`;

-- Delete existing data from User, SMTPSetting, ZoomSetting, GeneralSetting, Level, and Setting tables
SET SQL_SAFE_UPDATES = 0;

DELETE FROM SMTPSettings;
DELETE FROM ZoomSettings;
DELETE FROM GeneralSettings;
DELETE FROM Levels;
DELETE FROM Settings;
DELETE FROM RefreshTokens;
DELETE FROM Users;

SET SQL_SAFE_UPDATES = 1;

-- Insert User data
INSERT INTO Users (id, first_name, middle_name, last_name, address, email, mobile_number, created_by, created_on, updated_by, updated_on, STATUS, hash_password, ROLE)
VALUES ('30fcd978-f256-4733-840f-759181bc5e63', 'ABC', NULL, 'XYZ', 'ADDRESS', 'vuriloapp@gmail.com', '1234567890', '30fcd978-f256-4733-840f-759181bc5e63', '2022-11-04 10:35:19.3073004', '30fcd978-f256-4733-840f-759181bc5e63', '2022-11-04 10:35:19.3073004', 1, '+gURQgHBT1zJz5AljZhAMyaNRFQBVorq5HIlEmhf+ZQ=:BBLvXedGXzdz0ZlypoKQxQ==', 1);

-- Insert SMTPSetting data
INSERT INTO SMTPSettings (Id, Mail_Port, Mail_Server, Password, Reply_To, Sender_Email, Sender_Name, User_Name, UseSSL, Created_By, Created_On, Updated_By, Updated_On)
VALUES ('d3c343d8-adf8-45d4-afbe-e09c3285da24', 123, 'email-smtp.ap-south-1.amazonaws.com', 'password', 'support@vurilo.com', 'noreply@vurilo.com', 'Vurilo', 'username', true, '30fcd978-f256-4733-840f-759181bc5e63', '2022-11-04 10:35:19.3073004', '30fcd978-f256-4733-840f-759181bc5e63', '2022-11-04 10:35:19.3073004');

-- Insert ZoomSettings data
INSERT INTO ZoomSettings (Id, Sdk_Key, Sdk_Secret, OAuth_Account_Id, OAuth_Client_Id, OAuth_Client_Secret, Is_Recording_Enabled, Created_By, Created_On, Updated_By, Updated_On)
VALUES ('f41a902f-fabd-4749-ac28-91137f685cb8', 'sdk key value', 'sdk secret value', 'OAuth account id', 'OAuth client id', 'OAuth client secret', false, '30fcd978-f256-4733-840f-759181bc5e63', '2022-11-04 10:35:19.3073004', '30fcd978-f256-4733-840f-759181bc5e63', '2022-11-04 10:35:19.3073004');

INSERT INTO GeneralSettings (id, logo_url, company_name, company_address, company_contact_number, custom_configuration, email_signature, created_by, created_on, updated_by, updated_on)
VALUES ('2d7867fc-b7e7-461d-9257-d0990b5ac991', 'https://miniostorage-s3.apps.vurilo.com/standalone/public/2382fe86-f376-446a-a5ab-dc0d0f6e2ca2.png', 'Vurilo Academy', 'Kathmandu 56 Nepal Maharajgunj, Opposite to Australian Embassy', '9801230314', '{\"accent\":\"#0E99AC\"}', 'Best Regards!\nLearning & Training Department\nVurilo Nepal Pvt. Ltd.', '30fcd978-f256-4733-840f-759181bc5e63', '2022-11-04 10:35:19', '30fcd978-f256-4733-840f-759181bc5e63', '2023-09-11 08:58:34');

-- Insert Level data
INSERT INTO Levels (Id, Name, Slug, Is_Active, Created_By, Created_On)
VALUES ('7e6ff101-cfa2-4aec-bd25-42780be476c3', 'Beginner', 'beginner', false, '30fcd978-f256-4733-840f-759181bc5e63', '2022-11-04 10:35:19.3073004'), ('7df8d749-6172-482b-b5a1-016fbe478795', 'Intermediate', 'intermediate', false, '30fcd978-f256-4733-840f-759181bc5e63', '2022-11-04 10:35:19.3073004'), ('9be84cd8-1566-4af5-8442-61cb1796dc46', 'Advanced', 'advanced', false, '30fcd978-f256-4733-840f-759181bc5e63', '2022-11-04 10:35:19.3073004');

-- Insert Setting data
INSERT INTO Settings (`Key`, Value)
VALUES ('Storage', 'Server'), ('AWS_AccessKey', NULL), ('AWS_SecretKey', NULL), ('AWS_FileBucket', NULL), ('AWS_VideoBucket', NULL), ('AWS_CloudFront', NULL), ('AWS_RegionEndpoint', NULL), ('Server_Url', 'https://miniostorage-api.apps.vurilo.com'), ('Server_Bucket', 'standalone'), ('Server_AccessKey', '83b695782c5f09fb38046259'), ('Server_SecretKey', '0feaa3b6594ee559f1569c5dc3ea9e93b8ef0e'), ('Server_PresignedExpiryTime', 1000), ('Server_EndPoint', 'miniostorage-api.apps.vurilo.com'), ('Server_PresignedUrl', 'miniostorage-api.apps.vurilo.com');