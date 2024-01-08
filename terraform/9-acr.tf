resource "azurerm_container_registry" "this" {
  name                     = local.acr_name
  location                 = azurerm_resource_group.this.location
  resource_group_name      = azurerm_resource_group.this.name
  sku                      = "Basic"
  admin_enabled            = true
}
