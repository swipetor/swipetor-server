ARGS = $(filter-out $@,$(MAKECMDGOALS))

dbupdate:
	dotnet ef database update -p SwipetorApp

addmigration:
	dotnet ef migrations add $(M) -p SwipetorApp

remove-last-migration:
	dotnet ef migrations remove -p SwipetorApp

migration-script:
	dotnet ef migrations script --idempotent --output "script.sql" -p SwipetorApp $(FROM)
