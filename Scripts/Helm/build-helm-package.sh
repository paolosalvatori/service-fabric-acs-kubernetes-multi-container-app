#!/bin/bash
# Build helm package

# Build package
helm package /mnt/c/[PATH-TO-CHART]/TodoList

# Install package
helm install --name todolist TodoList-1.0.0.tgz

# Install package for ssl-todolist
helm install --name ssl-todolist --values /mnt/c/[PATH-TO-CHART]/ssl-todolist-values.yaml TodoList-1.0.0.tgz