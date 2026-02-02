#include <sqlite3.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <limits.h>    // PATH_MAX
#include <unistd.h>    // readlink
#include <libgen.h>    // dirname

char *docStr = "ITU-Minitwit Tweet Flagging Tool\n\n"
               "Usage:\n"
               "  flag_tool <tweet_id>...\n"
               "  flag_tool -i\n"
               "  flag_tool -h\n"
               "Options:\n"
               "-h            Show this screen.\n"
               "-i            Dump all tweets and authors to STDOUT.\n";

/* Safe callback that prints all columns */
static int callback(void *data, int argc, char **argv, char **azColName) {
    for (int i = 0; i < argc; i++) {
        printf("%s%s", argv[i] ? argv[i] : "NULL", (i == argc-1) ? "\n" : ",");
    }
    return 0;
}

/* Function to compute the database path dynamically */
void get_db_path(char *db_path, size_t size) {
    char exe_path[PATH_MAX];
    ssize_t len = readlink("/proc/self/exe", exe_path, sizeof(exe_path)-1);
    if (len == -1) {
        perror("readlink");
        exit(1);
    }
    exe_path[len] = '\0';

    char *project_root = dirname(exe_path);
    snprintf(db_path, size, "%s/tmp/minitwit", project_root);
}

int main(int argc, char *argv[]) {
    sqlite3 *db;
    char *zErrMsg = 0;
    int rc;
    char query[512];  // bigger buffer to safely hold queries
    const char *data = "Callback function called";

    /* Step 1: Compute DB path dynamically */
    char db_path[PATH_MAX];
    get_db_path(db_path, sizeof(db_path));
    printf("Opening database at: %s\n", db_path);

    /* Step 2: Open SQLite database */
    rc = sqlite3_open(db_path, &db);
    if (rc) {
        fprintf(stderr, "Can't open database: %s\n", sqlite3_errmsg(db));
        return 1;
    }

    /* Step 3: Handle -h argument (help) */
    if (argc == 2 && strcmp(argv[1], "-h") == 0) {
        fprintf(stdout, "%s\n", docStr);
        sqlite3_close(db);
        return 0;
    }

    /* Step 4: Handle -i argument (list all messages) */
    if (argc == 2 && strcmp(argv[1], "-i") == 0) {
        strcpy(query, "SELECT * FROM message");
        rc = sqlite3_exec(db, query, callback, (void *)data, &zErrMsg);
        if (rc != SQLITE_OK) {
            fprintf(stderr, "SQL error: %s\n", zErrMsg);
            sqlite3_free(zErrMsg);
        }
        sqlite3_close(db);
        return 0;
    }

    /* Step 4b: Flag messages by IDs */
    if (argc >= 2 && strcmp(argv[1], "-i") != 0 && strcmp(argv[1], "-h") != 0) {
        for (int i = 1; i < argc; i++) {
            snprintf(query, sizeof(query),
                     "UPDATE message SET flagged=1 WHERE message_id=%s",
                     argv[i]);
            rc = sqlite3_exec(db, query, callback, (void *)data, &zErrMsg);
            if (rc != SQLITE_OK) {
                fprintf(stderr, "SQL error: %s\n", zErrMsg);
                sqlite3_free(zErrMsg);
            } else {
                printf("Flagged entry: %s\n", argv[i]);
            }
        }
    }

    sqlite3_close(db);
    return 0;
}
