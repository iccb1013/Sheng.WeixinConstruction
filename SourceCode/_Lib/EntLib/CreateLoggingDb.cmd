sqlcmd -S (localdb)\v11.0 -E -i CreateLoggingDatabase.sql
sqlcmd -S (localdb)\v11.0 -E -i CreateLoggingDatabaseObjects.sql -d Logging