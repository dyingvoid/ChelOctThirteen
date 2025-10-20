Please, don't forget to add storage directory.
Web/appsettings.Development.json: "FileLoader": { "StoragePath": "%your_dir_here%" }

Run postgres image as the db https://hub.docker.com/_/postgres, since the app uses timestamptz date type.

Configure connection in appsettings.Development.json; Migrations will be applied on the startup.

To pull files use "[host]:[port]/load" endpoint