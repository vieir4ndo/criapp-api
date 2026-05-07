to run migration

dotnet ef migrations add AddQuestionsNewFields --project src/CRIApp.Repository --startup-project src/CRIApp.API

to apply migration
dotnet ef database update --project src/CRIApp.Repository/ --startup-project src/CRIApp.API

rollback database
dotnet ef database update AddCreatedAtToAnswerTable --project src/CRIApp.Repository --startup-project src/CRIApp.API

remove last migration
dotnet ef migrations remove --project src/CRIApp.Repository --startup-project src/CRIApp.API