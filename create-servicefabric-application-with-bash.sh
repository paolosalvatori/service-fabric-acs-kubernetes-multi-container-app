#!/bin/bash

sfctl application upload --path TodoApp\pkg\Release --show-progress
sfctl application provision --application-type-build-path TodoApp
sfctl application create --app-name fabric:/TodoApp --app-type TodoAppType --app-version 1.0.0