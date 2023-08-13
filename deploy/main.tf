provider "azurerm" {
  features {}
}

resource "azurerm_resource_group" "rg" {
  name     = lower("${var.prefix}_rg")
  location = var.location
}

variable "prefix" {
  description = "Prefix set appropriately to ensure that resources are unique."
}

variable "location" {
  description = "The Azure Region in which all resources in this example should be created."
  default = "EastUS"
}

resource "azurerm_storage_account" "storage" {
  name                      = lower("${var.prefix}storage")
  resource_group_name       = azurerm_resource_group.rg.name
  location                  = azurerm_resource_group.rg.location
  account_tier              = "Standard"
  account_kind              = "StorageV2"
  account_replication_type  = "LRS"
  enable_https_traffic_only = true
}

output "storage-connectionstring" {
  value     = azurerm_storage_account.storage.primary_blob_connection_string
  sensitive = true
}

resource "azurerm_service_plan" "app-plan" {
  name                = lower("${var.prefix}-app-plan")
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  os_type             = "Windows"
  sku_name            = "Y1"
}

resource "azurerm_application_insights" "appinsights" {
  name                = lower("${var.prefix}-appinsights")
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  application_type    = "web"
  tags                = {}
}

resource "azurerm_eventgrid_topic" "eventgrid" {
  name                = lower("${var.prefix}-event-grid")
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
}

resource "azurerm_storage_queue" "step1queue" {
  name                 = "step1-queue"
  storage_account_name = azurerm_storage_account.storage.name
}

resource "azurerm_storage_queue" "step2queue" {
  name                 = "step2-queue"
  storage_account_name = azurerm_storage_account.storage.name
}

resource "azurerm_eventgrid_event_subscription" "step1completed" {
  name  = "Step2Start"
  scope = azurerm_eventgrid_topic.eventgrid.id
  included_event_types = [
    "Step1Completed"
  ]
  labels = []

  storage_queue_endpoint {
    storage_account_id = azurerm_storage_account.storage.id
    queue_name         = azurerm_storage_queue.step2queue.name
  }
}

resource "azurerm_windows_function_app" "functions" {
  name                        = lower("${var.prefix}-functions")
  location                    = azurerm_resource_group.rg.location
  resource_group_name         = azurerm_resource_group.rg.name
  service_plan_id             = azurerm_service_plan.app-plan.id
  storage_account_name        = azurerm_storage_account.storage.name
  storage_account_access_key  = azurerm_storage_account.storage.primary_access_key
  tags                        = {}
  functions_extension_version = "~4"

  app_settings = {
    "StorageConnection" = azurerm_storage_account.storage.primary_connection_string
    "EventGridEndpoint" = azurerm_eventgrid_topic.eventgrid.endpoint
    "EventGridKey" = azurerm_eventgrid_topic.eventgrid.primary_access_key
    "WEBSITE_RUN_FROM_PACKAGE": "1"
  }
  
  site_config {
    application_insights_key               = azurerm_application_insights.appinsights.instrumentation_key
    application_insights_connection_string = azurerm_application_insights.appinsights.connection_string
    application_stack {
      dotnet_version = "v6.0"
    }
    
  }
}

