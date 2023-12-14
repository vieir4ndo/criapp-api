to run migration

dotnet ef migrations add AddUserResetPasswordCodeTable --project src/CRIApp.Repository/

to apply migration
dotnet ef database update --project src/CRIApp.Repository/