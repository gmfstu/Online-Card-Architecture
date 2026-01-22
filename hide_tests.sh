#!/bin/bash
CONTAINER="${1:?Usage: ./hide_tests.sh <container>}"
BACKUP_DIR="golden_tests_backup"
WORKDIR=$(docker inspect -f  '{{.Config.WorkingDir}}' $CONTAINER)


docker cp $CONTAINER:$WORKDIR/golden_tests $BACKUP_DIR
docker exec "$CONTAINER" rm -rf golden_tests golden_tests_backup build_docker.sh run_docker.sh hide_tests.sh restore_tests.sh
echo "Golden tests hidden (backed up to $BACKUP_DIR)"