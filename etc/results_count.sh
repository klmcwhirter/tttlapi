#/bin/bash

SCRIPT=results_count.js
CMD='mongo --quiet -u $MONGODB_USER -p $MONGODB_PASSWORD $MONGODB_DATABASE '$SCRIPT
POD=$(oc get pods --selector name=mongodb -o name | sed 's?pod/??')

# echo oc cp $SCRIPT $POD:/opt/app-root/src
oc cp $SCRIPT $POD:/opt/app-root/src 2>/dev/null

# echo oc exec $POD -- bash -lc $CMD
oc exec $POD -- bash -lc "$CMD" 2>/dev/null
