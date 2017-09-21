sqlcmd -S (localdb)\v11.0 -E -i CreateSemanticLoggingDatabase.sql
sqlcmd -S (localdb)\v11.0 -E -i CreateSemanticLoggingDatabaseObjects.sql -d Logging