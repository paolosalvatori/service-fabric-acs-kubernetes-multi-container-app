#!/bin/bash
# Install helm on the local machine

# Download Helm
curl https://raw.githubusercontent.com/kubernetes/helm/master/scripts/get > get_helm.sh

# Make get_helm executable
$ chmod 700 get_helm.sh

# Execute get_helm.sh
$ ./get_helm.sh

# Initialize helm
helm init --client-only