#!/bin/bash 
CONTAINER="${1:?Usage: ./hide_tests.sh <container>}"
BACKUP_DIR="golden_tests_backup"
WORKDIR=$(docker inspect -f '{{.Config.WorkingDir}}' $CONTAINER)


docker cp "$BACKUP_DIR" "${CONTAINER}:${WORKDIR}/golden_tests"
rm -rf "$BACKUP_DIR"
echo "Golden tests restored to container '$CONTAINER'"